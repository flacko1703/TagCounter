using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using HtmlTagCounter.Abstractions;
using HtmlTagCounter.Models;

namespace HtmlTagCounter.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private FileSelector _fileSelector;
        private RelayCommand _selectFileCommand;
        private RelayCommand _cancelParseCommand;
        private string _filePath;
        private int _currentUrlLine;
        private bool _isBreaked;
        private bool _showProgress;
        private bool _runButtonVisible;
        private bool _cancelButtonVisible;
        private string _elapsedTime;
        
        public MainViewModel()
        {
            _isBreaked = false;
            _showProgress = false;
            _runButtonVisible = true;
            _cancelButtonVisible = false;
            _fileSelector = new FileSelector();
            ReceivedDatas = new ObservableCollection<ReceivedData>();
        }
        
        public ObservableCollection<ReceivedData> ReceivedDatas { get; set; }
        public string FilePath
        {
            get { return _filePath; }
            set 
            { 
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }
        
        public int CurrentUrlLine
        {
            get { return _currentUrlLine; }
            set 
            { 
                _currentUrlLine = value;
                OnPropertyChanged(nameof(CurrentUrlLine));
            }
        }
        
        public string ElapsedTime
        {
            get { return _elapsedTime; }
            set 
            { 
                _elapsedTime = value;
                OnPropertyChanged(nameof(ElapsedTime));
            }
        }

        public bool IsBreaked
        {
            get { return _isBreaked; }
            set 
            { 
                _isBreaked = value;
                OnPropertyChanged(nameof(IsBreaked));
            }
        }
        
        public bool RunButtonVisible
        {
            get { return _runButtonVisible; }
            set 
            { 
                _runButtonVisible = value;
                OnPropertyChanged(nameof(RunButtonVisible));
            }
        }
        
        public bool CancelButtonVisible
        {
            get { return _cancelButtonVisible; }
            set 
            { 
                _cancelButtonVisible = value;
                OnPropertyChanged(nameof(CancelButtonVisible));
            }
        }
        
        public bool ShowProgress       
        {
            get { return _showProgress; }
            set 
            { 
                _showProgress = value;
                OnPropertyChanged(nameof(ShowProgress));
            }
        }

        private void SwitchButtons()
        {
            CancelButtonVisible = !CancelButtonVisible;
            RunButtonVisible = !RunButtonVisible;
        }

        private string[] ReadLines(string pathToFile)
        {
            string[] lines;
            pathToFile = Path.GetFullPath(pathToFile);
            lines = File.ReadAllLines(pathToFile);
            return lines;
        }

        private void BreakProcess()
        {
            _isBreaked = true;
        }

        public RelayCommand SelectFileCommand =>
            _selectFileCommand ?? (_selectFileCommand = new RelayCommand(obj =>
            {
                var path = _fileSelector.GetFile();
                if (String.IsNullOrEmpty(path)) return;
                _currentUrlLine = 0;
                _isBreaked = false;
                ReceivedDatas.Clear();
                SwitchButtons();
                var textLines = ReadLines(path);
                foreach (var line in textLines)
                {
                    ReceivedDatas.Add(new ReceivedData {UrlAddress = line});
                }

                Thread myThread = new Thread(GetAllTagsCount);
                
                myThread.Start(ReceivedDatas.ToList()); 
            }));
        
        public RelayCommand CancelParseCommand =>
            _cancelParseCommand ?? (_cancelParseCommand = new RelayCommand(obj =>
            {
                BreakProcess();

            }));

        private void GetAllTagsCount(object o)
        {
            var datas = (List<ReceivedData>) o;
            IUrlReader urlReader = new HttpPageUrlReader();
            var regexPattern = "<\\s*a[^>]*>(.*?)<\\s*/\\s*a>";
            TagCounter counter = new TagCounter(regexPattern);
            ShowProgress = true;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 10
            };
            Parallel.ForEach(datas, options, (data, state) =>
                {
                    if (_isBreaked)
                    {
                        state.Break();
                    }
                    
                    var result = urlReader.ReadPage(data.UrlAddress);
                    counter.FindTags(result);
                    data.TagCount = counter.Count;
                    CurrentUrlLine++;
                }
            );
            stopwatch.Stop();
            ElapsedTime = stopwatch.ElapsedMilliseconds.ToString();
            SwitchButtons();
            MessageBox.Show($"Finished! Elapsed time {ElapsedTime} ms");
        }
    }
}