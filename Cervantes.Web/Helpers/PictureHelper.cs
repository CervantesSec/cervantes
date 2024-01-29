namespace Cervantes.Server.Helpers;

public static class PictureHelper
{
    // some magic bytes for the most important image formats, see Wikipedia for more
    static readonly List<byte> jpg = new List<byte> { 0xFF, 0xD8 };
    static readonly List<byte> bmp = new List<byte> { 0x42, 0x4D };
    static readonly List<byte> gif = new List<byte> { 0x47, 0x49, 0x46 };
    static readonly List<byte> png = new List<byte> { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A};
    static readonly List<byte> svg_xml_small = new List<byte> { 0x3C, 0x3F, 0x78, 0x6D, 0x6C }; // "<?xml"
    static readonly List<byte> svg_xml_capital = new List<byte> { 0x3C, 0x3F, 0x58, 0x4D, 0x4C }; // "<?XML"
    static readonly List<byte> svg_small = new List<byte> { 0x3C, 0x73, 0x76, 0x67}; // "<svg"
    static readonly List<byte> svg_capital = new List<byte> { 0x3C, 0x53, 0x56, 0x47 }; // "<SVG"
    static readonly List<byte> intel_tiff = new List<byte> { 0x49, 0x49, 0x2A, 0x00};
    static readonly List<byte> motorola_tiff = new List<byte> { 0x4D, 0x4D, 0x00, 0x2A};

    static readonly List< (List<byte> magic, string extension)> imageFormats = new List<(List<byte> magic, string extension)>()
    {
        (jpg, "jpg"),
        (bmp, "bmp"),
        (gif, "gif"),
        (png, "png"),
        (svg_small, "svg"),
        (svg_capital, "svg"),
        (intel_tiff,"tif"),
        (motorola_tiff, "tif"),
        (svg_xml_small, "svg"),
        (svg_xml_capital, "svg")
    };

    public static string TryGetExtension(Byte[] array)
    {
        // check for simple formats first
        foreach (var imageFormat in imageFormats)
        {
            if (array.IsImage(imageFormat.magic))
            {
                if (imageFormat.magic != svg_xml_small && imageFormat.magic != svg_xml_capital)
                    return imageFormat.extension;

                // special handling for SVGs starting with XML tag
                int readCount = imageFormat.magic.Count; // skip XML tag
                int maxReadCount = 1024;

                do
                {
                    if (array.IsImage(svg_small, readCount) || array.IsImage(svg_capital, readCount))
                    {
                        return imageFormat.extension;
                    }
                    readCount++;
                }
                while (readCount < maxReadCount && readCount < array.Length - 1);

                return null;
            }
        }
        return null;
    }

    private static bool IsImage(this Byte[] array, List<byte> comparer, int offset = 0)
    {
        int arrayIndex = offset;
        foreach (byte c in comparer)
        {
            if (arrayIndex > array.Length -1 || array[arrayIndex] != c)
                return false;
            ++arrayIndex;
        }
        return true;
    }
}