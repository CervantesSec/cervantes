namespace Cervantes.IFR.CervantesAI;

/// <summary>
/// Kernel Memory document ids of a chat's records. They are derived from the chat and message ids, so deleting a
/// chat can remove its records by id instead of finding them through a search, which is capped and ranked.
/// </summary>
internal static class ChatMemoryIds
{
    public static string Context(Guid chatId) => $"chat-{chatId}-context";

    public static string Message(Guid chatId, Guid messageId) => $"chat-{chatId}-msg-{messageId}";
}
