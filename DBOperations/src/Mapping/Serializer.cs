using DBOperations.src.Exceptions;
using DBOperations.src.Mapping.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace DBOperations.src.Mapping
{
    internal class Serializer<T>
    {
        private readonly ICollection<MapperMember> _configationMembers = null;

        internal Serializer(ICollection<MapperMember> configationMembers)
        {
            _configationMembers = configationMembers;
        }

        internal IEnumerable<T> Constructs(DbDataReader dataReader)
        {
            var reader = dataReader;
            Type resultType = typeof(T);

            if (!resultType.IsClass)
                throw new Exception("Type could not be read! Because type isn't Class.");

            List<T> dataResult = new List<T>();
            var properties = resultType.GetProperties();
            while (reader.Read())
            {
                dataResult.Add((T)Construct(typeof(T), reader));
            }
            return dataResult;
        }

        private object Construct(Type resultType, IDataReader reader)
        {
            var properties = resultType.GetProperties();

            object newInstance = Activator.CreateInstance(resultType);

            foreach (var item in properties)
            {
                var entityName = _configationMembers.Where(m=>m.SourceMember == item.Name).FirstOrDefault();
                if (entityName == null)
                    throw new MapperMemberCannotFindException($"Mapper member cannot find {item.Name}");

                object value;

                if (item.PropertyType.IsPrimitive || item.PropertyType.Equals(typeof(decimal)) || item.PropertyType.Equals(typeof(Nullable<decimal>)) || item.PropertyType.Equals(typeof(string))
                    || item.PropertyType.Equals(typeof(DateTime)) || item.PropertyType.Equals(typeof(Nullable<DateTime>)) || item.PropertyType.Equals(typeof(Nullable<int>)) || item.PropertyType.IsEnum)
                    value = reader.GetValue(reader.GetOrdinal(entityName.DestinationMember));
                else
                    value = Construct(item.PropertyType, reader);

                if (value is DBNull)
                    value = GetDefault(item.PropertyType);

                item.SetValue(newInstance, value);
            }
            return newInstance;
        }

        private object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
