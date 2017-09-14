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
using System.IO;
using NAudio;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection VideoDevice;
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
            VideoDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in VideoDevice)
            {
                comboBox1.Items.Add(device.Name);
            }
            AudioDevice = new FilterInfoCollection(FilterCategory.AudioInputDevice);
            foreach (FilterInfo device in AudioDevice)
            {
                comboBox2.Items.Add(device.Name);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FinalFrame = new VideoCaptureDevice(VideoDevice[comboBox1.SelectedIndex].MonikerString);
            FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            FinalFrame.Start();
            button2.Enabled = true;
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
            string s = "";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    s += myBitmap.GetPixel(i, j).ToString();

                }
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream filestream = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate))
                {
                    using (TextWriter writer = new StreamWriter(filestream))
                    {
                        writer.WriteLine(s);
                    }
                }
            }




        }

        private void button5_Click(object sender, EventArgs e)
        {
            int devicenumber = comboBox2.SelectedIndex;
            soundStream = new NAudio.Wave.WaveIn();
            soundStream.DeviceNumber = devicenumber;
            soundStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(devicenumber).Channels);

            //NAudio.Wave.WaveInProvider waveIn = new NAudio.Wave.WaveInProvider(soundStream);

            //directsound = new NAudio.Wave.DirectSoundOut();
            //directsound.Init(waveIn);

            soundStream.StartRecording();
            button4.Enabled = true;
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button5.Enabled = true;
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            soundStream.DataAvailable +=new EventHandler<NAudio.Wave.WaveInEventArgs>(SoundStream_DataAvailable);
            fileWriter = new NAudio.Wave.WaveFileWriter("sound.wav", soundStream.WaveFormat);

            if (soundStream!=null)
            {
                soundStream.StopRecording();
                soundStream.Dispose();
                soundStream = null;
            }
        }

        private void SoundStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (fileWriter==null)
            {
                return;
            }
            fileWriter.Write(e.Buffer, 0, e.BytesRecorded);
        }
    }
}
