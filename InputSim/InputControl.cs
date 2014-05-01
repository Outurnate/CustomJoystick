using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InputSim
{
    public partial class InputControl : Form
    {
        private ArduinoInterface iface;
        private ArduinoInterface.KeyboardConfig config;

        public InputControl()
        {
            InitializeComponent();
            portName.Items.AddRange(SerialPort.GetPortNames());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            iface = new ArduinoInterface(portName.Text);
            try
            {
                iface.Open();
                updateSettings();
            }
            catch (UnauthorizedAccessException)
            {
                button2_Click(this, e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            iface.Close();
        }

        private void InputControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (iface != null)
                iface.Close();
        }

        private void invertX_CheckedChanged(object sender, EventArgs e)
        {
            updateSettings();
        }

        private void invertY_CheckedChanged(object sender, EventArgs e)
        {
            updateSettings();
        }

        private void mouseSens_Scroll(object sender, EventArgs e)
        {
            updateSettings();
        }

        private void updateSettings()
        {
            if (iface != null)
            {
                iface.MouseSensitivity = (float)(mouseSens.Value / 100);
                iface.InvertY = invertY.Checked;
                iface.InvertX = invertX.Checked;
                iface.Config = config;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            Stream file = openFileDialog.OpenFile();
            config = new ArduinoInterface.KeyboardConfig();
            config.LoadConfig(file);
            file.Close();
            updateSettings();
            fname.Text = openFileDialog.FileName;
        }
    }
}
