namespace SignalBrief.Shared.Newsletters;

public sealed class NewsletterIndex
{
    public string LatestDate { get; set; } = string.Empty;
    public List<NewsletterIndexItem> Issues { get; set; } = [];
}
