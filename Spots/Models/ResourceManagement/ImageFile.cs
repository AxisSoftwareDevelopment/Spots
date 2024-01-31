namespace Spots;
public class ImageFile
{
    public bool Loaded { get; set; }
    public byte[]? Bytes { get; set; }
    public string? ContentType { get; set; }
    public string? FileName { get; set; }
}