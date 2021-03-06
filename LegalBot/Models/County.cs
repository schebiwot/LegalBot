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
        public  SubCounty[] SubCounties {get; set;}
    }
    public class SubCounty{
        [JsonProperty("id")]
        public int Id{get;set;}
        [JsonProperty("name")]
        public string Name{get;set;}
        
         [JsonProperty("wards")]
        public string [] Wards {get; set;}
    }
}