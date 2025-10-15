using System.ComponentModel.DataAnnotations;

namespace proxy
{
    public class UserInput
    {
        public string id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string email { get; set; }
    }
}
