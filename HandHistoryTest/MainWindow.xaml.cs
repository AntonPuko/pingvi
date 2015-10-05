using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace HandHistoryTest
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

        private void StartButton_Click(object sender, RoutedEventArgs e) {
            var path = @"P:\HM2Archive\PokerStars\2015\07\09";
            var hhs = Directory.GetFiles(path);
            var boardRegex = new Regex(@"showed\s\[(..\s..)\]");
            foreach (var h in hhs) {

                var startStr = "*** SUMMARY ***";
                var endStr = "PokerStars";
               
                var t = File.ReadLines(h);

                StringBuilder strb = new StringBuilder();

                bool startWrite = false;
                foreach (var line in t) {
                    if (startWrite) {
                        strb.AppendLine(line);
                    }

                    if (line == startStr) startWrite = true;
                    if (line.Contains(endStr)) startWrite = false;
                }

                var results = strb.ToString();
                object locker = new object();

                lock (locker) {
                    Debug.Write(results);
                    Match match = boardRegex.Match(results);

                    while (match.Success)
                    {
                        Debug.WriteLine(match.Groups[1].Value);
                        match = match.NextMatch();
                    }

                    strb.Clear();
                }
              


                // = @"\b(\d+\W?руб)";


            }
        }
    }
}
