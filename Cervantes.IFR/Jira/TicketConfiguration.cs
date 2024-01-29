namespace Cervantes.IFR.Jira;

public class TicketConfiguration
{ 
        public string IssueType { get; set; }
        public Dictionary<string, string> Risk { get; set; }
        public List<string> Labels { get; set; }
        public List<string> Components { get; set; }
    
}