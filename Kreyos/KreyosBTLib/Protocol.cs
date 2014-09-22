using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Diagnostics;

namespace Kreyos.SDK.Bluetooth
{
    public class Protocol
    {

        #region Events
        public delegate void StringDataHandler(string fileName);
        public delegate void StringListHandler(IEnumerable<string> fileNames);
        public delegate void BytesDataHandler(byte[] data);
        public delegate void ElementParsedHandler(ElementDesc element);
        public delegate void ActivityDataHandler(ActivityDataDoc doc);
        public delegate void TodayActivityDataHandler(TodayActivity act);
        public delegate void SportDataHandler(SportsDataRow sports);

        public event ElementParsedHandler OnElementParsed;
        public event StringDataHandler OnFileReceived;
        //public event StringListHandler OnFileListed;
        public event StringDataHandler OnDeviceIDRead;
        //public event StringDataHandler OnDeviceStatusRead;
        public event ActivityDataHandler OnOverallActivitiesReceived;
        public event TodayActivityDataHandler OnTodayActivityReceived;
        public event SportDataHandler OnSportsDataReceived;

        public event BytesDataHandler OnActivityDataRead;
        public event BytesDataHandler OnSportsGridRead;
        public event BytesDataHandler OnActivityDataEnd;
        public event BytesDataHandler OnActivityDataPrepared;
        public event BytesDataHandler OnActivityDataSync;
        public event StringDataHandler OnFirmwareVersionRequest;
        #endregion

        #region ElementDefines
        private const int STLV_INVALID_HANDLE = -1;
        private const int STLV_PACKET_MAX_BODY_SIZE = 240;
        private const int STLV_HEAD_SIZE = 4;
        private const int STLV_PACKET_MAX_SIZE = (STLV_PACKET_MAX_BODY_SIZE + STLV_HEAD_SIZE);
        private const int MAX_ELEMENT_NESTED_LAYER = 4;
        private const int MIN_ELEMENT_SIZE = 2;
        private const int MAX_ELEMENT_TYPE_SIZE = 3;
        private const int MAX_ELEMENT_TYPE_BUFSIZE = (MAX_ELEMENT_TYPE_SIZE + 1);
        private const int HEADFIELD_VERSION = 0;
        private const int HEADFIELD_FLAG = 1;
        private const int HEADFIELD_BODY_LENGTH = 2;
        private const int HEADFIELD_SEQUENCE = 3;

        private string elementTypeEcho = "E";
        private string elementTypeClock = "C";

        public const string elementTypeMsgSMS = "MS";
        public const string elementTypeMsgFB = "MF";
        public const string elementTypeMsgTWI = "MT";

        private string msgSubTypeIdentity = "i";
        private string msgSubTypeMessage = "d";

        private const int ELEMENT_TYPE_CLOCK = 'C';
        private const int ELEMENT_TYPE_ECHO = 'E';
        private const int ELEMENT_TYPE_SPORT_HEARTBEAT = 'H';
        private const int ELEMENT_TYPE_GET_FILE = 'G';
        private const int ELEMENT_TYPE_GET_DATA = 'A';
        private const int ELEMENT_TYPE_LIST_FILES = 'L';
        private const int ELEMENT_TYPE_REMOVE_FILE = 'X';
        private const int ELEMENT_TYPE_SPORTS_DATA = 'A';
        private const int SUB_TYPE_SPORTS_DATA_ID = 'i';
        private const int SUB_TYPE_SPORTS_DATA_DATA = 'd';
        private const int SUB_TYPE_SPORTS_DATA_FLAG = 'f';

        private const int ELEMENT_TYPE_GET_GRID = 'R';
        private const int ELEMENT_TYPE_SN = 'S';
        private const int ELEMENT_TYPE_WATCHFACE = 'W';
        private const int ELEMENT_TYPE_SPORTS_GRID = 'R';

        private const int ELEMENT_TYPE_GESTURE_CONTROL = 'D';
        private const int ELEMENT_TYPE_WATCHCONFIG = 'P';

        private const int ELEMENT_TYPE_ALARM = 'I';
        private const int SUB_TYPE_ALARM_OPERATION = 'o';
        private const int SUB_TYPE_ALARM_VALUE = 'd';

        private const int ELEMENT_TYPE_FILE = 'F';
        private const int SUB_TYPE_FILE_NAME = 'n';
        private const int SUB_TYPE_FILE_DATA = 'd';
        private const int SUB_TYPE_FILE_END = 'e';

        private const int ELEMENT_TYPE_MESSAGE = 'M';
        private const int ELEMENT_TYPE_MESSAGE_SMS = 'S';
        private const int ELEMENT_TYPE_MESSAGE_FB = 'F';
        private const int ELEMENT_TYPE_MESSAGE_TW = 'T';
        private const int ELEMENT_TYPE_MESSAGE_WEATHER = 'W';
        private const int ELEMENT_TYPE_MESSAGE_BATTERY = 'B';
        private const int ELEMENT_TYPE_MESSAGE_CALL = 'C';
        private const int ELEMENT_TYPE_MESSAGE_REMINDER = 'R';
        private const int ELEMENT_TYPE_MESSAGE_RANGE = 'L';
        private const int SUB_TYPE_MESSAGE_IDENTITY = 'i';
        private const int SUB_TYPE_MESSAGE_MESSAGE = 'd';

        private const int ELEMENT_TYPE_ACTIVITY = 'Z';
        private const int SUB_TYPE_ACTIVITY_UTC = 't';
        private const int SUB_TYPE_ACTIVITY_LAT = 'l';
        private const int SUB_TYPE_ACTIVITY_LON = 'n';
        private const int SUB_TYPE_ACTIVITY_ALT = 'a';
        private const int SUB_TYPE_ACTIVITY_SPD = 's';
        private const int SUB_TYPE_ACTIVITY_DIS = 'd';
        private const int SUB_TYPE_ACTIVITY_HRT = 'h';
        private const int SUB_TYPE_ACTIVITY_CAL = 'c';
        private const int SUB_TYPE_ACTIVITY_ID = 'i';
        private const int ELEMENT_TYPE_FIRMWARE_VERSION = 'V';
        private const int ELEMENT_TYPE_ACTIVITY_DATA = 'N';
        private const int ELEMENT_TYPE_UNLOCK_WATCH = 'U';

        private const int ELEMENT_TYPE_DAILY_ACTIVITY = '0';
        private const int SUB_TYPE_TODAY_ATIME = '1';
        private const int SUB_TYPE_TODAY_STEPS = '2';
        private const int SUB_TYPE_TODAY_CAL = '3';
        private const int SUB_TYPE_TODAY_DIST = '4';
        #endregion

        #region Constants

        private const int maxBodySize = 200;
        private const int headSize = 4;
        private const int elementHeadSize = 2;
        private const byte continueElementTypeMarker = (byte)0x80;

        private const int headVersion = 0;
        private const int headFlag = 1;
        private const int bodyLength = 2;
        private const int packSequence = 3;

        private const int elementType = 0;
        private const int elementLength = 1;
        #endregion

        #region CommonMembers
        private BluetoothAgent btAgent;

        public Protocol(BluetoothAgent btAgent)
        {
            this.btAgent = btAgent;
        }
        #endregion

        #region ElementBuilder

        private int buildPacketHead(byte[] packet)
        {
            byte[] header = packet;
            header[headVersion] = (byte)0x01;
            header[headFlag] = (byte)0x80;
            header[bodyLength] = (byte)(packet.Length - headSize);
            header[packSequence] = (byte)0x00;
            return headSize;
        }

        private int buildElementHead(byte[] packet, int offset, string type, byte len)
        {
            byte[] typeBuffer = System.Text.Encoding.UTF8.GetBytes(type);
            for (int i = 0; i < typeBuffer.Length; ++i)
            {
                if (i != typeBuffer.Length - 1)
                {
                    packet[offset + i] = (byte)(typeBuffer[i] | continueElementTypeMarker);
                }
                else
                {
                    packet[offset + i] = typeBuffer[i];
                }
            }

            packet[offset + typeBuffer.Length] = len;
            return typeBuffer.Length + 1;
        }

        private int buildElement(byte[] packet, int offset, string type, byte[] data)
        {
            int cursor = offset;
            cursor += buildElementHead(packet, cursor, type, (byte)data.Length);
            cursor += fillElementData(packet, cursor, data, 0, data.Length);
            return cursor - offset;
        }

        private int fillElementData(byte[] packet, int offset, byte[] data, int from, int len)
        {
            int count = 0;
            for (int i = offset; count < len && i < packet.Length; ++count, ++i)
            {
                packet[i] = data[from + count];
            }
            return count;
        }

        private int estimateElementHeadSize(string type)
        {
            return estimateStringSize(type) + 1;
        }

        private int estimateStringSize(string type)
        {
            byte[] typeBuffer = System.Text.Encoding.UTF8.GetBytes(type);
            return typeBuffer.Length;
        }

        private byte[] buildSingleElementPacket(string type, byte[] data)
        {
            int packetSize = headSize + estimateElementHeadSize(type) + data.Length;
            int buildOffset = 0;
            byte[] packet = new byte[packetSize];
            buildOffset = buildPacketHead(packet);
            buildOffset += buildElementHead(packet, buildOffset, type, (byte)data.Length);
            buildOffset += fillElementData(packet, buildOffset, data, 0, data.Length);
            return packet;
        }
        #endregion

        #region SendPacket
        public void echo(string hint)
        {
            byte[] data = Encoding.UTF8.GetBytes(hint);
            byte[] packet = buildSingleElementPacket(elementTypeEcho, data);
            btAgent.SendBytes(packet);
        }

        public void readFile(string filename)
        {
            byte[] data = Encoding.UTF8.GetBytes(filename);
            byte[] packet = buildSingleElementPacket("G", data);
            btAgent.SendBytes(packet);
        }

        public void getSportsData()
        {
            byte[] data = Encoding.UTF8.GetBytes("dummy");
            byte[] packet = buildSingleElementPacket("A", data);
            btAgent.SendBytes(packet);
        }

        public void getSportsGrid()
        {
            byte[] data = Encoding.UTF8.GetBytes("dummy");
            byte[] packet = buildSingleElementPacket("R", data);
            btAgent.SendBytes(packet);
        }

        public void syncTime()
        {
            byte[] packet = buildSyncTimePack();
            btAgent.SendBytes(packet);
        }

        public byte[] buildSyncTimePack()
        {
            var now = DateTime.Now;
            byte[] data = new byte[8];
            data[0] = (byte)(now.Year % 100);
            data[1] = (byte)now.Month;
            data[2] = (byte)now.Day;
            data[3] = (byte)now.Hour;
            data[4] = (byte)now.Minute;
            data[5] = (byte)now.Second;
            data[6] = (byte)0x02; //windows phone
            data[7] = (byte)8;
            byte[] packet = buildSingleElementPacket(elementTypeClock, data);
            return packet;
        }

        public void notifyMessage(string type, string identity, string message)
        {

            byte[] identityData = Encoding.UTF8.GetBytes(identity);
            byte[] messageData = Encoding.UTF8.GetBytes(message);

            int elementDataLength =
                    estimateElementHeadSize(msgSubTypeIdentity) +
                    identityData.Length +
                    estimateElementHeadSize(msgSubTypeIdentity) +
                    messageData.Length;

            int packetLength = headSize + estimateElementHeadSize(type) + elementDataLength;
            if (packetLength > 240)
            {
                //Log.e("Protocol", "Notification Body size exceed max size");
                return;
            }

            int cursor = 0;
            byte[] packet = new byte[packetLength];
            cursor += buildPacketHead(packet);
            cursor += buildElementHead(packet, cursor, type, (byte)elementDataLength);
            cursor += buildElement(packet, cursor, msgSubTypeIdentity, identityData);
            cursor += buildElement(packet, cursor, msgSubTypeMessage, messageData);
            btAgent.SendBytes(packet);

        }

        public void getDeviceID()
        {
            byte[] data = Encoding.UTF8.GetBytes("dummy");
            byte[] packet = buildSingleElementPacket("S", data);
            btAgent.SendBytes(packet);
        }

        public void activityHeartbeat(string activityId)
        {
            byte[] data = Encoding.UTF8.GetBytes(activityId);
            byte[] packet = buildSingleElementPacket("H", data);
            btAgent.SendBytes(packet);
        }

        public bool sendStream(string name, StreamReader sr)
        {
            var buffer = Encoding.UTF8.GetBytes(sr.ReadToEnd());
            // var data = sr.ReadToEnd();
            // var buffer = new byte[data.Length];

            // int count = 0;
            // foreach (var i in data)
            // {
            //     buffer[count++] = (byte)i;
            // }

            return sendFileBuffer(name, buffer, buffer.Count());
        }

        public bool sendStream(string name, Stream stream)
        {
            byte[] buffer = new byte[500 * 1024]; //max 500k file
            int streamLen = 0;

            while (true)
            {
                byte[] temp = new byte[1024];
                try
                {
                    int bytesRead = stream.Read(buffer, streamLen, buffer.Length - streamLen);
                    if (bytesRead > 0)
                    {
                        streamLen += bytesRead;
                    }
                    else
                    {
                        break;
                    }
                }
                catch (IOException)
                {
                    return false;
                }
            }

            return sendFileBuffer(name, buffer, streamLen);
        }

        private bool sendFileBuffer(string fileName, byte[] buffer, int fileLen)
        {
            int packetSize = 200;
            string fileEndFlag = "\0";
            List<JobDesc> jobList = new List<JobDesc>();
            int sendPos = 0;
            while (sendPos < fileLen)
            {

                int toSentSize = fileLen - sendPos > packetSize ? packetSize : fileLen - sendPos;
                int elementDataLength = 2 + toSentSize;

                var fileNameBuf = Encoding.UTF8.GetBytes(fileName);
                if (sendPos == 0)
                {
                    //send file begin
                    elementDataLength += (2 + fileNameBuf.Length);
                }

                var fileEndFlagBuf = Encoding.UTF8.GetBytes(fileEndFlag);
                if (sendPos + packetSize >= fileLen)
                {
                    //send file end
                    elementDataLength += (2 + fileEndFlagBuf.Length);
                }

                int packetLength = headSize + 2 + elementDataLength;

                int cursor = 0;
                byte[] packet = new byte[packetLength];
                cursor += buildPacketHead(packet);
                cursor += buildElementHead(packet, cursor, "F", (byte)elementDataLength);
                if (sendPos == 0)
                {
                    //send file begin
                    cursor += buildElement(packet, cursor, "n", fileNameBuf);
                }

                byte[] temp = new byte[toSentSize];
                Array.Copy(buffer, sendPos, temp, 0, toSentSize);
                cursor += buildElement(packet, cursor, "d", temp);
                if (sendPos + packetSize >= fileLen)
                {
                    //send file end
                    cursor += buildElement(packet, cursor, "e", fileEndFlagBuf);
                }

                jobList.Add(new JobDesc(packet));

                sendPos += toSentSize;
            }

            btAgent.BatchRegisterJob(jobList);

            return true;
        }

        public void deleteFile(string fileName)
        {
            byte[] data = Encoding.UTF8.GetBytes(fileName);
            byte[] packet = buildSingleElementPacket("X", data);
            btAgent.SendBytes(packet);
        }

        public void getActivityData()
        {
            byte[] data = new byte[1];
            data[0] = (byte)'N';
            byte[] packet = buildSingleElementPacket("N", data);
            btAgent.SendBytes(packet);
        }

        public void listFile(string prefix)
        {
            byte[] data = Encoding.UTF8.GetBytes(prefix);
            byte[] packet = buildSingleElementPacket("X", data);
            btAgent.SendBytes(packet);
        }

        public void setWatchAlarm(
            int index,
            int mode,
            int monthday,
            int weekday,
            int hour,
            int minute
            )
        {
            byte[] data = new byte[6];
            data[0] = (byte)index;
            data[1] = (byte)mode;
            data[2] = (byte)monthday;
            data[3] = (byte)weekday;
            data[4] = (byte)hour;
            data[5] = (byte)minute;
            byte[] packet = buildSingleElementPacket("I", data);
            btAgent.SendBytes(packet);
        }

        public void setGestureControl(bool enable, bool isLeftHand, int[] action_map)
        {
            byte[] data = new byte[1];
            if (enable)
            {
                data[0] = 0x01;
            }
            if (isLeftHand)
            {
                data[0] |= 0x02;
            }
            byte[] packet = buildSingleElementPacket("D", data);
            btAgent.SendBytes(packet);
        }

        public void setWatchGrid()
        {
            //TODO
        }

        public void syncWatchConfig(
                 String[] worldClocks,
                 int[] worldClockOffset,
                 bool isDigitalClock,
                 int digitalClock,
                 int analogClock,
                 int sportsGrid,
                 int[] sportsGrids,
                 int[] goals,
                 int weight,
                 int height,
                 bool enableGesture,
                 bool isLeftHandGesture,
                 int[] gestureActionsTable,
                 bool isUkUnit)
        {
            byte[] data = buildWatchConfig(
                    worldClocks,
                    worldClockOffset,
                    isDigitalClock,
                    digitalClock,
                    analogClock,
                    sportsGrid,
                    sportsGrids,
                    goals,
                    weight,
                    height,
                    enableGesture,
                    isLeftHandGesture,
                    gestureActionsTable,
                    isUkUnit);

            byte[] packet = buildSingleElementPacket("P", data);
            btAgent.SendBytes(packet);
        }

        private static byte[] confSignature = new byte[] { 0xfa, 0xce, 0x00, 0x01 };
        public byte[] buildWatchConfig(
                 String[] worldClocks,
                 int[] worldClockOffset,
                 bool isDigitalClock,
                 int digitalClock,
                 int analogClock,
                 int sportsGrid,
                 int[] sportsGrids,
                 int[] goals,
                 int weight,
                 int height,
                 bool enableGesture,
                 bool isLeftHandGesture,
                 int[] gestureActionsTable,
                 bool isUkUnit
                )
        {

            byte[] placeholder = new byte[1];
            placeholder[0] = (byte)0xab;

            byte[] signatureBytes = confSignature;
            byte[] worldClocksTable = new byte[6 * 10];
            for (int i = 0; i < 6; ++i)
            {
                byte[] worldClockName = Encoding.UTF8.GetBytes(worldClocks[i]);
                int size = worldClockName.Length < 10 ? worldClockName.Length : 10;
                Array.Copy(worldClockName, 0, worldClocksTable, i * 10, size);
            }

            byte[] worldClocksOffsetTable = new byte[6];
            for (int i = 0; i < 6; ++i)
            {
                worldClocksOffsetTable[i] = (byte)worldClockOffset[i];
            }

            byte[] clockSettings = new byte[3];
            clockSettings[0] = (byte)(isDigitalClock ? 0x01 : 0x00);
            clockSettings[1] = (byte)digitalClock;
            clockSettings[2] = (byte)analogClock;

            byte[] grid = new byte[1];
            grid[0] = (byte)sportsGrid;

            byte[] gridTable = new byte[5];
            for (int i = 0; i < 5; ++i)
            {
                gridTable[i] = (byte)sportsGrids[i];
            }

            byte[] profile = new byte[3];
            profile[0] = (byte)weight;
            profile[1] = (byte)height;
            profile[2] = (byte)80; //TODO

            byte[] goalSteps = shortToByteArray((short)goals[0]);
            byte[] goalCalories = shortToByteArray((short)goals[1]);
            byte[] goalDistance = shortToByteArray((short)goals[2]);

            byte[] gestureFlag = new byte[1];
            gestureFlag[0] = 0;
            if (enableGesture) gestureFlag[0] |= 0x01;
            if (isLeftHandGesture) gestureFlag[0] |= 0x2;

            byte[] gestureActions = new byte[4];
            for (int i = 0; i < 4; ++i)
            {
                gestureActions[i] = (byte)(gestureActionsTable[i]);
            }

            byte[] lapLenBuf = shortToByteArray((short)400);


            byte[] isUkUnitBuf = new byte[1];
            isUkUnitBuf[0] = (byte)(isUkUnit ? 0x01 : 0x00);

            byte[] data = new byte[
                                   placeholder.Length +
                                   signatureBytes.Length +
                                   worldClocksTable.Length +
                                   worldClocksOffsetTable.Length +
                                   clockSettings.Length +
                                   grid.Length +
                                   gridTable.Length +
                                   goalSteps.Length +
                                   goalCalories.Length +
                                   goalDistance.Length +
                                   profile.Length +
                                   gestureFlag.Length +
                                   gestureActions.Length +
                                   lapLenBuf.Length +
                                   isUkUnitBuf.Length +
                                   1
                                   ];
            int cursor = 0;

            Array.Copy(placeholder, 0, data, cursor, placeholder.Length);
            cursor += placeholder.Length;

            Array.Copy(signatureBytes, 0, data, cursor, signatureBytes.Length);
            cursor += signatureBytes.Length;

            Array.Copy(goalSteps, 0, data, cursor, goalSteps.Length);
            cursor += goalSteps.Length;

            Array.Copy(goalCalories, 0, data, cursor, goalCalories.Length);
            cursor += goalCalories.Length;

            Array.Copy(goalDistance, 0, data, cursor, goalDistance.Length);
            cursor += goalDistance.Length;

            Array.Copy(lapLenBuf, 0, data, cursor, lapLenBuf.Length);
            cursor += lapLenBuf.Length;

            Array.Copy(worldClocksTable, 0, data, cursor, worldClocksTable.Length);
            cursor += worldClocksTable.Length;

            Array.Copy(worldClocksOffsetTable, 0, data, cursor, worldClocksOffsetTable.Length);
            cursor += worldClocksOffsetTable.Length;

            Array.Copy(clockSettings, 0, data, cursor, clockSettings.Length);
            cursor += clockSettings.Length;

            Array.Copy(grid, 0, data, cursor, grid.Length);
            cursor += grid.Length;

            Array.Copy(gridTable, 0, data, cursor, gridTable.Length);
            cursor += gridTable.Length;

            Array.Copy(isUkUnitBuf, 0, data, cursor, isUkUnitBuf.Length);
            cursor += isUkUnitBuf.Length;

            Array.Copy(profile, 0, data, cursor, profile.Length);
            cursor += profile.Length;

            Array.Copy(gestureFlag, 0, data, cursor, gestureFlag.Length);
            cursor += gestureFlag.Length;

            Array.Copy(gestureActions, 0, data, cursor, gestureActions.Length);
            cursor += gestureActions.Length;

            return data;
        }

        public static byte[] longToByteArray(long s)
        {
            byte[] targets = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                int offset = (targets.Length - 1 - i) * 8;
                targets[i] = (byte)((s >> offset) & 0xff);
            }
            return targets;
        }

        public static byte[] intToByteArray(int i)
        {
            byte[] result = new byte[4];
            result[0] = (byte)((i >> 24) & 0xFF);
            result[1] = (byte)((i >> 16) & 0xFF);
            result[2] = (byte)((i >> 8) & 0xFF);
            result[3] = (byte)((i >> 0) & 0xFF);
            return result;
        }

        public static byte[] shortToByteArray(short s)
        {
            byte[] targets = new byte[2];
            targets[0] = (byte)((s >> 8) & 0xff);
            targets[1] = (byte)((s >> 0) & 0xff);
            return targets;
        }

        public void unlockWatch()
        {
            byte[] data = new byte[1];
            data[0] = 1;
            byte[] packet = buildSingleElementPacket("U", data);
            btAgent.SendBytes(packet);
        }

        public void sendGPSInfo(short spd, short alt, int distance, int calories)
        {
            byte[] spdBuf = shortToByteArray(spd);
            byte[] altBuf = shortToByteArray(alt);
            byte[] disBuf = intToByteArray(distance);
            //byte[] calBuf = intToByteArray(calories);

            int elementDataLength = 0;
            elementDataLength += (2 + spdBuf.Length);
            elementDataLength += (2 + altBuf.Length);
            elementDataLength += (2 + disBuf.Length);
            //elementDataLength += (2 + calBuf.Length);

            int packetLength = headSize + estimateElementHeadSize("Z") + elementDataLength;

            int cursor = 0;
            byte[] packet = new byte[packetLength];
            cursor += buildPacketHead(packet);
            cursor += buildElementHead(packet, cursor, "Z", (byte)elementDataLength);
            cursor += buildElement(packet, cursor, "s", spdBuf);
            cursor += buildElement(packet, cursor, "a", altBuf);
            cursor += buildElement(packet, cursor, "d", disBuf);
            //cursor += buildElement(packet, cursor, "c", calBuf);
            btAgent.SendBytes(packet);

        }

        public void sendGPSInfo(Geoposition geoPosition, int distance, int calories)
        {
            var location = geoPosition.Coordinate;
            short spd = (short)(location.Speed * 100);
            short alt = (short)location.Altitude;

            this.sendGPSInfo(spd, alt, distance, calories);
        }
        #endregion

        #region ReadPacket
        private Stream fileStream = null;
        private string writingFileName = "";
        public bool readingFile { get; set; }
        private List<byte[]> fileRawData = new List<byte[]>();


        public async void handle_file(byte[] pack, int handle)
        {

            int element = get_first_sub_element(pack, handle);
            while (element != -1)
            {
                byte[] type_buf = get_element_type(pack, element);
                switch ((int)type_buf[0])
                {
                    case SUB_TYPE_FILE_NAME:
                        {
                            readingFile = true;
                            byte[] file_name_data = get_element_data(pack, element);
                            string file_name = Encoding.UTF8.GetString(file_name_data, 0, file_name_data.Length);

                            if (writingFileName != file_name && fileStream != null)
                            {
                                //flush old file
                                try
                                {
                                    await fileStream.FlushAsync();
                                }
                                catch (IOException)
                                {
                                }

                                /*
                                fileStream.Dispose();
                                fileStream = null;
                                */
                            }

                            writingFileName = file_name;

                            /*
                            if (fileStream == null)
                            {
                                // StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
                                // StorageFile storageFile = await storageFolder.CreateFileAsync(this.writingFileName, CreationCollisionOption.ReplaceExisting);
                                // fileStream = await storageFile.OpenStreamForWriteAsync();

                                StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                                var dataFolder = await local.CreateFolderAsync("DataFolder", CreationCollisionOption.OpenIfExists);
                                var file = await dataFolder.CreateFileAsync("DataFile.txt", CreationCollisionOption.ReplaceExisting);
                                fileStream = await file.OpenStreamForWriteAsync();

                            }
                            */
                        }
                        break;

                    case SUB_TYPE_FILE_DATA:
                        {
                            byte[] file_data = get_element_data(pack, element);
                            
                            /*
                            if (fileStream == null)
                                return;

                            try
                            {
                                fileStream.Write(file_data, 0, file_data.Length);
                                fileStream.Flush();
                            }
                            catch (IOException)
                            {
                            }
                            */ 

                            fileRawData.Add(file_data);

                        }
                        break;

                    case SUB_TYPE_FILE_END:

                        /*
                        if (fileStream == null)
                            return;
                       


                        //flush old file
                        try
                        {
                            await fileStream.FlushAsync();
                        }
                        catch (IOException)
                        {

                        }
                         */ 

                        byte[] buffer = buildDataBuffer(fileRawData);
                        ActivityDataDoc doc = LoadDataFromBuffer(buffer);


                        // if (this.OnFileReceived != null)
                        //   this.OnFileReceived(writingFileName);

                        if (this.OnOverallActivitiesReceived != null)
                            this.OnOverallActivitiesReceived(doc);

                        /*
                        fileStream.Dispose();
                        fileStream = null;
                         */
                        readingFile = false;
                        writingFileName = "";

                        break;
                }
                element = get_next_sub_element(pack, handle, element);
            }

        }

        public void handle_activity(byte[] pack, int handle)
        {

            int activityId = 0;
            byte[] sportsData = null;
            int msgId =  5;

            int element = get_first_sub_element(pack, handle);

            SportsDataRow sportsDataRow = null;
            int sports_type = 0;

            while (element != -1)
            {
                byte[] type_buf = get_element_type(pack, element);
                
                switch (type_buf[0])
                {
                    case SUB_TYPE_SPORTS_DATA_ID:
                        {
                            byte[] buf = get_element_data(pack, element);
                            activityId = (int)buf[0];
                        }
                        break;

                    case SUB_TYPE_SPORTS_DATA_DATA:
                        {
                            sportsData = get_element_data(pack, element);
                            sportsDataRow = SportsDataRow.loadFromBuffer(sportsData);
                        }
                        break;

                    case SUB_TYPE_SPORTS_DATA_FLAG:
                        {
                            byte[] buf = get_element_data(pack, element);
                            if ((buf[0] & 0x02) != 0)
                            {
                                msgId = 1;  //    msgId = MessageID.MSG_ACTIVITY_DATA_END;
                            }
                            else if ((buf[0] & 0x04) != 0)
                            {
                                msgId = 2;  //    msgId = MessageID.MSG_ACTIVITY_PREPARE;
                            }
                            else if ((buf[0] & 0x08) != 0)
                            {
                                msgId = 3;  //    msgId = MessageID.MSG_ACTIVITY_SYNC;
                            }
                            
                            if ((buf[0] & 0x10) != 0)
                            {
                                sports_type = (int)SportsDataRow.DataType.SPORTS_MODE_BIKING;
                            }
                            else if ((buf[0] & 0x20) != 0)
                            {
                                sports_type = (int)SportsDataRow.DataType.SPORTS_MODE_RUNNING;
                            }
                        }
                        break;
                }
                element = get_next_sub_element(pack, handle, element);
            }

            if (sportsDataRow != null)
            {
                sportsDataRow.sports_mode = sports_type;
            }

            switch (msgId)
            {
                case 0:
                    if (this.OnActivityDataRead != null)
                        this.OnActivityDataRead(sportsData);
                    break;
                case 1:
                    if (this.OnActivityDataEnd != null)
                        this.OnActivityDataEnd(sportsData);
                    break;
                case 2:
                    if (this.OnActivityDataPrepared != null)
                        this.OnActivityDataPrepared(sportsData);
                    break;
                case 3:
                    if (this.OnActivityDataSync != null)
                        this.OnActivityDataSync(sportsData);
                    break;
                case 5:
                    if (this.OnSportsDataReceived != null)
                        this.OnSportsDataReceived(sportsDataRow);
                    break; 
            }
        }

        public void handlePacket(byte[] packet)
        {
            byte[] pack = packet;

            int handle = get_first_element(packet);
            while (handle != -1)
            {
                byte[] type_buf = get_element_type(pack, handle);

                if (this.OnElementParsed != null)
                {
                    var element = new ElementDesc()
                    {
                        ElementType = (int)type_buf[0],
                        Data = get_element_data(pack, handle),
                    };
                    this.OnElementParsed(element);
                }

                switch ((int)type_buf[0])
                {

                    case ELEMENT_TYPE_FILE:
                        handle_file(pack, handle);
                        break;

                    case ELEMENT_TYPE_GET_DATA:
                        {
                            byte[] data_buf = get_element_data(pack, handle);
                            handle_activity(pack, handle);
                        }
                        break;

                    case ELEMENT_TYPE_GET_GRID:
                        {
                            byte[] data_buf = get_element_data(pack, handle);
                            if (this.OnSportsGridRead != null)
                                this.OnSportsGridRead(data_buf);
                        }
                        break;

                    case ELEMENT_TYPE_SN:
                        {
                            byte[] data_buf = get_element_data(pack, handle);
                            string deviceId = Encoding.UTF8.GetString(data_buf, 0, data_buf.Length);
                            if (this.OnDeviceIDRead != null)
                                this.OnDeviceIDRead(deviceId);
                        }
                        break;
                    case ELEMENT_TYPE_FIRMWARE_VERSION:
                        {
                            if (this.OnFirmwareVersionRequest != null) 
                            {
                                byte[] data_buf = get_element_data(pack, handle);
                                string firmwareVersion = Encoding.UTF8.GetString(data_buf, 0, data_buf.Length);
                                this.OnFirmwareVersionRequest(firmwareVersion);
                            }
                                
                        }
                        break;

                    case ELEMENT_TYPE_DAILY_ACTIVITY:
                        {
                            handle_today_activity(pack, handle);
                        }
                        break;
                }

                handle = get_next_element(pack, handle);

            }

        }

        #endregion

        #region ActivityDataParse
        private ActivityDataRow.DataType[] dataTypeMap = new ActivityDataRow.DataType[]
        {
            ActivityDataRow.DataType.DATA_COL_INVALID,
            ActivityDataRow.DataType.DATA_COL_STEP,
            ActivityDataRow.DataType.DATA_COL_DIST,
            ActivityDataRow.DataType.DATA_COL_CALS,
            ActivityDataRow.DataType.DATA_COL_CADN,
            ActivityDataRow.DataType.DATA_COL_HR,
        };

        private double GetValue(int rawValue, ActivityDataRow.DataType dtype)
        {

            double value = 0;
            switch (dtype)
            {
                case ActivityDataRow.DataType.DATA_COL_STEP:
                case ActivityDataRow.DataType.DATA_COL_CADN:
                case ActivityDataRow.DataType.DATA_COL_HR:
                    value = rawValue;
                    break;
                case ActivityDataRow.DataType.DATA_COL_DIST:
                case ActivityDataRow.DataType.DATA_COL_CALS:
                    value = rawValue % 100;
                    value /= 100.0;
                    value += rawValue / 100;
                    break;
            }
            return value;
        }

        private ActivityDataDoc LoadData(DataReader reader)
        {
            //read signature
            int signature = reader.ReadInt32();

            //read head
            ActivityDataDoc ret = new ActivityDataDoc();

            ret.version = (int)reader.ReadByte();
            ret.year = (int)reader.ReadByte();
            ret.month = (int)reader.ReadByte();
            ret.day = (int)reader.ReadByte();

            //read data
            while (reader.UnconsumedBufferLength > 0)
            {
                var row = new ActivityDataRow();

                //row head
                row.mode = reader.ReadByte();
                row.hour = reader.ReadByte();
                row.minute = reader.ReadByte();

                //row meta
                int size = (int)reader.ReadByte();
                byte[] meta = new byte[4];
                reader.ReadBytes(meta);

                //row data
                for (int i = 0, j = 0; meta[i] != 0 && i < 4 && j < size; ++i)
                {
                    int lvalue = meta[i] >> 4;
                    int rvalue = meta[i] & 0x0f;

                    if (j < size)
                    {
                        int rawValue = reader.ReadInt32();
                        ActivityDataRow.DataType dtype = dataTypeMap[lvalue];
                        double value = GetValue(rawValue, dtype);
                        row.data.Add(dtype, value);
                        j++;
                    }

                    if (j < size)
                    {
                        int rawValue = reader.ReadInt32();
                        ActivityDataRow.DataType dtype = dataTypeMap[rvalue];
                        double value = GetValue(rawValue, dtype);
                        row.data.Add(dtype, value);
                        j++;
                    }
                }
                ret.data.Add(row);

            }
            return ret;
        }

        public async Task<ActivityDataDoc> LoadDataFile(String fileName)
        {

            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            if (local != null)
            {
                // Get the DataFolder folder.
                StorageFolder dataFolder = await local.GetFolderAsync("DataFolder");

                // Get the file.
                StorageFile storageFile = await dataFolder.GetFileAsync("DataFile.txt");

                if (storageFile != null)
                {
                    using (IRandomAccessStream randomStream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        using (var dataReader = new DataReader(randomStream))
                        {
                            return LoadData(dataReader);
                        }
                    }
                }
            }


            /*
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile storageFile = await storageFolder.GetFileAsync(fileName);

            if (storageFile != null)
            {
                using (IRandomAccessStream randomStream = await storageFile.OpenAsync(FileAccessMode.Read))
                {
                    using (var dataReader = new DataReader(randomStream))
                    {
                        return LoadData(dataReader);
                    }
                }
            }
             */

            return null;
        }

        #endregion

        #region PacketParseUtility
        int GET_PACKET_END(byte[] pack)
        {
            return (STLV_HEAD_SIZE + get_body_length(pack));
        }

        int get_version(byte[] pack)
        {
            return pack[HEADFIELD_VERSION];
        }

        int get_body_length(byte[] pack)
        {
            return pack[HEADFIELD_BODY_LENGTH];
        }

        int get_sequence(byte[] pack)
        {
            return pack[HEADFIELD_SEQUENCE];
        }

        int get_flag(byte[] pack)
        {
            return pack[HEADFIELD_FLAG];
        }

        int get_first_element(byte[] pack)
        {
            if (get_body_length(pack) >= MIN_ELEMENT_SIZE)
                return STLV_HEAD_SIZE;
            else
                return STLV_INVALID_HANDLE;
        }

        int get_next_element(byte[] pack, int handle)
        {
            byte[] typebuf = get_element_type(pack, handle);
            int len = get_element_data_size(pack, handle);
            if (GET_PACKET_END(pack) - (handle + len + typebuf.Length) >= MIN_ELEMENT_SIZE)
                return handle + len;
            else
                return STLV_INVALID_HANDLE;
        }

        int get_first_sub_element(byte[] pack, int parent)
        {
            byte[] elementType = get_element_type(pack, parent);
            return parent + elementType.Length + 1;
        }

        int get_next_sub_element(byte[] pack, int parent, int handle)
        {
            byte[] parentType = get_element_type(pack, parent);
            int parent_body_len = get_element_data_size(pack, parent);

            byte[] elementType = get_element_type(pack, handle);
            int element_body_len = get_element_data_size(pack, handle);

            int parent_end = parent + parentType.Length + parent_body_len;
            int element_end = handle + elementType.Length + element_body_len;

            if (parent_end - element_end < MIN_ELEMENT_SIZE)
                return STLV_INVALID_HANDLE;
            else
                return element_end + 1;

        }

        byte[] get_element_type(byte[] pack, int handle)
        {
            byte[] buf = new byte[8];
            int cursor = 0;
            int pos = handle;
            while ((pack[pos] & 0x80) != 0)
            {
                if (cursor < buf.Length)
                    buf[cursor] = (byte)(pack[pos] & ~0x80);
                pos++;
                cursor++;
            }
            if (cursor < buf.Length)
                buf[cursor] = (byte)(pack[pos] & ~0x80);

            byte[] result = new byte[cursor + 1];
            Array.Copy(buf, 0, result, 0, cursor + 1);
            return result;
        }

        int get_element_data_size(byte[] pack, int handle)
        {
            int pos = handle + get_element_type(pack, handle).Length;
            return pack[pos];
        }

        byte[] get_element_data(byte[] pack, int handle)
        {
            int pos = handle + get_element_type(pack, handle).Length;
            int size = (int)pack[pos];
            byte[] result = new byte[size];
            Array.Copy(pack, pos + 1, result, 0, size);
            return result;
        }
        #endregion

        #region New Methods Added
        private byte[] buildDataBuffer(List<byte[]> buflist)
        {
            int size = 0;
            foreach (byte[] buf in buflist)
            {
                size += buf.Length;
            }

            int cursor = 0;
            byte[] ret = new byte[size];

            foreach (byte[] buf in buflist)
            {
                Array.Copy(buf, 0, ret, cursor, buf.Length);
                cursor += buf.Length;
            }
            return ret;
        }

        private void hexdump(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; )
            {
                String line = "";
                for (int j = 0; j < 16; ++j, ++i)
                {
                    if (i >= buffer.Length)
                    {
                        break;
                    }
                    else
                    {
                        line += String.Format("%02x ", buffer[i]);
                    }
                }
            }
        }

        public static int bytesToInt(byte[] buf, int start)
        {
            int b0 = buf[start + 0];
            b0 = b0 & 0x000000ff;
            int b1 = buf[start + 1];
            b1 = b1 & 0x000000ff;
            int b2 = buf[start + 2];
            b2 = b2 & 0x000000ff;
            int b3 = buf[start + 3];
            b3 = b3 & 0x000000ff;

            // TODO: verify whether it is big-endian or little-endian
            return (b0 | b1 << 8 | b2 << 16 | b3 << 24);
        }

        public ActivityDataDoc LoadDataFromBuffer(byte[] buffer)
        {
            hexdump(buffer);

            ActivityDataDoc ret = new ActivityDataDoc();
            ret.data = new List<ActivityDataRow>();

            int cursor = 4; // jump over signature

            ret.version = ((int)buffer[cursor++]) & 0x000000ff;
            ret.year = ((int)buffer[cursor++]) & 0x000000ff;
            ret.month = ((int)buffer[cursor++]) & 0x000000ff;
            ret.day = ((int)buffer[cursor++]) & 0x000000ff;

            while (cursor + 8 < buffer.Length)
            {
                ActivityDataRow row = new ActivityDataRow();

                row.mode = buffer[cursor++];
                row.hour = buffer[cursor++];
                row.minute = buffer[cursor++];
                row.data = new Dictionary<ActivityDataRow.DataType, double>();

                int size = buffer[cursor++];
                int[] meta = new int[size];

                int meta_cursor = 0;
                int meta_cnt = 0;
                for (; meta_cnt < 4 && meta_cursor < size; ++meta_cnt)
                {
                    int _2d = (int)buffer[cursor + meta_cnt];
                    _2d = _2d & 0x000000ff;
                    int left = _2d >> 4;
                    int right = _2d & 0x0f;
                    if (left != 0)
                    {
                        meta[meta_cursor++] = left;
                    }
                    else
                    {
                        break;
                    }
                    if (right != 0)
                    {
                        meta[meta_cursor++] = right;
                    }
                    else
                    {
                        break;
                    }
                }

                if (meta_cursor != size)
                {
                    return null;
                }

                cursor += 4;

                for (int i = 0; i < size; ++i)
                {
                    int dataType = meta[i];
                    if (cursor + 4 >= buffer.Length)
                        break;

                    int rawValue = bytesToInt(buffer, cursor);
                    double value = 0;
                    switch (dataType)
                    {
                        case (int)ActivityDataRow.DataType.DATA_COL_STEP:
                        case (int)ActivityDataRow.DataType.DATA_COL_CADN:
                        case (int)ActivityDataRow.DataType.DATA_COL_HR:
                            value = rawValue;
                            break;
                        case (int)ActivityDataRow.DataType.DATA_COL_DIST:
                        case (int)ActivityDataRow.DataType.DATA_COL_CALS:
                            value = rawValue % 100;
                            value /= 100.0;
                            value += rawValue / 100;
                            break;
                    }

                    row.data.Add((ActivityDataRow.DataType)dataType, value);
                    cursor += 4;
                }

                ret.data.Add(row);
            }

            return ret;
        }

        public void sendDailyActivityRequest()
        {
            byte[] dymmData = new byte[2];
            dymmData[0] = 0;
            dymmData[1] = 0;

            byte[] packet = buildSingleElementPacket("0", dymmData);
            btAgent.SendBytes(packet);
        }

        public void handle_today_activity(byte[] pack, int handle)
        {

            TodayActivity ta = new TodayActivity();

            int element = get_first_sub_element(pack, handle);
            while (element != -1)
            {
                byte[] type_buf = get_element_type(pack, element);
                byte[] value_buf = get_element_data(pack, element);
                switch (type_buf[0])
                {
                    case SUB_TYPE_TODAY_ATIME:
                        ta.time = bytesToShort(value_buf, 0);
                        break;
                    case SUB_TYPE_TODAY_STEPS:
                        ta.steps = bytesToShort(value_buf, 0);
                        break;
                    case SUB_TYPE_TODAY_CAL:
                        ta.calories = bytesToInt(value_buf, 0) / 100;
                        break;
                    case SUB_TYPE_TODAY_DIST:
                        ta.distance = (double)bytesToInt(value_buf, 0) / 100;
                        break;
                }
                element = get_next_sub_element(pack, handle, element);
            }

            if (this.OnTodayActivityReceived != null)
            {
                OnTodayActivityReceived(ta);
            }
        }

        public static int bytesToShort(byte[] buf, int start)
        {
            int b0 = buf[start + 0];
            b0 = b0 & 0x00ff;
            int b1 = buf[start + 1];
            b1 = b1 & 0x00ff;

            // TODO: verify whether it is big-endian or little-endian
            return (b0 | b1 << 8);
        }

        public void syncTimeFromInput(DateTime p_date)
        {
            byte[] packet = buildSyncTimePackFromInput(p_date);
            btAgent.SendBytes(packet);
        }

        public byte[] buildSyncTimePackFromInput(DateTime p_date)
        {

            byte[] data = new byte[8];
            data[0] = (byte)(p_date.Year % 100);
            data[1] = (byte)(p_date.Month - 1);
            data[2] = (byte)(p_date.Day);
            data[3] = (byte)(p_date.Hour);
            data[4] = (byte)(p_date.Minute);
            data[5] = (byte)(p_date.Second);
            data[6] = (byte)0x02;
            data[7] = (byte)8;
            byte[] packet = buildSingleElementPacket(elementTypeClock, data);
            return packet;
        }
        #endregion

    }
}
