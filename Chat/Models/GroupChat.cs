using ChatAPI.Types;
using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Models
{
    public class GroupChat
    {
        [Key]
        public Guid Id { get; set; }

        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; }

        [MinLength(2)]
        public string CoverImgPath { get; set; }

    }
}
