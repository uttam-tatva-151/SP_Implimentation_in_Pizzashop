using PMSCore.Beans;
using PMSCore.DTOs;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;
using PMSServices.Utilities.Mappers;

namespace PMSServices.Services
{
    public class KOTService : IKOTService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IOrdersService _ordersService;
        private readonly IInvoiceItemMappingRepo _invoiceItemMapping;
        private readonly ICategoryRepo _categoryRepo;


        public KOTService(IOrderRepo orderRepo, IOrdersService ordersService, ICategoryRepo categoryRepo, IInvoiceItemMappingRepo invoiceItemMapping)
        {
            _orderRepo = orderRepo;
            _ordersService = ordersService;
            _categoryRepo = categoryRepo;
            _invoiceItemMapping = invoiceItemMapping;


        }

        ResponseResult result = new();
        public async Task<ResponseResult> GetKOTs(string status, int categoryId)
        {
            try
            {
                List<KOTVM> listOfKOTs = new();
                List<KOTDTO> kots = await _orderRepo.GetKotDataAsync(status, categoryId);
                listOfKOTs = KOTMapper.ToViewModelList(kots);

                if (listOfKOTs == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.KOT_LIST);
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                    List<CategoryDetails> categoryDetails = new();
                    foreach (Category category in categories)
                    {
                        CategoryDetails categoryDetail = new()
                        {
                            id = category.CategoryId,
                            categoryName = category.CategoryName,
                            description = category.Description
                        };
                        categoryDetails.Add(categoryDetail);
                    }
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.KOT_LIST);
                    result.Data = (listOfKOTs, categoryDetails);
                    result.Status = ResponseStatus.Success;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateKOTItems(List<KOTVM.KOTItemsVM> kotItems,int orderId,int editorId)
        {
            try
            {
                if (kotItems == null || kotItems.Count == 0)
                {
                    result.Message = Constants.WARNING_INVALID_INPUT;
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                List<UpdateInvoiceItemModifierMappingDTO> mappingDTOs = InvoiceItemModifierMappingMapper.MapKOTItemsToUpdateInvoiceItemModifierMappingDTOList(kotItems,orderId,editorId);
                if (mappingDTOs != null)
                {
                    result = await _invoiceItemMapping.UpdateInvoiceItemModifierMappingsAsync(mappingDTOs);
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MAPPING_RELATIONS);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

    }
}
