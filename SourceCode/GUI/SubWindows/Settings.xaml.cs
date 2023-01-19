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
using Sim.Enums;
using Sim;
using Sim.GlmMath;

namespace Sim.GUI.SubWindows
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {

        private readonly MainWindow ParentMainWindow;

        public Settings(MainWindow window)
        {
            ResizeMode = ResizeMode.CanMinimize;
            ParentMainWindow = window;
            ResizeMode = ResizeMode.CanMinimize;
            InitializeComponent();
            Load();
        }

        private bool CheckSettings()
        {
            return double.TryParse(max_temperature_textbox.Text, out _) && double.TryParse(min_temperature_textbox.Text, out _) && double.TryParse(raycast_depth_step_textbox.Text, out _) && int.TryParse(raycast_rays_textbox.Text, out _) && double.TryParse(stefan_boltzmann_const_textbox.Text, out _) && double.TryParse(gravity_acceleration_textbox.Text, out _) && double.TryParse(start_acceleration_textbox.Text, out _) && int.TryParse(smoothness_textbox.Text, out _) && double.TryParse(seconds_per_tick_textbox.Text, out _) && double.TryParse(gravity_angle_textbox.Text, out _);
        }

        public void Load()
        {
            if (!min_temperature_textbox.IsFocused)
                min_temperature_textbox.Text = ParentMainWindow.Map.Physics.MinTemperature.ToString();
            if (!max_temperature_textbox.IsFocused)
                max_temperature_textbox.Text = ParentMainWindow.Map.Physics.MaxTemperature.ToString();
            if (!heat_radiation_render_combobox.IsFocused)
                heat_radiation_render_combobox.Text = (byte)ParentMainWindow.Map.Physics.HeatRender switch
            {
                (byte)HeatRadiationRenders.NONE => "none",
                (byte)HeatRadiationRenders.RC => "raycast",
                (byte)HeatRadiationRenders.RT => "raytrace",
                (byte)HeatRadiationRenders.LRT => "lazyrt",
                _ => "none",
            };
            if (!raycast_depth_step_textbox.IsFocused)
                raycast_depth_step_textbox.Text = ParentMainWindow.Map.Physics.CasterDepthStep.ToString();
            if (!raycast_rays_textbox.IsFocused)
                raycast_rays_textbox.Text = ParentMainWindow.Map.Physics.RaycastRayNumbers.ToString();
            if (!stefan_boltzmann_const_textbox.IsFocused)
                stefan_boltzmann_const_textbox.Text = ParentMainWindow.Map.Physics.StefanBoltzmannConst.ToString();
            if (!gravity_acceleration_textbox.IsFocused)
                gravity_acceleration_textbox.Text = ParentMainWindow.Map.Physics.GravityAcceleration.ToString();
            if (!start_acceleration_textbox.IsFocused)
                start_acceleration_textbox.Text = ParentMainWindow.Map.Physics.StartAcceleration.ToString();
            if (!smoothness_textbox.IsFocused)
                smoothness_textbox.Text = ParentMainWindow.Map.Physics.Smoothness.ToString();
            if (!seconds_per_tick_textbox.IsFocused)
                seconds_per_tick_textbox.Text = ParentMainWindow.Map.Physics.DeltaTime.ToString();
            if (!gravity_angle_textbox.IsFocused)
                gravity_angle_textbox.Text = ParentMainWindow.Map.Physics.GravityVectorAngle.ToString();


        }

        public void Save(object sender, RoutedEventArgs e)
        {
            if (!CheckSettings())
            {
                save_error_label.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                save_error_label.Visibility = Visibility.Hidden;
            }
            ParentMainWindow.Map.Physics.MaxTemperature = Convert.ToDouble(max_temperature_textbox.Text);
            ParentMainWindow.Map.Physics.MinTemperature = Convert.ToDouble(min_temperature_textbox.Text);
            HeatRadiationRenders render = heat_radiation_render_combobox.Text switch
            {
                "none" => HeatRadiationRenders.NONE,
                "raytrace" => HeatRadiationRenders.RT,
                "raycast" => HeatRadiationRenders.RC,
                "lazyrt" => HeatRadiationRenders.LRT,
                _ => HeatRadiationRenders.NONE,
            };
            ParentMainWindow.Map.Physics.HeatRender = render;
            ParentMainWindow.Map.Physics.CasterDepthStep = Convert.ToDouble(raycast_depth_step_textbox.Text);
            ParentMainWindow.Map.Physics.RaycastRayNumbers = Convert.ToInt32(raycast_rays_textbox.Text);
            ParentMainWindow.Map.Physics.StefanBoltzmannConst = Convert.ToDouble(stefan_boltzmann_const_textbox.Text);
            ParentMainWindow.Map.Physics.GravityAcceleration = Convert.ToDouble(gravity_acceleration_textbox.Text);
            ParentMainWindow.Map.Physics.StartAcceleration = Convert.ToDouble(start_acceleration_textbox.Text);
            ParentMainWindow.Map.Physics.Smoothness = Convert.ToInt32(smoothness_textbox.Text);
            ParentMainWindow.Map.Physics.DeltaTime = Convert.ToDouble(seconds_per_tick_textbox.Text);
            ParentMainWindow.Map.Physics.GravityVectorAngle = Convert.ToInt32(gravity_angle_textbox.Text);

            ParentMainWindow.Map.Physics.Update();
        }
    }
}
