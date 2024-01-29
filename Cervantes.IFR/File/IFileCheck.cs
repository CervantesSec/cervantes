namespace Cervantes.IFR.File;

public interface IFileCheck
{
     bool CheckFile(byte[] file);
     string GetExtension(byte[] file);


}