namespace Cervantes.CORE.ViewModel;

public class ProfileEdit
{
    public string Id { get; set; }
    public string FullName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    /// <summary>
    /// Phone Number
    /// </summary>
    public string PhoneNumber { get; set; }
    

    /// <summary>
    /// user Bio description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// User Position
    /// </summary>
    public string Position { get; set; }
}