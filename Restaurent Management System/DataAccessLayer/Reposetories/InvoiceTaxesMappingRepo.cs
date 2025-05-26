using Microsoft.EntityFrameworkCore;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class InvoiceTaxesMappingRepo : IInvoiceTaxesMappingRepo
    {
        private readonly AppDbContext _appDbContext;
        public InvoiceTaxesMappingRepo(AppDbContext appDbContext){
            _appDbContext = appDbContext;
        }

    
        public async Task<List<InvoiceTaxesMapping>> GetTaxesListForInvoiceAsync(int invoiceId)
        {
            return await _appDbContext.InvoiceTaxesMappings.Where(t=>t.InvoiceId == invoiceId).ToListAsync();
        }

    }
}
