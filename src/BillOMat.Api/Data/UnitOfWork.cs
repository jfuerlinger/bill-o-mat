using BillOMat.Api.Data.Repositories;

namespace BillOMat.Api.Data
{
    public class UnitOfWork 
        : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPatientRepository _patientRepository;
        private readonly InvoiceRepository _invoiceRepository;
        
        public IPatientRepository PatientRepository => _patientRepository;

        public IInvoiceRepository InvoiceRepository => _invoiceRepository;

        public UnitOfWork(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
            _patientRepository = new PatientRepository(_dbContext);
            _invoiceRepository = new InvoiceRepository(_dbContext);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
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
