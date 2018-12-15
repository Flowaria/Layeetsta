using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Layeetsta
{
    public class DownloadWindowModel : INotifyPropertyChanged
    {
        private bool _saveasguid = false;
        public bool SaveAsGuid
        {
            get
            {
                return _saveasguid;
            }
            set
            {
                _saveasguid = value;
                OnPropertyChanged("SaveAsGuid");
            }
        }

        private bool _saveaslap = false;
        public bool SaveAsLap
        {
            get
            {
                return _saveaslap;
            }
            set
            {
                _saveaslap = value;
                OnPropertyChanged("SaveAsLap");
            }
        }

        private bool _saveaszip = true;
        public bool SaveAsZip
        {
            get
            {
                return _saveaszip;
            }
            set
            {
                _saveaszip = value;
                OnPropertyChanged("SaveAsZip");
            }
        }

        private bool _overwrite = false;
        public bool AllowOverwrite
        {
            get
            {
                return _overwrite;
            }
            set
            {
                _overwrite = value;
                OnPropertyChanged("AllowOverwrite");
            }
        }

        private string _savepath = "";
        public string SavePath
        {
            get
            {
                return _savepath;
            }
            set
            {
                _savepath = value;
                OnPropertyChanged("SavePath");
            }
        }

        private bool _working = false;
        public bool Working
        {
            get
            {
                return _working;
            }
            set
            {
                _working = value;
                OnPropertyChanged("Working");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
