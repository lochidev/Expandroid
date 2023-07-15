using System.Text.Json.Serialization;

namespace MauiApp1.Models
{
    public class DictWrapper
    {
        [JsonPropertyName("globalVars")]
        public List<Var> global_vars { get; set; }
        [JsonPropertyName("matches")]
        public List<Match> matches { get; set; }
    }
    public class Match
    {
        public Match()
        {

        }
        public Match(Match og)
        {
            Trigger = og.Trigger;
            Replace = og.Replace;
            Vars = og.Vars;
            //Word = og.Word;
        }
        public string Trigger { get; set; }
        public string Replace { get; set; }
        public List<Var> Vars { get; set; }
        //public bool Word { get; set; }
    }
    public class Var
    {
        public string Name { get; set; }
        public string Type { get; set; } // echo, random, clipboard and date only supported
        public Params Params { get; set; }
    }
    public class Params
    {
        public string Echo { get; set; }
        public string Format { get; set; }
        public long Offset { get; set; } = 0;
        public string Cmd { get; set; }
        public List<string> Choices { get; set; }
    }
}
