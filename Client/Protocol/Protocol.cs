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

        private byte[] type;

        private byte[] content;

        // DEL
        private byte end = 127;

        public Protocol(byte[] type, byte[] content)
        {
            this.type = type;
            this.content = content;
        }

        public Protocol(byte[] bytes)
        {
            if (bytes[0] == header[0] && bytes[1] == header[1] && bytes[2] == header[2] && bytes[3] == header[3])
            {
                this.type = new byte[] { bytes[4], bytes[5] };

                List<byte> contentBytes = new List<byte>();

                for (int i = 6; i < bytes.Length; i++)
                {
                    contentBytes.Add(bytes[i]);
                }

                this.content = contentBytes.ToArray();
            }
        }

        public byte[] ToByteArray()
        {
            List<byte> protocol = new List<byte>();

            protocol.AddRange(this.header);
            protocol.AddRange(this.type);
            protocol.AddRange(this.content);
            protocol.AddRange(new byte[] { this.end });

            return protocol.ToArray();
        }
    }
}
