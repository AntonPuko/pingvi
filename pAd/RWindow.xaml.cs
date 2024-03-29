﻿using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace Pingvi
{
    /// <summary>
    ///     Interaction logic for RWindow.xaml
    /// </summary>
    public partial class RWindow : Window
    {
        // private NpgsqlConnection _npgSqlConnection;

        private bool _isResultRefreshing;

        // private Action _dbOpenAction;
        private DispatcherTimer _timer;

        private double _usdRubExRate;

        public RWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            try
            {
                var exchangeRatesUrl = "http://www.apilayer.net/api/live?access_key=b5a73c66b2b8f58fee2d61ac8e9695a2";

                string ratesJson;
                using (var webClient = new WebClient())
                {
                    ratesJson = webClient.DownloadString(exchangeRatesUrl);
                }

                var ratesJObect = JObject.Parse(ratesJson);
                _usdRubExRate = (double) ratesJObect["quotes"]["USDRUB"];
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            if (_usdRubExRate == 0) _usdRubExRate = 55;


            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };

            _timer.Tick += OnTimerTick;
            _timer.IsEnabled = true;
        }


        private void OnTimerTick(object sender, EventArgs e) {
            var tagCount = 0;
            double evBb100 = 0;
            double result = 0;
            double rake = 0;
            double rakeback = 0;

            var dtNow = DateTime.Now.Hour < 5
                ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")
                : DateTime.Now.ToString("yyyy-MM-dd");
            var statsUrl =
                "http://localhost:8001/query?q=select StatTourneyCount, StatEVBigBlindsPer100 from stats where HandTimestamp > {d \"" +
                dtNow + " 05:00:00 AM\"}";
            string statsJson;
            var tourneysUrl =
                "http://localhost:8001/query?q=select BuyInPlusRake, RakeInCents, WinningsInCents from TOURNAMENTS where FirstHandTimestamp > {d \"" +
                dtNow + " 05:00:00 AM\"} and FinishPosition > 0 ";
            string tourneysJson;


            using (var webClient = new WebClient())
            {
                try
                {
                    statsJson = webClient.DownloadString(statsUrl);
                    tourneysJson = webClient.DownloadString(tourneysUrl);
                }
                catch
                {
                    CleartStats();
                    return;
                }
            }

            try
            {
                var statsJObect = JObject.Parse(statsJson);
                var statsResults = statsJObect["Results"];

                //tagCount = int.Parse(statsResults[0]["TagCount"].ToString().Replace("В", "").Trim());
                evBb100 = double.Parse(statsResults[0]["EVbb100"].ToString().Replace("В", "").Trim());
            }
            catch
            {
                return;
            }


            try
            {
                var tourneysJObject = JObject.Parse(tourneysJson);
                var tourneysResults = tourneysJObject["Results"];


                foreach (var tourney in tourneysResults)
                {
                    tagCount++;
                    var tResult = (double.Parse(tourney["WinningsInCents"].ToString().Replace("В", "").Trim())
                                   - double.Parse(tourney["BuyInPlusRake"].ToString().Replace("В", "").Trim()))/100;
                    result += tResult;

                    var tRake = double.Parse(tourney["RakeInCents"].ToString().Replace("В", "").Trim())/100;
                    rake += tRake;
                }
            }
            catch
            {
                return;
            }


            //ChipsEVTourney = tagCount == 0 ?  0 : chipsEV/tagCount;


            const double vppMultiplicator = 5.5;
            //const double bonusFormula = 3.5/40000*600; old until 2016
            const double stepFormula = 50.0/1000;
            rakeback = rake*vppMultiplicator*stepFormula*_usdRubExRate;


            CountRun.Text = tagCount.ToString();
            RakeBackRun.Text = rakeback.ToString("#");

            if (_isResultRefreshing)
            {
                ResultRun.Text = result.ToString();
                if (result < 0) ResultRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                if (result > 0) ResultRun.Foreground = new SolidColorBrush(Color.FromRgb(90, 190, 80));

                EvBb100Run.Text = evBb100.ToString("0.0");
                if (evBb100 < 0) EvBb100Run.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                if (evBb100 > 0) EvBb100Run.Foreground = new SolidColorBrush(Color.FromRgb(90, 190, 80));
            }
        }

        // private bool isVisible = false;


        private void CleartStats() {
            ResultRun.Text = "";
            RakeBackRun.Text = "";
            EvBb100Run.Text = "";
            CountRun.Text = "";
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            //show or hide Results
            if (_isResultRefreshing)
            {
                ResultRun.Text = "";
                EvBb100Run.Text = "";
                ResultEnableButton.Background = new SolidColorBrush(Color.FromRgb(99, 99, 99));
                _isResultRefreshing = false;
            }
            else
            {
                ResultEnableButton.Background = new SolidColorBrush(Color.FromRgb(69, 96, 69));
                _isResultRefreshing = true;
            }
        }
    }
}