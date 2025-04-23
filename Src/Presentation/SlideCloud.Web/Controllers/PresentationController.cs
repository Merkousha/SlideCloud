using Microsoft.AspNetCore.Mvc;
using SlideCloud.Application.DTO.Presentation;
using SlideCloud.Application.Services;

namespace SlideCloud.Web.Controllers
{
    public class PresentationController : Controller
    {
        private readonly IPresentationGeneratorService _presentationGeneratorService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string TOPICS_SESSION_KEY = "PresentationTopics";
        private const string CONTENT_SESSION_KEY = "PresentationContent";

        public PresentationController(
            IPresentationGeneratorService presentationGeneratorService,
            IWebHostEnvironment webHostEnvironment)
        {
            _presentationGeneratorService = presentationGeneratorService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // پاک کردن داده‌های قبلی از session
            HttpContext.Session.Remove(TOPICS_SESSION_KEY);
            HttpContext.Session.Remove(CONTENT_SESSION_KEY);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateTopics([FromBody] TopicRequest request)
        {
            try
            {
                var result = await _presentationGeneratorService.GenerateTopics(request.Topic);

                // ذخیره سرفصل‌ها در session
                HttpContext.Session.SetString(TOPICS_SESSION_KEY, System.Text.Json.JsonSerializer.Serialize(result));

                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "خطا در ایجاد سرفصل‌ها" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateContent()
        {
            try
            {
                // بازیابی سرفصل‌ها از session
                var topicsJson = HttpContext.Session.GetString(TOPICS_SESSION_KEY);
                if (string.IsNullOrEmpty(topicsJson))
                {
                    return BadRequest(new { error = "سرفصل‌ها یافت نشد. لطفاً دوباره تلاش کنید." });
                }

                var topics = System.Text.Json.JsonSerializer.Deserialize<TopicSuggestion>(topicsJson);
                var result = await _presentationGeneratorService.GenerateContent(topics);

                // ذخیره محتوا در session
                HttpContext.Session.SetString(CONTENT_SESSION_KEY, System.Text.Json.JsonSerializer.Serialize(result));

                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "خطا در ایجاد محتوا" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePowerPoint()
        {
            try
            {
                // بازیابی محتوا از session
                var contentJson = HttpContext.Session.GetString(CONTENT_SESSION_KEY);
                if (string.IsNullOrEmpty(contentJson))
                {
                    return BadRequest(new { error = "محتوا یافت نشد. لطفاً دوباره تلاش کنید." });
                }

                var content = System.Text.Json.JsonSerializer.Deserialize<PresentationContent>(contentJson);
                var fileBytes = await _presentationGeneratorService.CreatePowerPoint(content);

                // ایجاد مسیر فایل در پوشه wwwroot
                var fileName = $"SlideCloud-{content.MainTitle}-{DateTime.Now:yyyyMMddHHmmss}.pptx";
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "downloads", fileName);

                // اطمینان از وجود پوشه downloads
                var downloadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "downloads");
                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                }

                // ذخیره فایل
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);

                // پاک کردن داده‌ها از session
                HttpContext.Session.Remove(TOPICS_SESSION_KEY);
                HttpContext.Session.Remove(CONTENT_SESSION_KEY);

                // برگرداندن URL فایل
                var fileUrl = $"/downloads/{fileName}";
                return Json(new PresentationResponse
                {
                    Success = true,
                    FileUrl = fileUrl
                });
            }
            catch (Exception ex)
            {
                return Json(new PresentationResponse
                {
                    Success = false,
                    ErrorMessage = "خطا در ساخت پاورپوینت"
                });
            }
        }
    }

    public class TopicRequest
    {
        public string Topic { get; set; }
    }
}