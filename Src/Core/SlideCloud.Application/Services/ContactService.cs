using SlideCloud.Application.DTO.Contact;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Domain.Models.ContactUs;

namespace SlideCloud.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> SubmitContactFormAsync(ContactDTO contactDto)
        {
            try
            {
                var contact = new Contact
                {
                    FullName = contactDto.FullName,
                    Email = contactDto.Email,
                    PhoneNumber = contactDto.PhoneNumber,
                    Message = contactDto.Message
                };

                await _unitOfWork.Contacts.AddAsync(contact);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}