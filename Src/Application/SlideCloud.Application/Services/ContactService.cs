using Microsoft.EntityFrameworkCore;
using SlideCloud.Application.DTO.Contact;
using SlideCloud.Application.Interfaces;
using SlideCloud.Domain.Models.ContactUs;
using SlideCloud.Infrastructure.Data;

namespace SlideCloud.Application.Services
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;

        public ContactService(ApplicationDbContext context)
        {
            _context = context;
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

                await _context.Contacts.AddAsync(contact);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 