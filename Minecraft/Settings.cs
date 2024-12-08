using Core.Interfaces;

namespace Minecraft;

public class Settings : ISettings
{
    public bool UseWireFrame { get; set; }
    public bool PrintFrameRate { get; set; }
}
