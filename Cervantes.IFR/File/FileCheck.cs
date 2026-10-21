using System.Xml;
using MimeDetective;

namespace Cervantes.IFR.File;

public class FileCheck: IFileCheck
{
    // Whitelist of dotless extensions, compared against the value returned by GetExtension,
    // which is also the extension used to name the stored file. Active content (html, svg,
    // xhtml, scripts, executables) is rejected because it is not listed here.
    private static readonly HashSet<string> allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        "png", "jpg", "jpeg", "gif", "webp", "bmp", "ico", "tif", "tiff", "heic", "heif", "avif",
        "pdf", "doc", "docx", "xls", "xlsx", "ppt", "pptx",
        "txt", "csv", "rtf", "xml", "json", "zip",
        // Video evidence (webm is detected as mkv, wmv as asf)
        "mp4", "m4v", "mov", "3gp", "mkv", "webm", "avi", "wmv", "asf", "mpg", "mpeg",
        // Network captures
        "pcap", "pcapng", "cap"
    };

    // Uploads that are rendered as <img> (user avatars) accept only these image types.
    private static readonly HashSet<string> allowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        "png", "jpg", "jpeg", "gif", "bmp", "tif", "tiff"
    };

    public FileCheck()
    {
    }

    public bool CheckFile(byte[] file)
    {
        try
        {
            var extension = GetExtension(file);
            return allowedExtensions.Contains(extension);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

    }

    // Stricter than CheckFile, for files users upload as attachments and that are served back from
    // /Attachments: an XML document whose root is XHTML or SVG would execute script when opened.
    // Scanner imports keep CheckFile because they legitimately carry markup (Nmap NSE output).
    public bool CheckAttachment(byte[] file)
    {
        if (!CheckFile(file))
        {
            return false;
        }

        if (!GetExtension(file).Equals("xml", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        try
        {
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore,
                XmlResolver = null
            };
            using var stream = new MemoryStream(file);
            using var reader = XmlReader.Create(stream, settings);
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (reader.NamespaceURI is "http://www.w3.org/1999/xhtml" or "http://www.w3.org/2000/svg")
                {
                    return false;
                }

                return !(reader.NamespaceURI.Length == 0 &&
                         (reader.LocalName.Equals("html", StringComparison.OrdinalIgnoreCase) ||
                          reader.LocalName.Equals("svg", StringComparison.OrdinalIgnoreCase)));
            }
        }
        catch (XmlException)
        {
            // Not well-formed XML: the CheckFile result stands.
        }

        return true;
    }

    public bool CheckImage(byte[] file)
    {
        return allowedImageExtensions.Contains(GetExtension(file));
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
            
            // AVIF shares the ISO BMFF "ftyp" box with MP4; MimeDetective reports it as mp4
            if (file.Length > 12 && file[4] == 0x66 && file[5] == 0x74 && file[6] == 0x79 && file[7] == 0x70)
            {
                var brand = System.Text.Encoding.ASCII.GetString(file, 8, 4);
                if (brand == "avif" || brand == "avis")
                {
                    return "avif";
                }
            }

            // Check for JSON signature (object or array)
            var trimmedContent = fileContent.Trim().TrimStart('﻿');
            if ((trimmedContent.StartsWith("{") && trimmedContent.EndsWith("}")) ||
                (trimmedContent.StartsWith("[") && trimmedContent.EndsWith("]")))
            {
                return "json";
            }


            var inspector = new ContentInspectorBuilder() {
                Definitions = new MimeDetective.Definitions.ExhaustiveBuilder() {
                    UsageType = MimeDetective.Definitions.Licensing.UsageType.PersonalNonCommercial
                }.Build()
            }.Build();

            // Skip a UTF-8 BOM so it cannot hide the real signature (e.g. BOM + <html>)
            var content = file;
            if (content.Length >= 3 && content[0] == 0xEF && content[1] == 0xBB && content[2] == 0xBF)
            {
                content = content[3..];
            }

            var results = inspector.Inspect(content);
            var ResultsByFileExtension = results.ByFileExtension();
            string extension;
            if (ResultsByFileExtension.Length != 0)
            {
                extension = ResultsByFileExtension.First().Extension;

            }else
            {
                extension = "txt";
            }
            return extension;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }
    
    

}