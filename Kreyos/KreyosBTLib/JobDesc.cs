
namespace Kreyos.SDK.Bluetooth
{
    public class JobDesc
    {
        public byte[] Data { get; set; }
        public bool IsQuitSignal { get; set; }

        public JobDesc(byte[] data)
        {
            this.Data = data;
            this.IsQuitSignal = false;
        }
    }
}
