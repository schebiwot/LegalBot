using Newtonsoft.Json; 

namespace LegalBot.Models
{
    public class County
    {   
        // Tells th class to look
        [JsonProperty("countyid")]
         public int CountyId { get; set; }
         [JsonProperty("countyname")]
        public string Countyname { get; set; }
    }
}