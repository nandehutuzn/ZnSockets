using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets
{
    public interface IAsyncTcpSocketClientEventDispatcher
    {
        Task OnServerConnected(AsyncTcpSocketClient client);

        Task OnServerDataReceived(AsyncTcpSocketClient client, byte[] data, int offset, int count);

        Task OnServerDisconnected(AsyncTcpSocketClient client);
    }
}
