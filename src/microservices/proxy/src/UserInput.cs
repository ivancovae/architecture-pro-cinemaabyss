using System.ComponentModel.DataAnnotations;

namespace proxy
{
    public class UserInput
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string email { get; set; }
    }
}
