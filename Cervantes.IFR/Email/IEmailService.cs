using System;
using System.Collections.Generic;
using Cervantes.CORE.Entities;

namespace Cervantes.IFR.Email;

public interface IEmailService
{
    void SendWelcome(string userId,string link);
    void SendAsignedProject(string userId,Guid projectId);
    void SendMemberAddedToProject(string addedUserId, string addedByUserId, Guid projectId);
    void SendAsignedTask(string userId, Guid? projectId, Guid taskId);
    void SendApiKeyCreated(string userId, string? keyName, string keyPrefix, DateTimeOffset? expiresAt);
    Task<bool> SendCveNotificationAsync(CveNotification notification);
    bool IsEnabled();
}