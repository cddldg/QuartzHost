using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzHost.Base
{
    public class RunConflictException : Exception
    {
        public RunConflictException(string message) : base(message)
        {
        }
    }
}