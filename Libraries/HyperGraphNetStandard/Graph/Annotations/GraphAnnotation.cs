using System.Diagnostics;

namespace Leger
{
    [DebuggerDisplay("{Namespace}:{Key}:{Value}")]
    public class Annotation
    {
        public string Namespace { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}