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
    public class FriendshipController : ControllerBase
    {

        public Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> _hubContext { get; }
        public IFriendshipManager _friendshipManager { get; }
        public IUserConnectionsManager _userConnectionsManager { get; }


        public FriendshipController(Microsoft.AspNetCore.SignalR.IHubContext<ChatHub> hubContext, IFriendshipManager friendshipManager
            , IUserConnectionsManager userConnectionsManager)
        {
            _hubContext = hubContext;
            _friendshipManager = friendshipManager;
            _userConnectionsManager = userConnectionsManager;
        }

        [HttpPost]
        [Route("SendFriendRequest")]
        public async Task<ActionResult> SendFriendRequestAsync([FromForm] Guid respondentUserId)
        {
            try
            {
                Guid currentUserId = new Guid();
                bool result = Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out currentUserId);
                if (result == false) return Unauthorized();
                if (!await _userConnectionsManager.IsOnline(currentUserId))
                    return BadRequest();
                var hub = _hubContext;
                return await _friendshipManager.SendFriendRequestAsync(hub, currentUserId, respondentUserId) ? Ok(): BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            
        }

        [HttpPost]
        [Route("AcceptFriendRequest")]
        public async Task<ActionResult> AcceptFriendRequestAsync([FromForm] Guid friendshipId)
        {
            try
            {
                Guid currentUserId = new Guid();
                bool result = Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out currentUserId);
                if (result == false) return Unauthorized();
                if (!await _userConnectionsManager.IsOnline(currentUserId))
                    return BadRequest();
                return await _friendshipManager.AcceptFriendRequestAsync(_hubContext, currentUserId, friendshipId) ? Ok() : BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
