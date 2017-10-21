using System;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using NAudio;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        private FilterInfoCollection Device;
        private VideoCaptureDevice FinalFrame;


        private FilterInfoCollection AudioDevice;
        NAudio.Wave.WaveIn sourceStream = null;
        NAudio.Wave.DirectSoundOut waveOut = null;
        NAudio.Wave.WaveFileWriter waveWriter = null;

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
                comboBox2.Items.Add(device.Name);
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
            if (FinalFrame.IsRunning)
            {
                FinalFrame.Stop();
            }
            else
            {
                MessageBox.Show("Not recording", "Please start to record");
            }
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
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
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
            using (FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open))
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

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) return;

            int deviceNumber = comboBox2.SelectedIndex;

            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = deviceNumber;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNumber).Channels);

            NAudio.Wave.WaveInProvider waveIn = new NAudio.Wave.WaveInProvider(sourceStream);

            waveOut = new NAudio.Wave.DirectSoundOut();
            waveOut.Init(waveIn);

            sourceStream.StartRecording();
            waveOut.Play();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Wave File (*.wav)|*.wav;";
            if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            int deviceNumber = comboBox2.SelectedIndex;

            sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.DeviceNumber = deviceNumber;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, NAudio.Wave.WaveIn.GetCapabilities(deviceNumber).Channels);

            sourceStream.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(sourceStream_DataAvailable);
            waveWriter = new NAudio.Wave.WaveFileWriter(saveFileDialog1.FileName, sourceStream.WaveFormat);

            sourceStream.StartRecording();
        }
        private void sourceStream_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (waveWriter == null) return;

            waveWriter.WriteData(e.Buffer, 0, e.BytesRecorded);
            waveWriter.Flush();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
            if (sourceStream != null)
            {
                sourceStream.StopRecording();
                sourceStream.Dispose();
                sourceStream = null;
            }
            if (waveWriter != null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            byte[] butes;
            openFileDialog1.Filter = "Wave File(*.wav) | *.wav; ";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                butes = File.ReadAllBytes(openFileDialog1.FileName);
            }
            else
            {
                return;
            }
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (FileStream filestream = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate))
                {
                    using (TextWriter writer = new StreamWriter(filestream))
                    {
                        writer.WriteLine(butes);
                    }
                }
                MessageBox.Show("Writed", "Good");

            }
            else
            {
                return;
            }

        }
    }
}

