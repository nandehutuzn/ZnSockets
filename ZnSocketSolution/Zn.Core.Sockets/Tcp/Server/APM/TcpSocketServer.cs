using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zn.Core.ToolHelpers;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Zn.Core.Sockets
{
    public class TcpSocketServer
    {
        #region Fields

        private LoggerHelper _log = LoggerHelper.Current;
        private TcpListener _listener;
        private readonly ConcurrentDictionary<string, TcpSocketSession> _sessions = new ConcurrentDictionary<string, TcpSocketSession>();
        private readonly object _opsLock = new object();
        private bool _isListening = false;

        #endregion

        #region Events

        public event EventHandler<TcpClientConnectedEventArgs> ClientConnected;
        public event EventHandler<TcpClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<TcpClientDataReceivedEventArgs> ClientDataReceived;

        internal void RaiseClientConnected(TcpSocketSession session)
        {
            try
            {
                ClientConnected?.Invoke(this, new TcpClientConnectedEventArgs(session));
            }
            catch (Exception ex)
            {
                HandleUserSideError(session, ex);
            }
        }

        internal void RaiseClientDisconnected(TcpSocketSession session)
        {
            try
            {
                ClientDisconnected?.Invoke(this, new TcpClientDisconnectedEventArgs(session));
            }
            catch (Exception ex)
            {
                HandleUserSideError(session, ex);
            }
            finally
            {
                TcpSocketSession sessionToBeThrowAway;
                _sessions.TryRemove(session.SessionKey, out sessionToBeThrowAway);
            }
        }

        internal void RaiseClientDataReceived(TcpSocketSession session, byte[] data, int dataOffset, int dataLength)
        {
            try
            {
                ClientDataReceived?.Invoke(this, new TcpClientDataReceivedEventArgs(session,
                    data, dataOffset, dataLength));
            }
            catch (Exception ex)
            {
                HandleUserSideError(session, ex);
            }
        }

        private void HandleUserSideError(TcpSocketSession session, Exception ex)
        {
            _log.Exception(string.Format("Session [{0}] error occurred in user side [{1}].", session, ex.Message));
        }

        #endregion
    }
}
