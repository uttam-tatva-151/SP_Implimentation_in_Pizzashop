using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class Taxesrepo : ITaxesRepo
    {
        private readonly AppDbContext _appDbContext;

        public Taxesrepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();
        public async Task<List<Taxis>> GetAllTaxesAsync(PaginationDetails paginationDetails)
        {
            IQueryable<Taxis> query = _appDbContext.Taxes.AsNoTracking().Where(t => t.Iscontinued == true);
            if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
            {
                query = query.Where(o => o.TaxName.Contains(paginationDetails.SearchQuery));
            }
            paginationDetails.TotalRecords = await query.CountAsync();
            return await query
                            .Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                            .Take(paginationDetails.PageSize)
                            .ToListAsync();

        }

        public async Task<ResponseResult> AddNewTaxAync(Taxis tax)
        {
            try
            {
                _appDbContext.Taxes.Add(tax);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.TAXES);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> UpdateTaxAsync(Taxis tax)
        {
            try
            {

                _appDbContext.Taxes.Update(tax);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.TAXES);
                result.Status = ResponseStatus.Success;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<Taxis> GetTaxByIdAsync(int taxId)
        {
            return await _appDbContext.Taxes.AsNoTracking().Where(t => t.TaxId == taxId && t.Iscontinued == true).FirstAsync();
        }

        public async Task<Taxis?> GetTaxByNameAsync(string taxName)
        {
            return await _appDbContext.Taxes.AsNoTracking().Where(t => t.TaxName.ToLower() == taxName.ToLower() && t.Iscontinued == true).FirstOrDefaultAsync();
        }

        public async Task<List<Taxis>> GetDefaultTaxesAsync()
        {
            return await _appDbContext.Taxes.AsNoTracking().Where(t => t.Isdefault == true && t.Iscontinued == true).ToListAsync();
        }

        public async Task<ResponseResult> AddTaxisMappingAsync(List<InvoiceTaxesMapping> invoiceTaxMappingList)
        {
            try
            {
                if (invoiceTaxMappingList == null || !invoiceTaxMappingList.Any())
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = MessageHelper.GetWarningMessageForNoSection(Constants.TAXES);
                    return result;
                }

                _appDbContext.InvoiceTaxesMappings.AddRange(invoiceTaxMappingList);
                await _appDbContext.SaveChangesAsync();

                result.Status = ResponseStatus.Success;
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.TAXES);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateTaxisMappingAsync(List<InvoiceTaxesMapping> invoiceTaxMappingList)
        {

            try
            {
                if (invoiceTaxMappingList == null || !invoiceTaxMappingList.Any())
                {
                    result.Status = ResponseStatus.NotFound;
                    result.Message = MessageHelper.GetWarningMessageForNoSection(Constants.TAXES);
                    return result;
                }

                _appDbContext.InvoiceTaxesMappings.UpdateRange(invoiceTaxMappingList);
                await _appDbContext.SaveChangesAsync();

                result.Status = ResponseStatus.Success;
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.TAXES);
            }
            catch (Exception ex)
            {
                result.Status = ResponseStatus.Error;
                result.Message = ex.Message;
            }

            return result;
        }

        public Task<List<InvoiceTaxesMapping>> GetTaxMappingsByInvoiceIdAsync(int invoiveId)
        {
            return _appDbContext.InvoiceTaxesMappings.AsNoTracking().Where(t => t.InvoiceId == invoiveId).ToListAsync();
        }
    }
}
