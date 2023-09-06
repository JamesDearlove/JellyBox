using CommunityToolkit.Mvvm.DependencyInjection;
using JellyBox.Models;
using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using SystemException = Jellyfin.Sdk.SystemException;

namespace JellyBox.Services
{
    public class JellyfinService    
    {
        private readonly SdkClientSettings _sdkClientSettings;
        private readonly IDynamicHlsClient _dynamicHlsClient;
        private readonly IItemsClient _itemsClient;
        private readonly ISystemClient _systemClient;
        private readonly IUserClient _userClient;
        private readonly IUserViewsClient _userViewsClient;
        private readonly ITvShowsClient _tvShowsClient;

        public PublicSystemInfo PublicSystemInfo { get; private set; }
        public UserDto LoggedInUser { get; private set; }

        public Blurhash.UWP.Decoder BlurhashDecoder = new Blurhash.UWP.Decoder();

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleService"/> class.
        /// </summary>
        /// <param name="sdkClientSettings">Instance of the <see cref="_sdkClientSettings"/>.</param>
        /// <param name="systemClient">Instance of the <see cref="ISystemClient"/> interface.</param>
        /// <param name="userClient">Instance of the <see cref="IUserClient"/> interface.</param>
        /// <param name="userViewsClient">Instance of the <see cref="IUserViewsClient"/> interface.</param>
        public JellyfinService(
            SdkClientSettings sdkClientSettings,
            IDynamicHlsClient dynamicHlsClient,
            IItemsClient itemsClient,
            ISystemClient systemClient,
            IUserClient userClient,
            IUserViewsClient userViewsClient,
            ITvShowsClient tvShowsClient)
        {
            _sdkClientSettings = sdkClientSettings;
            _dynamicHlsClient = dynamicHlsClient;
            _itemsClient = itemsClient;
            _systemClient = systemClient;
            _userClient = userClient;
            _userViewsClient = userViewsClient;
            _tvShowsClient = tvShowsClient;
        }

        // Temp code taken directly from SDK sample app
        public async Task<UserDto> RunAsync()
        {
            var validServer = false;
            do
            {
                // Prompt for server url.
                // Url must be proto://host/path
                // ex: https://demo.jellyfin.org/stable
                Console.Write("Server Url: ");
                //var host = "https://demo.jellyfin.org/stable";
                var host = "https://jimmyfin.jimmyd.dev";

                _sdkClientSettings.BaseUrl = host;
                try
                {
                    // Get public system info to verify that the url points to a Jellyfin server.
                    var systemInfo = await _systemClient.GetPublicSystemInfoAsync()
                        .ConfigureAwait(false);
                    validServer = true;

                    PublicSystemInfo = systemInfo;
                    Console.WriteLine($"Connected to {host}");
                    Console.WriteLine($"Server Name: {systemInfo.ServerName}");
                    Console.WriteLine($"Server Version: {systemInfo.Version}");
                }
                catch (InvalidOperationException ex)
                {
                    Console.Error.WriteLine("Invalid url");
                    Console.Error.WriteLine(ex);
                }
                catch (SystemException ex)
                {
                    Console.Error.WriteLine($"Error connecting to {host}");
                    Console.Error.WriteLine(ex);
                }
            }
            while (!validServer);

            var validUser = false;

            UserDto userDto = null!;
            do
            {
                try
                {
                    Console.Write("Username: ");
                    var username = "anotheruser";

                    Console.Write("Password: ");
                    var password = "";

                    Console.WriteLine($"Logging into {_sdkClientSettings.BaseUrl}");

                    // Authenticate user.
                    var authenticationResult = await _userClient.AuthenticateUserByNameAsync(new AuthenticateUserByName
                    {
                        Username = username,
                        Pw = password
                    })
                        .ConfigureAwait(false);

                    _sdkClientSettings.AccessToken = authenticationResult.AccessToken;
                    userDto = authenticationResult.User;

                    await _userClient.GetCurrentUserAsync().ContinueWith(user =>
                    {
                        LoggedInUser = user.Result;
                    });

                    Console.WriteLine("Authentication success.");
                    Console.WriteLine($"Welcome to Jellyfin - {userDto.Name}");
                    validUser = true;
                }
                catch (UserException ex)
                {
                    Console.Error.WriteLine("Error authenticating.");
                    Console.Error.WriteLine(ex);
                }
            }
            while (!validUser);

            return userDto;
        }


        /// <summary>
        /// Connects to server that is stored in the app data. Throws exception if no server is defined.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Throws if a server URI is not found.</exception>
        public Task<PublicSystemInfo> ConnectToServer()
        {
            var settings = Ioc.Default.GetService<SettingService>();
            var serverUri = settings.ServerUri;

            if (serverUri == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            return ConnectToServer(settings.ServerUri);
        }

        /// <summary>
        /// Connects to server at the given URI.
        /// </summary>
        /// <param name="serverUri">URI of the server to connect to.</param>
        /// <returns></returns>
        public Task<PublicSystemInfo> ConnectToServer(string serverUri)
        {
            _sdkClientSettings.BaseUrl = serverUri;

            return _systemClient.GetPublicSystemInfoAsync();
        }

        /// <summary>
        /// Authenticates with a username and password to the configured server.
        /// Run ConnectToServer before running this.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AuthWithPassword(string username, string password)
        {
            var authResult = await _userClient.AuthenticateUserByNameAsync(
                new AuthenticateUserByName { Username = username, Pw = password }
            ).ConfigureAwait(false);

            _sdkClientSettings.AccessToken = authResult.AccessToken;

            LoggedInUser = authResult.User;

            return authResult;
        }

        /// <summary>
        /// Authenticates with previously stored AccessToken. Throws exception if no token is found.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Task<UserDto> AuthWithToken()
        {
            var settings = Ioc.Default.GetService<SettingService>();
            var token = settings.AccessToken;

            if (token == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _sdkClientSettings.AccessToken = token;
            return _userClient.GetCurrentUserAsync();
        }

        public Task<UserDto> GetCurrentUser() => _userClient.GetCurrentUserAsync();

        public BaseMediaItem ConvertBaseItemDto(BaseItemDto itemDto)
        {
            switch (itemDto.Type)
            {
                case BaseItemKind.Movie:
                    return new Movie(itemDto);
                case BaseItemKind.Series:
                    return new TvShowSeries(itemDto);
                case BaseItemKind.Season:
                    return new TvShowSeason(itemDto);
                case BaseItemKind.Episode:
                    return new TvShowEpisode(itemDto);
                default:
                    return new BaseMediaItem(itemDto);
            }
        }

        public async Task<IList<BaseMediaItem>> GetUserResumeItems()
        {
            var apiResult = await _itemsClient.GetResumeItemsAsync(LoggedInUser.Id);
            return apiResult.Items.Select(x => ConvertBaseItemDto(x)).ToList();
        }


        public async Task<IList<TvShowSeason>> GetSeriesSeasons(Guid id)
        {
            var apiResult = await _tvShowsClient.GetSeasonsAsync(id);
            return apiResult.Items.Select(x => new TvShowSeason(x)).ToList();
        }

        public async Task<IList<TvShowEpisode>> GetSeriesEpisodes(Guid seriesId, Guid seasonId)
        {
            var apiResult = await _tvShowsClient.GetEpisodesAsync(seriesId, seasonId: seasonId);
            return apiResult.Items.Select(x => new TvShowEpisode(x)).ToList();
        }


        //public async void GetVideoStreamUri()
        //{
        //    var result = await _dynamicHlsClient.GetMasterHlsVideoPlaylistAsync()

        //}

        public Uri GetVideoHLSUri(Guid id, string mediaSourceId)
        {
            return new Uri($"{_sdkClientSettings.BaseUrl}/videos/{id}/master.m3u8?api_key={_sdkClientSettings.AccessToken}&MediaSourceId={mediaSourceId}");
        }

        public Uri GetImageUri(Guid itemId, ImageType imageType)
        {
            return new Uri($"{_sdkClientSettings.BaseUrl}/items/{itemId}/Images/{imageType}?api_key={_sdkClientSettings.AccessToken}");
        }

        public Uri GetImageUri(Guid itemId, ImageType imageType, int width, int height)
        {
            return new Uri($"{_sdkClientSettings.BaseUrl}/items/{itemId}/Images/{imageType}?api_key={_sdkClientSettings.AccessToken}&width={width}&height={height}");
        }
    }
}
