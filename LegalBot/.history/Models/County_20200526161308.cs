using Newtonsoft.Json; 

namespace LegalBot.Models
{
    public class County
    {   
        // Te
        [JsonProperty("countyid")]
         public int countyid { get; set; }
         [JsonProperty("countyname")]
        public string countyname { get; set; }
    }
}