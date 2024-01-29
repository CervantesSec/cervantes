/*using System.ComponentModel;
using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages;

public partial class Chat: ComponentBase
{
    [CascadingParameter] public HubConnection hubConnection { get; set; }
    [Parameter] public string CurrentMessage { get; set; }
    [Parameter] public string CurrentUserId { get; set; }
    [Parameter] public string CurrentUserEmail { get; set; }
    private List<ChatMessage> messages = new List<ChatMessage>();
    private async Task SubmitAsync()
    {
        if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(ContactId))
        {
            var chatHistory = new ChatMessage()
            {
                Message = CurrentMessage,
                ToUserId = ContactId,
                CreatedDate = DateTime.Now.ToUniversalTime()
            };
            await Http.PostAsJsonAsync("api/Chat/Send", chatHistory);
            chatHistory.FromUserId = CurrentUserId;
            await hubConnection.SendAsync("SendMessageAsync", chatHistory, CurrentUserEmail);
            CurrentMessage = string.Empty;
        }
    }
    protected override async Task OnInitializedAsync()
    {
        if (hubConnection == null)
        {
            hubConnection = new HubConnectionBuilder().WithUrl(navigationManager.ToAbsoluteUri("/chatHub")).Build();
        }
        if (hubConnection.State == HubConnectionState.Disconnected)
        {
            await hubConnection.StartAsync();
        }
        hubConnection.On<ChatMessage, string>("ReceiveMessage", async (message, userName) =>
        {
            if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId) || (ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
            {
                   
                if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId))
                {
                    messages.Add(new ChatMessage { Message = message.Message, CreatedDate = message.CreatedDate, FromUser = new ApplicationUser() { Email = CurrentUserEmail } } );
                    await hubConnection.SendAsync("ChatNotificationAsync", $"New Message From {userName}", ContactId, CurrentUserId);
                }
                else if ((ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
                {
                    messages.Add(new ChatMessage { Message = message.Message, CreatedDate = message.CreatedDate, FromUser = new ApplicationUser() { Email = ContactEmail } });
                }
                StateHasChanged();
            }
        });
        await GetUsersAsync();
        var state = await stateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        CurrentUserId = user.Claims.Where(a => a.Type == "sub").Select(a => a.Value).FirstOrDefault();
        CurrentUserEmail = user.Claims.Where(a => a.Type == "name").Select(a => a.Value).FirstOrDefault();
        if (!string.IsNullOrEmpty(ContactId))
        {
            await LoadUserChat(ContactId);
        }
    }
    public List<ApplicationUser> ChatUsers = new List<ApplicationUser>();
    [Parameter] public string ContactEmail { get; set; }
    [Parameter] public string ContactId { get; set; }
    async Task LoadUserChat(string userId)
    {
        var contact = await Http.GetFromJsonAsync<ApplicationUser>($"api/Chat/Users/{userId}");
        
        ContactId = contact.Id;
        ContactEmail = contact.FullName;
        navigationManager.NavigateTo($"chat/{ContactId}");
        messages = new List<ChatMessage>();
        messages = await Http.GetFromJsonAsync<List<ChatMessage>>($"api/Chat/{ContactId}");
        
    }
    private async Task GetUsersAsync()
    {
        ChatUsers = await Http.GetFromJsonAsync<List<ApplicationUser>>($"api/Chat/Users");
    }
}*/