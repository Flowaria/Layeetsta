using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layeetsta.Util
{
    public class AudioConverter
    {
        public static void MP3ToOGG(string mp3path, string oggpath)
        {
            Process p = new Process();
            ProcessStartInfo i = new ProcessStartInfo();
            i.FileName = Directory.GetCurrentDirectory()+"/Resources/ffmpeg.exe";
            i.Arguments = $"-i \"{mp3path}\" -c:a libvorbis \"{oggpath}\"";
            i.UseShellExecute = true;
            i.WindowStyle = ProcessWindowStyle.Hidden;

            p.StartInfo = i;
            p.Start();
            
            p.WaitForExit();
        }
    }
}
