
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using JellyBox.Services;
using Jellyfin.Sdk;
using LibVLCSharp.Platforms.UWP;
using LibVLCSharp.Shared;
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
        public ObservableCollection<BaseItemDto> Items { get; } = new ObservableCollection<BaseItemDto>();

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
        private async void DoAuth()
        {
            var systemInfo = await jellyfinService.ConnectToServer("");

            PublicSystemInfo = systemInfo;

            var authResult = await jellyfinService.AuthWithPassword("", "");

            LoggedInUser = authResult.User;
            Username = LoggedInUser.Name;
            
            var items = await jellyfinService.GetUserResumeItems();
            foreach (var item in items)
            {
                Items.Add(item);
                
            }
        }


    }
}
