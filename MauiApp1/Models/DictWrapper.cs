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
            Vars = new (og.Vars);
            //Word = og.Word;
        }
        public string Trigger { get; set; }
        public string Replace { get; set; }
        public List<Var> Vars { get; set; }
        //public bool Word { get; set; }
    }
    public class Var
    {
        public Var() { }
        public Var(Var og)
        {
            Name = og.Name;
            Type = og.Type;
            Params = new(og.Params);
        }
        public string Name { get; set; }
        public string Type { get; set; } // echo, random, clipboard and date only supported
        public Params Params { get; set; }
    }
    public class Params
    {
        public Params() { }
        public Params(Params og)
        {
            Echo = og.Echo;
            Format = og.Format;
            Offset = og.Offset;
            Cmd = og.Cmd;
            Choices = og.Choices;
        }
        public string Echo { get; set; }
        public string Format { get; set; }
        public long Offset { get; set; } = 0;
        public string Cmd { get; set; }
        public List<string> Choices { get; set; }
    }
}
