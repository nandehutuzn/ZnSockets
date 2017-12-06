using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets.Buffer
{
    public class UnableToAllocateBufferException : Exception
    {
        public UnableToAllocateBufferException()
           : base("Cannot allocate buffer after few trials.")
        {

        }
    }
}
