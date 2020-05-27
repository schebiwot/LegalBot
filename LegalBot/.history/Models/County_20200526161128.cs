namespace LegalBot.Models
{
    public class County
    {   
        [JsonProperty("id")]
         public int countyid { get; set; }
        public string countyname { get; set; }
    }
}