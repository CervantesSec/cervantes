using System;
using Cervantes.CORE.Entities;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.CORE.ViewModel;

public class TaskUpdateViewModel
{
    public Guid Id { get; set; }
    public TaskStatus Status { get; set; }
}