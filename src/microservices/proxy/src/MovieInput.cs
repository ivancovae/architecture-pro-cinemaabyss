using System.ComponentModel.DataAnnotations;

namespace proxy
{
    public class MovieInput
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public float rating { get; set; }
        public string[] genres { get; set; }
    }
}
