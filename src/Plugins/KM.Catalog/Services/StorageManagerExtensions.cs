namespace Km.Catalog.Services;

public static class StorageManagerExtensions
{
    public static async Task UploadAsync<TData>(this IStorageManager storageManager, string path, string mediaType, TData data)
    {
        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
        };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(data, options);
        using var stream = new MemoryStream(bytes);
        {
            _ = await storageManager.UploadAsync(path, mediaType, stream);
        }
    }
}
