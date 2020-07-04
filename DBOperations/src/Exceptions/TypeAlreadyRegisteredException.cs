using System;

namespace DBOperations.src.Exceptions
{
    public class TypeAlreadyRegisteredException:Exception
    {
        public TypeAlreadyRegisteredException(string message) : base(message) { }
    }
}
