using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace DWSInterface
{
    class MyViewModel : INotifyPropertyChanged
    {
        private double _imageWidth;
        private double _imageHeight;
        private string _readrateBarTitle;
        private string _readrateBarRate;
        private double _readrateBarValue;
        private string _readrateBarForegroundText;
        private string _readrateBarForegroundCount;
        private string _readrateBarBackgroundText;
        private string _readrateBarBackgroundCount;
        private string _dwsBarTitle;
        private string _dwsBarRate;
        private double _dwsBarValue;
        private string _dwsBarForegroundText;
        private string _dwsBarForegroundCount;
        private string _dwsBarBackgroundText;
        private string _dwsBarBackgroundCount;
        private Brush _bcrBackground;
        private Brush _bt01Background;
        private Brush _bt02Background;
        private Brush _bt03Background;
        private Brush _bt04Background;
        private Brush _bt05Background;
        private Brush _bt06Background;
        private Brush _tfBackground;
        private Brush _trBackground;
        private Brush _lfBackground;
        private Brush _lrBackground;
        private Brush _rfBackground;
        private Brush _rrBackground;
        private Brush _dimBackground;
        private Brush _weightBackground;
        private Brush _plcBackground;
        private Brush _panoromBackground;
        private string _barcode;
        private string _dimString;
        private string _id;
        private string _weightString;
        private string _volumeString;
        private BitmapImage _bmp;
        private string _dwsListCount;
        private string _hisListCount;
        private string _destination;
        private string _language;
        private string _resettitle;
        private string _lhwtitle;
        private string _trackidtitle;
        private string _chutetitle;
        private string _weighttitle;
        private string _volumetitle;
        private string _querytitle;
        private string _bcrtitle;
        private string _panoramtitle;
        private string _dimtitle;
        private string _scaletitle;

        public string ScaleTitle
        {
            get
            {
                return _scaletitle;
            }
            set
            {
                _scaletitle = value;
                OnPropertyChanged(nameof(ScaleTitle));
            }
        }
        public string DimTitle
        {
            get
            {
                return _dimtitle;
            }
            set
            {
                _dimtitle = value;
                OnPropertyChanged(nameof(DimTitle));
            }
        }
        public string PanoromTitle
        {
            get
            {
                return _panoramtitle;
            }
            set
            {
                _panoramtitle = value;
                OnPropertyChanged(nameof(PanoromTitle));
            }
        }
        public string BCRTitle
        {
            get
            {
                return _bcrtitle;
            }
            set
            {
                _bcrtitle = value;
                OnPropertyChanged(nameof(BCRTitle));
            }
        }
        public string QueryTitle
        {
            get
            {
                return _querytitle;
            }
            set
            {
                _querytitle = value;
                OnPropertyChanged(nameof(QueryTitle));
            }
        }
        public string VolumeTitle
        {
            get
            {
                return _volumetitle;
            }
            set
            {
                _volumetitle = value;
                OnPropertyChanged(nameof(VolumeTitle));
            }
        }
        public string WeightTitle
        {
            get
            {
                return _weighttitle;
            }
            set
            {
                _weighttitle = value;
                OnPropertyChanged(nameof(WeightTitle));
            }
        }
        public string ChuteTitle
        {
            get
            {
                return _chutetitle;
            }
            set
            {
                _chutetitle = value;
                OnPropertyChanged(nameof(ChuteTitle));
            }
        }
        public string TrackIDTitle
        {
            get
            {
                return _trackidtitle;
            }
            set
            {
                _trackidtitle = value;
                OnPropertyChanged(nameof(TrackIDTitle));
            }
        }
        public string LHWTitle
        {
            get
            {
                return _lhwtitle;
            }
            set
            {
                _lhwtitle = value;
                OnPropertyChanged(nameof(LHWTitle));
            }
        }
        public string ResetTitle
        {
            get
            {
                return _resettitle;
            }
            set
            {
                _resettitle = value;
                OnPropertyChanged(nameof(ResetTitle));
            }
        }
        public string Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
                OnPropertyChanged(nameof(Language));
            }
        }
        public string Destination
        {
            get
            {
                return _destination;
            }
            set
            {
                _destination = value;
                OnPropertyChanged(nameof(Destination));
            }
        }
        public string DWSListCount
        {
            get
            {
                return _dwsListCount;
            }
            set
            {
                _dwsListCount = value;
                OnPropertyChanged(nameof(DWSListCount));
            }
        }
        public string HisListCount
        {
            get
            {
                return _hisListCount;
            }
            set
            {
                _hisListCount = value;
                OnPropertyChanged(nameof(HisListCount));
            }
        }
        public BitmapImage BMP
        {
            get
            {
                return _bmp;
            }
            set
            {
                _bmp = value;
                OnPropertyChanged(nameof(BMP));
            }
        }
        public string VolumeString
        {
            get
            {
                return _volumeString;
            }
            set
            {
                _volumeString = value;
                OnPropertyChanged(nameof(VolumeString));
            }
        }
        public string WeightString
        {
            get
            {
                return _weightString;
            }
            set
            {
                _weightString = value;
                OnPropertyChanged(nameof(WeightString));
            }
        }
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        public string DimString
        {
            get
            {
                return _dimString;
            }
            set
            {
                _dimString = value;
                OnPropertyChanged(nameof(DimString));
            }
        }
        public string Barcode
        {
            get
            {
                return _barcode;
            }
            set
            {
                _barcode = value;
                OnPropertyChanged(nameof(Barcode));
            }
        }
        public Brush BCRBackground
        {
            get
            {
                return _bcrBackground;
            }
            set
            {
                _bcrBackground = value;
                OnPropertyChanged(nameof(BCRBackground));
            }
        }
        public Brush BT01Background
        {
            get
            {
                return _bt01Background;
            }
            set
            {
                _bt01Background = value;
                OnPropertyChanged(nameof(BT01Background));
            }
        }
        public Brush BT02Background
        {
            get
            {
                return _bt02Background;
            }
            set
            {
                _bt02Background = value;
                OnPropertyChanged(nameof(BT02Background));
            }
        }
        public Brush BT03Background
        {
            get
            {
                return _bt03Background;
            }
            set
            {
                _bt03Background = value;
                OnPropertyChanged(nameof(BT03Background));
            }
        }
        public Brush BT04Background
        {
            get
            {
                return _bt04Background;
            }
            set
            {
                _bt04Background = value;
                OnPropertyChanged(nameof(BT04Background));
            }
        }
        public Brush BT05Background
        {
            get
            {
                return _bt05Background;
            }
            set
            {
                _bt05Background = value;
                OnPropertyChanged(nameof(BT05Background));
            }
        }
        public Brush BT06Background
        {
            get
            {
                return _bt06Background;
            }
            set
            {
                _bt06Background = value;
                OnPropertyChanged(nameof(BT06Background));
            }
        }
        public Brush TFBackground
        {
            get
            {
                return _tfBackground;
            }
            set
            {
                _tfBackground = value;
                OnPropertyChanged(nameof(TFBackground));
            }
        }
        public Brush TRBackground
        {
            get
            {
                return _trBackground;
            }
            set
            {
                _trBackground = value;
                OnPropertyChanged(nameof(TRBackground));
            }
        }
        public Brush LFBackground
        {
            get
            {
                return _lfBackground;
            }
            set
            {
                _lfBackground = value;
                OnPropertyChanged(nameof(LFBackground));
            }
        }
        public Brush LRBackground
        {
            get
            {
                return _lrBackground;
            }
            set
            {
                _lrBackground = value;
                OnPropertyChanged(nameof(LRBackground));
            }
        }
        public Brush RFBackground
        {
            get
            {
                return _rfBackground;
            }
            set
            {
                _rfBackground = value;
                OnPropertyChanged(nameof(RFBackground));
            }
        }
        public Brush RRBackground
        {
            get
            {
                return _rrBackground;
            }
            set
            {
                _rrBackground = value;
                OnPropertyChanged(nameof(RRBackground));
            }
        }
        public Brush DIMBackground
        {
            get
            {
                return _dimBackground;
            }
            set
            {
                _dimBackground = value;
                OnPropertyChanged(nameof(DIMBackground));
            }
        }
        public Brush WeightBackground
        {
            get
            {
                return _weightBackground;
            }
            set
            {
                _weightBackground = value;
                OnPropertyChanged(nameof(WeightBackground));
            }
        }
        public Brush PLCBackground
        {
            get
            {
                return _plcBackground;
            }
            set
            {
                _plcBackground = value;
                OnPropertyChanged(nameof(PLCBackground));
            }
        }
        public Brush PanoromBackground
        {
            get
            {
                return _panoromBackground;
            }
            set
            {
                _panoromBackground = value;
                OnPropertyChanged(nameof(PanoromBackground));
            }
        }
        public double ImageWidth
        {
            get 
            { 
                return _imageWidth; 
            }
            set
            {
                _imageWidth = value;
                OnPropertyChanged(nameof(ImageWidth));
            }
        }

        public double ImageHeight
        {
            get { return _imageHeight; }
            set
            {
                _imageHeight = value;
                OnPropertyChanged(nameof(ImageHeight));
            }
        }
        public string ReadRateBarTitle
        {
            get { return _readrateBarTitle; }
            set
            {
                _readrateBarTitle = value;
                OnPropertyChanged(nameof(ReadRateBarTitle));
            }
        }
        public string ReadRateBarRate
        {
            get { return _readrateBarRate; }
            set
            {
                _readrateBarRate = value;
                OnPropertyChanged(nameof(ReadRateBarRate));
            }
        }
        public double ReadRateBarValue
        {
            get { return _readrateBarValue; }
            set
            {
                _readrateBarValue = value;
                OnPropertyChanged(nameof(ReadRateBarValue));
            }
        }
        public string ReadRateBarForegroundText
        {
            get { return _readrateBarForegroundText; }
            set
            {
                _readrateBarForegroundText = value;
                OnPropertyChanged(nameof(ReadRateBarForegroundText));
            }
        }
        public string ReadRateBarForegroundCount
        {
            get { return _readrateBarForegroundCount; }
            set
            {
                _readrateBarForegroundCount = value;
                OnPropertyChanged(nameof(ReadRateBarForegroundCount));
            }
        }
        public string ReadRateBarBackgroundText
        {
            get { return _readrateBarBackgroundText; }
            set
            {
                _readrateBarBackgroundText = value;
                OnPropertyChanged(nameof(ReadRateBarBackgroundText));
            }
        }
        public string ReadRateBarBackgroundCount
        {
            get { return _readrateBarBackgroundCount; }
            set
            {
                _readrateBarBackgroundCount = value;
                OnPropertyChanged(nameof(ReadRateBarBackgroundCount));
            }
        }
        public string DWSBarTitle
        {
            get { return _dwsBarTitle; }
            set
            {
                _dwsBarTitle = value;
                OnPropertyChanged(nameof(DWSBarTitle));
            }
        }
        public string DWSBarRate
        {
            get { return _dwsBarRate; }
            set
            {
                _dwsBarRate = value;
                OnPropertyChanged(nameof(DWSBarRate));
            }
        }
        public double DWSBarValue
        {
            get { return _dwsBarValue; }
            set
            {
                _dwsBarValue = value;
                OnPropertyChanged(nameof(DWSBarValue));
            }
        }
        public string DWSBarForegroundText
        {
            get { return _dwsBarForegroundText; }
            set
            {
                _dwsBarForegroundText = value;
                OnPropertyChanged(nameof(DWSBarForegroundText));
            }
        }
        public string DWSBarForegroundCount
        {
            get { return _dwsBarForegroundCount; }
            set
            {
                _dwsBarForegroundCount = value;
                OnPropertyChanged(nameof(DWSBarForegroundCount));
            }
        }
        public string DWSBarBackgroundText
        {
            get { return _dwsBarBackgroundText; }
            set
            {
                _dwsBarBackgroundText = value;
                OnPropertyChanged(nameof(DWSBarBackgroundText));
            }
        }
        public string DWSBarBackgroundCount
        {
            get { return _dwsBarBackgroundCount; }
            set
            {
                _dwsBarBackgroundCount = value;
                OnPropertyChanged(nameof(DWSBarBackgroundCount));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
