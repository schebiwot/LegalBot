namespace LegalBot.Models
{
    public class County
    {   
        [JsonProperty("countyid")]
         public int countyid { get; set; }
         [JsonProperty("diaplayName")]
        public string countyname { get; set; }
    }
}