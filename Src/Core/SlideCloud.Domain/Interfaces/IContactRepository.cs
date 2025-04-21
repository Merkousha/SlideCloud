using SlideCloud.Domain.Models.ContactUs;

namespace SlideCloud.Domain.Interfaces;

public interface IContactRepository : IRepository<Contact>
{
    IQueryable<Contact> GetAll();
}