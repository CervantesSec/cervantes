using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace Cervantes.CORE.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public string Role { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public Vector? Embedding { get; set; }
    public int MessageIndex { get; set; }  // Orden del mensaje en el chat
}