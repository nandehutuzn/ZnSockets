using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets.Buffer
{
    public interface ISegmentBufferManager
    {
        ArraySegment<byte> BorrowBuffer();

        IEnumerable<ArraySegment<byte>> BorrowBuffers(int count);

        void ReturnBuffer(ArraySegment<byte> buffer);

        void ReturnBuffers(IEnumerable<ArraySegment<byte>> buffers);

        void ReturnBuffers(params ArraySegment<byte>[] buffers);
    }
}
