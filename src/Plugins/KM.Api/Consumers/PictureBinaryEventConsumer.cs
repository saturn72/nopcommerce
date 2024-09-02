using KM.Api.Services.Media;
using Microsoft.Extensions.Caching.Memory;
using Nop.Services.Media;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace KM.Api.Consumer;

public class PictureBinaryEventConsumer :
    IConsumer<EntityInsertedEvent<PictureBinary>>,
    IConsumer<EntityUpdatedEvent<PictureBinary>>,
    IConsumer<EntityDeletedEvent<Picture>>
{
    private readonly IStorageManager _storageManager;
    private readonly IPictureService _pictureService;
    private readonly IPriorityQueue _queue;
    private readonly IReadOnlyDictionary<string, ResizeOptions> _resizeOptions;


    public PictureBinaryEventConsumer(
            IStorageManager storageManager,
            IPictureService pictureService,
            IPriorityQueue queue
        )
    {
        _storageManager = storageManager;
        _pictureService = pictureService;
        _queue = queue;

        _resizeOptions = new Dictionary<string, ResizeOptions> {
            {
                StorageConsts.Thumbnail,
                new() {
                    Size = new()
                    {
                        Height = 0,
                        Width = 74,
                    },
                    Mode = ResizeMode.Min,
                    Sampler = KnownResamplers.Lanczos3,
                }
            },
            {
                StorageConsts.Image,
                new()
                {
                    Size = new()
                    {
                        Height = 0,
                        Width = 120,
                    },
                    Mode = ResizeMode.Min,
                    Sampler = KnownResamplers.Lanczos3,
                }
            }
        };
    }

    public Task HandleEventAsync(EntityInsertedEvent<PictureBinary> eventMessage) => AddToUploadQueue(eventMessage.Entity);

    public Task HandleEventAsync(EntityUpdatedEvent<PictureBinary> eventMessage) => AddToUploadQueue(eventMessage.Entity);

    private Task AddToUploadQueue(PictureBinary pictureBinary)
    {
        _queue.Enqueue(BuildCacheKey(pictureBinary.PictureId), pictureBinary, UploadImageAsync);
        return Task.CompletedTask;
    }

    private string BuildWebpPath(string type, int pictureId) => $"/{type}/{pictureId}.webp";

    private async Task UploadImageAsync(PictureBinary pictureBinary)
    {
        var product = await _pictureService.GetPictureByIdAsync(pictureBinary.PictureId);

        foreach (var ro in _resizeOptions)
        {
            var p = BuildWebpPath(ro.Key, product.Id);
            using var inStream = new MemoryStream(pictureBinary.BinaryData);
            using var image = await Image.LoadAsync(inStream);
            using var outStream = new MemoryStream();
            {
                await image.SaveAsWebpAsync(outStream);
                image.Mutate(i => i.Resize(ro.Value));
                using var ms = new MemoryStream(outStream.GetBuffer());
                await _storageManager.UploadAsync(p, "image/webp", outStream.GetBuffer());
            }
        }
    }

    //we use pictureId and not pictureBinaryId to enable removal on picure deletion
    private string BuildCacheKey(int pictureId) => $"picture-binary-consumer:picture-id={pictureId}";

    public Task HandleEventAsync(EntityDeletedEvent<Picture> eventMessage)
    {
        var tasks = new List<Task>();
        _queue.Dequeue(BuildCacheKey(eventMessage.Entity.Id));
        foreach (var roKey in _resizeOptions.Keys)
            tasks.Add(_storageManager.DeleteAsync(BuildWebpPath(roKey, eventMessage.Entity.Id)));

        return Task.WhenAll(tasks);
    }

}
