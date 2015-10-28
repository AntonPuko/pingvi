using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AForge.Neuro;
using AForge.Neuro.Learning;



namespace NeuralLineDetection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private double[][] _inputs;
        private double[][] _outputs;
        private ActivationNetwork _network;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadDataButton_Click(object sender, RoutedEventArgs e) {
            var inputsBitmapsPaths = Directory.GetFiles(@"Data\Inputs\");
       
         

            _inputs = new double[inputsBitmapsPaths.Length][];
            _outputs = new double[inputsBitmapsPaths.Length][];

            //fill inputs and outputs massive
            for (int i = 0; i < inputsBitmapsPaths.Length; i++) {
                Bitmap letter = new Bitmap(inputsBitmapsPaths[i]);
                var inputLetter = inputsBitmapsPaths[i].Replace("Data\\Inputs\\", "")[0];
                _outputs[i] = OutputsFactory.CreateOutput(inputLetter);
                _inputs[i] = BitmapHelper.ProcessUnsafeBitmapIntoDoubleArray(letter);
                //_inputs[i] = CountBitmap(letter);
            }

          


        }

        
        private double[] CountBitmap(Bitmap letterBitmap) {
            var res = new double[letterBitmap.Size.Width * letterBitmap.Size.Height];
            int c = 0;
            for (int i = 0; i < letterBitmap.Width; i++)
            {
                for (int j = 0; j < letterBitmap.Height; j++)
                {
                    var pixelColor = letterBitmap.GetPixel(i, j);
                    if (pixelColor == Color.FromArgb(255, 255, 255, 255)) res[c] = 0.5;
                    else res[c] = -0.5;
                    c++;
                }
            }
            return res;
        }

        private void TeachNetworkButton_Click(object sender, RoutedEventArgs e) {

            _network = new ActivationNetwork(new BipolarSigmoidFunction(2.0), _inputs[0].Length, _inputs[0].Length, _outputs[0].Length);

            BackPropagationLearning teacher = new BackPropagationLearning(_network);
            teacher.LearningRate = 0.05;

            int epoch = 1;

            while (true) {
                var error = teacher.RunEpoch(_inputs, _outputs);

                Debug.WriteLine("epoch: " + epoch.ToString() + "error: " + error.ToString());
                epoch++;
                if (error < 0.1) break;
            }


        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _inputs.Length; i++) {
                double[] res = _network.Compute(_inputs[i]);
                int number = res.TakeWhile(r => !(r > 0.3)).Count();
                Debug.WriteLine(new LineLetter(number).Letter);
            }
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _network.Save(@"Data\network.net");
        }

        private void LoadNetworkButton_Click(object sender, RoutedEventArgs e) {
            _network = Network.Load(@"Data\network.net") as ActivationNetwork;
        }

        
    }

    public static class OutputsFactory {
        public static double[] CreateOutput(char letter) {
            switch (letter) {
                case 'f': return new double[] { 0.5, -0.5, -0.5, -0.5, -0.5, -0.5, -0.5, -0.5 };
                case 'l': return new double[] { -0.5, 0.5, -0.5, -0.5, -0.5, -0.5, -0.5, -0.5 };
                case 'r': return new double[] { -0.5, -0.5, 0.5, -0.5, -0.5, -0.5, -0.5, -0.5 };
                case 'c': return new double[] { -0.5, -0.5, -0.5, 0.5, -0.5, -0.5, -0.5, -0.5 };
                case 'x': return new double[] { -0.5, -0.5, -0.5, -0.5, 0.5, -0.5, -0.5, -0.5 };
                case 'b': return new double[] { -0.5, -0.5, -0.5, -0.5, -0.5, 0.5, -0.5, -0.5 };
                case 's': return new double[] { -0.5, -0.5, -0.5, -0.5, -0.5, -0.5, 0.5, -0.5 };
                case 'n': return new double[] { -0.5, -0.5, -0.5, -0.5, -0.5, -0.5, -0.5, 0.5 };
            }
            return null;
        }
    }


  
}
