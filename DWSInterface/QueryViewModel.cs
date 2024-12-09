using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DWSInterface
{
    class QueryViewModel : INotifyPropertyChanged
    {
        private ICommand _firstPageCommand;

        public ICommand FirstPageCommand
        {
            get
            {
                return _firstPageCommand;
            }

            set
            {
                _firstPageCommand = value;
            }
        }

        private ICommand _previousPageCommand;

        public ICommand PreviousPageCommand
        {
            get
            {
                return _previousPageCommand;
            }

            set
            {
                _previousPageCommand = value;
            }
        }

        private ICommand _nextPageCommand;

        public ICommand NextPageCommand
        {
            get
            {
                return _nextPageCommand;
            }

            set
            {
                _nextPageCommand = value;
            }
        }

        private ICommand _lastPageCommand;

        public ICommand LastPageCommand
        {
            get
            {
                return _lastPageCommand;
            }

            set
            {
                _lastPageCommand = value;
            }
        }
        private string _totaltitle;
        public string TotalTitle
        {
            get
            {
                return _totaltitle;
            }

            set
            {
                if (_totaltitle != value)
                {
                    _totaltitle = value;
                    OnPropertyChanged("TotalTitle");
                }
            }
        }
        private string _pagetitle;
        public string PageTitle
        {
            get
            {
                return _pagetitle;
            }

            set
            {
                if (_pagetitle != value)
                {
                    _pagetitle = value;
                    OnPropertyChanged("PageTitle");
                }
            }
        }
        private string _searchtimetitle;
        public string SearchTimeTitle
        {
            get
            {
                return _searchtimetitle;
            }

            set
            {
                if (_searchtimetitle != value)
                {
                    _searchtimetitle = value;
                    OnPropertyChanged("SearchTimeTitle");
                }
            }
        }
        private string _searchcodetitle;
        public string SearchCodeTitle
        {
            get
            {
                return _searchcodetitle;
            }

            set
            {
                if (_searchcodetitle != value)
                {
                    _searchcodetitle = value;
                    OnPropertyChanged("SearchCodeTitle");
                }
            }
        }
        private string _querytitle;
        public string QueryTitle
        {
            get
            {
                return _querytitle;
            }

            set
            {
                if (_querytitle != value)
                {
                    _querytitle = value;
                    OnPropertyChanged("QueryTitle");
                }
            }
        }
        private string _idtitle;
        public string IDTitle
        {
            get
            {
                return _idtitle;
            }

            set
            {
                if (_idtitle != value)
                {
                    _idtitle = value;
                    OnPropertyChanged("IDTitle");
                }
            }
        }
        private string _barcodetitle;
        public string BarcodeTitle
        {
            get
            {
                return _barcodetitle;
            }

            set
            {
                if (_barcodetitle != value)
                {
                    _barcodetitle = value;
                    OnPropertyChanged("BarcodeTitle");
                }
            }
        }
        private string _desttitle;
        public string DestTitle
        {
            get
            {
                return _desttitle;
            }

            set
            {
                if (_desttitle != value)
                {
                    _desttitle = value;
                    OnPropertyChanged("DestTitle");
                }
            }
        }
        private string _lengthtitle;
        public string LengthTitle
        {
            get
            {
                return _lengthtitle;
            }

            set
            {
                if (_lengthtitle != value)
                {
                    _lengthtitle = value;
                    OnPropertyChanged("LengthTitle");
                }
            }
        }
        private string _widthtitle;
        public string WidthTitle
        {
            get
            {
                return _widthtitle;
            }

            set
            {
                if (_widthtitle != value)
                {
                    _widthtitle = value;
                    OnPropertyChanged("WidthTitle");
                }
            }
        }
        private string _heighttitle;
        public string HeightTitle
        {
            get
            {
                return _heighttitle;
            }

            set
            {
                if (_heighttitle != value)
                {
                    _heighttitle = value;
                    OnPropertyChanged("HeightTitle");
                }
            }
        }
        private string _volumetitle;
        public string VolumeTitle
        {
            get
            {
                return _volumetitle;
            }

            set
            {
                if (_volumetitle != value)
                {
                    _volumetitle = value;
                    OnPropertyChanged("VolumeTitle");
                }
            }
        }
        private string _weighttitle;
        public string WeightTitle
        {
            get
            {
                return _weighttitle;
            }

            set
            {
                if (_weighttitle != value)
                {
                    _weighttitle = value;
                    OnPropertyChanged("WeightTitle");
                }
            }
        }
        private string _timestamptitle;
        public string TimestampTitle
        {
            get
            {
                return _timestamptitle;
            }

            set
            {
                if (_timestamptitle != value)
                {
                    _timestamptitle = value;
                    OnPropertyChanged("TimestampTitle");
                }
            }
        }
        private string _pictitle;
        public string PicTitle
        {
            get
            {
                return _pictitle;
            }

            set
            {
                if (_pictitle != value)
                {
                    _pictitle = value;
                    OnPropertyChanged("PicTitle");
                }
            }
        }
        private string _barcode;
        public string Barcode
        {
            get
            {
                return _barcode;
            }

            set
            {
                if (_barcode != value)
                {
                    _barcode = value;
                    OnPropertyChanged("Barcode");
                }
            }
        }
        private string _readRate;
        public string ReadRate
        {
            get
            {
                return _readRate;
            }

            set
            {
                if (_readRate != value)
                {
                    _readRate = value;
                    OnPropertyChanged("ReadRate");
                }
            }
        }
        private string _startTime;
        public string StartTime
        {
            get
            {
                return _startTime;
            }

            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged("StartTime");
                }
            }
        }
        private string _endTime;
        public string EndTime
        {
            get
            {
                return _endTime;
            }

            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    OnPropertyChanged("EndTime");
                }
            }
        }
        private int _pageSize;


        private int _currentPage;

        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged("CurrentPage");
                }
            }
        }

        private int _totalPage;

        public int TotalPage
        {
            get
            {
                return _totalPage;
            }

            set
            {
                if (_totalPage != value)
                {
                    _totalPage = value;
                    OnPropertyChanged("TotalPage");
                }
            }
        }



        private ObservableCollection<History> _tempSource;
        public ObservableCollection<History> TempSource
        {
            get
            {
                return _tempSource;
            }
            set
            {
                if (_tempSource != value)
                {
                    _tempSource = value;
                    OnPropertyChanged("TempSource");
                }
            }
        }




        private List<History> _source;
        public List<History> Source
        {
            get
            {
                return _source;
            }
            set
            {
                if (_source != value)
                {
                    _source = value;
                    OnPropertyChanged("Source");
                }
            }
        }
        private void FirstPageAction(object parameter)
        {
            CurrentPage = 1;
            /*
            List<History> result = _source.Take(Math.Min(_pageSize,_source.Count)).ToList();
            _tempSource.Clear();
            foreach (History h in result)
            {
                _tempSource.Add(h);
            }
            */
        }

        private void PreviousPageAction(object parameter)
        {
            if (CurrentPage == 1)
            {
                return;
            }
            /*
            List<History> result = new List<History>();

            if (CurrentPage == 2)
            {
                result = _source.Take(_pageSize).ToList();
            }
            else
            {
                result = _source.Skip((CurrentPage - 2) * _pageSize).Take(_pageSize).ToList();
            }

            _tempSource.Clear();

            foreach (History h in result)
            {
                _tempSource.Add(h);
            }
            */
            CurrentPage--;
        }

        private void NextPageAction(object parameter)
        {
            
            if (CurrentPage == _totalPage)
            {
                return;
            }
            /*
            List<History> result = new List<History>();

            result = _source.Skip(CurrentPage * _pageSize).Take(Math.Min(_pageSize, _source.Count - CurrentPage * _pageSize)).ToList();

            _tempSource.Clear();

            foreach (History h in result)
            {
                _tempSource.Add(h);
            }
            */
            CurrentPage++;
        }

        private void LastPageAction(object parameter)
        {
            CurrentPage = _totalPage;
            /*
            int skipCount = (_totalPage - 1) * _pageSize;
            int takeCount = _source.Count - skipCount;

            var result = _source.Skip(skipCount).Take(takeCount).ToList();

            _tempSource.Clear();

            foreach (History h in result)
            {
                _tempSource.Add(h);
            }
            */
        }
        public QueryViewModel()
        {
            _currentPage = 1;
            _pageSize = 20;
            _tempSource = new ObservableCollection<History>();
            _source = new List<History>();
            _firstPageCommand = new DelegateCommand(FirstPageAction);

            _previousPageCommand = new DelegateCommand(PreviousPageAction);

            _nextPageCommand = new DelegateCommand(NextPageAction);

            _lastPageCommand = new DelegateCommand(LastPageAction);
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == "Source")
            {
                try
                {
                    TotalPage = Convert.ToInt32(Math.Ceiling((1.0 * _source.Count) / (1.0 * _pageSize)));
                }
                catch
                {
                    TotalPage = 0;
                }
                CurrentPage = -1;
            }
            else if(propertyName == "CurrentPage")
            {
                if (CurrentPage > _totalPage)
                {
                    CurrentPage = _totalPage;
                    return;
                }
                if (CurrentPage <= 0)
                {
                    CurrentPage = 1;
                    return;
                }
                List<History> result;
                if (CurrentPage == 1)
                {
                    result = _source.Take(Math.Min(_pageSize, _source.Count)).ToList();
                }
                else
                {
                    int skipCount = (CurrentPage - 1) * _pageSize;
                    int takeCount = Math.Min(_pageSize, _source.Count - skipCount);
                    result = _source.Skip(skipCount).Take(takeCount).ToList();
                }
                TempSource.Clear();
                foreach (History h in result)
                {
                    TempSource.Add(h);
                }
            }
           
        }
    }
}
