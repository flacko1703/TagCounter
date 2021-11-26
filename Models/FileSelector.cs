using System;
using System.Windows;
using Microsoft.Win32;

namespace HtmlTagCounter.Models
{
    public class FileSelector
    {
        private string _filePath;

        public string FilePath => _filePath;
        
        public string GetFile()
        {
            
            try
            {   
                OpenFileDialog _openFileDialog = new OpenFileDialog();
                _openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                _openFileDialog.Multiselect = true;
                _openFileDialog.Filter = "Text files (*.txt)|*.txt";
                bool? result = _openFileDialog.ShowDialog();
                if (result == true)
                {
                    _filePath = _openFileDialog.FileName;
                    return _filePath;
                }
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e);
                throw;
            }
            
            
            return null;
        }
    }
}