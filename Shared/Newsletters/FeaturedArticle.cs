namespace SignalBrief.Shared.Newsletters;

public sealed class FeaturedArticle
{
    public string Kicker { get; set; } = "Featured Analysis";
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageAlt { get; set; } = string.Empty;
    public string ReadTime { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public List<string> BodyParagraphs { get; set; } = [];
}
