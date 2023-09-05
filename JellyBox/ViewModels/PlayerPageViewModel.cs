using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using JellyBox.Services;
using LibVLCSharp.Platforms.UWP;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace JellyBox.ViewModels
{
    [INotifyPropertyChanged]
    public partial class PlayerPageViewModel
    {
        private readonly JellyfinService jellyfinService;

        public PlayerPageViewModel()
        {
            jellyfinService = Ioc.Default.GetService<JellyfinService>();
        }

        ~PlayerPageViewModel()
        {
            MediaPlayer.Stop();
            MediaPlayer.Dispose();
        }


        private LibVLC LibVLC { get; set; }

        [ObservableProperty]
        private MediaPlayer mediaPlayer;

        [RelayCommand]
        public async void Initialise(InitializedEventArgs eventArgs)
        {
            LibVLC = new LibVLC(enableDebugLogs: true, eventArgs.SwapChainOptions);
            MediaPlayer = new MediaPlayer(LibVLC);

            var items = await jellyfinService.GetUserResumeItems();
            var selectedItem = items.FirstOrDefault();
            Console.Write(selectedItem.Name);

            if (selectedItem != null)
            {
                var uri = jellyfinService.GetVideoHLSUri(selectedItem.Id, selectedItem.Id.ToString("N"));
                using var media = new Media(LibVLC, uri);
                MediaPlayer.Play(media);
            }
        }


        [RelayCommand]
        public void Play()
        {
            //var uri = jellyfinService.GetVideoHLSUri(selectedItem.Id, selectedItem.Id.ToString("N"));
            //using var media = new Media(LibVLC, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4"));
            //using var media = new Media(LibVLC, uri);
            //MediaPlayer.Play(media);
        }
    }
}
