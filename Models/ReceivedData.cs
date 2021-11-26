namespace HtmlTagCounter.Models
{
    public class ReceivedData : BaseModel
    {
        private string _urlAddress;
        private int _tagCount;

        public string UrlAddress
        {
            get
            {
                return _urlAddress;
            }
            set
            {
                _urlAddress = value;
                OnPropertyChanged(nameof(UrlAddress));
            }
        }

        public int TagCount 
        { 
            get 
            { 
                return _tagCount; 
            }
            set
            {
                _tagCount = value;
                OnPropertyChanged(nameof(TagCount));
            }  
        }
    }
}