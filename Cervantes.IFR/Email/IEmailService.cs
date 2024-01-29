using System;
using System.Collections.Generic;

namespace Cervantes.IFR.Email;

public interface IEmailService
{
    void SendWelcome(string userId,string link);
    void SendAsignedProject(string userId,Guid projectId);
    void SendAsignedTask(string userId, Guid? projectId, Guid taskId);
    bool IsEnabled();
}