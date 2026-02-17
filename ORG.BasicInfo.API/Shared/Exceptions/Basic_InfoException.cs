using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORG.BasicInfo.API.Shared.Abstractions.Exceptions
{
    public abstract class Basic_InfoException : Exception
    {
        protected Basic_InfoException(string message) : base(message)
        {

        }
    }
}
