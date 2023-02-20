using Google.Apis.PhotosLibrary.v1;
using Google.Apis.PhotosLibrary.v1.Data;

namespace GooglePhotosBackup;

public class PhotoBackup
{
    private const int PageSize = 100;
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
                continue;
            }

            var stream = await _service.HttpClient.GetStreamAsync(url);
            await using var fileStream = new FileStream(localFilePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);
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
                yield return mediaItem;
            }

            pageToken = response.NextPageToken;
        } while (pageToken != null);
    }
}