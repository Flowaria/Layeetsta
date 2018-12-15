using Layeetsta.Util;
using Layeetsta.Web;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace Layeetsta
{
    public class LayestaInfo
    {
        public string SongName = "";
        public string Charter = "";
        public string GUID = "";
    }
    
    /// <summary>
    /// DownloadWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class DownloadWindow : MetroWindow
    {
        public List<LayestaInfo> Selections = new List<LayestaInfo>();

        public DownloadWindowModel setting = new DownloadWindowModel();

        private FolderBrowserDialog folderdialog = new FolderBrowserDialog();
        public DownloadWindow()
        {
            InitializeComponent();
            DataContext = setting;

            var defaultpath = Path.Combine(Directory.GetCurrentDirectory(), "Download");
            folderdialog.SelectedPath = defaultpath;
            setting.SavePath = defaultpath;
        }

        private Task _task;
        int Total = 0, Current = 0;
        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            setting.Working = true;

            Progress.Value = 0;
            Total = Selections.Count;
            Current = 0;

            _task = Task.WhenAll(Selections.Select(x => Process(x)).ToArray());
            await _task;

            setting.Working = false;
            Close();
        }

        private async Task Process(LayestaInfo level)
        {
            PROCESS:
            try
            {
                var basefilename = setting.SaveAsGuid ? level.GUID : string.Format("[{0}] {1}", level.Charter, level.SongName);
                foreach (char c in Path.GetInvalidFileNameChars())
                {
                    basefilename = basefilename.Replace(c, '_');
                }

                var layestafile = Path.Combine(setting.SavePath, basefilename + ".layesta");

                if (!File.Exists(layestafile) || setting.AllowOverwrite)
                    await MainWindow.API.DownloadChart(level.GUID, layestafile);

                if (setting.SaveAsLap)
                {
                    string lappath = Path.Combine(setting.SavePath, basefilename);
                    if (Directory.Exists(lappath))
                    {
                        if (setting.AllowOverwrite)
                        {
                            Directory.Delete(lappath, true);

                        }
                        else
                        {
                            Current += 1;
                            return;
                        }
                    }
                    Directory.CreateDirectory(lappath);

                    bool ExtractResult = false;
                    await Task.Run(() =>{
                        try
                        {
                            ZipFile.ExtractToDirectory(layestafile, lappath);
                            ExtractResult = true;
                        }
                        catch (InvalidDataException)
                        {
                            File.Delete(layestafile);
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { ErrorWindow.ShowException(ex, true); }));
                            Current += 1;
                        }
                    });
                    if(!ExtractResult)
                    {
                        goto PROCESS;
                    }

                    File.Delete(layestafile);
                    File.Delete(Path.Combine(lappath, "background_blur.png"));
                    File.Delete(Path.Combine(lappath, "info.bytes"));

                    var lap = new LapFile();
                    var charts = new List<string>();
                    foreach (var file in Directory.GetFiles(lappath))
                    {
                        var filename = System.IO.Path.GetFileName(file);
                        if (lap.BGA0Path == null && filename.Equals("background_gray.jpg"))
                        {
                            lap.BGA0Path = file;
                        }
                        if (lap.BGA1Path == null && filename.Equals("background_linear.jpg"))
                        {
                            lap.BGA1Path = file;
                        }
                        if (Path.GetExtension(file).Equals(".txt"))
                        {
                            var b64name = filename.Replace("chart_", "").Replace(".txt", "");
                            string decode_b64name = Encoding.UTF8.GetString(Convert.FromBase64String(b64name)) + ".txt";
                            File.Move(file, Path.Combine(lappath, decode_b64name));
                            charts.Add(decode_b64name);
                        }
                    }

                    await Task.Run(() =>
                    {
                        var mp3file = Path.Combine(lappath, "music.mp3");
                        AudioConverter.MP3ToOGG(mp3file, Path.Combine(lappath, Path.Combine(lappath, "music.ogg")));
                        File.Delete(mp3file);
                    });
                    

                    var basebgpath = Path.Combine(lappath, "background.jpg");
                    if (lap.BGA0Path == null)
                    {
                        lap.BGA0Path = basebgpath;
                    }
                    else if (lap.BGA1Path == null)
                    {
                        lap.BGA1Path = basebgpath;
                    }
                    else
                    {
                        lap.BGA2Path = basebgpath;
                    }

                    lap.Name = level.SongName;
                    lap.Designer = level.Charter;
                    lap.ProjectFolder = lappath;
                    lap.MusicPath = Path.Combine(lappath, "music.ogg");
                    lap.ChartPath = charts.First();

                    File.WriteAllText(Path.Combine(lappath, basefilename + ".lap"), lap.Serialization());

                    if (setting.SaveAsZip)
                    {
                        var zippath = Path.Combine(setting.SavePath, basefilename + ".zip");
                        if (File.Exists(zippath))
                            File.Delete(zippath);

                        ZipFile.CreateFromDirectory(lappath, Path.Combine(setting.SavePath, basefilename + ".zip"));
                        Directory.Delete(lappath, true);
                    }
                }
                Current += 1;
                
            }
            catch (LayestaWebAPINeedLoginException ex)
            {
                var mw = Owner as MainWindow;
                mw.TryLogin();

                goto PROCESS; //goto fuckyeah!!!!
            }
            catch(InvalidDataException ex)
            {
                Console.WriteLine(ex.Source);
                File.Delete(ex.Source);
                goto PROCESS;
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { ErrorWindow.ShowException(ex, true); }));
                Current += 1;
            }
            finally
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate { Progress.Value = (double)(Current / Total * 100.0); }));
            }
        }

        
        private void PathSearchButton_Click(object sender, RoutedEventArgs e)
        {
            folderdialog.ShowDialog();
            if(!String.IsNullOrWhiteSpace(folderdialog.SelectedPath))
            {
                setting.SavePath = folderdialog.SelectedPath;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(setting.Working)
            {
                var result = System.Windows.MessageBox.Show("Downloading progress is now running. are you sure to stop it?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Hand);
                if (result == MessageBoxResult.OK)
                {
                    if(_task != null && !_task.IsCompleted)
                    {
                        _task.Dispose();
                        foreach(var info in Selections)
                        {
                            var basefilename = setting.SaveAsGuid ? info.GUID : string.Format("[{0}] {1}", info.Charter, info.SongName);
                            foreach(var file in Directory.GetFiles(setting.SavePath))
                            {
                                if (Path.GetFileName(file).Contains(basefilename))
                                    File.Delete(file);
                            }
                            foreach (var dir in Directory.GetDirectories(setting.SavePath))
                            {
                                if (Path.GetDirectoryName(dir).Contains(basefilename))
                                    Directory.Delete(dir, true);
                            }
                        }
                    }
                }
                else
                    e.Cancel = true;
            }
        }
    }

    
}
