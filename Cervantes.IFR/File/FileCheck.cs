using MimeDetective;

namespace Cervantes.IFR.File;

public class FileCheck: IFileCheck
{
    private readonly List<string> bannedExtensions = new List<string> { ".exe", ".dll", ".js", ".php", ".py", ".rb", ".java", ".cs", ".ts", ".go", ".sh",".aspx", ".jsp" };
    private readonly List<string> bannedMimeTypes = new List<string> { "application/x-dosexec", "application/x-msdownload", "application/javascript", "application/x-httpd-php", "application/x-python-code", "application/x-ruby", "text/x-java-source", "text/x-csharp", "application/typescript", "text/x-go" };

    public FileCheck()
    {
    }

    public bool CheckFile(byte[] file)
    {
        try
        {

            var inspector = new ContentInspectorBuilder() {
                Definitions = new MimeDetective.Definitions.ExhaustiveBuilder() {
                    UsageType = MimeDetective.Definitions.Licensing.UsageType.PersonalNonCommercial
                }.Build()
            }.Build();
            
        
            var results = inspector.Inspect(file);
            var ResultsByFileExtension = results.ByFileExtension();
            var ResultsByMimeType = results.ByMimeType();

            foreach (var extension in ResultsByFileExtension)
            {
                if (bannedExtensions.Contains(extension.Extension))
                {
                    return false;
                }
            }
            foreach (var mime in ResultsByMimeType)
            {
                if (bannedMimeTypes.Contains(mime.MimeType))
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    

}