using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wrly.Utils;

namespace Wrly
{
    public static class GeneralExtentions
    {
        public static string GetDescription<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());
            if (fi == null)
            {
                return source.ToString();
            }
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }

        public static List<T> FromDataTable<T>(this DataTable table, string expression = "1=1")
        {
            List<T> listData = new List<T>();
            foreach (DataRow dataRow in table.Select(expression))
            {
                T item = GetItem<T>(dataRow);
                listData.Add(item);
            }
            return listData;
        }

        private static T GetItem<T>(DataRow dataRow)
        {
            var type = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn dataColumn in dataRow.Table.Columns)
            {
                var propertyInfo = type.GetProperties().FirstOrDefault(c => c.Name == dataColumn.ColumnName);
                if (propertyInfo != null && propertyInfo.GetSetMethod() != null)
                {
                    var pType = propertyInfo.GetType();
                    if (dataRow[dataColumn.ColumnName] == DBNull.Value)
                    {
                        propertyInfo.SetValue(obj, null);
                    }
                    else
                        propertyInfo.SetValue(obj, dataRow[dataColumn.ColumnName]);
                }
            }
            return obj;
        }

        public static T ToObject<T>(this string hash, T obj)
        {
            return GetItem<T>(hash, obj);
        }



        public static string GetSingleValue(this string hash, string key)
        {
            string strKey = "%&gt;s{;+#";
            hash = ValueEncryptionHelper.Decrypt(hash.Replace(" ", "+"), strKey);
            foreach (var objItem in hash.Split('&'))
            {
                if (key == objItem.Split('=')[0])
                {
                    return objItem.Split('=')[1];
                }
            }
            return string.Empty;
        }

        public static T Do<T>(Func<T> action, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    if (retry > 0)
                        Thread.Sleep(retryInterval);
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            throw new AggregateException(exceptions);
        }

        private static T GetItem<T>(string hash, T obj)
        {
            var type = typeof(T);
            if (obj == null)
                obj = Activator.CreateInstance<T>();

            string strKey = "%&gt;s{;+#";
            hash = ValueEncryptionHelper.Decrypt(hash.Replace(" ", "+"), strKey);
            foreach (var objItem in hash.Split('&'))
            {
                var propertyInfo = type.GetProperties().FirstOrDefault(c => c.Name == objItem.Split('=')[0]);
                if (propertyInfo != null)
                {
                    try
                    {
                        //var pType = propertyInfo.GetType();
                        //if (propertyInfo.PropertyType.FullName.Equals("System.Int32"))
                        //{
                        //    propertyInfo.SetValue(obj, Convert.ToInt32(objItem.Split('=')[1]));
                        //}
                        //else if (propertyInfo.PropertyType.FullName.Equals("System.Int16"))
                        //{
                        //    propertyInfo.SetValue(obj, Convert.ToInt16(objItem.Split('=')[1]));
                        //}
                        //else if (propertyInfo.PropertyType.FullName.Equals("System.Int64"))
                        //{
                        //    propertyInfo.SetValue(obj, Convert.ToInt64(objItem.Split('=')[1]));
                        //}
                        //else
                        //{

                        //var value = Convert.ChangeType(objItem.Split('=')[1],Nullable.GetUnderlyingType(propertyInfo.PropertyType));
                        //                        propertyInfo.SetValue(obj, (object)objItem.Split('=')[1]);
                        //}

                        //find the property type
                        Type propertyType = propertyInfo.PropertyType;

                        //Convert.ChangeType does not handle conversion to nullable types
                        //if the property type is nullable, we need to get the underlying type of the property
                        var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

                        //Returns an System.Object with the specified System.Type and whose value is
                        //equivalent to the specified object.
                        var propertyVal = Convert.ChangeType(objItem.Split('=')[1], targetType);

                        //Set the value of the property
                        propertyInfo.SetValue(obj, propertyVal, null);

                    }
                    catch (Exception e)
                    {
                        //   throw e;
                    }
                }
            }
            return obj;
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
