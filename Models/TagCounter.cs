using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace HtmlTagCounter.Models
{
    public class TagCounter
    {
        private int _count;
        private string _pattern;
        private Regex _regex;
        
        public int Count => _count;

        public TagCounter(string pattern)
        {
            _pattern = pattern;
            _regex = new Regex(_pattern);
        }

        public string[] FindTags(string sourceText)
        {
            MatchCollection mc = Regex.Matches(sourceText, _pattern);
            var matches = new string[mc.Count];
            _count = matches.Length;
            
            return matches;
        }
    }
}
    

