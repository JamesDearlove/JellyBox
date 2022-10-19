using JellyBox.ViewModels;
using JellyBox.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace JellyBox.Services
{
    public interface INavigationService
    {
        bool CanGoBack { get; }
        void GoBack();
        void Navigate<T>(object args = null);
    }

    public class NavigationService : INavigationService
    {
        private readonly Dictionary<Type, Type> viewMapping = new()
        {
            [typeof(HomePageViewModel)] = typeof(HomePage),
        };

        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public bool CanGoBack => _frame.CanGoBack;

        public void GoBack() => _frame.GoBack();

        public void Navigate<T>(object args)
        {
            _frame.Navigate(this.viewMapping[typeof(T)], args);
        }

    }
}
