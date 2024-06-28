using System;
using System.Collections.Generic;
using System.Text;

namespace DriveThru.Integration.Core.Helpers
{
    public static class ConversionHelper
    {
        public static IList<Guid> ConvertToGuidList(IList<string> from)
        {
            IList<Guid> to = null;

            if (from != null && from.Count > 0)
            {
                to = new List<Guid>();

                foreach (string item in from)
                {
                    to.Add(Guid.Parse(item));
                }
            }

            return to;
        }
    }
}
