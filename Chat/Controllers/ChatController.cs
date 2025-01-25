using ChatAPI.Dtos;
using ChatAPI.Hubs;
using ChatAPI.Services.Interfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using XAct.Security;


namespace ChatAPI.Controllers
{
    [ApiController]
    [Route("ChatAPI/[controller]")]
    public class ChatController : ControllerBase
    {
        public Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> _hubContext { get; }
        public IChatHubManager _chatHubManager { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }

        public ChatController(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, IChatHubManager chatHubManager
            , IUserConnectionsManager userConnectionsManager)
        {
            _hubContext = hubContext;
            _chatHubManager = chatHubManager;
            _userConnectionsManager = userConnectionsManager;
        }

        [HttpPost]
        [Route("PrivateChat")]
        [Authorize]
        public async Task<ActionResult> PrivateChat([FromForm] PrivateMessageDto messageDto)
        {
            Guid currentUserId = new Guid();
            bool result = Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out currentUserId);
            if(result == false) return Unauthorized();
            if (!await _userConnectionsManager.IsOnline(currentUserId))
                return BadRequest();
            
            if (result == true)
            {
                await _chatHubManager.PrivateMessageingAsync(_hubContext.Clients, _hubContext.Groups, currentUserId, messageDto);
                return Ok();
            }
            return BadRequest();
        }
    }

}
