namespace Expandroid.Models
{
    public static class Utils
    {
        public static string GetTheRealFormat(string format)
        {
            format = format.Replace("%Y", "yyyy");
            format = format.Replace("%m", "MM");
            format = format.Replace("%b", "MMM");
            format = format.Replace("%B", "MMMM");
            format = format.Replace("%h", "MMM");
            format = format.Replace("%d", "dd");
            format = format.Replace("%e", "d");
            format = format.Replace("%a", "ddd");
            format = format.Replace("%A", "dddd");
            format = format.Replace("%j", DateTime.Now.DayOfYear.ToString());
            format = format.Replace("%w", DateTime.Now.DayOfWeek.ToString());
            format = format.Replace("%u", (DateTime.Now.DayOfWeek + 1).ToString());
            format = format.Replace("%D", "MM/dd/yyyy");
            format = format.Replace("%F", "yyyy/MM/dd");
            format = format.Replace("%H", "HH");
            format = format.Replace("%I", "hh");
            format = format.Replace("%p", "tt");
            format = format.Replace("%M", "mm");
            format = format.Replace("%S", "ss");
            format = format.Replace("%R", "HH:mm");
            format = format.Replace("%T", "HH:mm:ss");
            format = format.Replace("%r", "hh:mm:ss tt");
            return format;
        }
        public static string GetOriginalFormat(string format)
        {
            format = format.Replace("yyyy", "%Y");
            format = format.Replace("MM", "%m");
            format = format.Replace("MMM", "%b");
            format = format.Replace("MMMM", "%B");
            format = format.Replace("dd", "%d");
            format = format.Replace("d", "%e");
            format = format.Replace("ddd", "%a");
            format = format.Replace("dddd", "%A");
            format = format.Replace("MM/dd/yyyy", "%D");
            format = format.Replace("yyyy/MM/dd", "%F");
            format = format.Replace("HH", "%H");
            format = format.Replace("hh", "%I");
            format = format.Replace("tt", "%p");
            format = format.Replace("mm", "%M");
            format = format.Replace("ss", "%S");
            format = format.Replace("HH:mm", "%R");
            format = format.Replace("HH:mm:ss", "%T");
            format = format.Replace("hh:mm:ss tt", "%r");
            return format;
        }
    }
}
