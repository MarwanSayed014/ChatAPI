using ChatAPI.Dtos;
using ChatAPI.Models;

namespace ChatAPI.Services.Interfaces
{
    public interface IGroupChatManager
    {
        Task<IEnumerable<GroupChat>> GetAllUserChatGroupsAsync(Guid userId);
        Task<GroupChat> GetGroupChatAsync(Guid groupId);
        Task CreateGroupChat(CreateGroupChatDto createGroupChatDto, Guid userId);
        Task DeleteGroupChat(Guid groupId, Guid groupAdminId);
        Task AddMember(Guid groupId, Guid memberId, Guid groupAdminId);
        Task DeleteMember(Guid groupId, Guid memberId, Guid groupAdminId);
        Task MakeAdmin(Guid groupId, Guid memberId, Guid groupAdminId);
        Task ChangeName(ChangeGroupNameDto changeGroupNameDto, Guid groupAdminId);
        Task ChangeCoverImage(ChangGroupCoverImagDto changGroupCoverIamgDto, Guid groupAdminId);
        Task<bool> IsAdmin(Guid groupId, Guid userId);
        Task<IEnumerable<User>> GetAllGroupMembers(Guid groupId);
        Task<bool> IsInGroup(Guid groupId, Guid userId);
    }
}
