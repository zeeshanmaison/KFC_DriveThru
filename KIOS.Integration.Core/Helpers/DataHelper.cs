using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;

namespace DriveThru.Integration.Core.Helpers
{
    public static class DataHelper
    {
        public static IList<T> ConvertToList<T>(DataTable dataTable)
        {
            IList<T> list = new List<T>();
            Type type = typeof(T);
            PropertyInfo[] propertyInfo = type.GetProperties();
            bool isDbNull = false;
            T item;

            if (dataTable != null && dataTable.Rows != null && dataTable.Rows.Count > 0)
            {
                IList<DataColumn> dataColumns = dataTable.Columns.Cast<DataColumn>().ToList();

                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    item = (T)Activator.CreateInstance(type);
                    DataRow dataRow = dataTable.Rows[rowIndex];

                    foreach (PropertyInfo property in propertyInfo)
                    {
                        if (property.CanWrite)
                        {
                            DataColumn dataColumn = dataColumns.Where(c => c.ColumnName.ToLowerInvariant() == property.Name.ToLowerInvariant()).SingleOrDefault();

                            if (dataColumn != null)
                            {
                                if (dataRow[property.Name] == DBNull.Value)
                                {
                                    isDbNull = true;
                                }
                                else
                                {
                                    isDbNull = false;
                                }


                                if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                                {
                                    if (isDbNull)
                                    {
                                        property.SetValue(item, null, null);
                                    }
                                    else
                                    {
                                        property.SetValue(item, Convert.ChangeType(dataRow[property.Name], Type.GetType(Nullable.GetUnderlyingType(property.PropertyType).ToString())), null);
                                    }
                                }
                                else
                                {
                                    if (isDbNull)
                                    {
                                        if (property.PropertyType.BaseType.Equals(typeof(Enum)))
                                        {
                                            property.SetValue(item, Enum.ToObject(property.PropertyType, 0), null);
                                        }
                                        else
                                        {
                                            property.SetValue(item, null, null);
                                        }
                                    }
                                    else
                                    {
                                        if (property.PropertyType.BaseType.Equals(typeof(Enum)))
                                        {
                                            property.SetValue(item, Enum.ToObject(property.PropertyType, Convert.ToInt32(dataRow[property.Name])), null);
                                        }
                                        else
                                        {
                                            property.SetValue(item, Convert.ChangeType(dataRow[property.Name], Type.GetType(property.PropertyType.ToString())), null);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    list.Add(item);
                }
            }

            return list;
        }

        public static DataTable ConvertToDataTable<T>(IList<T> list)
        {
            DataTable dataTable = new DataTable();

            if (list != null && list.Count > 0)
            {
                PropertyDescriptorCollection propertyDescriptor = TypeDescriptor.GetProperties(typeof(T));

                foreach (PropertyDescriptor property in propertyDescriptor)
                {
                    dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                }

                foreach (T item in list)
                {
                    DataRow dataRow = dataTable.NewRow();

                    foreach (PropertyDescriptor property in propertyDescriptor)
                    {
                        dataRow[property.Name] = property.GetValue(item) ?? DBNull.Value;
                    }

                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
    }
}
