namespace events.Models
{
    public class MovieDto
    {
        public int movieId;
        public string title = string.Empty;
        public string action = string.Empty;
        public int userId;
        public string[] genres = new string[] { };
        public int rating;
        public int description;
    }
}
