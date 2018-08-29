using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace Swarm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // startup swarm game
            m_swarmGame = new SwarmGame();

            // data
            m_scripts = new string[(int)PlotColor.Yellow + 1];

            Loaded += On_Loaded;
            Closed += On_Closed;
        }

        // constants
        private const float m_visitedOpacity = 0.40f;
        private const float m_occupiedOpacity = 1.0f;
        private const float m_duplicationOpacity = 0.75f;
        private const float m_defendedOpacity = 0.9f;
        private const float m_unoccupiedOpacity = 0.4f;
        private const int m_hWaterMark = 19;
        private const int m_wWaterMark = 79;
        private const int m_waitWaterMark = 2;
        private const PlotState m_stateWaterMark = PlotState.Forbidden;
        private const char m_swarmMessageMarker = '+';

        private delegate void DispatcherDeleage(object[] objs);

        // data
        private MainWindow m_root;
        private static List<string> m_uiWorkQueue;
        private DispatcherTimer m_uiTimer;
        private string[] m_scripts;
        private byte[][] m_scriptsEasy = new byte[][] { new byte[0], Properties.Resources.RedGreenComputerEasy, Properties.Resources.BlueYellowComputerEasy, Properties.Resources.RedGreenComputerEasy, Properties.Resources.BlueYellowComputerEasy };
        private byte[][] m_scriptsHard = new byte[][] { new byte[0], Properties.Resources.RedGreenComputerHard, Properties.Resources.BlueYellowComputerHard, Properties.Resources.RedGreenComputerHard, Properties.Resources.BlueYellowComputerHard };
        private byte[] m_scriptRandom = Properties.Resources.Random;
        private SwarmGame m_swarmGame;

        static MainWindow()
        {
            // initialize the work queue
            m_uiWorkQueue = new List<string>();
        }

        public void On_Closed(object sender, EventArgs args)
        {
            m_swarmGame.IsQuiting = true;
            m_uiTimer.Stop();
        }

        public void On_Loaded(object sender, EventArgs args)
        {
            SwarmWorker[] sw = new SwarmWorker[4];
            TextBlock text;
            Rectangle button;
            Slider slider;

            // capture the canvas reference
            m_root = this;

            // hookup start button
            text = (m_root.FindName("StartText") as TextBlock);
            text.MouseLeftButtonDown += StartButtonClick;
            button = (m_root.FindName("StartButton") as Rectangle);
            button.MouseLeftButtonDown += StartButtonClick;

            // hook up handlers for configuration
            HookRadioClick("RedRadio", RedClick);
            HookRadioClick("YellowRadio", YellowClick);
            HookRadioClick("BlueRadio", BlueClick);
            HookRadioClick("GreenRadio", GreenClick);
            HookRadioClick("FieldRadio", FieldClick);

            // hook up slider updates
            slider = (m_root.FindName("NumSwarmSlider") as Slider);
            slider.ValueChanged += NumSwarmClick;
            slider = (m_root.FindName("DuplicationSlider") as Slider);
            slider.ValueChanged += DuplicationClick;
            slider = (m_root.FindName("WinPerSlider") as Slider);
            slider.ValueChanged += WinPerClick;

            // setup the board based on the field config
            FieldClick(null, null);
            RedClick(null, null);
            YellowClick(null, null);
            BlueClick(null, null);
            GreenClick(null, null);

            // create the ui timer
            m_uiTimer = new DispatcherTimer();
            m_uiTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            m_uiTimer.Tick += new EventHandler(PaintUI);
            m_uiTimer.Start();
        }

        // UI interactions
        private void StartButtonClick(object o, MouseEventArgs e)
        {
            TextBlock text;
            Rectangle button;
            bool[] players;

            // check how many users are playing
            players = new bool[(int)PlotColor.Yellow + 1];

            players[(int)PlotColor.Yellow] = ("" != m_scripts[(int)PlotColor.Yellow]);
            players[(int)PlotColor.Red] = ("" != m_scripts[(int)PlotColor.Red]);
            players[(int)PlotColor.Blue] = ("" != m_scripts[(int)PlotColor.Blue]);
            players[(int)PlotColor.Green] = ("" != m_scripts[(int)PlotColor.Green]);
            if (!players[(int)PlotColor.Yellow] && !players[(int)PlotColor.Red] && !players[(int)PlotColor.Blue] && !players[(int)PlotColor.Green])
            {
                return;
            }

            // make title see through
            ModifyText("TitleShadow", "SWARM", 0.10f);
            ModifyText("TitleMain", "SWARM", 0.20f);

            // remove the click handlers
            text = (m_root.FindName("StartText") as TextBlock);
            text.MouseLeftButtonDown -= StartButtonClick;
            button = (m_root.FindName("StartButton") as Rectangle);
            button.MouseLeftButtonDown -= StartButtonClick;

            // Hide all components
            HideAll();

            // start the players
            for (int i = 1; i < (int)PlotColor.Yellow + 1; i++) if (players[i]) StartPlayer((PlotColor)i, m_swarmGame);

            // start the game
            m_swarmGame.Start(GetSliderValue("NumSwarmSlider"), GetSliderValue("DuplicationSlider"), GetSliderValue("WinPerSlider"), players);
        }

        private void HideAll()
        {
            // hide the buttons
            ModifyRectangle("StartButton", 0.0f);
            ModifyRectangle("RedRectangle", 0.0f);
            ModifyRectangle("YellowRectangle", 0.0f);
            ModifyRectangle("GreenRectangle", 0.0f);
            ModifyRectangle("BlueRectangle", 0.0f);
            ModifyRectangle("ConfigRectangle", 0.0f);
            ModifyRectangle("FieldRectangle", 0.0f);
            ModifyRectangle("HelpBox", 0.0f);

            ModifyText("StartText", "", 0.0f);
            ModifyText("ConfigText", "", 0.0f);
            ModifyText("FieldText", "", 0.0f);
            ModifyText("NumSwarmText", "", 0.0f);
            ModifyText("NumSwarmNumText", "", 0.0f);
            ModifyText("DuplicationText", "", 0.0f);
            ModifyText("DuplicationNumText", "", 0.0f);
            ModifyText("WinPerText", "", 0.0f);
            ModifyText("WinPerNumText", "", 0.0f);
            ModifyText("HelpText", "", 0.0f);

            ModifyRadioButton("RedRadio", 0.0f);
            ModifyRadioButton("YellowRadio", 0.0f);
            ModifyRadioButton("GreenRadio", 0.0f);
            ModifyRadioButton("BlueRadio", 0.0f);
            ModifyRadioButton("FieldRadio", 0.0f);

            ModifySlider("NumSwarmSlider", 0.0f);
            ModifySlider("DuplicationSlider", 0.0f);
            ModifySlider("WinPerSlider", 0.0f);
        }

        private void StartPlayer(PlotColor color, SwarmGame swarmGame)
        {
            Thread thread;
            SwarmWorker sw;

            // create the worker thread
            sw = new SwarmWorker();
            sw.Configure(color, m_scripts[(int)color], swarmGame);

            // kick off the thread
            thread = new Thread(new ThreadStart(sw.RunGame));
            thread.Start();
        }

        private string GetScript(PlotColor color, string player)
        {
            OpenFileDialog dialog;
            string script;

            // set it to the default
            script = "";

            // get the real implementation
            switch (player)
            {
                case "User":
                    // this is a human, so go get the script
                    dialog = new OpenFileDialog();
                    if (true == dialog.ShowDialog())
                    {
                        using (Stream stream = dialog.OpenFile())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                script = reader.ReadToEnd();
                            }
                        }
                    }
                    break;
                case "Easy":
                    script = GetManifestResource(m_scriptsEasy[(int)color]);
                    break;
                case "Hard":
                    script = GetManifestResource(m_scriptsHard[(int)color]);
                    break;
                case "Random":
                    script = GetManifestResource(m_scriptRandom);
                    break;
            }

            return script;
        }

        private string GetManifestResource(byte[] content)
        {
            // convert the byte[] into a string
            var script = System.Text.Encoding.Default.GetString(content);
            return script.Replace("\r\n", System.Environment.NewLine);
        }

        private void FieldClick(object o, EventArgs e)
        {
            string value;
            string transaction = "";

            value = GetRadioValue("FieldRadio");

            // setup board
            switch (value)
            {
                case "Open":
                    m_swarmGame.InitialFieldConfig(FieldConfiguration.Default, out transaction);
                    break;
                case "Quads":
                    m_swarmGame.InitialFieldConfig(FieldConfiguration.Quads, out transaction);
                    break;
                case "Hill":
                    m_swarmGame.InitialFieldConfig(FieldConfiguration.Hill, out transaction);
                    break;
                case "Maze":
                    m_swarmGame.InitialFieldConfig(FieldConfiguration.Maze, out transaction);
                    break;
            }

            PostMessage(PlotColor.Clear, transaction);
        }

        private void NumSwarmClick(object o, RoutedPropertyChangedEventArgs<double> e)
        {
            ModifyText("NumSwarmNumText", Convert.ToInt32(e.NewValue).ToString());
        }

        private void DuplicationClick(object o, RoutedPropertyChangedEventArgs<double> e)
        {
            ModifyText("DuplicationNumText", Convert.ToInt32(e.NewValue).ToString());
        }

        private void WinPerClick(object o, RoutedPropertyChangedEventArgs<double> e)
        {
            ModifyText("WinPerNumText", Convert.ToInt32(e.NewValue).ToString());
        }

        private void RedClick(object o, EventArgs e)
        {
            UpdateScript(PlotColor.Red, "RedRadio");
        }

        private void BlueClick(object o, EventArgs e)
        {
            UpdateScript(PlotColor.Blue, "BlueRadio");
        }

        private void GreenClick(object o, EventArgs e)
        {
            UpdateScript(PlotColor.Green, "GreenRadio");
        }

        private void YellowClick(object o, EventArgs e)
        {
            UpdateScript(PlotColor.Yellow, "YellowRadio");
        }

        private void UpdateScript(PlotColor color, string controlName)
        {
            string value;

            value = GetRadioValue(controlName);

            m_scripts[(int)color] = GetScript(color, value);
        }

        public static void PostMessage(PlotColor color, string msg)
        {
            // paint to screen
            lock (m_uiWorkQueue)
            {
                m_uiWorkQueue.Add(msg);
            }
        }

        public static void Winner(PlotColor color)
        {
            lock (m_uiWorkQueue)
            {
                m_uiWorkQueue.Add(color + " wins!");
            }
        }

        // executes on UI thread
        private void PaintUI(object o, EventArgs e)
        {
            lock (m_uiWorkQueue)
            {
                foreach (string str in m_uiWorkQueue)
                {
                    if (str.StartsWith(SwarmUtil.Seperator + ""))
                    {
                        ChangeState(str.Split(SwarmUtil.Seperator));
                    }
                    else
                    {
                        ModifyText("Status", str);
                    }
                }
                m_uiWorkQueue.Clear();
            }
        }

        // must be run on UI thread
        private void ChangeState(string[] states)
        {
            PlotColor color;
            PlotState state;
            int w, h;
            int wait;
            float opacity;

            foreach (string str in states)
            {
                if (SwarmUtil.Decode(str, out color, out h, out w, out state, out wait))
                {
                    opacity = m_visitedOpacity;
                    switch (state)
                    {
                        case PlotState.Occupied: opacity = m_occupiedOpacity; break;
                        case PlotState.Defended: opacity = m_defendedOpacity; break;
                        case PlotState.Duplication: opacity = m_duplicationOpacity; break;
                        case PlotState.Unoccupied: opacity = m_unoccupiedOpacity; break;
                        case PlotState.Forbidden: opacity = 0f; break;
                    }

                    FlipColor(PlotColor.Clear, h, w, (PlotColor.Clear == color) ? opacity : 0f);
                    FlipColor(PlotColor.Red, h, w, (PlotColor.Red == color) ? opacity : 0f);
                    FlipColor(PlotColor.Blue, h, w, (PlotColor.Blue == color) ? opacity : 0f);
                    FlipColor(PlotColor.Green, h, w, (PlotColor.Green == color) ? opacity : 0f);
                    FlipColor(PlotColor.Yellow, h, w, (PlotColor.Yellow == color) ? opacity : 0f);
                }
            }
        }

        private string GetRadioValue(string control)
        {
            RadioButton radio;
            int cnt = 0;

            while (null != (radio = (m_root.FindName(control + cnt) as RadioButton)))
            {
                if (true == radio.IsChecked)
                {
                    return (radio.Content as string);
                }
                cnt++;
            }

            return "";
        }

        private int GetSliderValue(string control)
        {
            Slider slider = (m_root.FindName(control) as Slider);
            return Convert.ToInt32(slider.Value);
        }

        private void HookRadioClick(string control, RoutedEventHandler handler)
        {
            RadioButton radio;
            int cnt = 0;

            while (null != (radio = (m_root.FindName(control + cnt) as RadioButton)))
            {
                radio.Click += handler;
                cnt++;
            }
        }

        private void FlipColor(PlotColor color, int height, int width, float opacity)
        {
            string controlName;

            controlName = "";
            switch (color)
            {
                case PlotColor.Red: controlName = "Red"; break;
                case PlotColor.Blue: controlName = "Blue"; break;
                case PlotColor.Green: controlName = "Green"; break;
                case PlotColor.Yellow: controlName = "Yellow"; break;
                case PlotColor.Clear: controlName = "Transparent"; break;
            }
            controlName += "_" + height + "_" + width;

            ModifyRectangle(controlName, opacity);
        }

        private void ModifyRectangle(string controlName, float opacity)
        {
            Rectangle rect;
            rect = (m_root.FindName(controlName) as Rectangle);

            rect.Opacity = opacity;
        }

        private void ModifyText(string controlName, string text)
        {
            ModifyText(controlName, text, 1.0f);
        }

        private void ModifyText(string controlName, string text, float opacity)
        {
            TextBlock t;

            t = (m_root.FindName(controlName) as TextBlock);

            t.Text = text;
            t.Opacity = opacity;
        }

        private void ModifyRadioButton(string controlName, float opacity)
        {
            RadioButton radio;
            int cnt = 0;

            while (null != (radio = (m_root.FindName(controlName + cnt) as RadioButton)))
            {
                radio.Opacity = opacity;
                cnt++;
            }
        }


        private void ModifySlider(string controlName, float opacity)
        {
            Slider slider;

            slider = (m_root.FindName(controlName) as Slider);
            slider.Opacity = opacity;
        }
    }

    public class SwarmWorker
    {
        private SwarmGame m_sg;
        private PlotColor m_color;

        public SwarmWorker()
        {
            m_color = PlotColor.Clear;
        }

        public bool Configure(PlotColor color, string script, SwarmGame sg)
        {
            string transaction;

            m_color = color;
            m_sg = sg;

            m_sg.Configure(color, script, out transaction);
            MainWindow.PostMessage(color, transaction);

            return true;
        }

        public void RunGame()
        {
            string transaction;

            while (PlotColor.Clear == m_sg.Winner && !m_sg.IsQuiting)
            {
                if (m_sg.NextTurn(m_color, out transaction))
                {
                    MainWindow.PostMessage(m_color, transaction);
                }

                // yield
                System.Threading.Thread.Sleep(0);
            }

            // Display the winner
            MainWindow.Winner(m_sg.Winner);
        }
    }
}
