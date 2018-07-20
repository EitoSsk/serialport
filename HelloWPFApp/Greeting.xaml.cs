using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Diagnostics;

namespace HelloWPFApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        Comport comport = new Comport();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenComPort(object sender, RoutedEventArgs e)
        {
            comport.PortOpen();
        }

        private void CloseComPort(object sender, RoutedEventArgs e)
        {
            comport.PortClose();
        }
    }

}
