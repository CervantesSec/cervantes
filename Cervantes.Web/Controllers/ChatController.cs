using System.Security.Claims;
using Cervantes.Contracts;
using Cervantes.CORE.Entities;
using Cervantes.CORE.ViewModel;
using Cervantes.IFR.CervantesAI;
using Cervantes.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel.Services;
using Markdig;

namespace Cervantes.Web.Controllers;

public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger = null;
    private IChatManager chatManager = null;
    private IChatMessageManager chatMessageManager = null;
    private readonly IWebHostEnvironment env;
    private IHttpContextAccessor HttpContextAccessor;
    private string aspNetUserId;
    private Sanitizer Sanitizer;
    private readonly IAiService _aiService = null;
    private IDocumentManager documentManager = null;
    private IProjectManager projectManager = null;
    
    public ChatController(IWebHostEnvironment env, ILogger<ChatController> logger,IHttpContextAccessor HttpContextAccessor, 
        IChatManager chatManager, IChatMessageManager chatMessageManager ,Sanitizer Sanitizer, IAiService _aiService,
        IDocumentManager documentManager, IProjectManager projectManager)
    {
        this.chatManager = chatManager;
        this.chatMessageManager = chatMessageManager;
        this.env = env;
        _logger = logger;
        aspNetUserId = HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        this.Sanitizer = Sanitizer;
        this._aiService = _aiService;
        this.documentManager = documentManager;
        this.projectManager = projectManager;
    }
    
    public IEnumerable<Chat> GetChats()
    {
        return chatManager.GetAll().Where(x => x.UserId == aspNetUserId);
    }
    
    public IEnumerable<ChatMessageViewModel> GetMessages(Guid chatId)
    {
        var messages = chatMessageManager.GetAll().Where(x => x.ChatId == chatId);
        var chatMessages = new List<ChatMessageViewModel>();
        foreach (var item in messages)
        {
            chatMessages.Add(new ChatMessageViewModel
            {
                Id = item.Id,
                ChatId = item.ChatId,
                Content = item.Content,
                IsUser = item.Role == "User",
                Timestamp = item.Timestamp,
                MessageIndex = item.MessageIndex
            });
        }
        return chatMessages;
    }
    
    public async Task<Chat> CreateChat(CreateChatViewModel model)
    {
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow,
            UserId = aspNetUserId
        };

        await chatManager.AddAsync(chat);
        await chatManager.Context.SaveChangesAsync();
        switch (model.Type)
        {
            case "Project":
                var project = projectManager.GetById(model.ProjectId);
                if (project != null)
                {
                    await _aiService.AddProject(chat.Id, project.Id);
                }
                break;
            case "Document":
                var doc = documentManager.GetById(model.DocumentId);
                if (doc != null)
                {
                    await _aiService.AddDocument(chat.Id, env.WebRootPath+"/"+doc.FilePath); 
                }
                break;
        }
        
        return chat;
    }
    
    public async Task<Chat> EditChat(Guid chatId, string title)
    {
        var chat = chatManager.GetById(chatId);
        if (chat == null)
        {
            return null;
        }
        
        chat.Title = title;

        await chatManager.Context.SaveChangesAsync();
        return chat;
    }
    
    public async Task<bool> DeleteChat(Guid chatId)
    {
        try
        {
            var chat = chatManager.GetById(chatId);
            if (chat == null)
            {
                return false;
            }
            chatManager.Remove(chat);
            await chatManager.Context.SaveChangesAsync();
            var messages = chatMessageManager.GetAll().Where(x => x.ChatId == chatId);
            foreach (var item in messages)
            {
                chatMessageManager.Remove(item);
            }
            await chatMessageManager.Context.SaveChangesAsync();
            await _aiService.DeleteContext(chatId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting chat");
            return false;
        }
    }
    
    public async Task<ChatMessageViewModel> AddMessage(ChatMessageCreateViewModel model)
    {
        try{
            
        var index = chatMessageManager.GetAll().Count(x => x.ChatId == model.ChatId);
        var chatMessage = new ChatMessage()
        {
            Id = Guid.NewGuid(),
            ChatId = model.ChatId,
            Content = Sanitizer.Sanitize(model.Content),
            Role = "User",
            Timestamp = DateTime.UtcNow,
            MessageIndex = index+1
        };

        await chatMessageManager.AddAsync(chatMessage);
        await chatMessageManager.Context.SaveChangesAsync();
        var msgId = Guid.NewGuid();
        model.Id = msgId;
        var msg = await _aiService.SendMessage(model);
        var msgConverted = Markdown.ToHtml(msg);
        var chatMessage2 = new ChatMessage()
        {
            Id = msgId,
            ChatId = model.ChatId,
            Content = msgConverted,
            Role = "Assistant",
            Timestamp = DateTime.UtcNow,
            MessageIndex = index+2
        };
        await chatMessageManager.AddAsync(chatMessage2);
        
        var chat = chatManager.GetById(model.ChatId);
        chat.LastMessageAt = DateTime.UtcNow;
        await chatManager.Context.SaveChangesAsync();
        await chatMessageManager.Context.SaveChangesAsync();
        
        ChatMessageViewModel chatMessageViewModel = new ChatMessageViewModel()
        {
            Id = chatMessage.Id,
            ChatId = chatMessage.ChatId,
            Content = msgConverted,
            IsUser = false,
            Timestamp = chatMessage.Timestamp,
            MessageIndex = chatMessage.MessageIndex
        };
        
        return chatMessageViewModel;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error deleting chat");
        throw;
    }
    }
    
}