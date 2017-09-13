using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection Device;
        private VideoCaptureDevice FinalFrame;

        public Form1()
        {
            InitializeComponent();

           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Device = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in Device)
            {
                comboBox1.Items.Add(device.Name);
            }
            comboBox1.SelectedIndex = 0;
            FinalFrame = new VideoCaptureDevice();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            FinalFrame = new VideoCaptureDevice(Device[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame +=new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
        }
        void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FinalFrame.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Bitmap myBitmap = new Bitmap(pictureBox1.Image);
            int width = pictureBox1.Image.Width;
            int height = pictureBox1.Image.Height;
            string s="";
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    s += myBitmap.GetPixel(i, j).ToString();
                    textBox1.AppendText(s);
                }
            }
            
            
            //saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            //if (saveFileDialog1.ShowDialog()==DialogResult.OK)
            //{
                
            //}
        }

   
    }
}
