using Newtonsoft.Json; 

namespace LegalBot.Models
{
    public class County
    {   
        // Tells the class to look out for 
        [JsonProperty("countyid")]
         public int CountyId { get; set; }
         [JsonProperty("countyname")]
        public string CountyName { get; set; }
    }
}