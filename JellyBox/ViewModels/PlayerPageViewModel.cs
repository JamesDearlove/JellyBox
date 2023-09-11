using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using JellyBox.Models;
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

        [ObservableProperty]
        private BaseMediaItem item;

        [ObservableProperty]
        private bool controlsVisible = true;

        // VLC player requirements
        private LibVLC LibVLC { get; set; }

        [ObservableProperty]
        private MediaPlayer mediaPlayer;

        public PlayerPageViewModel()
        {
            jellyfinService = Ioc.Default.GetService<JellyfinService>();
        }

        ~PlayerPageViewModel()
        {
            MediaPlayer.Stop();
            MediaPlayer.Dispose();
        }


        [RelayCommand]
        public void Initialise(InitializedEventArgs eventArgs)
        {
            LibVLC = new LibVLC(enableDebugLogs: true, eventArgs.SwapChainOptions);
            MediaPlayer = new MediaPlayer(LibVLC);
            
            // TODO: This could be a race condition
            if (Item != null)
            {
                var uri = jellyfinService.GetVideoHLSUri(Item.Id, Item.Id.ToString("N"));
                using var media = new Media(LibVLC, uri);
                MediaPlayer.Play(media);
                MediaPlayer.Media.StateChanged += Media_StateChanged;

                // TODO: This is very dirty, check if the user wants to resume first.
                if (Item.UserData.PlaybackPositionTicks != 0)
                {
                    MediaPlayer.SeekTo(TimeSpan.FromTicks(Item.UserData.PlaybackPositionTicks));
                }

            }
        }

        private void Media_StateChanged(object sender, MediaStateChangedEventArgs e)
        {
            switch (e.State)
            {
                case VLCState.NothingSpecial:
                    break;
                case VLCState.Opening:
                    break;
                case VLCState.Buffering:
                    break;
                case VLCState.Playing:
                    ControlsVisible = true;
                    break;
                case VLCState.Paused:
                    ControlsVisible = true;
                    break;
                case VLCState.Stopped:
                    break;
                case VLCState.Ended:
                    break;
                case VLCState.Error:
                    break;
                default:
                    break;
            }
        }

        public async Task InitialiseAsync(BaseMediaItem mediaItem)
        {
            Item = mediaItem;
        }


        [RelayCommand]
        public void Play()
        {
            if (MediaPlayer.IsPlaying)
            {
                MediaPlayer.Pause();
            }
            else
            {
                MediaPlayer.Play();
            }
        }

        [RelayCommand]
        public void Stop()
        {
            MediaPlayer.Stop();
        }
    }
}
