using PMSCore.Beans;
using PMSCore.DTOs;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;
using PMSServices.Utilities.Mappers;

namespace PMSServices.Services
{
    public class WaitingListService : IWaitingListService
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly IWaitingRepo _waitingRepo;
        private readonly ISectionRepo _sectionRepo;

        public WaitingListService(ICustomerRepo customerRepo, ISectionRepo sectionRepo, IWaitingRepo waitingRepo)
        {
            _customerRepo = customerRepo;
            _waitingRepo = waitingRepo;
            _sectionRepo = sectionRepo;
        }

        ResponseResult result = new();
        public async Task<ResponseResult> GetCustomerDetails(string emailId)
        {
            try
            {
                if (emailId != null)
                {
                    Customer? customer = await _customerRepo.GetCustomerDetailsByEmail(emailId);
                    CustomerDetails customerDetails = new();
                    if (customer == null)
                    {
                        customerDetails.CustomerName = "Guest";
                    }
                    else
                    {
                        customerDetails = ConvertCustomerToCustomerDetailsViewModel(customer);
                    }
                    result.Data = customerDetails;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.CUSTOMER);
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.CUSTOMER);
                    result.Status = ResponseStatus.NotFound;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        private static CustomerDetails ConvertCustomerToCustomerDetailsViewModel(Customer customer)
        {
            CustomerDetails customerDetails = new()
            {
                CustomerEmail = customer.EmailId,
                CustomerName = customer.CustName,
                CustomerPhone = customer.PhoneNumber,
                CustomerId = customer.CustId
            };
            return customerDetails;
        }

        public async Task<ResponseResult> AddWaitingToken(waitingTokenVM token)
        {
            try
            {
                if (token.CustomerId == 0)
                {
                    CustomerDTO newCustomer = new()
                    {
                        CustName = token.CustomerName,
                        PhoneNumber = token.PhoneNumber,
                        EmailId = token.Email
                    };

                    result = await _customerRepo.AddNewCustomerAsync(newCustomer);
                    if (result.Status != ResponseStatus.Success)
                    {
                        return result;
                    }
                }
                WaitingTokenDTO waitingToken = WaitingTokenMapper.WaitingTokenViewModelToDTO(token);
                result = await _waitingRepo.AddWaitingTokenAsync(waitingToken);
                if (result.Status == ResponseStatus.Success)
                {
                    List<WaitingTokenDTO> waitingTokens = await _waitingRepo.GetWaitingTokensBySectionAsync(token.SectionId);
                    if (waitingTokens != null)
                    {
                        List<waitingTokenVM> waitingTokenVMs = WaitingTokenMapper.WaitingTokensDTOListToViewModelList(waitingTokens);
                        result.Data = waitingTokenVMs;
                    }
                    else
                    {
                        result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.WAITING_TOKEN);
                        result.Status = ResponseStatus.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> EditWaitingToken(waitingTokenVM token)
        {
            try
            {
                result = await HandleCustomerDetails(token);
                if (result.Status == ResponseStatus.Success)
                {

                    WaitingTokenDTO? existingToken = await _waitingRepo.GetWaitingTokenByIdAsync(token.TokenId);
                    if(existingToken != null){
                    existingToken.NoOfPerson = token.NoOfPersons;
                    existingToken.CustomerId = token.CustomerId;
                    existingToken.Modifyat = DateTime.Now;
                    existingToken.SectionId = token.SectionId;
                    existingToken.Modifyby = token.EditorId;
                    result = await _waitingRepo.UpdateWaitingToken(existingToken);
                    }else{
                        result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.WAITING_TOKEN);
                        result.Status= ResponseStatus.NotFound;
                    }
                }
                List<WaitingTokenDTO> waitingTokens = await _waitingRepo.GetWaitingTokensBySectionAsync(token.SectionId);
                if (waitingTokens != null)
                {
                    List<waitingTokenVM> waitingTokenVMs = WaitingTokenMapper.WaitingTokensDTOListToViewModelList(waitingTokens);
                    result.Data = waitingTokenVMs;
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.WAITING_TOKEN);
                    result.Status = ResponseStatus.NotFound;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        private async Task<ResponseResult> HandleCustomerDetails(waitingTokenVM token)
        {
            if (token.CustomerId == 0)
            {
                result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.WAITING_TOKEN);
                result.Status = ResponseStatus.NotFound;
                return result;
            }
            else
            {
                Customer? customer = await _customerRepo.GetCustomerDetailsById(token.CustomerId);
                if (customer == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CUSTOMER);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                else
                {
                    bool isDuplicate = await _customerRepo.CheckForDuplicateCustomer(token.CustomerId, token.Email, token.PhoneNumber);
                    if (isDuplicate)
                    {
                        result.Message =MessageHelper.GetWarningMessageForAllReadyEntityExists(Constants.CUSTOMER);
                        result.Status = ResponseStatus.Success;
                        return result;
                    }
                    customer.CustName = token.CustomerName;
                    customer.PhoneNumber = token.PhoneNumber;
                    customer.EmailId = token.Email;
                    customer.Modifyat = DateTime.Now;
                    result = await _customerRepo.UpdateCustomerAsync(customer);
                    return result;
                }
            }
        }

        public async Task<ResponseResult> GetWaitingListBySectionId(int sectionId)
        {
            List<waitingTokenVM> waitingTokenVMs = new();
            try
            {
                List<WaitingTokenDTO> waitingTokens = await _waitingRepo.GetWaitingTokensBySectionAsync(sectionId);
                if (waitingTokens == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.WAITING_TOKEN);
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    waitingTokenVMs = WaitingTokenMapper.WaitingTokensDTOListToViewModelList(waitingTokens);
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.WAITING_TOKEN);
                    result.Status = ResponseStatus.Success;
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            result.Data = waitingTokenVMs;
            return result;
        }

        public async Task<ResponseResult> RemoveWaitingToken(waitingTokenVM tokenDetails)
        {
            try
            {
                WaitingTokenDTO? waitingToken = await _waitingRepo.GetWaitingTokenByIdAsync(tokenDetails.TokenId);
                if (waitingToken != null)
                {
                    waitingToken.IsActive = false;
                    waitingToken.Modifyat = DateTime.Now;
                    waitingToken.Modifyby = tokenDetails.EditorId;
                    result = await _waitingRepo.UpdateWaitingToken(waitingToken);
                    result.Data = await _waitingRepo.GetWaitingTokensBySectionAsync(waitingToken.SectionId);
                }
                else
                {
                    result.Message = MessageHelper.GetNotFoundMessage(Constants.WAITING_TOKEN);
                    result.Status = ResponseStatus.NotFound;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> GetSectionList()
        {
            try
            {
                List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                if (sections != null)
                {
                    List<SectionDetails> sectionList = ConvertSectionToSectionDetailsViewModel(sections);
                    result.Data = sectionList;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.SECTION_LIST);
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.SECTION_LIST);
                    result.Status = ResponseStatus.NotFound;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        private static List<SectionDetails> ConvertSectionToSectionDetailsViewModel(List<Section> sections)
        {
            List<SectionDetails> sectiondetails = new();
            foreach (Section section in sections)
            {
                SectionDetails temp = new()
                {
                    SectionId = section.SectionId,
                    SectionName = section.SectionName,
                    Description = section.Description,
                    WaitingTokensInQueue = section.WaitingLists.Count(w => w.Isactive == true),
                    TableDetails = section.Tables
                    .Where(table => table.Status != "Occupied" && table.Iscontinued == true) // Exclude tables with "Occupied" status
                    .Select(table => new TableDetails
                    {
                        TableId = table.TableId,
                        TableName = table.TableName,
                        Capacity = table.Capacity,
                        Status = table.Status
                    }).ToList()
                };
                sectiondetails.Add(temp);
            }
            return sectiondetails;
        }

        public async Task<ResponseResult> DeleteWaitingToken(int tokenId, int editorId)
        {
            try
            {
                waitingTokenVM tempToken = new()
                {
                    TokenId = tokenId,
                    EditorId = editorId,
                };
                result = await RemoveWaitingToken(tempToken);
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
