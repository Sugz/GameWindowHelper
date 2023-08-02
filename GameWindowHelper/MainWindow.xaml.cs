using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameWindowHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel m_mainViewModel;


        public MainWindow()
        {
            InitializeComponent();
            m_mainViewModel = App.Current.Services.GetService<MainViewModel>()!;
            DataContext = m_mainViewModel;
            FilterBox.Focus();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            m_mainViewModel.RemoveForegroundWindow();
        }

        private void lv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lv.SelectedItem = null;
        }
    }
}
