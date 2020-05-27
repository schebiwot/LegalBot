using 

namespace LegalBot.Models
{
    public class County
    {   
        [JsonProperty("countyid")]
         public int countyid { get; set; }
         [JsonProperty("countyname")]
        public string countyname { get; set; }
    }
}