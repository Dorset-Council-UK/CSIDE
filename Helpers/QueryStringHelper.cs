using System.Collections;
using System.Globalization;
using System.Web;

namespace CSIDE.Helpers
{
    public static class QueryStringHelper
    {
        public static string GetQueryString(object obj)
        {
            var result = new List<string>();
            var props = obj.GetType().GetProperties().Where(p => p.GetValue(obj, null) != null);
            foreach (var p in props)
            {
                var value = p.GetValue(obj, null);
                if (value is ICollection enumerable)
                {
                    result.AddRange(from object v in enumerable select string.Format(CultureInfo.InvariantCulture, "{0}={1}", p.Name, HttpUtility.UrlEncode(v.ToString())));
                }
                else
                {
                    //TODO - Hacky way of forcing the date to convert into ISO date format
                    if (p.PropertyType == typeof(DateOnly?) && value is not null)
                    {
                        value = (value as DateOnly?)?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    }
                    result.Add(string.Format(CultureInfo.InvariantCulture, "{0}={1}", p.Name, HttpUtility.UrlEncode(value?.ToString())));
                }
            }

            return string.Join("&", result.ToArray());
        }
    }
}
