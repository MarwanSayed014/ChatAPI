using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$ %^&*-]).{8,}$")]
        public string Password { get; set; }

        [Required]
        [MinLength(2)]
        public string ProfileImgPath { get; set; } = "/Images/Users/user.png";
    }
}
