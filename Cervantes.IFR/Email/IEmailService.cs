using System;
using System.Collections.Generic;

namespace Cervantes.IFR.Email;

public interface IEmailService
{
    void Send(EmailMessage emailMessage);
}