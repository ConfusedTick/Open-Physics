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
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using System.IO;
using Sim.GlmMath;
using Sim.GUI.SubWindows;
using Sim.Map;
using Sim.Particles;
using Sim.Simulation;
using Sim.Events;
using Sim;
using Microsoft.Win32;
using Sim.Utils;

namespace Sim.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MapBase Map;

        // Key equals to particle uid.
        public Dictionary<int, Rectangle> ParticlesRectangles { get; private set; } = new Dictionary<int, Rectangle>();
        public List<Rectangle> Dots = new List<Rectangle>();

        // Full size of particle label
        public double SizeMult { get; private set; } = 14.9d;

        // Current particle to place
        public int CurrentParticle { get; private set; } = (int)ParticleIds.WATER;

        public double Affection { get; private set; } = 0d;

        public bool HeatDisplay { get; private set; } = true;

        // Size of label with particle itself
        public double Size { get; private set; }

        // Window background color
        private readonly Color _backColor = Colors.Black;

        // All additional controls
        private readonly Label _infoLabel;
        private readonly Label _globalInfoLabel;
        private readonly Label _pausedLabel;
        private readonly Button _openSettings;
        private readonly Button _newPageButton;
        private readonly Button _pickUpMapSaveButton;
        private readonly Button _saveMapButton;
        private readonly Button _enableTemperatureGradient;

        private GradientStopCollection TemperatureGradientStops;

        private ParticleBase _watching;
        private readonly DispatcherTimer _watchingUpdater;

        private bool _showSettings = false;

        private bool IsOpenedSettings => _settingsWindow != null;
        private Settings _settingsWindow;

        private bool IsOpenedParticleSettings => _particleSettingsWindow != null;
        private ParticleSettings _particleSettingsWindow;

        private FontFamily MainFontFamily => new FontFamily(MainFontFamilyName);

        private const string MainFontFamilyName = "Consolas";
        private const int MainFontFamilySize = 15;
        private const int OpenSettingsButtonSize = 25;
        private const int NewPageButtonSize = 25;
        private const int PickUpMapSaveButtonSize = 25;
        private const int SaveMapButtonSize = 25;
        private const int TemperatureGradientCheckBoxSize = 25;
        // Indicates how much px part with a title takes
        private const int TitlePartHeight = 40;
        // Indicates how much px takes part with menu (settings, clear, etc...)
        private const int MenuPartWidth = 20;
        // Indicates border between blocks. (SizeMult / Size)
        private const double SizeMultToSizeCoeff = 0.95d;
        // Indicates deviation, which part of width should deviation take
        private const double SizeAddDeviationMult = 0.75d;
        // Indicates stack panel image border in GenerateStackPanel function
        private const double ImageBorderThickness = -0.5d;
        // Temperature gradient stroke thickness
        private const double RectangleStrokeThickness = 3;
        // Indicates visual info label update interval between ticks
        private const int WatchingUpdaterInterval = 75;

        private const string WindowTitle = "Open Physics - Demo HR2Pr";

        public MainWindow(MapBase map)
        {
            Logger.Log("Preparing window...", "MainWindow");
            Map = map;

            InitializeComponent();

            // Creating updating timer
            _watchingUpdater = new DispatcherTimer();
            _watchingUpdater.Tick += WindowUpdate;
            _watchingUpdater.Interval = TimeSpan.FromMilliseconds(WatchingUpdaterInterval);


            // Setting up simple window parameters
            Title = WindowTitle;
            SetupScreenSize();
            ResizeMode = ResizeMode.CanMinimize;
            Background = new SolidColorBrush(_backColor);
            main_canvas.Background = new SolidColorBrush(_backColor);


            // Global info label ---
            _globalInfoLabel = new Label
            {
                FontFamily = MainFontFamily,
                FontSize = MainFontFamilySize,
                Foreground = new SolidColorBrush(Colors.White),
            };
            Canvas.SetRight(_globalInfoLabel, MenuPartWidth);
            Canvas.SetTop(_globalInfoLabel, TitlePartHeight);
            _ = main_canvas.Children.Add(_globalInfoLabel);


            // Paused label ---
            _pausedLabel = new Label
            {
                FontFamily = MainFontFamily,
                FontSize = MainFontFamilySize,
                Foreground = new SolidColorBrush(Colors.Red),
                Content = "Paused",
                Visibility = Map.IsTicking ? Visibility.Hidden : Visibility.Visible,
            };
            Canvas.SetLeft(_pausedLabel, (int)(main_canvas.Width / 2) - MenuPartWidth);
            Canvas.SetTop(_pausedLabel, TitlePartHeight);
            _ = main_canvas.Children.Add(_pausedLabel);


            // ParticleBase info label ---
            _infoLabel = new Label
            {
                FontFamily = MainFontFamily,
                FontSize = MainFontFamilySize,
                Foreground = new SolidColorBrush(Colors.White),
            };
            Canvas.SetLeft(_infoLabel, 0);
            Canvas.SetTop(_infoLabel, TitlePartHeight);
            _ = main_canvas.Children.Add(_infoLabel);


            // Open settings button ---
            _openSettings = new Button
            {
                Focusable = false,
                FontFamily = MainFontFamily,
                Width = OpenSettingsButtonSize,
                Height = OpenSettingsButtonSize,
                Content = GenerateImageStackPanel(Directory.GetCurrentDirectory() + "/Assets/Buttons/openSettings.png"),
            };
            _openSettings.Click += OpenSettings;
            Canvas.SetBottom(_openSettings, 0);
            Canvas.SetLeft(_openSettings, 0);
            _ = main_canvas.Children.Add(_openSettings);


            // New page button ---
            _newPageButton = new Button
            {
                Focusable = false,
                FontFamily = MainFontFamily,
                Width = NewPageButtonSize,
                Height = NewPageButtonSize,
                Content = GenerateImageStackPanel(Directory.GetCurrentDirectory() + "/Assets/Buttons/newPage.png"),
            };
            _newPageButton.Click += ClearMap;
            Canvas.SetBottom(_newPageButton, 0 + OpenSettingsButtonSize);
            Canvas.SetLeft(_newPageButton, 0);
            _ = main_canvas.Children.Add(_newPageButton);



            // Pick up map save file button ---
            _pickUpMapSaveButton = new Button
            {
                Focusable = false,
                FontFamily = MainFontFamily,
                Width = PickUpMapSaveButtonSize,
                Height = PickUpMapSaveButtonSize,
                Content = GenerateImageStackPanel(Directory.GetCurrentDirectory() + "/Assets/Buttons/pickUpMapSave.png"),
            };
            _pickUpMapSaveButton.Click += PickUpMapSaveButtonClick;
            Canvas.SetBottom(_pickUpMapSaveButton, 0 + OpenSettingsButtonSize + NewPageButtonSize);
            Canvas.SetLeft(_pickUpMapSaveButton, 0);
            _ = main_canvas.Children.Add(_pickUpMapSaveButton);

            // Save map file button ---
            _saveMapButton = new Button
            {
                Focusable = false,
                FontFamily = MainFontFamily,
                Width = SaveMapButtonSize,
                Height = SaveMapButtonSize,
                Content = GenerateImageStackPanel(Directory.GetCurrentDirectory() + "/Assets/Buttons/saveMap.png"),
            };
            _saveMapButton.Click += SaveMapButtonClick;
            Canvas.SetBottom(_saveMapButton, 0 + OpenSettingsButtonSize + NewPageButtonSize + PickUpMapSaveButtonSize);
            Canvas.SetLeft(_saveMapButton, 0);
            _ = main_canvas.Children.Add(_saveMapButton);

            // Temperature gradient colors settings ---
            _enableTemperatureGradient = new Button
            {
                Focusable = false,
                FontFamily = MainFontFamily,
                Width = TemperatureGradientCheckBoxSize,
                Height = TemperatureGradientCheckBoxSize,
                Content = GenerateImageStackPanel(Directory.GetCurrentDirectory() + "/Assets/Buttons/enableTemperatureGradient.png"),
                
            };
            _enableTemperatureGradient.Click += TemperatureGradientCheckboxChanged;
            Canvas.SetBottom(_enableTemperatureGradient, 0 + OpenSettingsButtonSize + NewPageButtonSize + PickUpMapSaveButtonSize + SaveMapButtonSize);
            Canvas.SetLeft(_enableTemperatureGradient, 0);
            _ = main_canvas.Children.Add(_enableTemperatureGradient);

            // Setting up events
            KeyDown += Window_KeyDown;
            Closed += CloseMainWindow;
            main_canvas.MouseRightButtonDown += PlaceParticle;

            TemperatureGradientStops = new GradientStopCollection
            {
                new GradientStop(Colors.Cyan, 0),
                new GradientStop(Colors.LightCyan, .07),
                new GradientStop(Colors.Gray, .1),
                new GradientStop(Colors.Red, .75),
                new GradientStop(Colors.Yellow, 1),
            };

            LoadMap();

            Logger.Log("Window ready", "MainWindow", '!', ConsoleColor.Green);
            StartInfoUpdater();
            UpdateGlobalInfo();
        }

        private void TemperatureGradientCheckboxChanged(object sender, RoutedEventArgs e)
        {
            HeatDisplay = !HeatDisplay;
            foreach (ParticleBase part in Map.Particles)
            {
                UpdateParticle(part);
            }
        }

        private void SaveMapButtonClick(object sender, RoutedEventArgs e)
        {
            SaveMap();
        }

        private void PickUpMapSaveButtonClick(object sender, RoutedEventArgs e)
        {
            PickUpMapSave();
        }

        public void StartInfoUpdater()
        {
            _watchingUpdater.Start();
        }

        public void StopInfoUpdater()
        {
            _watchingUpdater.Stop();
        }

        private static StackPanel GenerateImageStackPanel(string source)
        {
            Image img = new Image
            {
                Source = new BitmapImage(new Uri(source))
            };

            StackPanel stackPnl = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(ImageBorderThickness),
            };
            _ = stackPnl.Children.Add(img);
            return stackPnl;
        }

        private void SetupScreenSize()
        {
            Size = SizeMult * SizeMultToSizeCoeff;
            Width = (double)(Map.Size.Width * SizeMult) + (double)(Map.Size.Width * SizeAddDeviationMult);
            Height = (double)(Map.Size.Height * SizeMult) + (double)(Map.Size.Height * SizeAddDeviationMult);
            main_canvas.Width = (double)(Map.Size.Width * SizeMult) + (double)(Map.Size.Width * SizeAddDeviationMult);
            main_canvas.Height = (double)(Map.Size.Height * SizeMult) + (double)(Map.Size.Height * SizeAddDeviationMult);
            Logger.Log($"Screen size updated to {Width}:{Height} - (ParticleSize:{Size})", "MainWindow");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    if (Map.IsTicking)
                    {
                        Map.StopTicking();
                    }
                    else
                    {
                        Map.StartTicking();
                    }
                    UpdateGlobalInfo();

                    break;

                case Key.F4:
                    _showSettings = !_showSettings;
                    UpdateGlobalInfo();
                    break;

                default:
                    break;
            }
        }

        public void UpdateParticle(ParticleBase part)
        {
            UpdateParticle(part, EventArgs.Empty);
        }

        // NOTE: Will run ONLY after the MainWindow is created for this map!
        private void NewParticleAdded(object sender, EventArgs ea)
        {
            ParticleAddedEventArgs e = (ParticleAddedEventArgs)ea;
            ParticleBase part = e.Particle;
            Rectangle rectangle = new Rectangle
            {
                Width = Size,
                Height = Size,
                Uid = part.Uid.ToString(),
                Fill = new SolidColorBrush(part.Color),

            };
            Canvas.SetLeft(rectangle, Math.Round(part.Position.X * SizeMult + 3 * Size, Map.Physics.Smoothness, MidpointRounding.ToEven));
            Canvas.SetBottom(rectangle, Math.Round(part.Position.Y * SizeMult, Map.Physics.Smoothness, MidpointRounding.ToEven));
            rectangle.MouseEnter += MouseHoverOn;
            rectangle.MouseLeave += MouseHoverOff;
            rectangle.MouseLeftButtonDown += ParticleRectangleMouseClick;
            rectangle.StrokeThickness = RectangleStrokeThickness;
            part.ParticlePositionChanged += UpdateParticle;
            part.ParticleRemoved += RemoveParticle;
            ParticlesRectangles.Add(part.Uid, rectangle);
            _ = main_canvas.Children.Add(rectangle);
            UpdateHeatColor(part);
        }

        private void UpdateHeatColor(ParticleBase part)
        {
            
            Rectangle rectangle = ParticlesRectangles[part.Uid];
            if (rectangle == null)
            {
                return;
            }

            if (!HeatDisplay)
            {
                rectangle.Fill = new SolidColorBrush(part.Color);
                return;
            }

            double gradientCoeff = (Math.Abs(Map.Physics.MinTemperature) + part.Temperature) / Map.Physics.TemperatureDelta;
            
            rectangle.Fill = new SolidColorBrush(TemperatureGradientStops.GetRelativeColor(gradientCoeff));
        }

        private void UpdateParticle(object sender, EventArgs e)
        {
            ParticleBase part = (ParticleBase)sender;

            ParticlePositionParameters Position = part.Position;


            if (HeatDisplay) UpdateHeatColor(part);
            else ParticlesRectangles[part.Uid].Fill = new SolidColorBrush(part.Color);

            if (Math.Round(Position.PreviousX, Map.Physics.Smoothness) == Math.Round(Position.X, Map.Physics.Smoothness) && Math.Round(Position.PreviousY, Map.Physics.Smoothness) == Math.Round(Position.Y, Map.Physics.Smoothness) && part.PreviousState == part.CurrentState)
            {
                return;
            }

            Rectangle rectangle = ParticlesRectangles[part.Uid];


            if (rectangle == null)
            {
                return;
            }

            _ = rectangle.Dispatcher.BeginInvoke(() => Canvas.SetLeft(rectangle, Math.Round(Position.X * SizeMult + 3 * Size, Map.Physics.Smoothness, MidpointRounding.ToEven)));
            _ = rectangle.Dispatcher.BeginInvoke(() => Canvas.SetBottom(rectangle, Math.Round(Position.Y * SizeMult, Map.Physics.Smoothness, MidpointRounding.ToEven)));
        }

        public void RemoveParticle(object sender, EventArgs e)
        {
            ParticleBase part = (ParticleBase)sender;
            Rectangle rectangle = ParticlesRectangles[part.Uid];

            if (rectangle == null)
            {
                return;
            }
            rectangle.MouseEnter -= MouseHoverOn;
            rectangle.MouseLeave -= MouseHoverOff;
            rectangle.MouseLeftButtonDown -= ParticleRectangleMouseClick;
            if (rectangle.IsMouseOver) _infoLabel.Content = "";
            main_canvas.Children.Remove(rectangle);
            ParticlesRectangles.Remove(part.Uid);
            if (IsOpenedParticleSettings && _particleSettingsWindow.Particle == part) _particleSettingsWindow.Close();
        }

        public void ShowParticleSettings(ParticleBase particle)
        {
            if (IsOpenedParticleSettings)
            {
                _particleSettingsWindow.ChangeParticle(particle);
                return;
            }
            _particleSettingsWindow = new ParticleSettings(this, particle)
            {
                Owner = this
            };
            _particleSettingsWindow.Closed += CloseParticleSettings;
            _particleSettingsWindow.Show();
        }

        public void PickUpMapSave()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location,
                Filter = "opm files (*.opm)|*.opm",
                FilterIndex = 1,
                RestoreDirectory = true,

                Title = "Load map"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            string filePath = dialog.FileName;
            if (File.Exists(filePath))
            {
                ChangeMap(MapBase.Load(filePath, Map.Physics));
            }
        }

        public void ChangeMap(MapBase mp)
        {
            ClearMap();
            Map = mp;
            Core.ChangeMap(Map);
            LoadMap();
            Logger.Log("New map opened");
        }

        public void SaveMap()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location,
                Filter = "opm files (*.opm)|*.opm",
                FilterIndex = 1,
                RestoreDirectory = true,

                Title = "Save map"
            };

            if (dialog.ShowDialog() == false)
            {
                return;
            }

            string filePath = dialog.FileName;

            Map.Save(filePath);
            Logger.Log("Map saved to " + filePath);
        }

        // NOTE: All map events should be initialized here
        public void LoadMap()
        {
            Logger.Log("Loading map...", "MainWindow");
            Map.Physics.PhysicParametersChanged += PhysicsChanged;
            Map.ParticleAdded += NewParticleAdded;
            Rectangle rectangle;
            foreach (ParticleBase part in Map.Particles.ToList())
            {
                rectangle = new Rectangle
                {
                    Width = Size,
                    Height = Size,
                    Uid = part.Uid.ToString(),
                    Fill = new SolidColorBrush(part.Color),

                };
                Canvas.SetLeft(rectangle, Math.Round(part.Position.X * SizeMult + 3 * Size, Map.Physics.Smoothness, MidpointRounding.ToEven));
                Canvas.SetBottom(rectangle, Math.Round(part.Position.Y * SizeMult, Map.Physics.Smoothness, MidpointRounding.ToEven));
                rectangle.MouseEnter += MouseHoverOn;
                rectangle.MouseLeave += MouseHoverOff;
                rectangle.MouseLeftButtonDown += ParticleRectangleMouseClick;
                rectangle.StrokeThickness = RectangleStrokeThickness;
                part.ParticlePositionChanged += UpdateParticle;
                part.ParticleRemoved += RemoveParticle;
                ParticlesRectangles.Add(part.Uid, rectangle);
                _ = main_canvas.Children.Add(rectangle);
                UpdateHeatColor(part);
            }
            Logger.Log("Map loaded", "MainWindow");
        }
        
        public double MapXToWindowX(double x)
        {
            return x * SizeMult + 3 * Size;
        }

        public double MapYToWindowY(double y)
        {
            return y * SizeMult;
        }

        public void Dot(double x, double y)
        {
            Rectangle dot = new Rectangle
            {
                Height = .9d,
                Width = .9d,
                Fill = new SolidColorBrush(Colors.White)
            };
            Canvas.SetLeft(dot, MapXToWindowX(x));
            Canvas.SetBottom(dot, MapYToWindowY(y));
            main_canvas.Children.Add(dot);
            Dots.Add(dot);
        }

        private void PlaceParticle(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver != main_canvas) return;
            Point p = Mouse.GetPosition(main_canvas);
            double x = p.X - MenuPartWidth - (SizeMult * SizeAddDeviationMult) - SizeMult;
            double y = main_canvas.Height - p.Y - (SizeMult / 2);

            double mx = x / SizeMult;
            double my = y / SizeMult;

            if (Map.IsInParticleArea(mx, my) == null) ParticleFactory.GetFactory(Map).AddNew(CurrentParticle, Math.Round(mx, MidpointRounding.AwayFromZero), Math.Round(my, MidpointRounding.AwayFromZero), Flags.Empty, Affection);
        }

        private void ParticleRectangleMouseClick(object sender, MouseButtonEventArgs e)
        {
            Rectangle rectangle = (Rectangle)sender;
            ShowParticleSettings(Map.ParticlesDictionary[Convert.ToInt32(rectangle.Uid)]);
        }

        private void MouseHoverOff(object sender, EventArgs e)
        {
            _infoLabel.Content = string.Empty;
            _watching = null;
        }

        private void CloseSettings(object sender, EventArgs e)
        {
            if (!IsOpenedSettings)
            {
                throw new InvalidOperationException("MainWindow.CloseSettings() - Settings window is not opened");
            }
            _settingsWindow.Closed -= CloseSettings;
            _settingsWindow = null;
        }

        private void CloseParticleSettings(object sender, EventArgs e)
        {
            if (!IsOpenedParticleSettings)
            {
                throw new InvalidOperationException("MainWindow.CloseParticleSettings() - ParticleBase settings is not opened");
            }
            _particleSettingsWindow.Closed -= CloseParticleSettings;
            _particleSettingsWindow = null;
        }

        public void Unsubscribe()
        {
            Map.Physics.PhysicParametersChanged -= PhysicsChanged;
            Map.ParticleAdded -= NewParticleAdded;
            _watchingUpdater.Stop();
            if (IsOpenedSettings)
            {
                _settingsWindow.Closed -= OpenSettings;
            }

            if (IsOpenedParticleSettings)
            {
                _particleSettingsWindow.Closed -= CloseParticleSettings;
            }

            KeyDown -= Window_KeyDown;
            Closed -= CloseMainWindow;
            Map.Physics.PhysicParametersChanged -= PhysicsChanged;

            foreach (Rectangle rectangle in ParticlesRectangles.Values)
            {
                rectangle.MouseEnter -= MouseHoverOn;
                rectangle.MouseLeave -= MouseHoverOff;
                rectangle.MouseDown -= ParticleRectangleMouseClick;
            }
            Map.Particles.ForEach(part => part.ParticlePositionChanged -= UpdateParticle);
            Map.Particles.ForEach(part => part.ParticleRemoved -= RemoveParticle);
            Logger.Log("Window events unsubscribed", "MainWindow");
        }

        private void OpenSettings(object sender, EventArgs e)
        {
            if (IsOpenedSettings)
            {
                //_settingsWindow.Topmost = true;
                //_settingsWindow.Topmost = false;
                return;
            }

            Settings settings = new Settings(this);
            settings.Closed += CloseSettings;
            settings.Show();
            settings.Owner = this;
            _settingsWindow = settings;
        }

        private void ClearMap(object sender, EventArgs e)
        {
            ClearMap();
        }

        public void ClearMap()
        {
            foreach(Rectangle rectangle in ParticlesRectangles.Values)
            {
                main_canvas.Children.Remove(rectangle);
            }

            ParticlesRectangles.Clear();
            Map.Clear();
            Logger.Log("Map cleaned", "MainWindow");
        }

        private void CloseMainWindow(object sender, EventArgs e)
        {
            Logger.Log("Application close initialized", "MainWindow");

            Unsubscribe();

            if (IsOpenedSettings)
            {
                _settingsWindow.Close();
            }

            if (IsOpenedParticleSettings)
            {
                _particleSettingsWindow.Close();
            }

        }

        private void PhysicsChanged(object sender, EventArgs e)
        {
            UpdateGlobalInfo();
        }

        public void UpdateGlobalInfo()
        {
            foreach(Rectangle rect in Dots)
            {
                main_canvas.Children.Remove(rect);
            }
            Dots.Clear();

            _pausedLabel.Visibility = Map.IsTicking ? Visibility.Hidden : Visibility.Visible;

            if (!_showSettings)
            {
                _globalInfoLabel.Content = "Press F4 for settings"
                    + "\nHover mouse on particle to see info"
                    + "\nPress space to pause/unpause";
                return;
            }

            _globalInfoLabel.Content = "World settings:"
                + "\nGVA=" + Map.Physics.GravityVectorAngle
                + "\nGA=" + Map.Physics.GravityAcceleration
                + "\nSPT=" + Map.Physics.DeltaTime
                + "\nSA=" + Map.Physics.StartAcceleration
                + "\nS=" + Map.Physics.Smoothness
                + "\nSBC=" + Map.Physics.StefanBoltzmannConst
                + "\nCDS=" + Map.Physics.CasterDepthStep
                + "\nMiT=" + Map.Physics.MinTemperature
                + "\nMaT=" + Map.Physics.MaxTemperature
                + "\nParC=" + Map.Particles.Count
                + "\nIntC=" + Map.Instruments.Count
                + "\nPerf=" + Math.Round(Map.Performance * 100) + "%";
        }

        private void WindowUpdate(object sender, EventArgs e)
        {
            UpdateWatchingParticleInfo(this, EventArgs.Empty);
            if (IsOpenedParticleSettings)
            {
                _particleSettingsWindow.UpdateParticleInfo();
            }
        }


        private void UpdateWatchingParticleInfo(object sender, EventArgs e)
        {
            
            UpdateGlobalInfo();
            if (_watching == null)
            {
                return;
            }

            ParticleBase particle = _watching;
            _infoLabel.Content =
                    "X=" + Math.Round(particle.Position.X, Map.Physics.Smoothness) + " M" +
                    "\nY=" + Math.Round(particle.Position.Y, Map.Physics.Smoothness) + " M" +
                    "\nId=" + particle.Id +
                    "\nName=" + particle.Name +
                    "\nUid=" + particle.Uid + 
                    "\nSize=" + particle.Size +
                    "\nSpeed=" + Math.Round(particle.Position.Speed, Map.Physics.Smoothness) + " M/T" +
                    "\nTemp=" + Math.Round(particle.Temperature, Map.Physics.Smoothness) + " C" +
                    "\nEmittingCoeff=" + Math.Round(particle.EmittingCoeff, Map.Physics.Smoothness) +
                    "\nAcceptanceCoeff=" + Math.Round(particle.AcceptanceCoeff, Map.Physics.Smoothness) +
                    "\nTransparency=" + Math.Round(particle.Transparency, Map.Physics.Smoothness) +
                    "\nHeatCapacity=" + Math.Round(particle.HeatCapacity, Map.Physics.Smoothness) + " J/KG * C" +
                    "\nHalfLenght=" + Math.Round(particle.halfLenght, Map.Physics.Smoothness) + " M" +
                    "\nAccel=" + Math.Round(particle.Position.Acceleration, Map.Physics.Smoothness) + " M/T2" +
                    "\nAngle=" + Math.Round((double)particle.Position.Angle, Map.Physics.Smoothness) + " DEG" +
                    "\nMass=" + Math.Round((double)particle.Mass, Map.Physics.Smoothness) + " KG" + 
                    "\nWeight=" + Math.Round(particle.Position.Weight, Map.Physics.Smoothness) + " N" +
                    "\nForce=" + Math.Round(particle.Position.NetForce, Map.Physics.Smoothness) + " N" +
                    "\nAggregState=" + particle.CurrentState +
                    "\nHeatBuffer=" + Math.Round(particle.HeatBuffer, Map.Physics.Smoothness) + " J" +
                    "\nRequireRandomTicks=" + particle.RequireRandomTick +
                    "\nRandomTicksRarity=" + particle.RandomTickRarity + 
                    "\nLifeTick=" + particle.LifeTick + 
                    "\nFixed=" + particle.Fixed;

        }

        private void MouseHoverOn(object sender, EventArgs e)
        {
            Rectangle rectangle = (Rectangle)sender;
            if (!Map.ParticlesDictionary.ContainsKey(Convert.ToInt32(rectangle.Uid)))
            {
                return;
            }
            ParticleBase particle = Map.ParticlesDictionary[Convert.ToInt32(rectangle.Uid)];
            if (particle != null)
            {
                _watching = particle;
            }
        }
    }
}
