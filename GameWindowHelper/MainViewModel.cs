using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace GameWindowHelper
{
    internal sealed partial class MainViewModel : ObservableObject
    {
        private RECT m_windowOriginalRect;
        private List<ProcessHelper>? m_processes;


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SetForegroundWindowCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveForegroundWindowCommand))]
        private ProcessHelper? m_activeProcess;


        [ObservableProperty]
        private string? m_filter;


        [ObservableProperty]
        private string? m_message = "No active window";


        [ObservableProperty]
        private List<ProcessHelper>? m_filteredProcesses;


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SetForegroundWindowCommand))]
        private ProcessHelper? m_selectedProcess;




        public MainViewModel()
        {
            GetProcesses();
        }




        partial void OnFilterChanged(string? value)
        {
            SetFilteredProcesses();
        }


        private void SetFilteredProcesses()
        {
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                FilteredProcesses = m_processes.Where(x => x.MainWindowTitle.ToLower().Contains(Filter.ToLower())).ToList();
            }
            else
            {
                FilteredProcesses = m_processes;
            }

            OnPropertyChanged(nameof(FilteredProcesses));
        }


        partial void OnSelectedProcessChanged(ProcessHelper? value)
        {
            if (ActiveProcess is not null)
                SelectedProcess = ActiveProcess;
        }




        [RelayCommand]
        public void GetProcesses()
        {
            m_processes = new List<ProcessHelper>();
            Process[] processlist = Process.GetProcesses();
            foreach (Process process in processlist)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    m_processes.Add(new ProcessHelper(process));
                }
            }

            SetFilteredProcesses();
        }



        [RelayCommand]
        public void ClearFilter()
        {
            Filter = "";
        }



        [RelayCommand(CanExecute = nameof(CanSetForegroundWindow))]
        public void SetForegroundWindow()
        {
            if (CanSetForegroundWindow() && GetWindowRect(SelectedProcess!.MainWindowHandle) is RECT rect)
            {
                if(App.Current.MainWindow.Left > rect.Left)
                {
                    App.Current.MainWindow.Left = 0;
                }

                ActiveProcess = SelectedProcess;
                m_windowOriginalRect = rect;
                SetWindowPos(ActiveProcess.MainWindowHandle, HWND_TOPMOST, rect.X(), -31, 0, 0, SWP_NOSIZE | SWP_SHOWWINDOW);
                Message = $"Setting {ActiveProcess.MainWindowTitle} as the active window";
            }
        }

        public bool CanSetForegroundWindow()
        {
            return SelectedProcess is not null && ActiveProcess is null;
        }



        [RelayCommand(CanExecute = nameof(CanRemoveForegroundWindow))]
        public void RemoveForegroundWindow()
        {
            if (CanRemoveForegroundWindow())
            {
                SetWindowPos(ActiveProcess!.MainWindowHandle, HWND_NOTOPMOST, m_windowOriginalRect.X(), m_windowOriginalRect.Y(), 0, 0, SWP_NOSIZE | SWP_SHOWWINDOW);
                ActiveProcess = null;
                Message = "No active window";
            }
        }

        public bool CanRemoveForegroundWindow()
        {
            return ActiveProcess is not null;
        }




        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool SetWindowPos(nint hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [LibraryImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool GetWindowRect(nint hwnd, out RECT lpRect);


        const int HWND_TOPMOST = -1;
        const int HWND_NOTOPMOST = -2;
        const short SWP_NOSIZE = 1;
        const int SWP_SHOWWINDOW = 0x0040;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner

            public override string ToString()
            {
                return $"X: {Left}, Y:{Top}, Width:{Right - Left}, Height: {Bottom - Top}";
            }

            public int X() => Left;
            public int Y() => Top;
            public int Width() => Right - Left;
            public int Height() => Bottom - Top;

        }


        private RECT? GetWindowRect(IntPtr winHandle)
        {
            if (!GetWindowRect(winHandle, out RECT rct))
            {
                Message = "*ERROR: Cannot find the position of the DSP window*";
            }

            return rct;
        }
    }
}
