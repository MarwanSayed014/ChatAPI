using ChatAPI.Dtos;
using ChatAPI.Helpers;
using ChatAPI.Models;
using ChatAPI.Repos.Interfaces;
using ChatAPI.Services.Interfaces;
using ChatAPI.Types;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ChatAPI.Services
{
    public class GroupChatManager : IGroupChatManager
    {
        public IGroupChatRepo _groupChatRepo { get; }
        public IGroupMessageRepo _groupMessageRepo { get; }
        public IGroupMessageStatusRepo _groupMessageStatusRepo { get; }
        public IUserGroupChatRepo _userGroupChatRepo { get; }

        public GroupChatManager(IGroupChatRepo groupChatRepo, IGroupMessageRepo groupMessageRepo,
            IGroupMessageStatusRepo groupMessageStatusRepo, IUserGroupChatRepo userGroupChatRepo)
        {
            _groupChatRepo = groupChatRepo;
            _groupMessageRepo = groupMessageRepo;
            _groupMessageStatusRepo = groupMessageStatusRepo;
            _userGroupChatRepo = userGroupChatRepo;
        }

        public async Task<IEnumerable<GroupChat>> GetAllUserChatGroupsAsync(Guid userId)
        {
            return (await _userGroupChatRepo.FindAsync(x=> x.UserId == userId))
                .Include(x=> x.GroupChat).Select(x=>x.GroupChat).ToList();
        }

        public async Task<GroupChat> GetGroupChatAsync(Guid groupId)
        {
            return (await _groupChatRepo.FindAsync(x=> x.Id == groupId)).SingleOrDefault();
        }

        public async Task CreateGroupChat(CreateGroupChatDto createGroupChatDto, Guid userId)
        {
            GroupChat groupChat = new GroupChat 
            {
                Name = createGroupChatDto.GroupName
            };
            if (createGroupChatDto.Image != null)
            {
                var dir = Directory.GetCurrentDirectory() + "/wwwroot/Images/Groups/";
                var ex = ServerFile.GetExtension(createGroupChatDto.Image.FileName);
                var imageName = Guid.NewGuid() + ex;
                var imagePath = dir + imageName;
                ServerFile.Upload(createGroupChatDto.Image, imagePath);
                groupChat.CoverImgPath = $"/Images/Groups/{imageName}";
            }
            await _groupChatRepo.CreateAsync(groupChat);
            await _groupChatRepo.SaveAsync();

            await _userGroupChatRepo.CreateAsync(new UserGroupChat
            {
                GroupChatId = groupChat.Id,
                UserId = userId,
                GroupUserRolesTypes = GroupUserRolesTypes.Admin
            });

            foreach (var memberId in createGroupChatDto.MembersIds.Distinct().Where(x=> x != userId))
            {
                await _userGroupChatRepo.CreateAsync(new UserGroupChat
                {
                    GroupChatId = groupChat.Id,
                    UserId = memberId,
                    GroupUserRolesTypes = GroupUserRolesTypes.Member
                });
            }

            await _userGroupChatRepo.SaveAsync();
        }

        public async Task DeleteGroupChat(Guid groupId, Guid groupAdminId)
        {
            var groupChat = await GetGroupChatAsync(groupId);
            if(groupChat != null)
            {
                if (await IsAdmin(groupChat.Id, groupAdminId)) 
                {
                    _groupChatRepo.DeleteAsync(groupChat);
                    _groupChatRepo.SaveAsync();
                    ServerFile.Delete(Directory.GetCurrentDirectory() + "/wwwroot" + groupChat.CoverImgPath);
                }
            }
        }

        public async Task AddMember(Guid groupId, Guid memberId, Guid groupAdminId)
        {
            var groupChat = await GetGroupChatAsync(groupId);
            if (groupChat != null)
            {
                if (await IsAdmin(groupChat.Id, groupAdminId))
                {
                    if(! await IsInGroup(groupId, memberId))
                    {
                        await _userGroupChatRepo.CreateAsync(new UserGroupChat
                        {
                            GroupChatId = groupId,
                            UserId = memberId,
                            GroupUserRolesTypes = GroupUserRolesTypes.Member
                        });
                        await _userGroupChatRepo.SaveAsync();
                    }
                }
            }
        }

        public async Task DeleteMember(Guid groupId, Guid memberId, Guid groupAdminId)
        {
            var groupChat = await GetGroupChatAsync(groupId);
            if (groupChat != null)
            {
                if (await IsAdmin(groupChat.Id, groupAdminId))
                {
                    if (await IsInGroup(groupId, memberId))
                    {
                        var userGroupChat = (await _userGroupChatRepo.FindAsync(
                            x=> x.GroupChatId == groupId && x.UserId == memberId)).SingleOrDefault();
                        if (userGroupChat != null)
                        {
                            await _userGroupChatRepo.DeleteAsync(userGroupChat);
                            await _userGroupChatRepo.SaveAsync();
                        } 
                    }
                }
            }
        }

        public async Task MakeAdmin(Guid groupId, Guid memberId, Guid groupAdminId)
        {
            var groupChat = await GetGroupChatAsync(groupId);
            if (groupChat != null)
            {
                if (await IsAdmin(groupChat.Id, groupAdminId))
                {
                    if (await IsInGroup(groupId, memberId))
                    {
                        var userGroupChat = (await _userGroupChatRepo.FindAsync(
                            x => x.GroupChatId == groupId && x.UserId == memberId)).SingleOrDefault();
                        if (userGroupChat != null)
                        {
                            userGroupChat.GroupUserRolesTypes = GroupUserRolesTypes.Admin;
                            await _userGroupChatRepo.UpdateAsync(userGroupChat);
                            await _userGroupChatRepo.SaveAsync();
                        }
                    }
                }
            }
        }

        public async Task ChangeName(ChangeGroupNameDto changeGroupNameDto, Guid groupAdminId)
        {
            var groupChat = await GetGroupChatAsync(changeGroupNameDto.GroupId);
            if (groupChat != null)
            {
                if (await IsAdmin(groupChat.Id, groupAdminId))
                {
                    groupChat.Name = changeGroupNameDto.Name;
                    await _groupChatRepo.UpdateAsync(groupChat);
                    await _groupChatRepo.SaveAsync();
                } 
            }
        }

        public async Task ChangeCoverImage(ChangGroupCoverImagDto changGroupCoverIamgDto, Guid groupAdminId)
        {
            if(changGroupCoverIamgDto.Image == null) return;
            GroupChat groupChat = await GetGroupChatAsync(changGroupCoverIamgDto.GroupId);
            if (groupChat != null)
            {
                if (await IsAdmin(groupChat.Id, groupAdminId)) 
                {
                    
                    if (groupChat.CoverImgPath != "/Images/Groups/group.png")
                    {
                        ServerFile.Delete(Directory.GetCurrentDirectory() + "/wwwroot" + groupChat.CoverImgPath);
                    }
                    var dir = Directory.GetCurrentDirectory() + "/wwwroot/Images/Groups/";
                    var ex = ServerFile.GetExtension(changGroupCoverIamgDto.Image.FileName);
                    var imageName = Guid.NewGuid() + ex;
                    var imagePath = dir + imageName;
                    ServerFile.Upload(changGroupCoverIamgDto.Image, imagePath);
                    groupChat.CoverImgPath = $"/Images/Groups/{imageName}";
                }
            }
        }

        public async Task<bool> IsAdmin(Guid groupId, Guid userId)
        {
            var groupChat = await GetGroupChatAsync(groupId);

            if (groupChat != null)
            {
                return (await _userGroupChatRepo.FindAsync(x=> x.GroupChatId == groupId &&
                x.UserId == userId &&
                x.GroupUserRolesTypes == GroupUserRolesTypes.Admin)).Count() > 0 ? true : false;
            }
            return false;
        }

        public async Task<IEnumerable<User>> GetAllGroupMembers(Guid groupId)
        {
            return (await _userGroupChatRepo.FindAsync(x=> x.GroupChatId == groupId))
                .Include(x=> x.User).Select(x=> x.User).ToList();
        }

        public async Task<bool> IsInGroup(Guid groupId, Guid userId)
        {
            var groupChat = await GetGroupChatAsync(groupId);

            if (groupChat != null)
            {
                return (await _userGroupChatRepo.FindAsync(x => x.GroupChatId == groupId &&
                x.UserId == userId)).Count() > 0 ? true : false;
            }
            return false; ;
        }
    }
}
