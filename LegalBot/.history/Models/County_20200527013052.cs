using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq; 

namespace LegalBot.Models
{
    public class County
    {   
        // Tells the class to look out for 
        [JsonProperty("countyid")]
         public int CountyId { get; set; }

         [JsonProperty("countyname")]
        public string CountyName { get; set; } 

        [JsonProperty("sub_counties")]
        public List<string> SubCountyName
    {
        get{return subCounty;}
        set{this.subCounty = SubCountyName;}
    }
    }
}