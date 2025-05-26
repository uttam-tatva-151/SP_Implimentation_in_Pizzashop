using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class TaxesAndFeesService : ITaxesAndFeesService
    {
        private readonly ITaxesRepo _taxRepo;

        public TaxesAndFeesService(ITaxesRepo taxRepo)
        {
            _taxRepo = taxRepo;
        }

        ResponseResult result = new();

        #region  Tax for CRUD 
        public async Task<ResponseResult> GetTaxes(PaginationDetails paginationDetails)
        {
            // return await _taxRepo.GetAllTaxesAsync(paginationDetails);
            try
            {
                List<Taxis> taxes = await _taxRepo.GetAllTaxesAsync(paginationDetails);
                if (taxes == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.TAXES);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                List<TaxDetails> taxList = ConvertTaxesToTaxDetailsViewModel(taxes);
                result.Data = taxList;
                result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.TAXES);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> AddNewTax(TaxDetails taxDetails)
        {
            try
            {
                Taxis? existingTax = await _taxRepo.GetTaxByNameAsync(taxDetails.TaxName);
                if (existingTax != null)
                {
                    result.Message = MessageHelper.GetWarningMessageForAllReadyEntityExists(taxDetails.TaxName);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }

                Taxis newTax = new()
                {
                    TaxName = taxDetails.TaxName,
                    TaxValue = taxDetails.TaxValue,
                    TaxType = taxDetails.TaxType,
                    Isdefault = taxDetails.Isdefault
                };
                newTax.Isdefault = taxDetails.Isdefault;
                newTax.Createby = taxDetails.EditorId;
                newTax.Createat = DateTime.Now;
                newTax.Iscontinued = true;
                result = await _taxRepo.AddNewTaxAync(newTax);
                if (result.Status == ResponseStatus.Success)
                {
                    PaginationDetails paginationDetails = new();
                    List<Taxis> taxes = await _taxRepo.GetAllTaxesAsync(paginationDetails);
                    if (taxes == null)
                    {
                        result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.TAXES);
                        result.Status = ResponseStatus.NotFound;
                        return result;
                    }
                    List<TaxDetails> taxList = ConvertTaxesToTaxDetailsViewModel(taxes);
                    result.Data = (taxList, paginationDetails);
                    result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.TAXES);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateTax(TaxDetails taxDetails)
        {
            try
            {
                Taxis existingTax = await _taxRepo.GetTaxByIdAsync(taxDetails.TaxId);
                if (existingTax == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.TAXES);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                existingTax.TaxType = taxDetails.TaxType;
                existingTax.TaxName = taxDetails.TaxName;
                existingTax.Isdefault = taxDetails.Isdefault;
                existingTax.Isenabled = taxDetails.Isenabled;
                existingTax.TaxValue = taxDetails.TaxValue;
                existingTax.Modifyby = taxDetails.EditorId;
                existingTax.Modifyat = DateTime.Now;

                result = await _taxRepo.UpdateTaxAsync(existingTax);
                if (result.Status == ResponseStatus.Success)
                {
                    PaginationDetails paginationDetails = new();
                    List<Taxis> taxes = await _taxRepo.GetAllTaxesAsync(paginationDetails);
                    if (taxes == null)
                    {
                        result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.TAXES);
                        result.Status = ResponseStatus.NotFound;
                        return result;
                    }
                    List<TaxDetails> taxList = ConvertTaxesToTaxDetailsViewModel(taxes);
                    result.Data = (taxList, paginationDetails);
                    result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.TAXES);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;

        }
        public async Task<ResponseResult> DeleteTax(int taxId, int editorId)
        {
            try
            {
                Taxis existingTax = await _taxRepo.GetTaxByIdAsync(taxId);
                if (existingTax == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.TAXES);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                existingTax.Iscontinued = false;
                existingTax.Modifyby = editorId;
                existingTax.Modifyat = DateTime.Now;

                result = await _taxRepo.UpdateTaxAsync(existingTax);
                if (result.Status == ResponseStatus.Success)
                {
                    PaginationDetails paginationDetails = new();
                    List<Taxis> taxes = await _taxRepo.GetAllTaxesAsync(paginationDetails);
                    if (taxes == null)
                    {
                        result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.TAXES);
                        result.Status = ResponseStatus.NotFound;
                        return result;
                    }
                    List<TaxDetails> taxList = ConvertTaxesToTaxDetailsViewModel(taxes);
                    result.Data = (taxList, paginationDetails);
                    result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.TAXES);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        #endregion

        #region Helper Methods
        private static List<TaxDetails> ConvertTaxesToTaxDetailsViewModel(List<Taxis> taxes)
        {
            List<TaxDetails> taxList = new();
            foreach (Taxis taxis in taxes)
            {
                TaxDetails temp = new()
                {
                    TaxId = taxis.TaxId,
                    TaxName = taxis.TaxName,
                    TaxType = taxis.TaxType,
                    Isenabled = taxis.Isenabled ?? false,
                    Isdefault = taxis.Isdefault,
                    TaxValue = taxis.TaxValue
                };
                taxList.Add(temp);
            }
            return taxList;
        }

        #endregion
    }
}
