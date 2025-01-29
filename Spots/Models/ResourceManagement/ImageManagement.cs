namespace Spots.ResourceManager;

public static class ImageManagement
{
    public static async Task<ImageFile?> PickImageFromInternalStorage()
    {
        try
        {
            FileResult? result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                // TODO: use dynamic resources
                Title = "Select an image"
            });

            if (result?.ContentType == "image/pgn" ||
                result?.ContentType == "image/jpeg" ||
                result?.ContentType == "image/jpg" ||
                result?.ContentType == "pgn" ||
                result?.ContentType == "jpeg" ||
                result?.ContentType == "jpg")
            {
                return await FileResultToImageFile(result);
            }
            else
            {
                //TODO: Error handling
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR --------------------> " + ex.Message);
            // TODO: Exception Handling
            return null;
        }
    }

    private static async Task<ImageFile?> FileResultToImageFile(FileResult file)
    {
        try
        {
            MemoryStream memStream = new();
            Stream? stream = await FileResultToStream(file);

            if(stream != null)
            {
                stream.CopyTo(memStream);

                return new ImageFile()
                {
                    Bytes = memStream.ToArray(),
                    ContentType = file.ContentType,
                    FileName = file.FileName
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            // TODO: ERROR HANDLING
            Console.WriteLine("ERROR ------------> " +  ex.Message);
            return null;
        }
    }

    #region Utilities
    public static Stream ByteArrayToStream(byte[] bytes)
    {
        return new MemoryStream(bytes);
    }

    private static async Task<Stream?> FileResultToStream(FileResult file)
    {
        return await file.OpenReadAsync();
    }
    #endregion
}
