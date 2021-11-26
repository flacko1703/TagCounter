namespace HtmlTagCounter.Abstractions
{
    public interface IUrlReader
    {
        string UrlAddress { get; }
        string ReadPage(string url);
    }
}