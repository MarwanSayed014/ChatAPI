using ChatAPI.Attributes;
using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Dtos
{
    public class CreateGroupChatDto
    {        
        [Required]
        [MaxLength(30, ErrorMessage = "Group should have max 30 characters")]
        public string GroupName { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Group should have at least 1 member")]
        [MaxLength(100, ErrorMessage = "Group should have max 100 member")]
        public IEnumerable<Guid> MembersIds { get; set; }

        [MaxFileSize(10_000_000, true, ErrorMessage = "The file should be less than 10 MB")]
        [AllowedExtensions([".png", ".jpg", ".jpeg"], true,
            ErrorMessage = "The file should be in this extensions (png, jpg, jpeg)")]
        public IFormFile? Image { get; set; }
    }
}
