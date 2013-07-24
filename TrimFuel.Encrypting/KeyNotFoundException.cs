using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Encrypting
{
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException(Exception innerException)
            : base("Secret key was not found.", innerException)
        {
        }
    }
}
