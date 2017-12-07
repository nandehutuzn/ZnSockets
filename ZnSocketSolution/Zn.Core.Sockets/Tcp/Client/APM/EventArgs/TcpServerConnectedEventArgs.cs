﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets
{
    public class TcpServerConnectedEventArgs : EventArgs
    {
        public TcpServerConnectedEventArgs(IPEndPoint remoteEP)
       : this(remoteEP, null)
        {
        }

        public TcpServerConnectedEventArgs(IPEndPoint remoteEP, IPEndPoint localEP)
        {
            if (remoteEP == null)
                throw new ArgumentNullException("remoteEP");

            this.RemoteEndPoint = remoteEP;
            this.LocalEndPoint = localEP;
        }

        public IPEndPoint RemoteEndPoint { get; private set; }
        public IPEndPoint LocalEndPoint { get; private set; }

        public override string ToString()
        {
            return this.RemoteEndPoint.ToString();
        }
    }
}
