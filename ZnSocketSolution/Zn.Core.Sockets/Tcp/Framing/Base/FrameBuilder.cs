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
            Encoder = encoder ?? throw new ArgumentNullException("encoder");
            Decoder = decoder ?? throw new ArgumentNullException("decoder");
        }

        public IFrameEncoder Encoder { get; private set; }

        public IFrameDecoder Decoder { get; private set; }
    }
}
