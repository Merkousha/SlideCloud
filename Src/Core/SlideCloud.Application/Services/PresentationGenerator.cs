using System.Text.Json;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using SlideCloud.Application.DTO.Presentation;
using Text = DocumentFormat.OpenXml.Presentation.Text;

namespace SlideCloud.Application.Services;

public interface IPresentationGeneratorService
{
    Task<TopicSuggestion> GenerateTopics(string userInput);
    Task<PresentationContent> GenerateContent(TopicSuggestion approvedTopics);
    Task<byte[]> CreatePowerPoint(PresentationContent content);
}

public class PresentationGeneratorService : IPresentationGeneratorService
{
    Kernel _kernel;
    string modelId = "gpt-4o-2024-11-20";
    string endpoint = "https://api.avalai.ir/v1";
    string apiKey = "aa-VckNj9CcU1uLMczXGigPhavLokYY4V2meOFzglIDDq3j6kIJ";
    // Create a history store the conversation
    ChatHistory history = new ChatHistory();




    public async Task<TopicSuggestion> GenerateTopics(string mainTitle)
    {

        // Create a kernel with Azure OpenAI chat completion
        var openAiClientOption = new OpenAIClientOptions
        {
            Endpoint = new Uri(endpoint),
        };
        var apiCredential = new System.ClientModel.ApiKeyCredential(apiKey);
        var openAiClient = new OpenAIClient(apiCredential, openAiClientOption);
        var builder = Kernel.CreateBuilder();
        builder.Services.AddOpenAIChatCompletion(modelId, openAiClient);


        // Build the kernel
        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Enable planning
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };


        string topicSkillPrompt = $@"
        Based on the following presentation topic, suggest:
        1. A clear title
        2. 5-10 relevant subtopics
        3. Keywords related to this topic

        User Topic: {mainTitle}

        Provide the response in JSON format: ";
        // JSON format for the response
        topicSkillPrompt += @"
        {
            'Title': 'main title',
            'Subtopics': ['topic1', 'topic2', ...],
            'Keywords': ['keyword1', 'keyword2', ...]
        }";

        // Add user input
        history.AddUserMessage(topicSkillPrompt);

        // Get the response from the AI
        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);


        // Add the message from the agent to the chat history
        history.AddMessage(result.Role, result.Content ?? string.Empty); ;
        //var topicFunction = kernel.(topicSkillPrompt);
        //var result = await topicFunction.InvokeAsync(userInput);

        try
        {
            var ClearResult = CleanChatMessageContent(result);
            return JsonSerializer.Deserialize<TopicSuggestion>(ClearResult) ?? throw new InvalidOperationException("Failed to deserialize topic suggestion")
                ?? throw new InvalidOperationException("Failed to deserialize topic suggestion");
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Error deserializing topic suggestion");
            throw;
        }
    }



    public async Task<PresentationContent> GenerateContent(TopicSuggestion approvedTopics)
    {
        // Create a kernel with Azure OpenAI chat completion
        var openAiClientOption = new OpenAIClientOptions
        {
            Endpoint = new Uri(endpoint),
        };
        var apiCredential = new System.ClientModel.ApiKeyCredential(apiKey);
        var openAiClient = new OpenAIClient(apiCredential, openAiClientOption);
        var builder = Kernel.CreateBuilder();
        builder.Services.AddOpenAIChatCompletion(modelId, openAiClient);


        // Build the kernel
        Kernel kernel = builder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        // Enable planning
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        string subtopics = "";
        foreach (var subtopic in approvedTopics.Subtopics)
        {
            subtopics += $"{subtopic}, ";
        }

        string contentSkillPrompt = $@"
        Create a detailed presentation content based on:
        Title: {approvedTopics.Title}
        Topics: {subtopics}

        For each topic provide:
        1. Slide title
        2. Slide content (in bullet points)
        3. Mermaid diagram (if applicable)
        4. Speaker notes that explains Slide content

        Format the response in JSON:";

        contentSkillPrompt += @"
           {
            'MainTitle': 'title',
            'Slides': [
                {
                    'Title': 'slide title',
                    'Content': 'bullet points',
                    'MermaidDiagram': 'diagram code',
                    'SpeakerNotes': 'notes'
                }
            ]
            }";


        // Add user input
        history.AddUserMessage(contentSkillPrompt);

        // Get the response from the AI
        var result = await chatCompletionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAIPromptExecutionSettings,
            kernel: kernel);


        // Add the message from the agent to the chat history
        history.AddMessage(result.Role, result.Content ?? string.Empty); ;

        try
        {
            var clearResult = CleanChatMessageContent(result);
            return JsonSerializer.Deserialize<PresentationContent>(clearResult) ?? throw new InvalidOperationException("Failed to deserialize topic suggestion")
                ?? throw new InvalidOperationException("Failed to deserialize topic suggestion");
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "Error deserializing topic suggestion");
            throw;
        }

    }




    public Task<byte[]> CreatePowerPoint(PresentationContent content)
    {
        using (var memoryStream = new MemoryStream())
        using (var presentationDocument = PresentationDocument.Create(memoryStream, PresentationDocumentType.Presentation))
        {
            var presentationPart = CreatePresentationPart(presentationDocument);
            var slideMasterPart = CreateSlideMasterPart(presentationPart);
            var slideIdList = new SlideIdList();

            uint slideId = 255;
            foreach (var slideContent in content.Slides)
            {
                var slidePart = CreateSlidePart(presentationPart, slideContent);
                AddSlideToSlideIdList(slideIdList, slideId, slidePart, presentationPart);
                slideId++;

                if (!string.IsNullOrEmpty(slideContent.SpeakerNotes))
                {
                    AddSpeakerNotes(slidePart, slideContent);
                }
            }

            presentationPart.Presentation.AppendChild(slideIdList);
            return Task.FromResult(memoryStream.ToArray());
        }
    }

    private PresentationPart CreatePresentationPart(PresentationDocument presentationDocument)
    {
        var presentationPart = presentationDocument.AddPresentationPart();
        presentationPart.Presentation = new Presentation();
        return presentationPart;
    }

    private SlideMasterPart CreateSlideMasterPart(PresentationPart presentationPart)
    {
        var slideMasterPart = presentationPart.AddNewPart<SlideMasterPart>();
        var slideMaster = new SlideMaster();
        slideMaster.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
        slideMaster.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
        slideMasterPart.SlideMaster = slideMaster;

        var slideLayoutPart = slideMasterPart.AddNewPart<SlideLayoutPart>();
        slideLayoutPart.SlideLayout = new SlideLayout();

        return slideMasterPart;
    }

    private SlidePart CreateSlidePart(PresentationPart presentationPart, DTO.Presentation.Slide slideContent)
    {
        var slidePart = presentationPart.AddNewPart<SlidePart>();
        var slide = new DocumentFormat.OpenXml.Presentation.Slide(
            new CommonSlideData(
                new ShapeTree(
                    new Shape(
                        new TextBody(
                            new Paragraph(
                                new Run(new Text(slideContent.Title))
                            )
                        )
                    )
                )
            )
        );
        slidePart.Slide = slide;

        if (slideContent.Content.Any())
        {
            var contentShape = new Shape(
                new TextBody(
                    new Paragraph(
                        new Run(new Text(string.Join("\n", slideContent.Content)))
                    )
                )
            );
            slide.CommonSlideData.ShapeTree.AppendChild(contentShape);
        }

        return slidePart;
    }

    private void AddSlideToSlideIdList(SlideIdList slideIdList, uint slideId, SlidePart slidePart, PresentationPart presentationPart)
    {
        slideIdList.AppendChild(new SlideId
        {
            Id = slideId,
            RelationshipId = presentationPart.GetIdOfPart(slidePart)
        });
    }

    private void AddSpeakerNotes(SlidePart slidePart, DTO.Presentation.Slide slideContent)
    {
        var notesSlidePart = slidePart.AddNewPart<NotesSlidePart>();
        var notesSlide = new NotesSlide(
            new CommonSlideData(
                new ShapeTree(
                    new Shape(
                        new TextBody(
                            new Paragraph(
                                new Run(new Text(slideContent.SpeakerNotes))
                            )
                        )
                    )
                )
            )
        );
        notesSlidePart.NotesSlide = notesSlide;
    }

    private static string CleanChatMessageContent(ChatMessageContent result)
    {
        return result.Content
            .Replace("```", "")
            .Replace("json", "");
    }
}