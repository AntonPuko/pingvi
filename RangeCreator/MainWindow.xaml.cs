using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace RangeCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Range _range;
        private TextBlock[] _handsTextBoxArray;
        private int _decision1 = 0;
        private int _decision2 = 0;
        private double _stat = 0;
        private string _rangePath;

        private bool _isProbCountMode;
        public MainWindow()
        {
            InitializeComponent();

            _handsTextBoxArray = HandsMatrixGrid.Children.OfType<TextBlock>().ToArray();
            StatTextBox.TextChanged += StatTextBox_TextChanged;

            CreateRangeButtonClick(null, null);
        }

        private void StatTextBox_TextChanged(object sender, RoutedEventArgs e) {
            try {
                _stat = Double.Parse(((TextBox) sender).Text);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
           
        }

        

        private void CreateRangeButtonClick(object sender, RoutedEventArgs e) {
            const string fullRange =
                "22+, A2s+, K2s+, Q2s+, J2s+, T2s+, 92s+, 82s+, 72s+, 62s+, 52s+, 42s+, 32s, A2o+, K2o+, Q2o+, J2o+, T2o+, 92o+, 82o+, 72o+, 62o+, 52o+, 42o+, 32o";
            _range = new Range(RangeNameTextBox.Text, fullRange);
            _rangePath = "P:\\Dropbox\\pAd\\pAd\\bin\\Debug\\Data";
            foreach (var tb in _handsTextBoxArray)
            {
                ClearTbInMatrix(tb); 
            }

            CountActions();
            
        }



        private void DecisionButton1_Click(object sender, RoutedEventArgs e)
        {
            _decision1 =  SwitchDecision((Button) sender, _decision1);
        }

        private void DecisionButton2_Click(object sender, RoutedEventArgs e)
        {
           _decision2 = SwitchDecision((Button)sender, _decision2);
        }

        private int SwitchDecision(Button senderButton, int decision) {
            if (decision < 6) decision++;
            else decision = 0;

            switch (decision) {
                case 0:
                    senderButton.Content = "0-None";
                    break;
                case 1:
                    senderButton.Content = "1-FOLD";
                    break;
                case 2:
                    senderButton.Content = "2-LIMP";
                    break;
                case 3:
                    senderButton.Content = "3-OPEN";
                    break;
                case 4:
                    senderButton.Content = "4-CALL";
                    break;
                case 5:
                    senderButton.Content = "5-3BET";
                    break;
                case 6:
                    senderButton.Content = "6-PUSH";
                    break;

            }

            senderButton.Background = new SolidColorBrush(GetColorByDecision(decision));
            return decision;
        }

        private void ClearTbInMatrix(TextBlock tb) {
            tb.Background = new SolidColorBrush(Color.FromRgb(222,222,222));
            if (tb.Text.Length > 2) tb.Text = tb.Text.Substring(0, 3);
           
        }

        private void ColorHandsInMatrix() {

            Debug.WriteLine(_handsTextBoxArray.Length);
            foreach (var tb in _handsTextBoxArray) {
                ClearTbInMatrix(tb);

                bool tbIsSuited = tb.Tag.ToString()[2] == 's';

                var fhand = _range.Hands.First( h =>
                            h.Name[0] == tb.Tag.ToString()[0] && h.Name[2] == tb.Tag.ToString()[1] &&
                            h.IsSuited == tbIsSuited);
                var hands = _range.Hands.Where(h =>
                            h.Name[0] == tb.Tag.ToString()[0] && h.Name[2] == tb.Tag.ToString()[1] &&
                            h.IsSuited == tbIsSuited);

                foreach (var h in hands) {
                    if (h.D1 != fhand.D1 || h.D2 != fhand.D2 || h.S1 != fhand.S1) {
                        fhand.D1 = 0;
                        fhand.D2 = 0;
                        fhand.S1 = 0;
                        break;
                    }
                }

                tb.Background = new LinearGradientBrush(GetColorByDecision(fhand.D1), GetColorByDecision(fhand.D2), new Point(0.6, 0.6), new Point(0.7, 0.7));
                tb.Text = String.Format("{0} \n{1}", tb.Text, fhand.S1);
                
    

            }
        }

        private Color GetColorByDecision(int decision) {
            switch (decision) {
                case 0: return Color.FromRgb(255, 255, 255);
                case 1: return Color.FromRgb(180, 180, 180);
                case 2: return Color.FromRgb(255, 255, 0);
                case 3: return Color.FromRgb(146, 205, 220);
                case 4: return Color.FromRgb(220, 220, 0);
                case 5: return Color.FromRgb(146, 208, 80);
                case 6: return Color.FromRgb(177, 160, 199);
                default:
                        return Color.FromRgb(255, 255, 255);
            }
        }

        private void Tb1_MouseUp(object sender, MouseButtonEventArgs e) {
            TextBlock targetTb = (TextBlock) sender;

            bool tbIsSuited = targetTb.Tag.ToString()[2] == 's';
            ClearTbInMatrix(targetTb);
           // targetTb.Background = new SolidColorBrush(Color.FromRgb(255,255,255));
          //  targetTb.Text = "";

            targetTb.Background = new LinearGradientBrush(GetColorByDecision(_decision1), GetColorByDecision(_decision2), new Point(0.5,0.5), new Point(0.6,0.6));
            targetTb.Text = String.Format("{0} \n{1}", targetTb.Text, _stat);


            var hands = _range.Hands.Where(h =>
                            h.Name[0] == targetTb.Tag.ToString()[0] && h.Name[2] == targetTb.Tag.ToString()[1] &&
                            h.IsSuited == tbIsSuited);

            Debug.WriteLine(hands.Count());
            foreach (var hand in hands) {
                hand.D1 = _decision1;
                hand.D2 = _decision2;
                hand.S1 = _stat;
            }
            CountActions();
        }

        private void SaveRangeButton_Click(object sender, RoutedEventArgs e)
        {
            var path = String.Format("{0}\\{1}.xml",_rangePath, RangeNameTextBox.Text);
            _range.Name = RangeNameTextBox.Text;
            XmlRangeHelper.Save(_range, path);
        }

  

        private void OpenRangeButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                _range = XmlRangeHelper.Load(openFileDialog.FileName);
                _rangePath = openFileDialog.FileName;
                _rangePath = _rangePath.Remove(_rangePath.LastIndexOf('\\')+1);
                RangeNameTextBox.Text = _range.Name;
            }
              
               
            ColorHandsInMatrix();
            CountActions();
        }


        private void CountActions() {
            NoneLabel.Content = CountRangeDecisionPersents(1, 0);
            NoneLabel2.Content = CountRangeDecisionPersents(2, 0);
            FoldLabel.Content = CountRangeDecisionPersents(1, 1);
            FoldLabel2.Content = CountRangeDecisionPersents(2, 1);
            LimpLabel.Content = CountRangeDecisionPersents(1, 2);
            LimpLabel2.Content = CountRangeDecisionPersents(2, 2);
            OpenLabel.Content = CountRangeDecisionPersents(1, 3);
            OpenLabel2.Content = CountRangeDecisionPersents(2, 3);
            CallLabel.Content = CountRangeDecisionPersents(1, 4);
            CallLabel2.Content = CountRangeDecisionPersents(2, 4);
            Bet3Label.Content = CountRangeDecisionPersents(1, 5);
            Bet3Label2.Content = CountRangeDecisionPersents(2, 5);
            PushLabel.Content = CountRangeDecisionPersents(1, 6);
            PushLabel2.Content = CountRangeDecisionPersents(2, 6);

            if (FoldLabel.Content.ToString() != "-") {
                VpipLabel.Content = 100 - Double.Parse(CountRangeDecisionPersents(1, 1));   
            }
            if (FoldLabel2.Content.ToString() != "-") {
                VpipLabel2.Content = 100 - Double.Parse(CountRangeDecisionPersents(2, 1)); 
            }
            
        }

        private string CountRangeDecisionPersents(int decisionNumber, int decisionType) {
            if (_range == null) return "-";
            if (decisionNumber > 2 || decisionNumber < 1) return "-";
            if (decisionType > 6 || decisionNumber < 0) return "-";

            var rangeLength = _range.Hands.Count();

            double handsCount;

            if (_isProbCountMode) {
                handsCount = decisionNumber == 1 ? _range.Hands.Where(hand => hand.D1 == decisionType).Sum(hand => hand.S1 / 100)
                : _range.Hands.Where(hand => hand.D2 == decisionType).Sum(hand => 1 - hand.S1 / 100);
            }
            else {
                handsCount = decisionNumber == 1 ? _range.Hands.Count(h => h.D1 == decisionType)
               : _range.Hands.Count(h => h.D2 == decisionType);
            }

            return (handsCount/(double)rangeLength*100.0).ToString("F1");
        }



        private void SwitchModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isProbCountMode == false) {
                _isProbCountMode = true;
                SwitchModeButton.Content = "ToDecMode";
                CountActions();
            }
            else {
                _isProbCountMode = false;
                SwitchModeButton.Content = "ToProbMode";
                CountActions();
            }
        }
        

    }
}
