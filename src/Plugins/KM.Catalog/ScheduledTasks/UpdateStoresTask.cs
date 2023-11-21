namespace Km.Catalog.ScheduledTasks;

public partial class UpdateStoresTask : IScheduleTask
{
    #region fields
    private readonly IRepository<StoreSnapshot> _storeSnapshotRepository;
    private readonly IStoreService _storeService;
    private readonly IStorageManager _storageManager;
    private readonly IEventPublisher _eventPublisher;
    private readonly ISettingService _settingService;
    private readonly IPictureService _pictureService;
    private readonly IMediaItemInfoService _mediaItemInfoService;
    private readonly ILogger _logger;

    private static Queue<DateTime> _updateRequestQueue = new();

    #endregion

    #region ctor
    public UpdateStoresTask(
        IRepository<StoreSnapshot> storeSnapshotRepository,
        IStoreService storeService,
        IEventPublisher eventPublisher,
        IStorageManager storageManager,
        ISettingService settingService,
        IPictureService pictureService,
        IMediaItemInfoService mediaItemInfoService,
        ILogger logger)
    {
        _storeSnapshotRepository = storeSnapshotRepository;
        _storeService = storeService;
        _eventPublisher = eventPublisher;
        _storageManager = storageManager;
        _settingService = settingService;
        _pictureService = pictureService;
        _logger = logger;
        _mediaItemInfoService = mediaItemInfoService;
    }

    #endregion

    internal static void EnqueueStoresUpdateRequest() =>
        _updateRequestQueue.Enqueue(DateTime.UtcNow);

    public async Task ExecuteAsync()
    {

        if (_updateRequestQueue.Count == 0)
            return;

        var cur = DateTime.UtcNow;
        var iteration = TimeSpan.FromMinutes(5);

        //remove all update request from the past 5 minutes
        while (_updateRequestQueue.TryPeek(out var result) && cur - result <= iteration)
            _updateRequestQueue.Dequeue();

        await _logger.InformationAsync("Start store updating process");

        var last = await _storeSnapshotRepository.GetLatestAsync();

        var v = (uint)1;
        if (last != null && last.Version < uint.MaxValue)
            v = last.Version++;

        var snapshot = new StoresSnapshotInfo
        {
            stores = await GetStoreInfoAsync(),
            version = v
        };

        var e = new StoreSnapshot { Json = toJson(), Version = v };
        await _storeSnapshotRepository.InsertAsync(e);
        
        try
        {
            await _storageManager.UploadAsync("catalog/stores.json", "application /json", snapshot);
        }
        catch (Exception ex)
        {
            EnqueueStoresUpdateRequest();
            throw ex;
        }
        await _eventPublisher.PublishAsync(new EntityUpdatedEvent<StoreSnapshot>(e));

        string toJson()
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };

            return JsonSerializer.Serialize(snapshot, options);
        }
    }

    private async Task<IEnumerable<StoreInfo>> GetStoreInfoAsync()
    {
        var stores = await _storeService.GetAllStoresAsync();
        var res = new List<StoreInfo>();

        foreach (var store in stores)
        {
            var sis = await _settingService.LoadSettingAsync<StoreInformationSettings>(store.Id);
            if (sis.StoreClosed)
                continue;

            await _logger.InformationAsync($"{nameof(UpdateStoresTask)}: start procesing store: {store.Name}, with Id: {store.Id}");
            var logoPicture = await _pictureService.GetPictureByIdAsync(sis.LogoPictureId);
            await _logger.InformationAsync($"{nameof(UpdateStoresTask)}: Upload store logo thumbnail");
            var thumb = await _mediaItemInfoService.GetOrCreateMediaItemInfoAsync(Consts.MediaTypes.Thumbs, logoPicture, 0);
            await _logger.InformationAsync($"{nameof(UpdateStoresTask)}: Upload store logo image");
            var pic = await _mediaItemInfoService.GetOrCreateMediaItemInfoAsync(Consts.MediaTypes.Images, logoPicture, 0);

            res.Add(new StoreInfo
            {
                id = store.Id.ToString(),
                name = store.Name,
                logoThumb = thumb,
                logoPicture = pic
            });
        }
        return res;
    }
}
