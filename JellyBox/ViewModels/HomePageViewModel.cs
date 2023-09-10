
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using JellyBox.Models;
using JellyBox.Services;
using Jellyfin.Sdk;
using LibVLCSharp.Platforms.UWP;
using LibVLCSharp.Shared;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace JellyBox.ViewModels
{
    [INotifyPropertyChanged]
    public partial class HomePageViewModel
    {
        public ObservableCollection<BaseMediaItem> ContinueWatchingItems { get; } = new ObservableCollection<BaseMediaItem>();

        [ObservableProperty]
        private string username = "Not Logged In";

        [ObservableProperty]
        private UserDto? loggedInUser;

        [ObservableProperty]
        private PublicSystemInfo? publicSystemInfo;

        [ObservableProperty]
        public BaseItemDto? selectedItem;

        private readonly JellyfinService jellyfinService;

        public HomePageViewModel()
        {
            jellyfinService = Ioc.Default.GetService<JellyfinService>();
        }



        [RelayCommand]
        public void PlayPage()
        {
            Ioc.Default.GetService<INavigationService>().Navigate<PlayerPageViewModel>("");
        }


        [RelayCommand]
        public void LoadPage()
        {
            DoAuth();
        }


        [RelayCommand]
        private async void DoAuth()
        {
            var systemInfo = await jellyfinService.ConnectToServer("https://jimmyfin.jimmyd.dev");

            PublicSystemInfo = systemInfo;

            var authResult = await jellyfinService.AuthWithPassword("anotheruser", "");

            LoggedInUser = authResult.User;
            Username = LoggedInUser.Name;
            
            var items = await jellyfinService.GetUserResumeItems();
            foreach (var item in items)
            {
                ContinueWatchingItems.Add(item);
            }

            PopulateImages();
        }

        private async void PopulateImages()
        {
            foreach (var cwItem in ContinueWatchingItems)
            {
                var uri = jellyfinService.GetImageUri(cwItem.Id, ImageType.Primary, 450, 255);
                cwItem.PrimaryImage = await ImageCache.Instance.GetFromCacheAsync(uri);
            }
        }

        [RelayCommand]
        public void ContinueWatchingClick(BaseMediaItem clickedItem)
        {
            if (clickedItem != null)
            {
                Ioc.Default.GetService<INavigationService>().Navigate<PlayerPageViewModel>(clickedItem);
            }
        }
    }
}
