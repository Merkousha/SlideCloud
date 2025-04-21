using SlideCloud.Domain.Interfaces;
using SlideCloud.Infrastructure.Repositories;

namespace SlideCloud.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDocumentRepository _documentRepository;
        private ICategoryRepository _categoryRepository;
        private IDocumentTypeRepository _documentTypeRepository;
        private IUserRepository _userRepository;
        private IContactRepository _contactRepository;
        private bool _disposed = false;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IDocumentRepository Documents
        {
            get
            {
                _documentRepository ??= new DocumentRepository(_context);
                return _documentRepository;
            }
        }

        public ICategoryRepository Categories
        {
            get
            {
                _categoryRepository ??= new CategoryRepository(_context);
                return _categoryRepository;
            }
        }

        public IDocumentTypeRepository DocumentTypes
        {
            get
            {
                _documentTypeRepository ??= new DocumentTypeRepository(_context);
                return _documentTypeRepository;
            }
        }

        public IUserRepository Users
        {
            get
            {
                _userRepository ??= new UserRepository(_context);
                return _userRepository;
            }
        }

        public IContactRepository Contacts
        {
            get
            {
                _contactRepository ??= new ContactRepository(_context);
                return _contactRepository;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }



    }
}