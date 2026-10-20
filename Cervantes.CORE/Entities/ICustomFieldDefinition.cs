namespace Cervantes.CORE.Entities;

/// <summary>
/// The columns the four custom field definitions (vuln, project, target, client) share, so a data restore can handle
/// them in one place.
/// </summary>
public interface ICustomFieldDefinition
{
    Guid Id { get; set; }
    string Name { get; set; }
    string Label { get; set; }
    string Options { get; set; }
    string DefaultValue { get; set; }
    string Description { get; set; }
    string UserId { get; set; }
}

/// <summary>The columns the four custom field values share.</summary>
public interface ICustomFieldValue
{
    Guid Id { get; set; }
    string Value { get; set; }
    string UserId { get; set; }
}
