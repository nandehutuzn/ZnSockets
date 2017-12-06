using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Zn.Core.Sockets.Buffer;
using Zn.Core.ToolHelpers;

namespace Zn.Core.Sockets
{
    public sealed class TcpSocketSession
    {
        #region Fields

        private static readonly LoggerHelper _log = LoggerHelper.Current;
        private TcpClient _tcpClient;
        private readonly TcpSocketServerConfiguration _configuration;
        private readonly ISegmentBufferManager _bufferManager;
        private readonly TcpSocketServer _server;
        private readonly string _sessionKey;
        private Stream _stream;
        private ArraySegment<byte> _receiveBuffer = default(ArraySegment<byte>);
        private int _receiveBufferOffset = 0;
        private IPEndPoint _remoteEndPoint;
        private IPEndPoint _localEndPoint;

        private int _state;
        private const int _none = 0;
        private const int _connecting = 1;
        private const int _connected = 2;
        private const int _disposed = 5;

        #endregion

        #region Constructors

        public TcpSocketSession(TcpClient tcpClient, TcpSocketServerConfiguration configuration
            , ISegmentBufferManager bufferManager, TcpSocketServer server)
        {
            _tcpClient = tcpClient?? throw new ArgumentNullException("tcpClient"); 
            _configuration = configuration ?? throw new ArgumentNullException("configuration"); 
            _bufferManager = bufferManager?? throw new ArgumentNullException("bufferManager"); 
            _server = server ?? throw new ArgumentNullException("server");

            _sessionKey = Guid.NewGuid().ToString();
            this.StartTime = DateTime.UtcNow;

            SetSocketOptions();

            _remoteEndPoint = this.RemoteEndPoint;
            _localEndPoint = this.LocalEndPoint;
        }

        #endregion

        #region Properties

        public string SessionKey { get { return _sessionKey; } }
        public DateTime StartTime { get; private set; }
        public TimeSpan ConnectTimeout { get { return _configuration.ConnectTimeout; } }

        private bool Connected { get { return _tcpClient != null && _tcpClient.Connected; } }
        public IPEndPoint RemoteEndPoint { get { return Connected? (IPEndPoint)_tcpClient.Client.RemoteEndPoint : _remoteEndPoint; } }
        public IPEndPoint LocalEndPoint { get { return Connected ? (IPEndPoint)_tcpClient.Client.LocalEndPoint : _localEndPoint; } }

        public Socket Socket { get { return Connected ? _tcpClient.Client : null; } }
        public Stream Stream { get { return _stream; } }
        public TcpSocketServer Server { get { return _server; } }

        public TcpSocketConnectionState State {
            get {
                switch (_state)
                {
                    case _none:
                        return TcpSocketConnectionState.None;
                    case _connecting:
                        return TcpSocketConnectionState.Connecting;
                    case _connected:
                        return TcpSocketConnectionState.Connected;
                    case _disposed:
                        return TcpSocketConnectionState.Closed;
                    default:
                        return TcpSocketConnectionState.Closed;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("SessionKey[{0}], RemoteEndPoint[{1}], LocalEndPoint[{2}]",
                this.SessionKey, this.RemoteEndPoint, this.LocalEndPoint);
        }

        #endregion

        #region Process

        internal void Start()
        {
            int origin = Interlocked.CompareExchange(ref _state, _connecting, _none);
            if (origin == _disposed)
            {
                throw new ObjectDisposedException("This tcp socket session has been disposed when connecting.");
            }
            else if (origin != _none)
            {
                throw new InvalidOperationException("This tcp socket session is in invalid state when connecting.");
            }

            try
            {


            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                Close(true);
                throw;
            }
        }

        public void Close()
        {
            Close(true);
        }

        private void Close(bool shallNotifyUserSide)
        {
            if (Interlocked.Exchange(ref _state, _disposed) == _disposed)
                return;

            Shutdown();

            if (shallNotifyUserSide)
            {
                
            }

            Clean();
        }

        public void Shutdown()
        {
            if (_tcpClient != null && _tcpClient.Connected)
            {
                _tcpClient.Client.Shutdown(SocketShutdown.Send);
            }
        }

        private void Clean()
        {
            try
            {
                _stream?.Dispose();
                _tcpClient?.Close();
            }
            catch { }
            finally
            {
                _stream = null;
                _tcpClient = null;
            }

            if (_receiveBuffer != default(ArraySegment<byte>))
                _configuration.BufferManager.ReturnBuffer(_receiveBuffer);

            _receiveBuffer = default(ArraySegment<byte>);
            _receiveBufferOffset = 0;
        }

        private void SetSocketOptions()
        {
            _tcpClient.ReceiveBufferSize = _configuration.ReceiveBufferSize;
            _tcpClient.SendBufferSize = _configuration.SendBufferSize;
            _tcpClient.ReceiveTimeout = (int)_configuration.ReceiveTimeout.TotalMilliseconds;
            _tcpClient.SendTimeout = (int)_configuration.SendTimeout.TotalMilliseconds;
            _tcpClient.NoDelay = _configuration.NoDelay;
            _tcpClient.LingerState = _configuration.LingerState;

            if (_configuration.KeepAlive)
            {
                _tcpClient.Client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.KeepAlive,
                    (int)_configuration.KeepAliveInterval.TotalMilliseconds);
            }

            _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, _configuration.ReuseAddress);
        }

        private Stream NegotiateStream(Stream stream)
        {
            if (!_configuration.SslEnabled)
                return stream;

            var validateRemoteCertificate = new RemoteCertificateValidationCallback(
                (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                =>
                {
                    if (sslPolicyErrors == SslPolicyErrors.None)
                        return true;

                    if (_configuration.SslPolicyErrorsBypassed)
                        return true;
                    else
                        _log.Exception(string.Format("Session [{0}] error occurred when validating remote certificate: [{1}], [{2}].",
                            this, this.RemoteEndPoint, sslPolicyErrors));

                    return false;
                });

            var sslStream = new SslStream(stream,
                false, validateRemoteCertificate, null, _configuration.SslEncryptionPolicy);

            IAsyncResult ar = null;
            if (!_configuration.SslClientCertificateRequired)
            {
                ar = sslStream.BeginAuthenticateAsServer(
                    _configuration.SslServerCertificate, null, _tcpClient);
            }
            else
            {
                ar = sslStream.BeginAuthenticateAsServer(
                  _configuration.SslServerCertificate, // The X509Certificate used to authenticate the server.
                  _configuration.SslClientCertificateRequired, // A Boolean value that specifies whether the client must supply a certificate for authentication.
                  _configuration.SslEnabledProtocols, // The SslProtocols value that represents the protocol used for authentication.
                  _configuration.SslCheckCertificateRevocation, // A Boolean value that specifies whether the certificate revocation list is checked during authentication.
                  null, _tcpClient);
            }
            if (!ar.AsyncWaitHandle.WaitOne(ConnectTimeout))
            {
                Close(false); // ssl negotiation timeout
                throw new TimeoutException(string.Format(
                    "Negotiate SSL/TSL with remote [{0}] timeout [{1}].", this.RemoteEndPoint, ConnectTimeout));
            }

            // When authentication succeeds, you must check the IsEncrypted and IsSigned properties 
            // to determine what security services are used by the SslStream. 
            // Check the IsMutuallyAuthenticated property to determine whether mutual authentication occurred.
            _log.Exception(string.Format(
                "Ssl Stream: SslProtocol[{0}], IsServer[{1}], IsAuthenticated[{2}], IsEncrypted[{3}], IsSigned[{4}], IsMutuallyAuthenticated[{5}], "
                + "HashAlgorithm[{6}], HashStrength[{7}], KeyExchangeAlgorithm[{8}], KeyExchangeStrength[{9}], CipherAlgorithm[{10}], CipherStrength[{11}].",
                sslStream.SslProtocol,
                sslStream.IsServer,
                sslStream.IsAuthenticated,
                sslStream.IsEncrypted,
                sslStream.IsSigned,
                sslStream.IsMutuallyAuthenticated,
                sslStream.HashAlgorithm,
                sslStream.HashStrength,
                sslStream.KeyExchangeAlgorithm,
                sslStream.KeyExchangeStrength,
                sslStream.CipherAlgorithm,
                sslStream.CipherStrength));

            return sslStream;
        }

        #endregion
    }
}
