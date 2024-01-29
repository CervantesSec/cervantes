namespace Cervantes.CORE.ViewModel;

public class TaskNoteEditViewModel
{
    public Guid Id { get; set; }
    /// <summary>
    /// Task Name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Task Note description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Id Project
    /// </summary>
    public Guid TaskId { get; set; }
}