using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegalBot.Models
{
    public class UserDetails
    {
        public string FullName { get; set; }
        public  County { get; set; }
        public string SubCounty { get; set; }
        public string Ward { get; set; }
        public string Language { get; set; }

    }
}
