using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zn.Core.Sockets
{
    public class FrameBuilder : IFrameBuilder
    {
        public FrameBuilder(IFrameEncoder encoder, IFrameDecoder decoder)
        {
            if(encoder == null)
                throw new ArgumentNullException("encoder");
            if (decoder == null)
                throw new ArgumentNullException("decoder");

            Encoder = encoder;
            Decoder = decoder;
        }

        public IFrameEncoder Encoder { get; private set; }

        public IFrameDecoder Decoder { get; private set; }
    }
}
