using System;

namespace DBOperations.src.Exceptions
{
    public class MapperMemberCannotFindException : Exception
    {
        public MapperMemberCannotFindException(string message) : base(message) { }
    }
}
