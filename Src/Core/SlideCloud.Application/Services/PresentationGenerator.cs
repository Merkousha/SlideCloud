using System.Text.Json;
using ClosedXML.Excel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using SlideCloud.Application.DTO.Presentation;

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
        2. 3-5 relevant subtopics
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
        4. Speaker notes

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

    public async Task<byte[]> CreatePowerPoint(PresentationContent content)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Presentation");

        // Add title slide
        worksheet.Cell("A1").Value = content.MainTitle;
        worksheet.Cell("A1").Style.Font.FontSize = 24;
        worksheet.Cell("A1").Style.Font.Bold = true;

        int row = 3;
        foreach (var slide in content.Slides)
        {
            // Slide title
            worksheet.Cell($"A{row}").Value = slide.Title;
            worksheet.Cell($"A{row}").Style.Font.FontSize = 18;
            worksheet.Cell($"A{row}").Style.Font.Bold = true;
            row++;

            // Slide content
            worksheet.Cell($"A{row}").Value = slide.Content[0];
            row++;

            // Speaker notes
            if (!string.IsNullOrEmpty(slide.SpeakerNotes))
            {
                worksheet.Cell($"B{row}").Value = "Notes:";
                worksheet.Cell($"B{row}").Style.Font.Italic = true;
                worksheet.Cell($"C{row}").Value = slide.SpeakerNotes;
                row++;
            }

            row += 2; // Add space between slides
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
    private static string CleanChatMessageContent(ChatMessageContent result)
    {
        return result.Content
            .Replace("```", "") // Convert literal \n to actual newline
                                //.Replace("\\n", "") // Convert literal \n to actual newline
                                //.Replace("\r", "") // Remove \r characters
                                //.Replace("\\", "")  // Escape the backslash
                                //.Replace("\"", "")  // Remove quotes
            .Replace("json", ""); // Remove 'json' if needed
    }
}