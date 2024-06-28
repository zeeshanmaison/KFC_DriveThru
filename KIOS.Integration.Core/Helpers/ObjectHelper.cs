using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DriveThru.Integration.Core.Helpers
{
    public static class ObjectHelper
    {
        public static byte[] GetByteArrayFromObject(object value)
        {
            return Encoding.UTF8.GetBytes(JsonHelper.Serialize(value));
        }

        public static object GetObjectFromByteArray(byte[] value)
        {
            return JsonHelper.Deserialize<object>(Encoding.UTF8.GetString(value));
        }

        public static T GetObjectFromByteArray<T>(byte[] value)
        {
            return JsonHelper.Deserialize<T>(Encoding.UTF8.GetString(value));
        }

        public static bool IsOfType<T>(object value)
        {
            return value is T;
        }

        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            IDictionary<string, T> dictionary = new Dictionary<string, T>();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                object value = property.GetValue(source);

                if (IsOfType<T>(value))
                {
                    dictionary.Add(property.Name, (T)value);
                }
            }

            return dictionary;
        }
    }
}
