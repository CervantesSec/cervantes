using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Cervantes.CORE.ViewModels;
using Cervantes.Web.Controllers;
using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using TaskStatus = Cervantes.CORE.Entities.TaskStatus;

namespace Cervantes.Web.Components.Pages;

public partial class Calendar: ComponentBase
{
    private CalendarViewModel model;
    private List<BreadcrumbItem> _items;
    private List<CustomItem> _events;
    [Inject] private CalendarController _calendarController { get; set; }

    protected override async Task OnInitializedAsync()
    {

        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/",icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(localizer["calendar"], href: null, disabled: true, icon: Icons.Material.Filled.CalendarMonth)
        };
        _events = new List<CustomItem>();
        model = new CalendarViewModel();
        model = _calendarController.Get();

        foreach (var pro in model.Projects)
        {
            
            for(DateTime counter = pro.StartDate; counter <= pro.EndDate; counter = counter.AddDays(1))
            {
                var item = new CustomItem
                {
                    Start = counter,
                    End = counter,
                    Title = pro.Name,
                    Color = Color.Dark
                };
            
                _events.Add(item);
            }
           
        }
        
        foreach (var task in model.Tasks)
        {
            
            for(DateTime counter = task.StartDate; counter <= task.EndDate; counter = counter.AddDays(1))
            {
                var item = new CustomItem();
                item.Start = counter;
                item.End = counter;
                item.Title = task.Name;
                switch (task.Status)
                {
                    case TaskStatus.Backlog:
                        item.Color = Color.Info;
                        break;
                    case TaskStatus.Blocked:
                        item.Color = Color.Error;
                        break;
                    case TaskStatus.Done:
                        item.Color = Color.Success;
                        break;
                    case TaskStatus.InProgress:
                        item.Color = Color.Warning;
                        break;
                    case TaskStatus.ToDo:
                        item.Color = Color.Primary;
                        break;
                    
                }

                _events.Add(item);
            }
           
        }
        

    }
    private string GetColor(Color color) => $"var(--mud-palette-{color.ToDescriptionString()})";

   

    private class CustomItem : CalendarItem
    {
        public string Title { get; set; } = string.Empty;
        public Color Color { get; set; } = Color.Primary;
    }
    
}