
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
        private string username = "Not Attempted";

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
        private async void AttemptLogin()
        {
            var result = await jellyfinService.RunAsync();

            LoggedInUser = result;
            PublicSystemInfo = jellyfinService.PublicSystemInfo;

            var items = await jellyfinService.GetUserResumeItems();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        private LibVLC LibVLC { get; set; }

        [ObservableProperty]
        private MediaPlayer mediaPlayer;

        [RelayCommand]
        public void Initialise(InitializedEventArgs eventArgs)
        {
            LibVLC = new LibVLC(enableDebugLogs: true, eventArgs.SwapChainOptions);
            MediaPlayer = new MediaPlayer(LibVLC);
        }

        [RelayCommand]
        public void Play()
        {
            var uri = jellyfinService.GetVideoHLSUri(selectedItem.Id, selectedItem.Id.ToString("N"));
            using var media = new Media(LibVLC, uri);
            MediaPlayer.Play(media);
        }

    }
}
