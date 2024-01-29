namespace Cervantes.CORE.ViewModel;

public class TargetServiceEditViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    /// <summary>
    /// Description of the service
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Port of the service
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Version of the service
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Notes of the service
    /// </summary>
    public string Note { get; set; }
    public Guid TargetId { get; set; }
}
