namespace Km.Catalog.Services;

public class MediaItemInfoService : IMediaItemInfoService
{
    #region fields

    private readonly IRepository<KmMediaItemInfo> _mediaItemInfoRepository;
    private readonly IRepository<PictureBinary> _pictureBinaryRepository;
    private readonly IStorageManager _storageManager;
    private readonly IReadOnlyDictionary<string, ResizeOptions> _resizeOptions;

    #endregion

    #region ctor
    public MediaItemInfoService(
        IRepository<KmMediaItemInfo> mediaItemInfoRepository,
        IRepository<PictureBinary> pictureBinayRepository,
        IStorageManager storageManager)
    {
        _mediaItemInfoRepository = mediaItemInfoRepository;
        _pictureBinaryRepository = pictureBinayRepository;
        _storageManager = storageManager;

        _resizeOptions = new Dictionary<string, ResizeOptions>
        {
            { Consts.MediaTypes.Thumbnail,
                new()
                {
                    Size = new()
                    {
                        Height = 0,
                        Width = 74,
                    },
                    Mode = ResizeMode.Min,
                    Sampler = KnownResamplers.Lanczos3,
                }
            },{
                Consts.MediaTypes.Image,
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
    #endregion


    public async Task<KmMediaItemInfo> GetOrCreateMediaItemInfoAsync(string type, Picture picture, int displayOrder)
    {
        var e = await _mediaItemInfoRepository.Table
                 .FirstOrDefaultAsync(p =>
                    p.EntityType == typeof(Picture).Name &&
                    p.EntityId == picture.Id &&
                    p.Type == type);

        if (e == default)
        {

            var pb = await _pictureBinaryRepository.Table
                .FirstOrDefaultAsync(pb => pb.PictureId == picture.Id);

            using var inStream = new MemoryStream(pb.BinaryData);
            using var image = await Image.LoadAsync(inStream);
            using var outStream = new MemoryStream();
            {
                await image.SaveAsWebpAsync(outStream);

                image.Mutate(i => i.Resize(_resizeOptions[type]));
                e = new KmMediaItemInfo
                {
                    EntityType = typeof(Picture).Name,
                    EntityId = picture.Id,
                    Type = type,
                    Uri = $"/{type}/{picture.Id}.webp",
                    BinaryData = outStream.GetBuffer(),
                };
            }
            await _mediaItemInfoRepository.InsertAsync(e);
        }

        if (e.Storage == null || e.StorageIdentifier == null)
        {
            IStorageManager.StoredFileInfo sfi = null;
            using var ms = new MemoryStream(e.BinaryData);
            {
                sfi = await _storageManager.UploadAsync(e.Uri, "image/webp", ms);
            }
            if (sfi != null)
            {
                e.Storage = sfi.Storage;
                e.StorageIdentifier = sfi.StorageIdentifier;
                await _mediaItemInfoRepository.UpdateAsync(e);
            }
        }
        return e;
    }
}
