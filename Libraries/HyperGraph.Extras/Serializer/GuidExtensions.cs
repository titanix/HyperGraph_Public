using System;

namespace Leger.Extra
{
	static class GuidExtensions
	{
		internal static string ToNodeName(this Guid guid)
		{
			return "_" + guid.ToString("N");
		}
	}
}
