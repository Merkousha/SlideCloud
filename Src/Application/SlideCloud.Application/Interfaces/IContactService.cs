using SlideCloud.Application.DTO.Contact;

namespace SlideCloud.Application.Interfaces
{
    public interface IContactService
    {
        Task<bool> SubmitContactFormAsync(ContactDTO contactDto);
    }
} 