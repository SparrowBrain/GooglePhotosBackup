using Google.Apis.PhotosLibrary.v1;
using Google.Apis.PhotosLibrary.v1.Data;
using NLog;

namespace GooglePhotosBackup
{
    public class PhotoBackup
    {
        private const int PageSize = 100;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        private readonly PhotosLibraryService _service;

        public PhotoBackup(PhotosLibraryService service)
        {
            _service = service;
        }

        public async Task DownloadAllMediaItems(string localFolderPath)
        {
            await foreach (var mediaItem in GetAllMediaItems())
            {
                var url = mediaItem.BaseUrl;
                var fileName = mediaItem.Filename;
                var localFilePath = Path.Combine(localFolderPath, fileName);

                if (File.Exists(localFilePath))
                {
                    Logger.Debug($"File {fileName} already exists in the destination folder");
                    continue;
                }

                try
                {
                    var stream = await _service.HttpClient.GetStreamAsync(url);
                    await using var fileStream = new FileStream(localFilePath, FileMode.Create);
                    await stream.CopyToAsync(fileStream);
                    Logger.Info($"File {fileName} has been downloaded successfully");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Error downloading file {fileName}");
                }
            }
        }

        private async IAsyncEnumerable<MediaItem> GetAllMediaItems()
        {
            string? pageToken = null;

            do
            {
                var request = _service.MediaItems.Search(new SearchMediaItemsRequest()
                {
                    PageSize = PageSize,
                    PageToken = pageToken,
                });

                var response = await request.ExecuteAsync();

                foreach (var mediaItem in response.MediaItems)
                {
                    Logger.Debug($"Found media item {mediaItem.Filename} with id {mediaItem.Id}");
                    yield return mediaItem;
                }

                pageToken = response.NextPageToken;
            } while (pageToken != null);
        }
    }
}