using System;
using System.Collections.Generic;
using System.Text;

namespace DriveThru.Integration.Core.Helpers
{
    public static class DateTimeHelper
    {
        public static string GetIsoStandardDateTime(this DateTime? value)
        {
            string dateTime = string.Empty;

            if (value.HasValue)
            {
                dateTime = value.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            }

            return dateTime;
        }

        public static string GetIsoStandardDateTime(this DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }
    }
}
