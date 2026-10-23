using Cervantes.CORE;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Tests.Web.Controllers;

// SetValues replaces every stored value, so a required field left out of the request must be refused before any
// value is removed (the fake managers throw if the action reaches the database).
public class RequiredCustomFieldTests
{
    private static readonly Guid FieldId = Guid.NewGuid();

    private static string BadRequestText(IActionResult result) =>
        Assert.IsType<string>(Assert.IsType<BadRequestObjectResult>(result).Value);

    [Fact]
    public async Task ProjectSetValues_MissingRequired_Returns400()
    {
        var fields = new ProjectFieldManager();
        fields.Items.Add(new ProjectCustomField { Id = FieldId, Label = "Owner", IsRequired = true, IsActive = true });
        var controller = new ProjectCustomFieldController(fields, null!, new EveryMembership(),
            NullLogger<ProjectCustomFieldController>.Instance, ControllerFakes.Accessor(), null!);

        var result = await controller.SetValues(Guid.NewGuid(), new Dictionary<Guid, string> { [FieldId] = " " });

        Assert.Contains("Owner", BadRequestText(result));
    }

    [Fact]
    public async Task ClientSetValues_MissingRequired_Returns400()
    {
        var fields = new ClientFieldManager();
        fields.Items.Add(new ClientCustomField { Id = FieldId, Label = "Sector", IsRequired = true, IsActive = true });
        var controller = new ClientCustomFieldController(fields, null!, NullLogger<ClientCustomFieldController>.Instance,
            ControllerFakes.Accessor(), null!);

        var result = await controller.SetValues(Guid.NewGuid(), new Dictionary<Guid, string>());

        Assert.Contains("Sector", BadRequestText(result));
    }

    [Fact]
    public async Task TargetSetValues_MissingRequired_Returns400()
    {
        var fields = new TargetFieldManager();
        fields.Items.Add(new TargetCustomField { Id = FieldId, Label = "Environment", IsRequired = true, IsActive = true });
        var targets = new TargetListManager();
        var target = new Target { Id = Guid.NewGuid(), Name = "web" };
        targets.Items.Add(target);
        var controller = new TargetCustomFieldController(fields, null!, targets, new EveryMembership(),
            NullLogger<TargetCustomFieldController>.Instance, ControllerFakes.Accessor(), null!);

        var result = await controller.SetValues(target.Id, new Dictionary<Guid, string>());

        Assert.Contains("Environment", BadRequestText(result));
    }

    [Fact]
    public async Task VulnSetValues_MissingRequired_Returns400()
    {
        var fields = new VulnFieldManager();
        fields.Items.Add(new VulnCustomField { Id = FieldId, Label = "Ticket", IsRequired = true, IsActive = true });
        var vulns = new VulnListManager();
        var vuln = new Vuln { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid() };
        vulns.Items.Add(vuln);
        var controller = new VulnCustomFieldController(fields, null!, vulns, new EveryMembership(),
            NullLogger<VulnCustomFieldController>.Instance, ControllerFakes.Accessor(), null!);

        var result = await controller.SetValues(vuln.Id, new Dictionary<Guid, string>());

        Assert.Contains("Ticket", BadRequestText(result));
    }

    [Fact]
    public async Task VulnSetValues_TemplateIsExempt()
    {
        var fields = new VulnFieldManager();
        fields.Items.Add(new VulnCustomField { Id = FieldId, Label = "Ticket", IsRequired = true, IsActive = true });
        var vulns = new VulnListManager();
        var template = new Vuln { Id = Guid.NewGuid(), Template = true };
        vulns.Items.Add(template);
        var controller = new VulnCustomFieldController(fields, new VulnValueManager(), vulns, new EveryMembership(),
            NullLogger<VulnCustomFieldController>.Instance, ControllerFakes.Accessor(), null!);

        // Reaching the database means the template passed the required-field check.
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => controller.SetValues(template.Id, new Dictionary<Guid, string>()));
        Assert.Equal("the action reached the database", error.Message);
    }

    [Fact]
    public async Task TargetEdit_MissingRequired_Returns400BeforeSaving()
    {
        var fields = new TargetFieldManager();
        fields.Items.Add(new TargetCustomField { Id = FieldId, Label = "Environment", IsRequired = true, IsActive = true });
        var otherField = Guid.NewGuid();
        var targets = new TargetListManager();
        var target = new Target { Id = Guid.NewGuid(), Name = "web" };
        targets.Items.Add(target);
        var controller = new TargetController(targets, null!, null!, new EveryMembership(), NullLogger<TargetController>.Instance,
            null!, null!, null!, ControllerFakes.Accessor(Permissions.TargetsEdit), null!, null!, fields, null!);

        var result = await controller.Edit(new TargetEditViewModel
        {
            Id = target.Id, Name = "renamed",
            CustomFieldValues = new Dictionary<Guid, string> { [otherField] = "x" }
        });

        Assert.Contains("Environment", BadRequestText(result));
        Assert.Equal("web", target.Name);
    }
}
