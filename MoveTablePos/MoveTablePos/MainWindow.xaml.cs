using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace MoveTablePos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenAndMoveButton_Click(object sender, RoutedEventArgs e)
        {
            Process paint = new Process();
            paint.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

            paint.StartInfo.FileName = @TablePathTextBox.Text;
            paint.Start();
            Thread.Sleep(1000);

            IntPtr id = paint.MainWindowHandle;
            Console.Write(id);
            MoveWindow(paint.MainWindowHandle, int.Parse(LeftTextBox.Text), int.Parse(TopTextBox.Text), 900, 800, true);

        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);
    }
}
