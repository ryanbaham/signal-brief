namespace SignalBrief.Shared.Newsletters;

public sealed class NewsletterIssue
{
    public string NewsletterTitle { get; set; } = "Signal Brief";
    public string EditionLabel { get; set; } = "Weekly Tech Intelligence";
    public string IssueDate { get; set; } = string.Empty;
    public string TldrHeading { get; set; } = "The most important signals";
    public List<string> TldrItems { get; set; } = [];
    public FeaturedArticle Featured { get; set; } = new();
    public string ArticleSectionKicker { get; set; } = "Briefing Queue";
    public string ArticleSectionTitle { get; set; } = "Worth your next coffee";
    public List<NewsletterArticle> Articles { get; set; } = [];
    public string FooterNote { get; set; } = string.Empty;
}
