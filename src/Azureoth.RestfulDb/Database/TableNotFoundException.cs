using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azureoth.RestfulDb.Database
{
    public class TableNotFoundException : Exception
    {
        public TableNotFoundException(string message) : base(message)
        {
        }

        public TableNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
