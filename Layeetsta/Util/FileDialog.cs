using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layeetsta.Util
{
    public class FileDialog
    {
        static OpenFileDialog odialog = null;
        static SaveFileDialog sdialog = null;
        static FileDialog()
        {
            sdialog = new SaveFileDialog();
            sdialog.Title = "Save an Layesta File";
            sdialog.Filter = "Layesta File|*.layesta";
        }

        public static string LayestaSave()
        {
            sdialog.ShowDialog();
            if (sdialog.FileName != "")
            {
                return sdialog.FileName;
            }
            return null;
        }
    }
}
