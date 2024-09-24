using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManlikaBank.Models
{
    public class ReturnModel
    {
        public bool IsSuccessful { get; set; }
        public string ErrorMessage { get; set; }
    }
}