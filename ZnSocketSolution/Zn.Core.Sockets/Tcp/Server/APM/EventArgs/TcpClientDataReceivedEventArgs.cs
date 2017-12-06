using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets
{
    public class TcpClientDataReceivedEventArgs : EventArgs
    {
        public TcpSocketSession Session { get; private set; }
        public byte[] Data { get; private set; }
        public int DataOffset { get; private set; }
        public int DataLength { get; private set; }

        public TcpClientDataReceivedEventArgs(TcpSocketSession session, byte[] data)
            :this(session, data, 0, data.Length)
        {

        }

        public TcpClientDataReceivedEventArgs(TcpSocketSession session, byte[] data, int dataOffset, int dataLength)
        {
            Session = session;
            Data = data;
            DataOffset = dataOffset;
            DataLength = dataLength;
        }

        public override string ToString()
        {
            return string.Format("{0}", this.Session);
        }
    }
}
