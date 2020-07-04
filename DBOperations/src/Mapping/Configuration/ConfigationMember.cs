using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DBOperations.src.Mapping.Configuration
{
    public class ConfigationMember<T>: ConfigationMember
    {
        public ConfigationMember()
        {
            Paramlist = new List<MapperMember>();
            Type = typeof(T);
        }

       public ConfigationMember<T> HasMember<D>(Expression<Func<T, D>> expression, string destination)
       {
            var memberExpression = (MemberExpression)expression.Body;

            if (string.IsNullOrEmpty(destination))
                throw new ArgumentNullException(nameof(destination));

            Paramlist.Add(new MapperMember
            {
                SourceMember= memberExpression.Member.Name,
                DestinationMember= destination
            });

            return this;
       }

    }

    public abstract class ConfigationMember
    {
        internal Type Type { get; set; }

        internal ICollection<MapperMember> Paramlist { get; set; }
    }
}
