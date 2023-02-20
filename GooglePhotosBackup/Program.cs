using Google.Apis.Auth.OAuth2;
using Google.Apis.PhotosLibrary.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GooglePhotosBackup
{
    internal class Program
    {
        private const string LocalPath = @"E:\My Pictures\Zenas";
        private const string ServiceKeyFilePath = "serviceKey.json";

        private static async Task Main(string[] args)
        {
            //var configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            //    .Build();

            //var apiKey = configuration.GetSection("GooglePhotosBackup")["ApiKey"];

            //var service = new PhotosLibraryService(new BaseClientService.Initializer()
            //{
            //    ApplicationName = "GooglePhotosBackup",
            //    ApiKey = apiKey
            //});

            var service = await CreatePhotosLibraryService(ServiceKeyFilePath);

            var backup = new PhotoBackup(service);
            await backup.DownloadAllMediaItems(LocalPath);
        }

        private static async Task<PhotosLibraryService> CreatePhotosLibraryService(string jsonKeyFilePath)
        {
            UserCredential credential;
            await using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                var clientSecrets = await GoogleClientSecrets.FromStreamAsync(stream);

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets.Secrets,
                    new[] { PhotosLibraryService.Scope.PhotoslibraryReadonly },
                    "user", CancellationToken.None, new FileDataStore("Photos.Download"));
            }

            var service = new PhotosLibraryService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Photos Backup",
            });

            return service;
        }
    }
}