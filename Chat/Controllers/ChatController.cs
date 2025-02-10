using ChatAPI.Dtos;
using ChatAPI.Hubs;
using ChatAPI.Services;
using ChatAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using XAct.Security;
using Microsoft.AspNetCore.Authorization;



namespace ChatAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("ChatAPI/[controller]")]
    public class ChatController : ControllerBase
    {
        public IPrivateMessageManager _messageManager { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }

        public ChatController(IPrivateMessageManager messageManager
            , IUserConnectionsManager userConnectionsManager)
        {
            _messageManager = messageManager;
            _userConnectionsManager = userConnectionsManager;
        }

        [HttpPost]
        [Route("PrivateChat")]
        public async Task<ActionResult> PrivateChat([FromForm] PrivateMessageDto messageDto)
        {
            Guid currentUserId = new Guid();
            bool result = Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out currentUserId);
            if(result == false) return Unauthorized();
            if (!await _userConnectionsManager.IsOnline(currentUserId))
                return BadRequest();
            
            if (result == true)
            {
                await _messageManager.PrivateMessageingAsync(currentUserId, messageDto);
                return Ok();
            }
            return BadRequest();
        }
    }

}
