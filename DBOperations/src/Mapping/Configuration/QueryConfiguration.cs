using DBOperations.src.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DBOperations.src.Mapping.Configuration
{
    public abstract class QueryConfiguration
    {
        public QueryConfiguration()
        {
            CreateConfigurationList();
        }

        private static ICollection<ConfigationMember> _CONFIGURATIONS = null;
        private static object _OBJECTList = new object();
        private static object _OBJECT = new object();
        private static QueryConfiguration _queryConfiguration = null;

        public ConfigationMember<T> RegisterType<T>()
        {
            var member = new ConfigationMember<T>()
            {
                Type = typeof(T)
            };

            _CONFIGURATIONS.Add(member);

            return member;
        }

        internal static void CreateConfigurationList()
        {
            if(_CONFIGURATIONS == null)
            {
                lock (_OBJECTList)
                {
                    _CONFIGURATIONS = new List<ConfigationMember>();
                }
            }
        }

        public static QueryConfiguration CreateaConfiguration<T>() where T: QueryConfiguration
        {
            if(_queryConfiguration == null)
            {
                lock (_OBJECT)
                {
                    _queryConfiguration = (T)Activator.CreateInstance(typeof(T));
                }
            }

            return _queryConfiguration;
        }

        public static ICollection<MapperMember> GetMembers(Type type)
        {
           var configuration = _CONFIGURATIONS.Where(m => m.Type == type).FirstOrDefault();

            if (configuration == null)
                throw new TypeCannotFindException($"Type is not registerd {type.Name}");

            return configuration.Paramlist;
        }

        public static Dictionary<string, object> GetTypeKeyValues<T>(T model)
        {
            var members = _CONFIGURATIONS.Where(m => m.Type == typeof(T)).FirstOrDefault();
            if (members == null)
                throw new TypeCannotFindException($"{typeof(T).Name} could't find");

            var collection = new Dictionary<string, object>();

            foreach (var item in typeof(T).GetProperties())
            {
                var fieldName = members.Paramlist.Where(m => m.SourceMember == item.Name).Select(m => m.DestinationMember).FirstOrDefault();

                var val = item.GetValue(model);

                if (!item.PropertyType.IsArray && fieldName != null)
                {
                    if(val==null)
                       collection.Add(fieldName, DBNull.Value);
                    else
                       collection.Add(fieldName,val);
                }

            }

            return collection;
        }
    }

}
