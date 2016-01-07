using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            new[] { 0.0, 0.0},
        };
        
        private int[] _decisions = {0,0,0,0,0};
        private double[] _sizes = { 0, 0, 0, 0,0 };
        private double[] _probabilities = {  0,0,0,0,0};


        private double[] _decisionFreqRangeArray = new double[7];
        private double[] _decisionFreqHandArray = new double[7];

        public MainWindow()
        {
            InitializeComponent();
            _handsTextBoxArray = HandsMatrixGrid.Children.OfType<TextBlock>().ToArray();
            CreateRangeBtn_Click(null, null);
        }

        private void Tb_MouseLeftBtnUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock targetTb = (TextBlock)sender;
            ColorTextBLock(targetTb);
            SetHandDecisions(targetTb);

        }

        private void Tb_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock targetTb = (TextBlock)sender;
            SetHandStats(targetTb);
        }

        private void SetHandStats(TextBlock targetTb) {
            GetHandDecisionsFreq(targetTb);
            StringBuilder str = new StringBuilder();

            str.Append("Hand Freq: \n");
            for (int i = 0; i < _decisionFreqHandArray.Length; i++)
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
                str.Append(decisionName + ": " + _decisionFreqHandArray[i].ToString("##.#") + "\n");
            }

            str.Append("sum: " + _decisionFreqHandArray.Sum().ToString("##.#"));
            HandStatsTextBlock.Text = str.ToString();
        }

        private void GetHandDecisionsFreq(TextBlock targetTb) {
            
            bool tbIsSuited = targetTb.Tag.ToString()[2] == 's';
            var hand = _range.Hands.FirstOrDefault(h =>
                          h.Name[0] == targetTb.Tag.ToString()[0] && h.Name[2] == targetTb.Tag.ToString()[1] &&
                          h.IsSuited == tbIsSuited);


            for (int i = 0; i < _decisionFreqHandArray.Length; i++)
            {
                _decisionFreqHandArray[i] = 0;
            }

            for (int i = 0; i < _decisionFreqRangeArray.Length; i++)
            {
                var matchedDecisions = hand.Decisions.Where(d => d.Value == i);
                if (!matchedDecisions.Any()) continue;
                var sum = matchedDecisions.Sum(dec => dec.Probability);
                _decisionFreqRangeArray[i] += sum;
            }
        }

        private void SetHandDecisions(TextBlock targetTb) {
            bool tbIsSuited = targetTb.Tag.ToString()[2] == 's';

            var hands = _range.Hands.Where(h =>
                          h.Name[0] == targetTb.Tag.ToString()[0] && h.Name[2] == targetTb.Tag.ToString()[1] &&
                          h.IsSuited == tbIsSuited);
            foreach (var hand in hands)
            {
                for (int i = 0; i < _decisions.Length; i ++)
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

                for (int i = 0; i < _decisions.Length; i++)
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
                new GradientStop(GetColorByDecision(_decisions[3]), _probRanges[3][1]/100.0),

                new GradientStop(GetColorByDecision(_decisions[4]), _probRanges[4][0]/100.0),
                new GradientStop(GetColorByDecision(_decisions[4]), _probRanges[4][1]/100.0)
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
            _probRanges[4][0] = _probRanges[3][1];
            _probRanges[4][1] = _probRanges[4][0] + _probabilities[4];
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
            double.TryParse(ProbTb5.Text, out _probabilities[4]);
        }

        private void ParseSizeValues()
        {
            double.TryParse(SizeTb1.Text, out _sizes[0]);
            double.TryParse(SizeTb2.Text, out _sizes[1]);
            double.TryParse(SizeTb3.Text, out _sizes[2]);
            double.TryParse(SizeTb4.Text, out _sizes[3]);
            double.TryParse(SizeTb5.Text, out _sizes[4]);
        }


        private void updateFreqTextBox() {
            CountRangeStats();
            StringBuilder str = new StringBuilder();

            str.Append("Range Freq: \n");
            for (int i = 0; i < _decisionFreqRangeArray.Length; i ++)
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
                str.Append(decisionName + ": " + _decisionFreqRangeArray[i].ToString("##.#") + "\n");
            }

            str.Append("sum: " +_decisionFreqRangeArray.Sum().ToString("##.#"));
            RangeStatsTextBlock.Text = str.ToString();

        }

        private void CountRangeStats() {
            for (int i = 0; i < _decisionFreqRangeArray.Length; i++)
            {
                _decisionFreqRangeArray[i] = 0;
            }
           
            foreach (var h in _range.Hands)
            {
                for (int i = 0; i < _decisionFreqRangeArray.Length; i++)
                {
                    var matchedDecisions = h.Decisions.Where(d => d.Value == i);
                    if (!matchedDecisions.Any()) continue;
                    var sum = matchedDecisions.Sum(dec => dec.Probability);

                    _decisionFreqRangeArray[i] += sum / 1326;
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

        private void ImportFromTxtButton_Click(object sender, RoutedEventArgs e)
        {
            ImportRangeFromPioTxt();
            ColorHandsInMatrix();
            updateFreqTextBox();
        }

        private void ImportRangeFromPioTxt() {
         
                var path = @"P:\range.txt";
                string line;
                using (StreamReader sr = new StreamReader(path))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("Hand")) continue;

                        var lineMass = line.Split(',');
                        var hName = lineMass[0];
                        string hNameShuffleSuits = hName[0].ToString() + hName[3] + hName[2] + hName[1];
                    


                        double[] probs = new double[5];

                        probs[0] = double.Parse(lineMass[1]);
                        probs[1] = double.Parse(lineMass[2]);
                        probs[2] = lineMass.Length >= 4 ? double.Parse(lineMass[3]) : 0.0;
                        probs[3] = lineMass.Length >= 5 ? double.Parse(lineMass[4]) : 0.0;
                        probs[4] = lineMass.Length >= 6 ? double.Parse(lineMass[5]) : 0.0;



                        var rHands = _range.Hands.Where(h =>
                        h.Name == hName || h.Name == hNameShuffleSuits); //

                     //   if (rHand == null)
                     //   {
                     //       Debug.WriteLine("No hand: " + hName);
                    //        return;
                     //   }

                        foreach (var rHand in rHands)
                        {
                            for (int i = 0; i < probs.Length; i++)
                            {
                                rHand.Decisions[i].Value = _decisions[i];
                                rHand.Decisions[i].Probability = probs[i];
                                rHand.Decisions[i].Size = _sizes[i];
                            }
                        }

                       
                    }
                }
            
     
        }
    }
}


