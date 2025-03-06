using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Dtos
{
    public class ChangeGroupNameDto
    {
        [Required]
        public Guid GroupId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name{ get; set; }
    }
}
