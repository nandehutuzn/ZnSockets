using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zn.Core.Sockets.Buffer;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Zn.Core.Sockets
{
    public sealed class TcpSocketServerConfiguration
    {
        public TcpSocketServerConfiguration()
            :this(new SegmentBufferManager(1024, 8192, 1, true))
        {

        }

        public TcpSocketServerConfiguration(ISegmentBufferManager bufferManager)
        {

        }

        public ISegmentBufferManager BufferManager { get; set; }

        public int ReceiveBufferSize { get; set; }

        public int SendBufferSize { get; set; }

        public TimeSpan ReceiveTimeout { get; set; }

        public TimeSpan SendTimeout { get; set; }

        public bool NoDelay { get; set; }

        public LingerOption LingerState { get; set; }

        public bool KeepAlive { get; set; }

        public TimeSpan KeepAliveInterval { get; set; }

        public bool ReuseAddress { get; set; }

        public int PendingConnectionBacklog { get; set; }

        public bool AllowNatTraversal { get; set; }

        public bool SslEnabled { get; set; }

        public X509Certificate2 SslServerCertificate { get; set; }

        public EncryptionPolicy SslEncryptionPolicy { get; set; }

        public SslProtocols SslEnabledProtocols { get; set; }

        public bool SslClientCertificateRequired { get; set; }

        public bool SslCheckCertificateRevocation { get; set; }

        public bool SslPolicyErrorsBypassed { get; set; }

        public TimeSpan ConnectTimeout { get; set; }

        public IFrameBuilder FrameBuilder { get; set; }
    }
}
