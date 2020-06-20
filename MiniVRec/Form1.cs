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

using NAudio.Wave;
using NAudio.Lame;
using NAudio.Codecs;
using NAudio.CoreAudioApi;

using Emugen.Thread;

namespace VoiceRecoder
{
    public partial class Form1 : Form
    {
        const string configFilePath = "data/config.cs";

        WaveInEvent waveIn;
        WaveFileWriter waveWriter;

        //BinaryWriter binaryWriter;
        List<Byte[]> stockWaveWriteDatas = new List<byte[]>();
        object stockWaveWriteDatasLock = new object();

        EasyLoopThread mainFileWriteThread;

        bool isStart = false;

        string targetRootDirectory = "";
        //string targetSubDirectory = "";

        int inputDeviceNo = 0;
        Random random = new Random();

        Emugen.Script.Script<ConfigScriptAPI> script;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosed += Form1_FormClosed;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            mainFileWriteThread = new EasyLoopThread(mainFileWriteThreadAction, null, 100);

            script = new Emugen.Script.Script<ConfigScriptAPI>(configFilePath, new ConfigScriptAPI());
            script.Run();
            script.Run();

            if (script.api.OutputDirectoryPath != "")
            {
                targetRootDirectory = textBox1.Text = script.api.OutputDirectoryPath;
            }
            else
            {
                targetRootDirectory = textBox1.Text = System.IO.Directory.GetCurrentDirectory();
            }

            //WaveInEvent.DeviceCount、WaveInEvent.GetCapabilities
            for (var i = 0; i < WaveInEvent.DeviceCount; i++)
            {
                var p = WaveInEvent.GetCapabilities(i);

                comboBox1.Items.Add(p.ProductName);
                if (script.api.DeviceName == p.ProductName)
                {
                    inputDeviceNo = i;
                }
            }
            comboBox1.SelectedIndex = inputDeviceNo;

            checkBox1.Checked = script.api.IsMonaural;

            comboBox2.Items.Add("mp3");
            comboBox2.Items.Add("wav");
            if(script.api.SaveType=="mp3")
            {
                comboBox2.SelectedIndex = 0;
            }
            else
            {
                comboBox2.SelectedIndex = 1;
            }

            {
                var btn = new VoiceRecoder.EmugenWFUI.Button(this, 5, 5, 160, 60, "録音開始", 24);
                btn.Click = () =>
                {

                    if (isStart)
                    {
                        btnStop_Click();
                        isStart = !isStart;
                        btn.Text = "録音開始";
                    }
                    else
                    {
                        btnStart();
                        isStart = !isStart;
                        btn.Text = "録音終了";
                    }
                };
            }
            {
                var btn = new VoiceRecoder.EmugenWFUI.Button(this, 5, 70, 160, 40, "フォルダを開く", 14);
                btn.Click = () =>
                {
                    var dir = CreateTargetDirectory();
                    System.Diagnostics.Process.Start(dir);
                };
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainFileWriteThread.isBreak = true;
        }

        void mainFileWriteThreadAction( object[] args )
        {
            if (waveWriter == null) return;
            lock(stockWaveWriteDatasLock)
            {
                foreach (var bin in stockWaveWriteDatas)
                {
                    waveWriter.Write(bin, 0, bin.Length);
                    waveWriter.Flush();
                }
                stockWaveWriteDatas.Clear();
            }
        }


        string targetFilePath;
        private void btnStart()
        {
            var thread = new EasyThread(btnStart_Core, null);
        }

        private void btnStart_Core( object[] args)
        {

            waveIn = new WaveInEvent();
            waveIn.DeviceNumber = inputDeviceNo;

            var channel = WaveIn.GetCapabilities(inputDeviceNo).Channels;
            if (checkBox1.Checked) channel = 1;
            waveIn.WaveFormat = new WaveFormat(44100, channel);

            var dir = CreateTargetDirectory();

            var filePath = targetFilePath = GenerateNewFilePath();
            waveWriter = new WaveFileWriter(filePath, waveIn.WaveFormat);
            //memoryStream = new MemoryStream();
            //binaryWriter = new BinaryWriter(new FileStream(filePath, FileMode.Create));

            waveIn.DataAvailable += (_, ee) =>
            {
                try
                {
                    lock (stockWaveWriteDatasLock)
                    {
                        stockWaveWriteDatas.Add(ee.Buffer);
                    }
                }
                catch
                {

                }
            };
            waveIn.RecordingStopped += (_, __) =>
            {
                // Todo : ここで、開放系の処理を入れたほうがいいかどうか？
            };

            waveIn.StartRecording();
        }

        private void btnStop_Click()
        {
            var thread = new EasyThread(btnStop_Click_Core, null);
        }

        private void btnStop_Click_Core( object[] args )
        {
            // todo : 
            // この処理が
            // waveIn.RecordingStopped などのイベントの呼び出しとうまく一致していないと思われる。。。それでエラーが出る
            waveIn?.StopRecording();
            waveIn?.Dispose();
            waveIn = null;

            //waveWriter?.Close(); //todo : ? ココが悪さしてるっぽい...waveIn.RecordingStoppedで例外を出させてるんだが。ここでクローズしないと、ファイルの変換ができない
            //waveWriter = null;


            //if (binaryWriter == null) return;
            lock (stockWaveWriteDatasLock)
            {
                foreach (var bin in stockWaveWriteDatas)
                {
                    waveWriter.Write(bin, 0, bin.Length);
                }
                stockWaveWriteDatas.Clear();

                waveWriter.Close();
                waveWriter = null;
            }

            if (script.api.SaveType == "mp3")
            {
                ConvertWavStreamToMp3File(targetFilePath);
                File.Delete(targetFilePath);
            }

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            targetRootDirectory = textBox1.Text;
            if (script != null) script.api.OutputDirectoryPath = targetRootDirectory.Replace("\\","/");
            SaveConfig();
        }

        private string CreateTargetDirectory()
        {
            //var dir = $"{targetRootDirectory}/{targetSubDirectory}";
            var dir = $"{targetRootDirectory}";
            System.IO.Directory.CreateDirectory(dir);
            return dir;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var i = (ComboBox)sender;
            inputDeviceNo = i.SelectedIndex;

            var p = WaveInEvent.GetCapabilities(inputDeviceNo);
            script.api.DeviceName = p.ProductName;

            SaveConfig();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            script.api.IsMonaural = ((CheckBox)sender).Checked;
            SaveConfig();

        }

        private void SaveConfig()
        {
            if (script == null) return;
            using( var p = new System.IO.StreamWriter( configFilePath, false,  Encoding.UTF8))
            {
                p.WriteLine( $"DeviceName = \"{script.api.DeviceName}\";");
                p.WriteLine( $"OutputDirectoryPath = \"{script.api.OutputDirectoryPath}\";");
                p.WriteLine( $"SaveType = \"{script.api.SaveType}\";");
                p.WriteLine( $"IsMonaural = {SaveConfigBoolToString(script.api.IsMonaural)};");
            }
        }

        private string SaveConfigBoolToString( bool i )
        {
            if(i) return "true";
            else return "false";
        }

        private string GenerateNewFilePath()
        {
            var dir = CreateTargetDirectory();
            var now = DateTime.Now;
            var res = $"{dir}/{now.ToString("yyyy_MMdd_HHmmss")}_{random.Next(0, 99).ToString("00")}.wav";

            while(System.IO.File.Exists(res) )
            {
                res = $"{dir}/{now.ToString("yyyy_MMdd_HHmmss")}_{random.Next(0, 99).ToString("00")}.wav";
            }
            return res;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dir = CreateTargetDirectory();
            //System.Diagnostics.Process.Start("EXPLORER.EXE", dir);
            System.Diagnostics.Process.Start(dir);
        }


        public static void ConvertWavStreamToMp3File(string loadFileName)
        {
            var fileType = "mp3";
            var savetFilename = loadFileName.Substring(0, loadFileName.LastIndexOf('.')) + "." + fileType;
            while (true)
            {
                Emugen.Thread.Sleep.Do(100);
                var isOK = true;

                try
                {
                    using (var s = new FileStream(loadFileName, FileMode.Open, FileAccess.Read))
                    {
                        var ms = new MemoryStream();
                        //ms.CopyTo(s);
                        s.CopyTo(ms);
                        ConvertWavStreamToMp3File(ref ms, savetFilename);
                    }
                }
                catch
                {
                    isOK = false;
                }
                if (isOK) break;
            }

        }

        public static void ConvertWavStreamToMp3File(ref MemoryStream ms, string savetofilename)
        {
            //rewind to beginning of stream 
            ms.Seek(0, SeekOrigin.Begin);

            using (var retMs = new MemoryStream())
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(savetofilename, rdr.WaveFormat, LAMEPreset.VBR_90))
            {
                rdr.CopyTo(wtr);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            script.api.SaveType = comboBox2.SelectedItem.ToString();
            SaveConfig();
        }

        public class ConfigScriptAPI
        {
            public string DeviceName = "";
            public string OutputDirectoryPath = "";
            public string SaveType = "";
            public bool IsMonaural = false;
        }
    }
}
