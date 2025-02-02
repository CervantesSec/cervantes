using System;

namespace Cervantes.CORE.ViewModels;

public class MemberViewModel
{
    public Guid ProjectId { get; set; }
    public HashSet<string> MemberId { get; set; }
}