namespace Server
{
    public class DataReceivedEventArgs
    {
        public DataReceivedEventArgs(byte[] data)
        {
            this.Data = data;
        }

        public byte[] Data
        {
            get;
            private set;
        }
    }
}