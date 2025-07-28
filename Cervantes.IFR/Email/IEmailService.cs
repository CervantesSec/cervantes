using System;
using System.Collections.Generic;
using Cervantes.CORE.Entities;

namespace Cervantes.IFR.Email;

public interface IEmailService
{
    void SendWelcome(string userId,string link);
    void SendAsignedProject(string userId,Guid projectId);
    void SendAsignedTask(string userId, Guid? projectId, Guid taskId);
    Task<bool> SendCveNotificationAsync(CveNotification notification);
    bool IsEnabled();
}