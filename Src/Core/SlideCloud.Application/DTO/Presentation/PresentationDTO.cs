namespace SlideCloud.Application.DTO.Presentation;

public class TopicSuggestion
{
    public string Title { get; set; } = string.Empty;
    public List<string> Subtopics { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
}

public class PresentationContent
{
    public string MainTitle { get; set; } = string.Empty;
    public List<Slide> Slides { get; set; } = new();
}

public class Slide
{
    public string Title { get; set; } = string.Empty;
    public List<string> Content { get; set; } = new(); // Change from string to List<string>
    public string? MermaidDiagram { get; set; }
    public string SpeakerNotes { get; set; } = string.Empty;
}
