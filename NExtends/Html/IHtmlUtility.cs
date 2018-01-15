namespace NExtends.Html
{
    public interface IHtmlUtility
    {
        string SanitizeHtml(string source);
        string StripHtml(string source);
    }
}