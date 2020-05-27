namespace LegalBot.Models
{
    public class County
    {   
        [JsonProperty("countyid")]
         public int countyid { get; set; }
         
        public string countyname { get; set; }
    }
}