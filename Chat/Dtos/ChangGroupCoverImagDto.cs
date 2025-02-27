using ChatAPI.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Dtos
{
    public class ChangGroupCoverImagDto
    {
        [Required]
        public Guid GroupId { get; set; }

        [Required]
        [MaxFileSize(10_000_000, true, ErrorMessage = "The file should be less than 10 MB")]
        [AllowedExtensions([".png", ".jpg", ".jpeg"], true,
            ErrorMessage = "The file should be in this extensions (png, jpg, jpeg)")]
        public IFormFile Image { get; set; }
    }
}
