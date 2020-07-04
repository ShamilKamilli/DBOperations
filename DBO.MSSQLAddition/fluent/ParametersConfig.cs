using DBOperations.src.Mapping.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBO.MSSQLAddition.fluent
{
    public abstract class ParametersConfig
    {
        public ParametersConfig()
        {
            CreateConfigurationList();
        }

        private static ICollection<ConfigationMember> _CONFIGURATIONS = null;
        private static object _OBJECTList = new object();
        private static object _OBJECT = new object();
        private static ParametersConfig _queryConfiguration = null;

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
            if (_CONFIGURATIONS == null)
            {
                lock (_OBJECTList)
                {
                    _CONFIGURATIONS = new List<ConfigationMember>();
                }
            }
        }

        public static void CreateaConfiguration<T>() where T : QueryConfiguration
        {
            if (_queryConfiguration == null)
            {
                lock (_OBJECT)
                {
                    _queryConfiguration = (T)Activator.CreateInstance(typeof(T));
                }
            }
        }

        internal static ICollection<MapperMember> GetMembers(Type type)
        {
            var configuration = _CONFIGURATIONS.Where(m => m.Type == type).FirstOrDefault();

            if (configuration == null)
                throw new TypeCannotFindException($"Type is not registerd {type.Name}");

            return configuration.Paramlist;
        }
    }
}
