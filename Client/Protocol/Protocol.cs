using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Protocol
    {
        // CHAT
        private byte[] header = new byte[] { 67, 72, 65, 84};

        // DEL
        private byte end = 127;

        public Protocol(byte[] type, byte[] content)
        {
            this.Type = type;
            this.Content = content;
        }

        public byte[] Type
        {
            get;
            private set;
        }

        public byte[] Content
        {
            get;
            private set;
        }

        public Protocol(byte[] bytes)
        {
            if (bytes[0] == header[0] && bytes[1] == header[1] && bytes[2] == header[2] && bytes[3] == header[3])
            {
                this.Type = new byte[] { bytes[4], bytes[5] };

                List<byte> contentBytes = new List<byte>();

                for (int i = 6; i < bytes.Length; i++)
                {
                    contentBytes.Add(bytes[i]);
                }

                this.Content = contentBytes.ToArray();
            }
        }

        public byte[] ToByteArray()
        {
            List<byte> protocol = new List<byte>();

            protocol.AddRange(this.header);
            protocol.AddRange(this.Type);
            protocol.AddRange(this.Content);
            protocol.AddRange(new byte[] { this.end });

            return protocol.ToArray();
        }
    }
}
