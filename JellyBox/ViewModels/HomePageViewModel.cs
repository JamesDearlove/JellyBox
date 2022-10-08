#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using JellyBox.Services;
using Jellyfin.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JellyBox.ViewModels
{
    [INotifyPropertyChanged]
    public partial class HomePageViewModel
    {
        public ObservableCollection<object> Items { get; } = new ObservableCollection<object>();

        [ObservableProperty]
        private string username = "Not Attempted";

        [ObservableProperty]
        private UserDto? loggedInUser;

        [ObservableProperty]
        private PublicSystemInfo? publicSystemInfo;

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
        }

    }

}
