using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.Presentation;
using SlideCloud.Application.Services;

namespace SlideCloud.Web.Controllers
{
    public class PresentationController : Controller
    {

        private readonly IPresentationGeneratorService _presentationGeneratorService;
        public PresentationController(IPresentationGeneratorService presentationGeneratorService)
        {
            _presentationGeneratorService = presentationGeneratorService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateTopics([FromBody] string topic)
        {
            // TODO: Implement topic generation logic
            var x = await _presentationGeneratorService.GenerateTopics(topic);
            return Json(x);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateContent([FromBody] TopicSuggestion topics)
        {
            // TODO: Implement content generation logic
            var x = await _presentationGeneratorService.GenerateContent(topics);
            return Json(x);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePowerPoint([FromQuery] PresentationContent content)
        {
            // TODO: Implement PowerPoint creation logic
            return File(new byte[0], "application/vnd.openxmlformats-officedocument.presentationml.presentation", "presentation.pptx");
        }
    }
}