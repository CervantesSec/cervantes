namespace Cervantes.IFR.CervantesAI;

public class AiConfiguration: IAiConfiguration
{
    public bool Enabled { get; set; }
    public string Type { get; set; }
    public string ApiKey { get; set; }
    public string Endpoint { get; set; }
    public string Model { get; set; }
    public int MaxTokens { get; set; }
    

}