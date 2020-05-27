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
        public Dictionary<string,List<SubCounty>> Sub {get; set;}
    }
    public class SubCounty{
        [JsonProperty("sub_counties")]
        public string Id{get;set;}
        public string Name{get;set;}
    }
}