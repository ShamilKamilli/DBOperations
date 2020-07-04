using DBOperations.src.Exceptions;
using DBOperations.src.Mapping.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.MSSQL.Extensions
{
    public static class CommandExtensions
    {
        public static SqlCommand AddCommandParameters<T>(this SqlCommand sqlCommand,T model)
        {
            var members = QueryConfiguration.GetMembers(typeof(T));

            if (members == null)
                throw new TypeCannotFindException($"{typeof(T).Name} could't fidn");

            foreach (var item in typeof(T).GetProperties())
            {
                var field = members.Where(m => m.SourceMember == item.Name).FirstOrDefault();

                var val = item.GetValue(model);

                if (field != null)
                {
                    var param = new SqlParameter()
                    {
                        Value = val,
                        DbType = field.Type,
                        Direction = field.Direction,
                        ParameterName = field.DestinationMember
                    };

                    if (field.Size != null)
                        param.Size = (int)field.Size;

                    sqlCommand.Parameters.Add(param);
                }
            }

            return sqlCommand;
        }

        public static T GetParamValues<T>(this SqlCommand sqlCommand,T model)
        {
            var instance  = Activator.CreateInstance<T>();

            foreach (var item in model.GetType().GetProperties())
            {
                var index = sqlCommand.Parameters.IndexOf(item.Name);
                var sqlParameter = sqlCommand.Parameters[index];
                item.SetValue(instance, sqlParameter.Value);
            }

            return instance;
        }

        public static SqlCommand SetCommandType(this SqlCommand sqlCommand, CommandType commandType)
        {
            sqlCommand.CommandType = commandType;

            return sqlCommand;
        }

        public static SqlCommand SetCommandQuery(this SqlCommand sqlCommand, string query)
        {
            sqlCommand.CommandText = query;

            return sqlCommand;
        }

        public static SqlCommand AddOutParameter(this SqlCommand sqlCommand, SqlParameter sqlParameter)
        {
            sqlCommand.Parameters.Add(sqlParameter);

            return sqlCommand;
        }

        public static SqlCommand AddCommandParameter(this SqlCommand sqlCommand, string key, object value)
        {
            sqlCommand.Parameters.AddWithValue($"@{key}", value);

            return sqlCommand;
        }

        public static SqlCommand AddCommandParameters(this SqlCommand sqlCommand, Dictionary<string, object> keyValuePairs)
        {
            foreach (var item in keyValuePairs)
            {
                sqlCommand.Parameters.AddWithValue($"@{item.Key}", item.Value);
            }

            return sqlCommand;
        }
    }
}
