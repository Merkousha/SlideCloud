using Microsoft.EntityFrameworkCore;
using SlideCloud.Domain.Interfaces;
using SlideCloud.Infrastructure.Data;
using SlideCloud.Infrastructure.Repositories;

namespace SlideCloud.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDocumentRepository _documentRepository;
        private ICategoryRepository _categoryRepository;
        private IDocumentTypeRepository _documentTypeRepository;
        private IUserRepository _userRepository;
        private bool _disposed = false;

        public UnitOfWork(ApplicationDbContext context)
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

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
} 