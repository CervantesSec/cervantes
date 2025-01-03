namespace Cervantes.IFR.CervantesAI;

public class TextEmbedding
{
    public bool Enabled { get; set; }
    public string TextEmbeddingType { get; set; }
    public string TextEmbeddingModel { get; set; }
    public string TextEmbeddingEndpoint { get; set; }
    public string TextEmbeddingApiKey { get; set; }
}