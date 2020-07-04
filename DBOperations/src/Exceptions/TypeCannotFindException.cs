using System;

namespace DBOperations.src.Exceptions
{
    public class TypeCannotFindException:Exception
    {
        public TypeCannotFindException(string message) : base(message) { }
    }
}
