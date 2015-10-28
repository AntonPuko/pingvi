using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Pingvi
{
    /// <summary>
    /// Interaction logic for LWindow.xaml
    /// </summary>
    public partial class LWindow : Window {
        private Run[] _rMass;

        private int _player;

        
      /// <summary>
      /// 
      /// </summary>
        /// <param name="player"> 0 - hero 1 - left 2 - right</param>
      /// <param name="top"></param>
      /// <param name="left"></param>
        public LWindow(int player) {
            InitializeComponent();
            _player = player;
            InitTextBlock();
       

      
      }

        private void InitTextBlock() {
            if (LineTextBlock == null) return;
            _rMass = new Run[10];
            for (int i = 0; i < _rMass.Length; i++) {
                _rMass[i] = new Run("");
                _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                LineTextBlock.Inlines.Add(_rMass[i]);
            }
        }

        public void OnNewLineInfo(LineInfo lineInfo) {
            for (int i = 0; i < _rMass.Length;i++) {
                _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                _rMass[i].Background = new SolidColorBrush(Color.FromArgb(255, 42, 42, 42));
            }
            
            string line = "";
            switch (_player) {
                case 0: line = lineInfo.Elements.HeroPlayer.Line; break;
                case 1: line = lineInfo.Elements.LeftPlayer.Line; break;
                case 2: line = lineInfo.Elements.RightPlayer.Line; break;
            }

            for (int i = 0; i < line.Length; i++) {
                switch (line[i]) {
                    case '|': 
                        _rMass[i].Text = "S";
                        _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
                        _rMass[i].Background = new SolidColorBrush(Color.FromRgb(128,128,128));
                        break;
                    case 'f':
                        _rMass[i].Text = "F";
                        _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        _rMass[i].Background = new SolidColorBrush(Color.FromRgb(64, 128, 128));
                        break;
                    case 'x':
                        _rMass[i].Text = "X";
                        _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        _rMass[i].Background = new SolidColorBrush(Color.FromRgb(192, 192, 192));
                        break;
                    case 'l':
                        _rMass[i].Text = "L";
                        _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        _rMass[i].Background = new SolidColorBrush(Color.FromRgb(255, 128, 255));
                        break;
                    case 'c':
                        _rMass[i].Text = "C";
                        _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        _rMass[i].Background = new SolidColorBrush(Color.FromArgb(220,255, 255, 0));
                        break;
                    case 'b':
                        _rMass[i].Text = "B";
                        _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        _rMass[i].Background = new SolidColorBrush(Color.FromRgb(255, 128, 64));
                        break;
                    case 'r':
                        _rMass[i].Text = "R";
                        _rMass[i].Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                        _rMass[i].Background = new SolidColorBrush(Color.FromRgb(255, 100, 100));
                        break;
                }
            }
        }

        
   
    }
}
