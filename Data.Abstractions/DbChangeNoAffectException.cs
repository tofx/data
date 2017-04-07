using System;

namespace TOF.Data.Abstractions
{
    public class DbChangeNoAffectException : Exception
    {
        public DbChangeNoAffectException() : base("Statement is executed but no any row affected.") { }
    }
}
