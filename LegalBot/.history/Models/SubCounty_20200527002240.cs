using Newtonsoft.Json; 

namespace LegalBot.Models
{
    public class SubCounty
    {
        [JsonProperty("countyname")]
        public string CountyName { get; set; }
        
    }
}