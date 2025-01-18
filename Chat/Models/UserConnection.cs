using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAPI.Models
{
    public class UserConnection
    {
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public string SignalRConnectionId { get; set; }
    }
}
