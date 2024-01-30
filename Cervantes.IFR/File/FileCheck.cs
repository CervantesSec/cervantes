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
    
    public string GetExtension(byte[] file)
    {
        try
        {
            // Convert the byte array to a string for easier inspection
            string fileContent = System.Text.Encoding.UTF8.GetString(file);

           
           
            
            // Check for docx, xlsx, pptx, and zip signatures
            if (file.Length > 4 && file[0] == 0x50 && file[1] == 0x4B && file[2] == 0x03 && file[3] == 0x04)
            {
                
                if (fileContent.Trim().StartsWith("{") && fileContent.Trim().EndsWith("}"))
                {
                    return "json";
                }
                else if (fileContent.Contains("word/"))
                {
                    return "docx";
                }
                else if (fileContent.Contains("xl/"))
                {
                    return "xlsx";
                }
                else if (fileContent.Contains("ppt/"))
                {
                    return "pptx";
                }
            }
            
            // Check for JSON signature
            if (fileContent.Trim().StartsWith("{") && fileContent.Trim().EndsWith("}"))
            {
                return "json";
            }

            
            var inspector = new ContentInspectorBuilder() {
                Definitions = new MimeDetective.Definitions.ExhaustiveBuilder() {
                    UsageType = MimeDetective.Definitions.Licensing.UsageType.PersonalNonCommercial
                }.Build()
            }.Build();
            
        
            var results = inspector.Inspect(file);
            var ResultsByFileExtension = results.ByFileExtension();
            
            return ResultsByFileExtension.First().Extension;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    

}