using ChatAPI.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Dtos
{
    public class PrivateMessageDto 
    {
        [Required]
        [MinLength(1, ErrorMessage = "Message should be have at least 1 character")]
        [MaxLength(250, ErrorMessage = "Message Length should be less than or equal 250")]
        public string Text { get; set; }
        [Required]
        public Guid AnotherUserId { get; set; }
        [MaxLength(10,ErrorMessage = "Number of files should be less than or equal 10")]
        [MaxFileSize(10_000_000, true, ErrorMessage = "The file should be less than 10 MB")]
        [AllowedExtensions([".png", ".jpg", ".jpeg"], true,
            ErrorMessage = "The file should be in this extensions (png, jpg, jpeg)")]
        public IEnumerable<IFormFile>? Files { get; set; }

    }
}
