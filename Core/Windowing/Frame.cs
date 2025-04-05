using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpEngine.Core.Windowing;

public class Frame
{
    public Frame(double frameTime)
    {
        FrameTime = frameTime;
    }

    public double FrameTime { get; set; }
    public float FrameRate => (float)(1 / FrameTime);
}
