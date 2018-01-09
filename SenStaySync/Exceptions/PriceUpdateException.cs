using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Exceptions
{
    public class PriceUpdateException : Exception
    {
        public PriceUpdateException()
        {
        }

        public PriceUpdateException(string message)
        : base(message)
        {
        }

        public PriceUpdateException(string message, Exception inner)
        : base(message, inner)
        {
        }


    }
}
