namespace Kreyos.SDK.Bluetooth
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using Windows.Networking.Proximity;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;
    using System.Diagnostics;
    using Windows.Foundation;

    public enum EBTConnect
    {
        None, // Neutral
        BluetoothIsOff, // Bluetooth of the mobile phone is off
        CapPermission,  // Capabilities ID_CAP_NETWORKING, ID_CAP_PROXIMITY is not enabled
        Error
    };

    public class BluetoothAgent
    {
        //singleton
        private static BluetoothAgent _instance = null;
        private BluetoothAgent()
        {
            this.Connected = false;
            this.InnerProtocol = new Protocol(this);
        }

        private PeerInformation peerDevice = null;
        private StreamSocket btSocket = null;
        private DataWriter dataWriter = null;
        private DataReader dataReader = null;
        private Queue<JobDesc> jobQueue = new Queue<JobDesc>();
        private Thread SendThread = null;
        private Thread RecvThread = null;
        private Thread WatchDog = null;
        private bool QuitFlag = false;
        private bool StopAgent = false;
        private bool IsRunning = false;
        private AutoResetEvent sendJobEnqued = new AutoResetEvent(false);

        public bool Connected { get; set; }
        public Protocol InnerProtocol { get; set; }
        public string ErrorMessage { get; private set; }
        public EBTConnect ConnectInfo { get; private set; }

        static BluetoothAgent()
        {
            BluetoothAgent._instance = new BluetoothAgent();
        }

        public static BluetoothAgent Instance
        {
            get { return BluetoothAgent._instance; }
        }

        public static async Task<IList<string>> GetPairedDevices()
        {
            PeerFinder.AlternateIdentities["Bluetooth:PAIRED"] = "";

            //var devices = await PeerFinder.FindAllPeersAsync();
            var result = new List<string>();
            var devices = await PeerFinder.FindAllPeersAsync();

            if (devices.Count == 0)
            {
                return new List<string>();
            }

            foreach (var device in devices)
            {
                if (device.DisplayName.StartsWith("Kreyos") ||
                    device.DisplayName.StartsWith("Meteor"))
                {
                    result.Add(device.DisplayName);
                }
            }
            
            return result;
        }

        private async Task<bool> Connect(string deviceName)
        {
            if (this.Connected)
            {
                if (this.peerDevice.DisplayName == deviceName)
                    return true;
                else
                    this.Disconnect();
            }

            PeerFinder.AlternateIdentities["Bluetooth:PAIRED"] = "";

            var devices = await PeerFinder.FindAllPeersAsync();
            if (devices.Count == 0)
            {
                return false;
            }

            foreach (var device in devices)
            {
                if (device.DisplayName == deviceName)
                {
                    this.peerDevice = device;
                    break;
                }
            }

            if (this.peerDevice == null)
            {
                return false;
            }

            StreamSocket s = new StreamSocket();
            s.Control.OutboundBufferSizeInBytes = 128;

            try
            {
                this.ConnectInfo = EBTConnect.None;
                await s.ConnectAsync(this.peerDevice.HostName, "1");
            }
            catch (System.Exception ex)
            {
                // Bluetooth is off
                if ((uint)ex.HResult == 0x8007048F)
                {
                    this.ConnectInfo = EBTConnect.BluetoothIsOff;
                }
                // ID_CAP_NETWORKING, ID_CAP_PROXIMITY Capabilities
                else if ((uint)ex.HResult == 0x80070005)
                {
                    this.ConnectInfo = EBTConnect.CapPermission;
                }
                else
                {
                    this.ConnectInfo = EBTConnect.Error;
                }

                this.ErrorMessage = ex.Message;
                return false;
            }

            //This would ask winsock to do an SPP lookup for us; i.e. to resolve the port the 
            //device is listening on
            //await s.ConnectAsync(peerInfo.HostName, "{00001101-0000-1000-8000-00805F9B34FB}");

            this.dataWriter = new DataWriter(s.OutputStream);
            this.dataReader = new DataReader(s.InputStream);
            this.btSocket = s;
            this.Connected = true;

            this.QuitFlag = false;

            this.SendThread = new Thread(SendProc);
            this.SendThread.Start();

            this.RecvThread = new Thread(ReadProc);
            this.RecvThread.Start();

            //send sync time job
            this.InnerProtocol.syncTime();

            return true;
        }

        private async void Disconnect()
        {
            if (!this.Connected)
                return;

            try
            {
                this.QuitFlag = true;
                if (this.RecvThread != null)
                {
                    this.RecvThread.Join();
                    this.RecvThread = null;
                }

                if (this.SendThread != null)
                {
                    this.SendThread.Join();
                    this.SendThread = null;
                }

                if (this.dataWriter != null)
                {
                    await this.dataWriter.FlushAsync();
                    this.dataWriter.Dispose();
                    this.dataWriter = null;
                }

                if (this.dataReader != null)
                {
                    this.dataReader.Dispose();
                    this.dataReader = null;
                }

                if (this.btSocket != null)
                {
                    this.btSocket.Dispose();
                    this.btSocket = null;
                }
            }
            catch (System.Exception e)
            {
                this.ErrorMessage = e.Message;
            }
            this.Connected = false;
        }

        public async Task<bool> Start(string deviceName)
        {
            if (IsRunning)
                return true;

            bool succ = await Connect(deviceName);
            if (succ)
            {
                this.WatchDog = new Thread(WatchDogProc);
                this.WatchDog.Start();
            }

            return succ;
        }

        public void Stop()
        {
            this.StopAgent = true;
            this.WatchDog = null;

            Disconnect();
        }

        public void SendBytes(byte[] packet)
        {
            lock (this.jobQueue)
            {
                this.jobQueue.Enqueue(new JobDesc(packet));
                this.sendJobEnqued.Set();
            }
        }

        public void BatchRegisterJob(IEnumerable<JobDesc> jobs)
        {
            lock (this.jobQueue)
            {
                foreach (var job in jobs)
                {
                    this.jobQueue.Enqueue(job);
                }
                this.sendJobEnqued.Set();
            }
        }

        private async void ReConnect()
        {
            await Connect(this.peerDevice.DisplayName);
        }

        private void WatchDogProc()
        {
            while (!this.StopAgent)
            {
                if (this.Connected)
                {
                    Thread.Sleep(30 * 1000);
                    continue;
                }
                ReConnect();
            }
        }

        private async void SendProc()
        {
            while (!this.QuitFlag)
            {
                var job = DequeueJob();
                if (job == null)
                {
                    this.sendJobEnqued.WaitOne(30 * 1000);
                }
                else
                {
                    if (job.IsQuitSignal)
                    {
                        return;
                    }

                    await SendJob(job);
                    Thread.Sleep(10);
                }
            }
        }

        private void ReadProc()
        {
            this.dataReader.InputStreamOptions = InputStreamOptions.Partial;
            while (!this.QuitFlag)
            {
                ReadPacket();
                Thread.Sleep(10);
            }
        }

        private JobDesc DequeueJob()
        {
            lock (this.jobQueue)
            {
                try
                {
                    return this.jobQueue.Dequeue();
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }

        private const int SPP_PACKET_DATA_SIZE = 62; // This is because the mtu in wp is 63.
        private async Task SendJob(JobDesc job)
        {
            byte[] p = job.Data;
            int byte_sent = 0;
            int byte_to_send = p.Length;
            while (byte_to_send - byte_sent > 0)
            {
                int byteLeft = byte_to_send - byte_sent;
                int subPacketSize = byteLeft;
                if (subPacketSize > SPP_PACKET_DATA_SIZE)
                    subPacketSize = SPP_PACKET_DATA_SIZE;

                byte[] temp = new byte[subPacketSize + 1];
                temp[0] = 0;
                if (byte_sent == 0)
                    temp[0] |= 0x01;
                if (subPacketSize == byteLeft)
                    temp[0] |= 0x02;
                Array.Copy(p, byte_sent, temp, 1, subPacketSize);
                dataWriter.WriteBytes(temp);

                await dataWriter.StoreAsync();
                await dataWriter.FlushAsync();

                byte_sent += subPacketSize;
            }
        }

        private async void ReadPacket()
        {
            try
            {
                var bytesRead = await this.dataReader.LoadAsync(64);
                if (bytesRead == 0)
                {
                    return;
                }

                byte[] data = new byte[bytesRead];
                this.dataReader.ReadBytes(data);
                handleIncomeData(data, data.Length);
            }
            catch (Exception e)
            {
                this.ErrorMessage = e.Message;
                return;
            }
        }

        public PeerInformation PeeredDevice { get { return peerDevice; } }

        private byte[] readerBuffer = new byte[256];
        private int byteReceived = 0;
        private int stlvPacketLen = 0;
        public void handleIncomeData(byte[] buffer, int length)
        {
            if (buffer[1] != 0x01 && buffer[2] != 0x80 && buffer[4] != 0x00)
                return;

            int payloadLen = ((int)buffer[0] & 0x000000ff) >> 2;

            if ((buffer[0] & 0x01) != 0)
            {
                //begin of packet
                byteReceived = 0;
                stlvPacketLen = ((int)buffer[3] & 0x000000ff) + 4;
                if (stlvPacketLen >= 256)
                {
                    byteReceived = 0;
                    return;
                }
            }

            Array.Copy(buffer, 1, readerBuffer, byteReceived, payloadLen);
            byteReceived += payloadLen;

            if ((buffer[0] & 0x02) != 0)
            {
                if (byteReceived != stlvPacketLen)
                {
                    return;
                }

                //end of packet
                byte[] packet = new byte[byteReceived];
                Array.Copy(readerBuffer, 0, packet, 0, byteReceived);
                byteReceived = 0;

                InnerProtocol.handlePacket(packet);
            }
        }
    }
}
