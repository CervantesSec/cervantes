using System.Text.RegularExpressions;
using Cervantes.IFR.CervantesAI;

namespace Cervantes.Tests;

public class ChatMemoryIdsTests
{
    // Kernel Memory accepts only letters, digits, '.', '_' and '-' in a document id.
    private static readonly Regex ValidDocumentId = new("^[A-Za-z0-9._-]+$");

    [Fact]
    public void Ids_AreValidKernelMemoryDocumentIds()
    {
        var chat = Guid.NewGuid();
        Assert.Matches(ValidDocumentId, ChatMemoryIds.Context(chat));
        Assert.Matches(ValidDocumentId, ChatMemoryIds.Message(chat, Guid.NewGuid()));
    }

    [Fact]
    public void Ids_AreStablePerChat()
    {
        var chat = Guid.NewGuid();
        var message = Guid.NewGuid();

        Assert.Equal(ChatMemoryIds.Context(chat), ChatMemoryIds.Context(chat));
        Assert.Equal(ChatMemoryIds.Message(chat, message), ChatMemoryIds.Message(chat, message));
        Assert.NotEqual(ChatMemoryIds.Context(chat), ChatMemoryIds.Context(Guid.NewGuid()));
        Assert.NotEqual(ChatMemoryIds.Message(chat, message), ChatMemoryIds.Message(Guid.NewGuid(), message));
        Assert.NotEqual(ChatMemoryIds.Context(chat), ChatMemoryIds.Message(chat, message));
    }
}
