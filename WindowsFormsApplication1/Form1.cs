using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using NAudio;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        OpenFileDialog o = new OpenFileDialog();
        private FilterInfoCollection Device;
        private VideoCaptureDevice FinalFrame;


        private FilterInfoCollection AudioDevice;
        private NAudio.Wave.WaveIn soundStream;
        private NAudio.Wave.DirectSoundOut directsound;

        private NAudio.Wave.WaveFileWriter fileWriter;
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
            AudioDevice = new FilterInfoCollection(FilterCategory.AudioInputDevice);
            foreach (FilterInfo device in AudioDevice)
            {
                //comboBox2.Items.Add(device.Name);
            }
            FinalFrame = new VideoCaptureDevice();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            FinalFrame = new VideoCaptureDevice(Device[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
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

        private async void button3_ClickAsync(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                await Task.Run(() => WriteinFile());
                MessageBox.Show("Writed", "Good");
               
            }

            button4.Visible = true;


        }

        private async Task WriteinFile()
        {
            Bitmap myBitmap = new Bitmap(pictureBox1.Image);
            int width = pictureBox1.Image.Width;
            int height = pictureBox1.Image.Height;
            using (FileStream filestream = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate))
            {
                using (TextWriter writer = new StreamWriter(filestream))
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            writer.WriteLine(Convert.ToString(myBitmap.GetPixel(i, j).R, 2).PadLeft(8, '0'));
                            writer.WriteLine(Convert.ToString(myBitmap.GetPixel(i, j).G, 2).PadLeft(8, '0'));
                            writer.WriteLine(Convert.ToString(myBitmap.GetPixel(i, j).B, 2).PadLeft(8, '0'));




                        }
                    }

                }
            }
            return;

        }

        private async void button4_Click(object sender, EventArgs e)
        {
            o.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (o.ShowDialog() == DialogResult.OK)
            {
                await Task.Run(() => Recover());
                MessageBox.Show("Recovered", "Good");

            }
            else
            {
                return;
            }
        }

        private async Task Recover()
        {
            int width = pictureBox1.Image.Width;
            int height = pictureBox1.Image.Height;
            Bitmap b = new Bitmap(width, height);
            using (FileStream fs = new FileStream(o.FileName, FileMode.Open))
            {
                using (TextReader reader = new StreamReader(fs))
                {

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {

                            b.SetPixel(i, j, Color.FromArgb(Convert.ToInt32(reader.ReadLine(), 2), Convert.ToInt32(reader.ReadLine(), 2), Convert.ToInt32(reader.ReadLine(), 2)));
                            pictureBox2.Image = b;
                        }

                    }

                    //reader.ReadAsync();
                }
            }
        }
    }
}

