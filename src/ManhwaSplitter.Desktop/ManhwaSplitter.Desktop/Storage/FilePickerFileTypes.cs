using Avalonia.Platform.Storage;

namespace ManhwaSplitter.Desktop.Storage;

public static class FilePickerFileTypes
{
    public static readonly FilePickerFileType Images = new("Images")
    {
        Patterns = ["*.png", "*.pbm", "*.webp", "*.tif", "*.tiff", "*.jpg", "*.jpeg", "*.qoi", "*.tga", "*.bmp"],
        AppleUniformTypeIdentifiers = ["public.image", "public.png", "public.pbm", "public.webp", "public.tiff", "public.jpeg", "com.truevision.tga-image", "com.microsoft.bmp"],
        MimeTypes = ["image/*", "image/png", "image/x-portable-bitmap", "image/webp", "image/tiff", "image/jpeg", "image/x-tga", "image/bmp"]
    };
}