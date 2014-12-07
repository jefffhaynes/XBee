using System.Collections.Generic;

namespace XBee.Frames
{
    public interface IRxIndicatorSampleFrame : IRxIndicatorFrame
    {
        Sample GetSample();
    }
}
