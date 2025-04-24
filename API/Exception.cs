using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CloudSalon.API
{
    public class InvalidException : Exception
    {
        public InvalidException()
        {
            this.Messages = new List<string>();
        }
        public List<string> Messages { get; set; }
    }

    public class IllegalParameterException : Exception
    {

    }
}