namespace Cervantes.IFR.File;

public interface IFileCheck
{
     bool CheckFile(byte[] file);
     bool CheckAttachment(byte[] file);
     // Scanner import files: CheckFile, plus any well-formed XML whose root is not XHTML, SVG or html
     bool CheckImport(byte[] file);
     bool CheckImage(byte[] file);
     string GetExtension(byte[] file);


}