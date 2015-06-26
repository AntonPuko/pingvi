using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
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
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace Pingvi
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow()
        {
            InitializeComponent();
        }

        private Action _dbOpenAction;
        private DispatcherTimer _timer;
        private NpgsqlConnection _npgSqlConnection;
        private void Window_Loaded(object sender, RoutedEventArgs e) {

                _timer = new DispatcherTimer() {
                    Interval = TimeSpan.FromSeconds(3),
                };

                _timer.Tick += OnTimerTick;
                _timer.IsEnabled = true;
            }

        private void OnTimerTick(object sender, EventArgs e) {
            int tagCount = 0;
            double chipsEV = 0;
            double ChipsEVTourney = 0;
            double result = 0;
            double rake = 0;
            double rakeback = 0;

            
            var dtNow = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
          

            string statsURL ="http://localhost:8001/query?q=select StatTourneyCount, StatAllInEVAdjustedChips from stats where HandTimestamp > {d \"" + dtNow +" 07:00:00 AM\"}";
            string statsJson;

            string tourneysURL = "http://localhost:8001/query?q=select BuyInPlusRake, RakeInCents, WinningsInCents from TOURNAMENTS where FirstHandTimestamp > {d \"" + dtNow + " 07:00:00 AM\"} and FinishPosition > 0 ";
            string tourneysJson;
            
            using (var webClient = new System.Net.WebClient()) {
                try {
                    statsJson = webClient.DownloadString(statsURL);
                    tourneysJson = webClient.DownloadString(tourneysURL);
                }
                catch  {
                    MessageBox.Show("HM не запущен!");
                    _timer.IsEnabled = false;
                    CleartStats();
                    return;
                }
                
            }

            var statsJObect = JObject.Parse(statsJson);
            var statsResults = statsJObect["Results"];

            tagCount = int.Parse(statsResults[0]["TagCount"].ToString().Replace("В", "").Trim());
            chipsEV = double.Parse(statsResults[0]["Chips(EVAdjusted)"].ToString().Replace("В", "").Trim());


            ChipsEVTourney = chipsEV / tagCount;

            var tourneysJObject = JObject.Parse(tourneysJson);
            var tourneysResults = tourneysJObject["Results"];

          

            foreach (var tourney in tourneysResults) {
                var tResult = (double.Parse(tourney["WinningsInCents"].ToString().Replace("В", "").Trim() )
                    -double.Parse( tourney["BuyInPlusRake"].ToString().Replace("В", "").Trim())) / 100;
                result += tResult;

                var tRake = double.Parse(tourney["RakeInCents"].ToString().Replace("В", "").Trim()) / 100;
                rake += tRake;
            }

            const double vppMultiplicator = 5.5;
            const double bonusFormula = 3.5/40000*600;
            rakeback = rake * vppMultiplicator * bonusFormula;



            CountRun.Text = tagCount.ToString();
            ResultRun.Text = result.ToString();
            if (result < 0) ResultRun.Foreground = new SolidColorBrush(Color.FromRgb(255,125,125));
            if (result > 0) ResultRun.Foreground = new SolidColorBrush(Color.FromRgb(90, 190, 80));
            RakeBackRun.Text = rakeback.ToString("##.#");

            ChipsEvRun.Text = ChipsEVTourney.ToString("##.#");
            if (ChipsEVTourney < 0) ChipsEvRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
            if (ChipsEVTourney > 0) ChipsEvRun.Foreground = new SolidColorBrush(Color.FromRgb(90, 190, 80));
        }

        private bool isVisible = false;


        private void CleartStats() {
            ResultRun.Text = "-";
            RakeBackRun.Text = "-";
            ChipsEvRun.Text = "-";
            CountRun.Text = "-";
        }



     

        private void RichTextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
              if (!_timer.IsEnabled) _timer.IsEnabled = true;
        }
     
            
        
    }
}
