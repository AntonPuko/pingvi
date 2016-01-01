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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PokerModel;

namespace GTORangeCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GtoRange _range;
        private string _rangePath;
        private TextBlock[] _handsTextBoxArray;

      
        

        private double[][] _probRanges = new[]
        {
            new[] {0.0, 0.0},
            new[] { 0.0, 0.0},
            new[] { 0.0, 0.0},
            new[] { 0.0, 0.0},
        };
        
        private int[] _decisions = {0,0,0,0};
        private double[] _sizes = { 0, 0, 0, 0 };
        private double[] _probabilities = {  0,0,0,0};


        private double[] _decisionFreqArray = new double[7];

        public MainWindow()
        {
            InitializeComponent();
            _handsTextBoxArray = HandsMatrixGrid.Children.OfType<TextBlock>().ToArray();
            CreateRangeBtn_Click(null, null);
        }

        private void Tb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock targetTb = (TextBlock)sender;
            

            ColorTextBLock(targetTb);
            SetHandDecisions(targetTb);

        }

        private void SetHandDecisions(TextBlock targetTb) {
            bool tbIsSuited = targetTb.Tag.ToString()[2] == 's';

            var hands = _range.Hands.Where(h =>
                          h.Name[0] == targetTb.Tag.ToString()[0] && h.Name[2] == targetTb.Tag.ToString()[1] &&
                          h.IsSuited == tbIsSuited);
            foreach (var hand in hands)
            {
                for (int i = 0; i < 4; i ++)
                {
                    hand.Decisions[i].Value = _decisions[i];
                    hand.Decisions[i].Size = _sizes[i];
                    hand.Decisions[i].Probability = _probabilities[i];
                }

            }
   /*
      public int Value { get; set; }
        [XmlAttribute]
        public int Size { get; set; }
        [XmlAttribute]
        public int Probability { get; set; } */
    }

        private void DecBtn_OnClickecBtn_Click(object sender, RoutedEventArgs e) {
            var btn = (Button) sender;
            var tag = Int32.Parse(btn.Tag.ToString());
            _decisions[tag] = SwitchDecision(btn, _decisions[tag]);
        }

        private int SwitchDecision(Button senderButton, int decision)
        {
            if (decision < 6) decision++;
            else decision = 0;

            switch (decision)
            {
                case 0:
                    senderButton.Content = "N";
                    break;
                case 1:
                    senderButton.Content = "F";
                    break;
                case 2:
                    senderButton.Content = "L";
                    break;
                case 3:
                    senderButton.Content = "O";
                    break;
                case 4:
                    senderButton.Content = "C";
                    break;
                case 5:
                    senderButton.Content = "3";
                    break;
                case 6:
                    senderButton.Content = "P";
                    break;

            }

            senderButton.Background = new SolidColorBrush(GetColorByDecision(decision));
            return decision;
        }

        private void CreateRangeBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tb in _handsTextBoxArray)
            {
                ClearTbInMatrix(tb);
            }
            const string fullRange =
             "22+, A2s+, K2s+, Q2s+, J2s+, T2s+, 92s+, 82s+, 72s+, 62s+, 52s+, 42s+, 32s, A2o+, K2o+, Q2o+, J2o+, T2o+, 92o+, 82o+, 72o+, 62o+, 52o+, 42o+, 32o";
            _range = new GtoRange(RangeNameTextBox.Text, fullRange);
            _rangePath = "P:\\Dropbox\\pAd\\pAd\\bin\\Debug\\Data\\GtoRanges\\";
        }

        private void SaveRangeButton_Click(object sender, RoutedEventArgs e)
        {
            var path = String.Format("{0}\\{1}.xml", _rangePath, RangeNameTextBox.Text);
            _range.Name = RangeNameTextBox.Text;
            XmlGtoRangeHelper.Save(_range, path);
        }

        private void OpenRangeButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _range = XmlGtoRangeHelper.Load(openFileDialog.FileName);
                    _rangePath = openFileDialog.FileName;
                    _rangePath = _rangePath.Remove(_rangePath.LastIndexOf('\\') + 1);
                    RangeNameTextBox.Text = _range.Name;
                }
                catch
                {
                    MessageBox.Show("Error!");
                }
            }

            ColorHandsInMatrix();
            updateFreqTextBox();
        }

        private void ColorHandsInMatrix() {
            foreach (var tb in _handsTextBoxArray)
            {
                ClearTbInMatrix(tb);

                bool tbIsSuited = tb.Tag.ToString()[2] == 's';


                var fhand = _range.Hands.First(h =>
                       h.Name[0] == tb.Tag.ToString()[0] && h.Name[2] == tb.Tag.ToString()[1] &&
                       h.IsSuited == tbIsSuited);

                for (int i = 0; i < 4; i++)
                {
                    _decisions[i] = fhand.Decisions[i].Value;
                    _sizes[i] = fhand.Decisions[i].Size;
                    _probabilities[i] = fhand.Decisions[i].Probability;
                }

                CountProbRanges();

                ColorTextBLock(tb);
            }
        }

        private void SizeTb_KeyUp(object sender, KeyEventArgs e)
        {
            ParseSizeValues();
        }

        private void ClearTbInMatrix(TextBlock tb)
        {
            tb.Background = new SolidColorBrush(Color.FromRgb(222, 222, 222));

        }

        private void ColorTextBLock(TextBlock tb) {

            var gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1 , 0);

            var stopCollection = new GradientStopCollection()
            {
                new GradientStop(GetColorByDecision(_decisions[0]), _probRanges[0][0]/100.0),
                new GradientStop(GetColorByDecision(_decisions[0]), _probRanges[0][1]/100.0),

                new GradientStop(GetColorByDecision(_decisions[1]), _probRanges[1][0]/100.0),
                new GradientStop(GetColorByDecision(_decisions[1]), _probRanges[1][1]/100.0),

                new GradientStop(GetColorByDecision(_decisions[2]), _probRanges[2][0]/100.0),
                new GradientStop(GetColorByDecision(_decisions[2]), _probRanges[2][1]/100.0),

                new GradientStop(GetColorByDecision(_decisions[3]), _probRanges[3][0]/100.0),
                new GradientStop(GetColorByDecision(_decisions[3]), _probRanges[3][1]/100.0)
            };

            gradient.GradientStops = stopCollection;
            tb.Background = gradient;
        }

        private Color GetColorByDecision(int decision)
        {
            switch (decision)
            {
                case 0: return Color.FromRgb(255, 255, 255);
                case 1: return Color.FromRgb(180, 180, 180);
                case 2: return Color.FromRgb(255, 255, 0);
                case 3: return Color.FromRgb(146, 205, 220);
                case 4:return Color.FromRgb(220, 220, 0);
                case 5: return Color.FromRgb(146, 208, 80);
                case 6: return Color.FromRgb(177, 160, 199);
                default:
                    return Color.FromRgb(255, 255, 255);
            }
        }



        private void CountProbRanges() {
            _probRanges[0][0] = 0;
            _probRanges[0][1] = _probabilities[0];
            _probRanges[1][0] = _probRanges[0][1];
            _probRanges[1][1] = _probRanges[1][0] + _probabilities[1];
            _probRanges[2][0] = _probRanges[1][1];
            _probRanges[2][1] = _probRanges[2][0] + _probabilities[2];
            _probRanges[3][0] = _probRanges[2][1];
            _probRanges[3][1] = _probRanges[3][0] + _probabilities[3];
        }
        
    

        private void ProbTb_KeyUp(object sender, KeyEventArgs e)
        {
            ParseProbValues();
            CountProbRanges();

            var probSum = _probabilities.Sum();
            ProbSum.Text = probSum.ToString();
            if (probSum == 100)
            {
                ProbSum.Background = new SolidColorBrush(Colors.GreenYellow);
            }
            else
            {
                ProbSum.Background = new SolidColorBrush(Colors.Pink);
            }
        }

        private void ParseProbValues() {
            double.TryParse(ProbTb1.Text, out _probabilities[0]);
            double.TryParse(ProbTb2.Text, out _probabilities[1]);
            double.TryParse(ProbTb3.Text, out _probabilities[2]);
            double.TryParse(ProbTb4.Text, out _probabilities[3]);
        }

        private void ParseSizeValues()
        {
            double.TryParse(SizeTb1.Text, out _sizes[0]);
            double.TryParse(SizeTb2.Text, out _sizes[1]);
            double.TryParse(SizeTb3.Text, out _sizes[2]);
            double.TryParse(SizeTb4.Text, out _sizes[3]);
        }


        private void updateFreqTextBox() {
            CountRangeStats();
            StringBuilder str = new StringBuilder();

            str.Append("Range Freq: \n");
            for (int i = 0; i < _decisionFreqArray.Length; i ++)
            {
                var decisionName = "";
                switch (i)
                {
                    case 0: decisionName = "none"; break;
                    case 1: decisionName = "fold"; break;
                    case 2: decisionName = "limp"; break;
                    case 3: decisionName = "open/iso"; break;
                    case 4: decisionName = "call"; break;
                    case 5: decisionName = "3bet"; break;
                    case 6: decisionName = "push"; break;
                    default: decisionName = ""; break;
                }
                str.Append(decisionName + ": " + _decisionFreqArray[i].ToString("##.#") + "\n");
            }

            str.Append("sum: " +_decisionFreqArray.Sum().ToString("##.#"));
            RangeStatsTextBlock.Text = str.ToString();

        }

        private void CountRangeStats() {
            for (int i = 0; i < _decisionFreqArray.Length; i++)
            {
                _decisionFreqArray[i] = 0;
            }
           
            foreach (var h in _range.Hands)
            {
                for (int i = 0; i < _decisionFreqArray.Length; i++)
                {
                    var des = h.Decisions.FirstOrDefault(d => d.Value == i);
                    if (des == null) continue;
                    _decisionFreqArray[i] += des.Probability / 1326;
                }
               
            }
        }

        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) / correctionFactor + red;
                green = (255 - green) / correctionFactor + green;
                blue = (255 - blue) / correctionFactor + blue;
            }
         
            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }

        private void ProbTb_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var btn = (TextBox) sender;
            btn.Text = "";
        }
    }
}


