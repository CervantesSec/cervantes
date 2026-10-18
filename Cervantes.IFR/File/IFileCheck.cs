namespace Cervantes.IFR.File;

public interface IFileCheck
{
     bool CheckFile(byte[] file);
     bool CheckImage(byte[] file);
     string GetExtension(byte[] file);


}