using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenStaySync.Exceptions
{
    public class PriceUpdateInformation : Exception
    {
        public PriceUpdateInformation()
        { }

        public PriceUpdateInformation(string message) : base(message)
        { }

        public PriceUpdateInformation(string message, Exception inner)
        : base(message, inner)
        {
        }

    }
}
