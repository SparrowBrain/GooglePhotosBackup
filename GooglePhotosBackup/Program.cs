using Google.Apis.Auth.OAuth2;
using Google.Apis.PhotosLibrary.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Extensions.Configuration;

namespace GooglePhotosBackup
{
    internal class Program
    {
        private const string ClientSecretsJson = "client_secret.json";

        private static async Task Main(string[] args)
        {
            var localPath = GetLocalPath();
            var service = await CreatePhotosLibraryService();

            var backup = new PhotoBackup(service);
            await backup.DownloadAllMediaItems(localPath);
        }

        private static string GetLocalPath()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var localPath = configuration.GetSection("GooglePhotosBackup")["LocalPath"];


            return localPath ?? throw new Exception("Invalid configuration. Missing LocalPath");
        }

        private static async Task<PhotosLibraryService> CreatePhotosLibraryService()
        {
            UserCredential credential;
            await using (var stream = new FileStream(ClientSecretsJson, FileMode.Open, FileAccess.Read))
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