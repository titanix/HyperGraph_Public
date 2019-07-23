namespace Leger.Extra.IO
{
    public abstract class EntityDisplaySettings
    {
        public bool DisplayIds { get; set; }
        public uint FontSize { get; set; }
        public string Color { get; set; }
    }

    public class NodeDisplaySettings : EntityDisplaySettings { }

    public class EdgeDisplaySettings : EntityDisplaySettings { }
}
