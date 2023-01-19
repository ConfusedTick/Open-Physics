using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sim.Particles;

namespace Sim.GUI.SubWindows
{
    /// <summary>
    /// Логика взаимодействия для ParticleSettings.xaml
    /// </summary>
    public partial class ParticleSettings : Window
    {

        public ParticleBase Particle { get; private set; }

        private readonly MainWindow ParentMainWindow;

        public ParticleSettings(MainWindow parent, ParticleBase particle)
        {
            ResizeMode = ResizeMode.CanMinimize;
            ParentMainWindow = parent;
            InitializeComponent();
            Particle = particle;
            Load();
        }

        private bool Check()
        {
            return double.TryParse(angle_textbox.Text, out _) && double.TryParse(acceleration_textbox.Text, out _) && double.TryParse(x_position_textbox.Text, out _) && double.TryParse(y_position_textbox.Text, out _) && double.TryParse(mass_textbox.Text, out _) && double.TryParse(temperature_textbox.Text, out _) && double.TryParse(netforce_textbox.Text, out _);
        }

        private void Save()
        {
            ParentMainWindow.StopInfoUpdater();
            if (!Check())
            {
                save_error_label.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                save_error_label.Visibility = Visibility.Hidden;
            }


            Particle.Position.SetAngle(Convert.ToInt32(angle_textbox.Text));
            Particle.Position.SetAcceleration(Convert.ToDouble(acceleration_textbox.Text));
            Particle.Position.SetForce(Convert.ToDouble(netforce_textbox.Text));
            if (fixed_button.IsChecked ?? true)
            {
                Particle.Position.SetXForced(Convert.ToDouble(x_position_textbox.Text));
                Particle.Position.SetYForced(Convert.ToDouble(y_position_textbox.Text));
            }
            else
            {
                Particle.Position.SetX(Convert.ToDouble(x_position_textbox.Text));
                Particle.Position.SetY(Convert.ToDouble(y_position_textbox.Text));
            }

            Particle.SetMass(Convert.ToDouble(mass_textbox.Text));
            Particle.SetTemperature(Convert.ToDouble(temperature_textbox.Text));
            Particle.SetFixed((bool)fixed_button.IsChecked);
            ParentMainWindow.StartInfoUpdater();
            ParentMainWindow.UpdateParticle(Particle);
        }

        private void Load()
        {
            if (!angle_textbox.IsFocused)
            {
                angle_textbox.Text = Particle.Position.Angle.ToString();
            }

            if (!acceleration_textbox.IsFocused)
            {
                acceleration_textbox.Text = Particle.Position.Acceleration.ToString();
            }

            if (!x_position_textbox.IsFocused)
            {
                x_position_textbox.Text = Particle.Position.X.ToString();
            }

            if (!y_position_textbox.IsFocused)
            {
                y_position_textbox.Text = Particle.Position.Y.ToString();
            }

            if (!mass_textbox.IsFocused)
            {
                mass_textbox.Text = Particle.Mass.ToString();
            }

            if (!temperature_textbox.IsFocused)
            {
                temperature_textbox.Text = Particle.Temperature.ToString();
            }

            if (!netforce_textbox.IsFocused)
            {
                netforce_textbox.Text = Particle.Position.NetForce.ToString();
            }

            if (!fixed_button.IsFocused)
            {
                fixed_button.IsChecked = Particle.Fixed;
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public void UpdateParticleInfo()
        {
            Load();
        }

        public void ChangeParticle(ParticleBase particle)
        {
            Particle = particle;
            Load();
        }
    }
}
