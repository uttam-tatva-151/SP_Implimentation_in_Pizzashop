using PMSCore.Beans;

namespace PMSData.Interfaces
{
    public interface IInvoiceTaxesMappingRepo
    {
        Task<List<InvoiceTaxesMapping>> GetTaxesListForInvoiceAsync(int invoiceId);
    }
}
