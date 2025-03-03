using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChatAPI.Services.Interfaces;
using ChatAPI.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using ChatAPI.Dtos;

namespace ChatAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("ChatAPI/[controller]")]
    public class GroupChatController : ControllerBase
    {
        public IUserConnectionsManager _userConnectionsManager { get; }
        public IGroupChatManager _groupChatManager { get; }
        public GroupChatController(IUserConnectionsManager userConnectionsManager, IGroupChatManager groupChatManager)
        {
            _userConnectionsManager = userConnectionsManager;
            _groupChatManager = groupChatManager;
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateGroupChatDto createGroupChatDto)
        {
            Guid currentUserId = new Guid();
            bool result = Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out currentUserId);
            if (result == false) return Unauthorized();
            if (!await _userConnectionsManager.IsOnline(currentUserId))
                return BadRequest();
            if (result == true)
            {
                await _groupChatManager.CreateGroupChat(createGroupChatDto, currentUserId);
                return Ok();
            }
            return BadRequest();
        }
    }
}
