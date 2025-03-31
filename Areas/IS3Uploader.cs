namespace SlideCloud.Areas
{
    public interface IS3Uploader
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
