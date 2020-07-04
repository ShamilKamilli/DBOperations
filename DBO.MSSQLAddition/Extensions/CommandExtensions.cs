using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.MSSQLAddition.Extensions
{
    public static class CommandExtensions
    {
        public static SqlCommand AddCommandParameters<T>(this SqlCommand dbCommand, T model)
        {
            foreach (var item in typeof(T).GetProperties())
            {
                dbCommand.Parameters.AddWithValue($"@{item.Name}", item.GetValue(model));
            }
            return dbCommand;
        }

        public static SqlCommand AddCommandParameters(this SqlCommand dbCommand, Dictionary<string, object> keyValuePairs)
        {
            foreach (var item in keyValuePairs)
            {
                dbCommand.Parameters.AddWithValue(item.Key, item.Value);
            }
            return dbCommand;
        }
        public static SqlCommand AddCommandParameter(this SqlCommand dbCommand, string key, object value)
        {
            dbCommand.Parameters.AddWithValue(key, value);
            return dbCommand;
        }

        public static SqlCommand SetCommandType(this SqlCommand dbCommand, CommandType commandType)
        {
            dbCommand.CommandType = commandType;

            return dbCommand;
        }

        public static SqlCommand SetCommandQuery(this SqlCommand dbCommand, string query)
        {
            dbCommand.CommandText = query;

            return dbCommand;
        }
    }
}
