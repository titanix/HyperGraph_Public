using System;

namespace Leger
{
    public static class GuidHelper
    {
        public static Guid? ToGuid(this string str)
        {
            Guid result;
            if (Guid.TryParse(str, out result))
            {
                return result;
            }
            return null;
        }
    }
}