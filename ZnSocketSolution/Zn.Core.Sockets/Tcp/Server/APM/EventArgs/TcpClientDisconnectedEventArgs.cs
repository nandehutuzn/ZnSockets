using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets
{
    public class TcpClientDisconnectedEventArgs : EventArgs
    {
        public TcpSocketSession Session { get; private set; }

        public TcpClientDisconnectedEventArgs(TcpSocketSession session)
        {
            if(session == null)
                throw new ArgumentNullException("session");
            Session = session;
        }

        public override string ToString()
        {
            return string.Format("{0}", this.Session);
        }
    }
}
