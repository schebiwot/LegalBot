using Newtonsoft.Json; 

namespace LegalBot.Models
{
    public class County
    {   
        // Tells th
        [JsonProperty("countyid")]
         public int countyid { get; set; }
         [JsonProperty("countyname")]
        public string countyname { get; set; }
    }
}