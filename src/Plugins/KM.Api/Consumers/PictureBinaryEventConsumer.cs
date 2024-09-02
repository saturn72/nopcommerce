using KM.Api.Services.Media;
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
    private readonly IReadOnlyDictionary<string, ResizeOptions> _resizeOptions;

    public PictureBinaryEventConsumer(
            IStorageManager storageManager,
            IPictureService pictureService)
    {
        _storageManager = storageManager;
        _pictureService = pictureService;

        _resizeOptions = new Dictionary<string, ResizeOptions> {
            {"thumbnail", new() {
                Size = new()
                {
                    Height = 0,
                    Width = 74,
                },
                Mode = ResizeMode.Min,
                Sampler = KnownResamplers.Lanczos3,
            }
            },
            {"image", new()
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
    private string BuildPath(int pictureId) => $"pictures/{pictureId}";

    public Task HandleEventAsync(EntityInsertedEvent<PictureBinary> eventMessage) => AddOrUpdatePictureBinaries(eventMessage.Entity);

    public Task HandleEventAsync(EntityUpdatedEvent<PictureBinary> eventMessage) => AddOrUpdatePictureBinaries(eventMessage.Entity);

    private async Task AddOrUpdatePictureBinaries(PictureBinary pictureBinary)
    {
        var p = await _pictureService.GetPictureByIdAsync(pictureBinary.PictureId);


        foreach (var ro in _resizeOptions)
        {
            using var inStream = new MemoryStream(pictureBinary.BinaryData);
            using var image = await Image.LoadAsync(inStream);
            using var outStream = new MemoryStream();
            {
                await image.SaveAsWebpAsync(outStream);
                image.Mutate(i => i.Resize(ro.Value));
                using var ms = new MemoryStream(outStream.GetBuffer());
                await _storageManager.UploadAsync(
                    $"/{ro.Key}/{pictureBinary.PictureId}.webp",
                    "image/webp", outStream.GetBuffer());
            }
        }
    }

    public Task HandleEventAsync(EntityDeletedEvent<Picture> eventMessage) => _storageManager.DeleteAsync(BuildPath(eventMessage.Entity.Id));
}
