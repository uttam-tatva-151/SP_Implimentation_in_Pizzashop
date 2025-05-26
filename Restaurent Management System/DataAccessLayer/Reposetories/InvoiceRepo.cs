using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class InvoiceRepo : IInvoiceRepo
    {
        private readonly AppDbContext _appDbContext;
        public InvoiceRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        readonly ResponseResult result = new();

        public async Task<Invoice> GetInvoiceNumberAsync(int orderId)
        {
            return await _appDbContext.Invoices.AsNoTracking().Where(i => i.OrderId == orderId).FirstAsync();
        }

        public async Task<ResponseResult> AddInvoiceAsync(Invoice invoice)
        {
            try{
                _appDbContext.Invoices.Add(invoice);
                await _appDbContext.SaveChangesAsync();
                result.Data = invoice.InvoiceId;
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.INVOICE);
                result.Status = ResponseStatus.Success;
            }catch(Exception ex){
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
    }
}
