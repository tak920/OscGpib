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
using VisaComLib;


namespace OscGpib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string cmd = cmd_box.Text;
            sending_cmd.Content = cmd;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string cmd = cmd_input.Text;
            sending_cmd.Content = cmd;

        }

        private void Meas1_Set_Btn_Click(object sender, RoutedEventArgs e)
        {
            string meas = Meas1_Select.Text;
            string cmd = "MEASUrement:MEAS1:TYPe " + meas;
            Send_SCPI(cmd);
        }
        private void Meas1_Get_Btn_Click(object sender, RoutedEventArgs e)
        {
            string meas = Meas1_Select.Text;
            string cmd = "MEASUrement:MEAS1:VALUE?";
            Send_SCPI(cmd);

        }

        private void Meas2_Set_Btn_Click(object sender, RoutedEventArgs e)
        {
            string meas = Meas1_Select.Text;
            string cmd = "MEASUrement:MEAS2:TYPe " + meas;
            Send_SCPI(cmd);
        }
        private void Meas2_Get_Btn_Click(object sender, RoutedEventArgs e)
        {
            string meas = Meas1_Select.Text;
            string cmd = "MEASUrement:MEAS1:VALUE?";
            Send_SCPI(cmd);
        }

        private void Send_SCPI(string cmd)
        {
            // ResourceManager rm = new ResourceManager();
            // FormattedIO488 inst = new FormattedIO488();
            // inst.IO = rm.Open("GPIB0::1::INSTR");
            // inst.WriteString(":HEAD OFF");
            // inst.WriteString(cmd);

            MessageBox.Show("送信 ---> " + cmd);
            // inst.IO.Close();
        }
        private void MeasRefLevel()
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager();
            FormattedIO488 inst = new FormattedIO488();
            inst.IO = (IMessage)rm.Open("GPIB0::1::INSTR");
            inst.IO.Timeout = 500;
            inst.WriteString(":HEAD OFF");

            string cmd = "ACQuire:STOPAfter SEQUENCE";  //{RUNSTop|SEQuence}
            inst.WriteString(cmd);

            string meas = Meas_Max_or_Min.Text;
            cmd = "MEASUrement:MEAS1:TYPe " + meas;
            inst.WriteString(cmd);

            meas = Meas_Rise_or_Fall.Text;
            cmd = "MEASUrement:MEAS2:TYPe " + meas;
            inst.WriteString(cmd);

            cmd = "MEASUrement:MEAS1:VALUE?";
            inst.WriteString(cmd);
            var respons = inst.ReadString();
            respons = respons.Replace("\n", "");
            var voltage = double.Parse(respons);
            output_volt.Content = voltage;

            var refLevelLowVolt = voltage * 0.1;
            var refLevelHighVolt = voltage * 0.9;

            cmd = "CURSOR:HBARS:POSITION2 " + refLevelHighVolt;
            inst.WriteString(cmd);

            cmd = "CURSOR:HBARS:POSITION1 " + refLevelLowVolt;
            inst.WriteString(cmd);

            cmd = "MEASUrement:REFLevel:METHOD ABSolute";
            inst.WriteString(cmd);

            cmd = "MEASUrement:REFLevel:ABSolute:High " + refLevelHighVolt;
            inst.WriteString(cmd);

            cmd = "MEASUrement:REFLevel:ABSolute:Low " + refLevelLowVolt;
            inst.WriteString(cmd);

            cmd = "MEASUrement:MEAS2:VALUE?";
            inst.WriteString(cmd);
            respons = inst.ReadString();
            respons = respons.Replace("\n", "");
            var tr = double.Parse(respons);
            tr = tr * (10 ^ 9);
            output_tr.Content = tr;


        }
    }
}
