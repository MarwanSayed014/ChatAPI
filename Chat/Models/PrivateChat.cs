using ChatAPI.Types;
using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Models
{
    public class PrivateChat
    {
        [Key]
        public Guid Id { get; set; }
    }
}
