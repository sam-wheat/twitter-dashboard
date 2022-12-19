namespace Dashboard.Model;

public record Tweet
{
    public string ID { get; set; }
    public string[]? Hashtags { get; set; }
    public string Text { get; set; }
}
