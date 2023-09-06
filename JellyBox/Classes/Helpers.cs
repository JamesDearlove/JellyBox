using CommunityToolkit.Mvvm.DependencyInjection;
using JellyBox.Services;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace JellyBox
{
    internal class Helpers
    {
        /// <summary>
        /// Generates an image from a blurhash to use on a XAML page.
        /// </summary>
        /// <param name="hash">The blurhash to generate from.</param>
        /// <returns>A SoftwareBitmap wrapped in a SoftwareBitmapSource.</returns>
        public static Task<SoftwareBitmapSource> GenerateBlurHash(string hash)
        {
            return GenerateBlurHash(hash, 128, 128);
        }

        /// <summary>
        /// Generates an image from a blurhash to use on a XAML page.
        /// </summary>
        /// <param name="hash">The blurhash to generate from.</param>
        /// <param name="width">Width of the image</param>
        /// <param name="height">Height of the image</param>
        /// <returns>A SoftwareBitmap wrapped in a SoftwareBitmapSource.</returns>
        public static async Task<SoftwareBitmapSource> GenerateBlurHash(string hash, int width, int height)
        {
            var image = Ioc.Default.GetService<JellyfinService>().BlurhashDecoder.Decode(hash, width, height);
            var softwareImage = new SoftwareBitmapSource();
            await softwareImage.SetBitmapAsync(image);
            return softwareImage;
        }
    }
}