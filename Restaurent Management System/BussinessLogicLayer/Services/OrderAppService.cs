using System.Data;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class OrderAppService : IOrderAppService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IInvoiceRepo _invoiceRepo;
        private readonly ITableRepo _tableRepo;
        private readonly ISectionRepo _sectionRepo;
        private readonly IPaymentRepo _paymentRepo;
        private readonly ICustomerRepo _customerRepo;
        private readonly ITaxesRepo _taxRepo;
        private readonly IFeedbackRepo _feedbackRepo;
        private readonly IWaitingListService _waitingListService;


        public OrderAppService(IOrderRepo orderRepo,IFeedbackRepo feedbackRepo, ICustomerRepo customerRepo, ITaxesRepo taxesRepo, IPaymentRepo paymentRepo, IInvoiceRepo invoiceRepo, ITableRepo tableRepo, ISectionRepo sectionRepo, IWaitingListService waitingListService)
        {
            _orderRepo = orderRepo;
            _tableRepo = tableRepo;
            _taxRepo = taxesRepo;
            _feedbackRepo = feedbackRepo;
            _customerRepo = customerRepo;
            _invoiceRepo = invoiceRepo;
            _paymentRepo = paymentRepo;
            _sectionRepo = sectionRepo;
            _waitingListService = waitingListService;
        }

        ResponseResult result = new();
        public async Task<ResponseResult> GetSectionList()
        {
            try
            {
                List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                if (sections != null)
                {

                    List<SectionDetails> sectionList = await ConvertSectionToSectionDetailsViewModel(sections);
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


        public async Task<ResponseResult> GetTableViews(int sectionId)
        {
            try
            {
                if (sectionId == 0)
                {
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.SECTION);
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    List<Table> tableList = await _tableRepo.GetTableListBySectionIdAsync(sectionId);
                    List<TableViewOrderAppVM> tableViewList = new();
                    foreach (Table table in tableList)
                    {
                        TableViewOrderAppVM tempTableViewVM = new();
                        if (table.Status == Constants.TABLE_OCCUPIED)
                            tempTableViewVM = await GetTableViewDetails(table.TableId, table);
                        else
                        {
                            tempTableViewVM.Status = table.Status;
                            tempTableViewVM.TableId = table.TableId;
                            tempTableViewVM.TableName = table.TableName;
                            tempTableViewVM.SectionId = table.SectionId;
                            tempTableViewVM.Capacity = table.Capacity;
                        }
                        tableViewList.Add(tempTableViewVM);
                    }
                    result.Data = tableViewList;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.SECTION);
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

        private async Task<TableViewOrderAppVM> GetTableViewDetails(int tableId, Table table)
        {
            Order? orderData = await _orderRepo.GetOrderDetailsByTableAssign(tableId);
            TableViewOrderAppVM tableViewOrderAppVM = new();
            if (orderData == null)
            {
                return tableViewOrderAppVM;
            }
            if (orderData.Status == Constants.ORDER_IN_PROGRESS || orderData.Status == Constants.ORDER_SERVED)
            {
                tableViewOrderAppVM.Status = "Running";
            }
            else
            {
                tableViewOrderAppVM.Status = "Assigned";
            }
            tableViewOrderAppVM.Capacity = table.Capacity;
            tableViewOrderAppVM.TableAssign = orderData.Createat;
            tableViewOrderAppVM.OrderId = orderData.OrderId;
            tableViewOrderAppVM.TableName = table.TableName;
            tableViewOrderAppVM.TableId = table.TableId;
            tableViewOrderAppVM.SectionId = table.SectionId;
            tableViewOrderAppVM.OrderAmount = orderData.PaymentDetails?.FirstOrDefault()?.ActualPrice ?? 0m;

            return tableViewOrderAppVM;
        }

        public async Task<(int Assigned, int Running, int Available)> GetTableStatusCountsAsync(int sectionId)
        {
            int assigned = 0;
            int running = 0;
            int available = 0;

            if (sectionId == 0)
                return (assigned, running, available);

            List<Table> tableList = await _tableRepo.GetTableListBySectionIdAsync(sectionId);

            foreach (Table table in tableList)
            {
                if (table.Status == Constants.TABLE_OCCUPIED)
                {
                    Order? order = await _orderRepo.GetOrderDetailsByTableAssign(table.TableId);
                    if (order != null && (order.Status == Constants.ORDER_IN_PROGRESS || order.Status == Constants.ORDER_SERVED))
                        running++;
                    else
                        assigned++;
                }
                else
                {
                    available++;
                }
            }

            return (assigned, running, available);
        }


        public async Task<ResponseResult> GetWaitingListBySectionId(int sectionId)
        {
            return await _waitingListService.GetWaitingListBySectionId(sectionId);
        }

        public async Task<ResponseResult> AssignTable(waitingTokenVM tokenDetails)
        {
            try
            {
                if (tokenDetails.CustomerId == 0)
                {
                    result = await CreateNewCustomer(tokenDetails);
                    tokenDetails.CustomerId = (int)result.Data;
                    if (result.Status != ResponseStatus.Success) return result;
                }

                Order newOrder = new()
                {
                    CustomerId = tokenDetails.CustomerId,
                    Status = Constants.ORDER_PENDING,
                    NuOfPersons = tokenDetails.NoOfPersons,
                    DeliverOnTime = false,
                    Iscontinued = true,
                    Createat = DateTime.Now,
                    Createby = tokenDetails.EditorId
                };
                result = await _orderRepo.AddOrderAsync(newOrder);
                int OrderId = (int)result.Data;
                if (result.Status == ResponseStatus.Success && OrderId > 0)
                {
                    result = await CreatePaymentEntry(OrderId);
                    int PaymentId = (int)result.Data;
                    if (result.Status == ResponseStatus.Success && PaymentId > 0)
                    {
                        result = await CreateOrderDetailsEntry(OrderId, PaymentId, tokenDetails.TableIds, tokenDetails.EditorId);
                        if (result.Status == ResponseStatus.Success)
                        {
                            if (tokenDetails.TokenId > 0) result = await _waitingListService.RemoveWaitingToken(tokenDetails);
                            if (result.Status == ResponseStatus.Success)
                            {
                                Invoice invoice = CreateInvoice(OrderId, tokenDetails.EditorId);
                                result = await _invoiceRepo.AddInvoiceAsync(invoice);
                                if (result.Status == ResponseStatus.Success)
                                {
                                    int invoiceId = (int)result.Data;
                                    List<Taxis> invoiceTaxMappingList = await _taxRepo.GetDefaultTaxesAsync();
                                    if (invoiceTaxMappingList != null && invoiceTaxMappingList.Count > 0)
                                    {
                                        List<InvoiceTaxesMapping> invoiceTaxesMappings = new();
                                        await _feedbackRepo.AddCustomerFeedbackAsync(new ());
                                        foreach (Taxis tax in invoiceTaxMappingList)
                                        {
                                            InvoiceTaxesMapping invoiceTaxesMapping = new()
                                            {
                                                InvoiceId = invoiceId,
                                                TaxId = tax.TaxId,
                                                InvoiceTaxValue = tax.TaxValue,
                                                TaxType = tax.TaxType,
                                            };
                                            invoiceTaxesMappings.Add(invoiceTaxesMapping);
                                        }
                                        result = await _taxRepo.AddTaxisMappingAsync(invoiceTaxesMappings);
                                    }
                                    if (result.Status == ResponseStatus.Success)
                                    {
                                        result.Data = OrderId;
                                        result.Message = Constants.SUCCESS_ASSIGN_TABLE;
                                        result.Status = ResponseStatus.Success;
                                        return result;
                                    }
                                }

                            }
                        }
                    }
                }
                result.Message = Constants.ERROR_TABLE_ASSIGNING;
                result.Status = ResponseStatus.Error;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }




        #region Private helper methods

        private async Task<List<SectionDetails>> ConvertSectionToSectionDetailsViewModel(List<Section> sections)
        {
            List<SectionDetails> sectiondetails = new();
            foreach (Section section in sections)
            {
                SectionDetails temp = new()
                {
                    SectionId = section.SectionId,
                    SectionName = section.SectionName,
                    Description = section.Description
                };
                sectiondetails.Add(temp);
            }
            sectiondetails = await GetSectionSummary(sectiondetails);
            return sectiondetails;
        }

        private async Task<List<SectionDetails>> GetSectionSummary(List<SectionDetails> sectiondetails)
        {
            foreach (SectionDetails section in sectiondetails)
            {

                List<Table> tables = await _tableRepo.GetTableListBySectionIdAsync(section.SectionId);

                foreach (Table table in tables)
                {
                    if (table.Status == Constants.TABLE_OCCUPIED)
                    {
                        Order? order = await _orderRepo.GetOrderDetailsByTableAssign(table.TableId);
                        if (order != null && (order.Status == Constants.ORDER_IN_PROGRESS || order.Status == Constants.ORDER_SERVED))
                            section.Running++;
                        else
                            section.Assigned++;
                    }
                    else
                    {
                        section.Available++;
                    }
                }
            }
            return sectiondetails;
        }
        private async Task<ResponseResult> CreateNewCustomer(waitingTokenVM tokenDetails)
        {
            if (!string.IsNullOrEmpty(tokenDetails.Email) && !string.IsNullOrEmpty(tokenDetails.PhoneNumber))
            {
                Customer? existingCustomer = await _customerRepo.GetCustomerDetailsByEmailOrPhone(tokenDetails.Email, tokenDetails.PhoneNumber);
                if (existingCustomer != null)
                {
                    result.Data = existingCustomer.CustId;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.CUSTOMER);
                    result.Status = ResponseStatus.Success;
                }
            }
            else
            {
                result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.CUSTOMER);
                result.Status = ResponseStatus.NotFound;
                return result;
            }
            CustomerDTO newCustomer = new()
            {
                CustName = tokenDetails.CustomerName,
                PhoneNumber = tokenDetails.PhoneNumber,
                EmailId = tokenDetails.Email
            };
            return await _customerRepo.AddNewCustomerAsync(newCustomer);
        }

        private static Invoice CreateInvoice(int orderId, int createdBy)
        {
            string invoiceNumber = GenerateInvoiceNumber(orderId);

            Invoice invoice = new()
            {
                OrderId = orderId,
                InvoiceNumber = invoiceNumber,
                CreateAt = TimeOnly.FromDateTime(DateTime.Now),
                CreateBy = createdBy,
                Isactive = true,
            };

            return invoice;
        }

        private static string GenerateInvoiceNumber(int orderId)
        {
            Int32 year = DateTime.Now.Year;
            var uniqueNumber = orderId.ToString("D5"); // Ensures sequential numbers are 5 digits (e.g., 00001).

            return $"#DOM{year}{uniqueNumber}";
        }

        private async Task<ResponseResult> CreateOrderDetailsEntry(int orderId, int paymentId, List<int> tableIds, int EditorId)
        {
            try
            {
                OrderDetail orderDetail = new()
                {
                    OrderId = orderId,
                    TableId = tableIds.ToArray(),
                    PaymentId = paymentId,
                    Createdat = DateTime.Now,
                    Createdby = EditorId
                };
                result = await _orderRepo.AddOrderDetialsAsync(orderDetail);
                if (result.Status == ResponseStatus.Success)
                {
                    result = await UpdateTablesStatus(tableIds);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            result.Data = orderId;
            return result;
        }

        private async Task<ResponseResult> UpdateTablesStatus(List<int> tableIds)
        {
            try
            {
                List<Table> tables = await _tableRepo.GetTableListFromTableIdsAsync(tableIds.ToArray());
                foreach (Table table in tables)
                {
                    table.Status = Constants.TABLE_OCCUPIED;
                }
                result = await _tableRepo.MassUpdateTablesAsync(tables);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        private async Task<ResponseResult> CreatePaymentEntry(int orderId)
        {
            try
            {
                PaymentDetail paymentDetail = new()
                {
                    OrderId = orderId,
                    PaymentMethod = Constants.PAYMENT_METHOD_CASH,
                    ActualPrice = 0,
                    TotalPrice = 0,
                    PaymentStatus = Constants.PAYMENT_PENDING,
                    Createat = DateTime.Now,
                };
                result = await _paymentRepo.AddPaymentDetailsAsync(paymentDetail);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            return result;
        }
        #endregion
    }
}
