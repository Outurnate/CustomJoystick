using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InputSim
{
    public partial class InputControl : Form
    {
        private ArduinoInterface iface;

        public InputControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            iface = new ArduinoInterface(portName.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = false;
            iface.Close();
        }

        private void mouseSens_Scroll(object sender, EventArgs e)
        {
            iface.MouseSensitivity = (float)(mouseSens.Value / 100);
        }
    }
}
