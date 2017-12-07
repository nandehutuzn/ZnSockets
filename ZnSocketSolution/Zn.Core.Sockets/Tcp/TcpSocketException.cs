using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets
{
    public class TcpSocketException : Exception
    {
        public TcpSocketException(string message)
            : base(message)
        { }

        public TcpSocketException(string message, Exception innerException)
            :base(message, innerException)
        {

        }
    }
}
