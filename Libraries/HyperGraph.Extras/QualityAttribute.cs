using System;

namespace Leger.Extra
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class QualityAttribute : Attribute
	{
		public QualityLevel Level { get; set; }
	}

	[Flags]
	public enum QualityLevel
	{
		CodeIncomplete = 1,
		Untested = 2,
		ManuallyTested = 4,
		AutomatedTests = 8
	}
}
