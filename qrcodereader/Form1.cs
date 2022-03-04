using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using ZXing.Aztec;

namespace qrcodereader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        FilterInfoCollection camera_info;
        VideoCaptureDevice camera_capture;

        private void Form1_Load(object sender, EventArgs e)
        {
            camera_info = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            foreach (FilterInfo webcam in camera_info)
            {
                comboBox1.Items.Add(webcam.Name);
            }

            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            camera_capture = new VideoCaptureDevice(camera_info[comboBox1.SelectedIndex].MonikerString);
            camera_capture.NewFrame += camera_frame;
            camera_capture.Start();
            timer1.Start();
            
        }

        private void camera_frame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                BarcodeReader qrcode_read = new BarcodeReader();
                Result result = qrcode_read.Decode((Bitmap)pictureBox1.Image);

                if (result != null)
                {
                    textBox1.Text = result.ToString();
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (camera_capture.IsRunning)
            {
                camera_capture.Stop();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StreamWriter writeFile = File.CreateText("qrcode.txt");
            writeFile.WriteLine("Date: " + DateTime.Now.ToString());
            writeFile.WriteLine(textBox1.Text);
            writeFile.Close();
            textBox1.Text = "";
            MessageBox.Show("Text created");
        }
    }
}
