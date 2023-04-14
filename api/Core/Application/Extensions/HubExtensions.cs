using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models.Constants;
using Microsoft.AspNetCore.SignalR;

namespace Application.Extensions;

public static class HubExtensions
{
    public static async Task<AppUser?> GetCallerAsAppUserAsync(HubCallerContext context, IHubCallerClients clients, IUserRepository userRepository)
    {
        var username = context.User?.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(username))
        {
            await clients.Caller.SendAsync(CommonHubMethodNameConstants.NoUserWasProvided);
            return null;
        }

        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user is null)
        {
            await clients.Caller.SendAsync(CommonHubMethodNameConstants.NoUserWithSuchName);
            return null;
        }

        return user;
    }
}