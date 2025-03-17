using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.Web.Controllers;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using NLog.Targets;
using Severity = MudBlazor.Severity;
using Task = System.Threading.Tasks.Task;

namespace Cervantes.Web.Components.Pages.Chat;

public partial class Chat: ComponentBase
{
    [Inject] private ChatController _chatController { get; set; }
    [Inject] private ProjectController _ProjectController { get; set; }
    [Inject] private DocumentController _DocumentController { get; set; }
   private List<ChatMessageViewModel> _messages = new();
    private List<ChatViewModel> _chatList = new();
    private List<Project> Projects = new();
    private List<Document> Documents = new();
    private string _currentMessage = "";
    private bool _isProcessing;
    private bool _isCreating;
    private Guid _currentChatId = Guid.Empty;
    private List<BreadcrumbItem> _items;
    private bool _isChatListVisible = true;
    private CreateChatViewModel CreateChatViewModel = new CreateChatViewModel();
    CreateChatModelFluentValidator createChatModelFluentValidator = new CreateChatModelFluentValidator();
    MudForm form;
    DialogOptions mediumWidth = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };

    protected override async Task OnInitializedAsync()
    {
        _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem(localizer["home"], href: "/", disabled: false, icon: Icons.Material.Filled.Home),
            new BreadcrumbItem(localizer["chat"], href: null, disabled: true, icon: Icons.Material.Filled.Chat)
        };
      
        var chats = _chatController.GetChats().ToList();
        foreach (var item in chats)
        {
            _chatList.Add(new ChatViewModel
            {
                Id = item.Id,
                Title = item.Title,
                LastMessageAt = item.LastMessageAt
            });
        }

    }

    private string GetMessageClass(bool isUser)
    {
        return isUser ? "mb-4 pa-4 ml-auto" : "mb-4 pa-10 mr-auto";
    }

    private string GetMessageStyle(bool isUser)
    {
        return isUser 
            ? "max-width: 80%; background-color: #3459e6" 
            : "max-width: 80%; background-color: #27272fff";
    }

    private void StartNewChat()
    {
        if (_isChatListVisible)
        {
            _isChatListVisible = false;
            CreateChatViewModel = new CreateChatViewModel();
            CreateChatViewModel.ProjectId = Guid.Empty;
            CreateChatViewModel.DocumentId = Guid.Empty;
            Projects = new List<Project>();
            Projects = _ProjectController.Get().ToList();
            Console.WriteLine(Projects.Count());
            Documents = new List<Document>();
            Documents = _DocumentController.Get().ToList();
        }
        else
        {
            _isChatListVisible = true;
        }
        
        StateHasChanged();
    }
    
    private async Task  CreateChat()
    {
        await form.Validate();

        if (form.IsValid)
        {
            _isCreating = true;
            var response =  await _chatController.CreateChat(CreateChatViewModel);
            if (response != null)
            {
                Snackbar.Add(@localizer["chatCreated"], Severity.Success);
                var newChat = new ChatViewModel
                {
                    Id = response.Id,
                    Title = response.Title,
                    LastMessageAt = response.LastMessageAt
                };
                _chatList.Add(newChat);
                //LoadChat(newChat.Id);
                _currentChatId = newChat.Id;
                _isChatListVisible = true;
                _isCreating = false;
                StateHasChanged();
            }
            else
            {
                Snackbar.Add(@localizer["chatCreatedError"], Severity.Error);
            }

        }
    }
    
    private async Task  DeleteChat(ChatViewModel chat)
    {
        var parameters = new DialogParameters { ["chat"]=chat };

        var dialog =  await Dialog.ShowEx<DeleteChatDialog>("Edit", parameters,mediumWidth);
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            _chatList.Remove(chat);
            _currentChatId = Guid.Empty;
            StateHasChanged();
        }
        
    }
    
    public class CreateChatModelFluentValidator : AbstractValidator<CreateChatViewModel>
    {
        public CreateChatModelFluentValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(1,100);
        }
        
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<CreateChatViewModel>.CreateWithOptions((CreateChatViewModel)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
    private void LoadChat(Guid chatId)
    {
        _currentChatId = chatId;
        _messages.Clear();
        _messages = _chatController.GetMessages(chatId).ToList();
    }

    private async Task SendMessage()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_currentMessage))
                return;

            if (_currentChatId == Guid.Empty)
            {
                StartNewChat();
            }

            var userMessage = new ChatMessageCreateViewModel()
            {
                Content = _currentMessage,
                IsUser = true,
                ChatId = _currentChatId,
                Timestamp = DateTime.Now
            };
            
            ChatMessageViewModel msg = new ChatMessageViewModel()
            {
                Id = Guid.NewGuid(),
                ChatId = _currentChatId,
                Content = _currentMessage,
                IsUser = true,
                Timestamp = DateTime.Now
            };
            _messages.Add(msg);
            
            var messageToSend = _currentMessage;
            _currentMessage = "";
            _isProcessing = true;

            var response = await _chatController.AddMessage(userMessage);
            if (response != null)
            {
                _messages.Add(response);
                // Update chat list
                var currentChat = _chatList.FirstOrDefault(c => c.Id == _currentChatId);
                if (currentChat != null)
                {
                    currentChat.LastMessageAt = DateTime.Now;
                }

                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add("Failed to get AI response. Please try again.", Severity.Error);
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            _isProcessing = false;
        }
    }
}