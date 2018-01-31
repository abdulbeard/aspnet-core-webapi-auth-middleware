using System;

namespace MiddlewareAuth.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime Epoch = new DateTime(1970, 1, 1);
        public static long TotalSecondsSinceEpoch(this DateTime date)
        {
            return (long)(date - Epoch).TotalSeconds;
        }
    }
}
