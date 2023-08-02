using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;

namespace GameWindowHelper
{
    internal partial class ProcessHelper : ObservableObject
    {
        private readonly Process m_process;

        public nint MainWindowHandle => m_process.MainWindowHandle;
        public string MainWindowTitle => m_process.MainWindowTitle;

        [ObservableProperty]
        public ImageSource m_icon;


        public ProcessHelper(Process process)
        {
            m_process = process;
            try
            {
                Icon? icon = System.Drawing.Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                if (icon is not null)
                {
                    Icon = ToImageSource(icon);
                }
                else
                {
                    SetDefaultIcon();
                }
            }
            catch
            {
                SetDefaultIcon();
            }
        }

        [MemberNotNull(nameof(m_icon))]
        private void SetDefaultIcon()
        {
            BitmapImage myBitmapImage = new();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(@"/Resources/jim_dead.png", UriKind.RelativeOrAbsolute);
            myBitmapImage.DecodePixelWidth = 16;
            myBitmapImage.EndInit();

            Icon = myBitmapImage;
        }



        private static ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }
    }
}
