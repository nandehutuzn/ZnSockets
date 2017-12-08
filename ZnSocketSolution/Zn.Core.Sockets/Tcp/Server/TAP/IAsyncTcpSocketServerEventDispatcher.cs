using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets
{
    public interface IAsyncTcpSocketServerEventDispatcher
    {
        Task OnSessionStarted(AsyncTcpSocketSession session);

        Task OnSessionDataReceived(AsyncTcpSocketSession session, byte[] data, int offset, int count);

        Task OnSessionClosed(AsyncTcpSocketSession session);
    }
}
