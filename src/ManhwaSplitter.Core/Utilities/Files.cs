namespace ManhwaSplitter.Core.Utilities;

public static class Files
{
    public static bool IsImageExtension(string extension)
    {
        return extension switch
        {
            ".png" => true,
            ".pbm" => true,
            ".webp" => true,
            ".tif" => true,
            ".tiff" => true,
            ".jpg" => true,
            ".jpeg" => true,
            ".qoi" => true,
            ".tga" => true,
            ".bmp" => true,
            _ => false
        };
    }
}