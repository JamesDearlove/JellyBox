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

        public PublicSystemInfo PublicSystemInfo { get; private set; }
        public UserDto LoggedInUser { get; private set; }

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
            IUserViewsClient userViewsClient)
        {
            _sdkClientSettings = sdkClientSettings;
            _dynamicHlsClient = dynamicHlsClient;
            _itemsClient = itemsClient;
            _systemClient = systemClient;
            _userClient = userClient;
            _userViewsClient = userViewsClient;
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
                var host = "https://demo.jellyfin.org/stable";

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
                    var username = "demo";

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


        public async Task<IReadOnlyList<BaseItemDto>> GetUserResumeItems()
        {
            var result = await _itemsClient.GetResumeItemsAsync(LoggedInUser.Id);
            
            return result.Items;
        }

        //public async void GetVideoStreamUri()
        //{
        //    var result = await _dynamicHlsClient.GetMasterHlsVideoPlaylistAsync()

        //}

        public Uri GetVideoHLSUri(Guid id, string mediaSourceId)
        {
            return new Uri($"{_sdkClientSettings.BaseUrl}/videos/{id}/master.m3u8?api_key={_sdkClientSettings.AccessToken}&MediaSourceId={mediaSourceId}");
        }
    }
}
