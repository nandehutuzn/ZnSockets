using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets
{
    internal class DefaultAsyncTcpSocketServerEventDispatcher : IAsyncTcpSocketServerEventDispatcher
    {
        private Func<AsyncTcpSocketSession, byte[], int, int, Task> _onSessionDataReceived;
        private Func<AsyncTcpSocketSession, Task> _onSessionStarted;
        private Func<AsyncTcpSocketSession, Task> _onSessionClosed;

        public DefaultAsyncTcpSocketServerEventDispatcher()
        {
        }

        public DefaultAsyncTcpSocketServerEventDispatcher(
            Func<AsyncTcpSocketSession, byte[], int, int, Task> onSessionDataReceived,
            Func<AsyncTcpSocketSession, Task> onSessionStarted,
            Func<AsyncTcpSocketSession, Task> onSessionClosed)
            : this()
        {
            _onSessionDataReceived = onSessionDataReceived;
            _onSessionStarted = onSessionStarted;
            _onSessionClosed = onSessionClosed;
        }

        public async Task OnSessionClosed(AsyncTcpSocketSession session)
        {
            if (_onSessionClosed != null)
                await _onSessionClosed(session);
        }

        public async Task OnSessionDataReceived(AsyncTcpSocketSession session, byte[] data, int offset, int count)
        {
            if (_onSessionDataReceived != null)
                await _onSessionDataReceived(session, data, offset, count);
        }

        public async Task OnSessionStarted(AsyncTcpSocketSession session)
        {
            if (_onSessionStarted != null)
                await _onSessionStarted(session);
        }
    }
}
