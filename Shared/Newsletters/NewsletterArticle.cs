namespace SignalBrief.Shared.Newsletters;

public sealed class NewsletterArticle
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageAlt { get; set; } = string.Empty;
    public List<string> BodyParagraphs { get; set; } = [];
}
