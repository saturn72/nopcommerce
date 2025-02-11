using KedemMarket.Common;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Events;
using Nop.Services.Media;

namespace KedemMarket.Navbar.Consumers;
public class ClearNavbarCacheConsumer :
    IConsumer<EntityDeletedEvent<Picture>>,
    IConsumer<EntityUpdatedEvent<Picture>>,

    IConsumer<EntityDeletedEvent<Video>>,
    IConsumer<EntityUpdatedEvent<Video>>,

    IConsumer<EntityDeletedEvent<Category>>,
    IConsumer<EntityInsertedEvent<Category>>,
    IConsumer<EntityUpdatedEvent<Category>>,

    IConsumer<EntityDeletedEvent<ProductPicture>>,
    IConsumer<EntityInsertedEvent<ProductPicture>>,
    IConsumer<EntityUpdatedEvent<ProductPicture>>,

    IConsumer<EntityDeletedEvent<ProductVideo>>,
    IConsumer<EntityInsertedEvent<ProductVideo>>,
    IConsumer<EntityUpdatedEvent<ProductVideo>>,

    IConsumer<EntityDeletedEvent<Vendor>>,
    IConsumer<EntityInsertedEvent<Vendor>>,
    IConsumer<EntityUpdatedEvent<Vendor>>

{
    private readonly IStorageManager _storageManager;
    private readonly IPictureService _pictureService;

    public ClearNavbarCacheConsumer(
        IStorageManager storageManager,
        IPictureService pictureService)
    {
        _storageManager = storageManager;
        _pictureService = pictureService;
    }


    public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
    {
        await DeleteAllImagesFromStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
    {
        await UploadToStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
    {
        await UploadToStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Picture> eventMessage)
    {
        await DeleteAllImagesFromStorageByPictureIdAsync(eventMessage.Entity.Id);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Picture> eventMessage)
    {
        await UploadToStorageByPictureIdAsync(eventMessage.Entity.Id);
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Video> eventMessage)
    {
        await DeleteVideoFromStorageByVideodAsync(eventMessage.Entity.Id);
    }

    public Task HandleEventAsync(EntityUpdatedEvent<Video> eventMessage)
    {
        return Task.CompletedTask;
        //do nothing at this point
        //await UploadVideoToStorageAsync(eventMessage.Entity.VideoUrl);
    }

    public async Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage)
    {
        await DeleteAllImagesFromStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }
    public async Task HandleEventAsync(EntityInsertedEvent<ProductPicture> eventMessage)
    {
        await UploadToStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<ProductPicture> eventMessage)
    {
        await UploadToStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }

    public async Task HandleEventAsync(EntityDeletedEvent<ProductVideo> eventMessage)
    {
        await DeleteVideoFromStorageByVideodAsync(eventMessage.Entity.VideoId);
    }

    public Task HandleEventAsync(EntityInsertedEvent<ProductVideo> eventMessage)
    {
        return Task.CompletedTask;
        //do nothing at this point }
    }
    public Task HandleEventAsync(EntityUpdatedEvent<ProductVideo> eventMessage)
    {
        return Task.CompletedTask;
        //do nothing at this point
    }

    public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage)
    {
        await DeleteAllImagesFromStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }

    public async Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage)
    {
        await UploadToStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }

    public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage)
    {
        await UploadToStorageByPictureIdAsync(eventMessage.Entity.PictureId);
    }
    private async Task UploadToStorageByPictureIdAsync(int pictureId)
    {
        if (pictureId == 0)
            return;

        var paths = GetStorageImagePaths(pictureId);
        var pictureBinary = await _pictureService.GetPictureBinaryByPictureIdAsync(pictureId);

        var tasks = new[]{
            KmConsts.MediaTypes.Thumbnail,
            KmConsts.MediaTypes.Image
        }.Select(mt => _storageManager.UploadByKmMediaTypeAsync(mt, pictureBinary)).ToArray();

        await Task.WhenAll(tasks);
    }

    private async Task DeleteAllImagesFromStorageByPictureIdAsync(int pictureId)
    {
        if (pictureId == 0)
            return;
        var paths = GetStorageImagePaths(pictureId);
        await Task.WhenAll(paths.Select(_storageManager.DeleteAsync).ToArray());
    }

    private string[] GetStorageImagePaths(int pictureId)
    {
        return new[]
        {
            _storageManager.GetWebpPath(KmConsts.MediaTypes.Thumbnail, pictureId),
            _storageManager.GetWebpPath(KmConsts.MediaTypes.Image, pictureId)
        };
    }

    private async Task DeleteVideoFromStorageByVideodAsync(int videoId)
    {
        if (videoId == 0)
            return;

        var path = _storageManager.GetWebpPath(KmConsts.MediaTypes.Video, videoId);
        await _storageManager.DeleteAsync(path);
    }
}
