using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly IOrderRepo _orderRepo;

        public CustomerService(ICustomerRepo customerRepo, IOrderRepo orderRepo)
        {
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
        }

        ResponseResult result = new();
        public async Task<ResponseResult> GetCustomerList(PaginationDetails paginationDetails)
        {
            try
            {
                List<CustomerDetails> customersList = await _customerRepo.GetCustomersAsync(paginationDetails);
                if (customersList == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CUSTOMER);
                    result.Status = ResponseStatus.Error;
                }
                else
                {
                    result.Data = customersList;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.CUSTOMER);
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

        public async Task<ResponseResult> ExportCustomerDataAsync(PaginationDetails paginationDetails)
        {
            byte[] fileContent = await GetFilteredCustomerDataAsync(paginationDetails);
            string fileName = $"CustomerData_{DateOnly.FromDateTime(DateTime.Now).ToString(Constants.DATE_FORMATE)}.xlsx";
            result.Data = (fileContent, Constants.EXCEL_CONTENT_TYPE, fileName);
            if (fileContent == null)
            {
                result.Message = Constants.EXPORT_FILE_GENERATION_ERROR;
                result.Status = ResponseStatus.Error;
            }
            else
            {
                result.Message = Constants.EXPORT_FILE_GENERATION_SUCCESS;
                result.Status = ResponseStatus.Success;
            }
            return result;
        }


        public async Task<ResponseResult> GetCustomerHistoryAsync(int customerId)
        {
            try
            {
                List<Order> orders = await _orderRepo.GetOrdersDataByCutomerIdAsync(customerId);

                if (orders == null || !orders.Any())
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CUSTOMER);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }

                Customer customer = orders.First().Customer;
                CustomerHistory customerHistory = new()
                {
                    CustomerId = customer.CustId,
                    CustomerName = customer.CustName,
                    CustomerPhone = customer.PhoneNumber,
                    
                    MaxBill = orders.Max(o => o.PaymentDetails.Max(pd => pd.TotalPrice) ?? 0m),
                    AvgBill = orders.Average(o => o.PaymentDetails.Average(pd => pd.TotalPrice) ?? 0m),
                    FirstVisit = orders.Min(o => o.Customer.Createat),
                    Visits = orders.Count,
                    Orders = orders.Select(o => new CustomerHistory.OrderViewModel
                    {
                        OrderId = o.OrderId,
                        OrderStatus = o.Status,
                        OrderDate = DateOnly.FromDateTime(o.Createat),
                        PaymentStatus = o.PaymentDetails.FirstOrDefault()?.PaymentStatus ?? Constants.PAYMENT_PENDING,
                        NumberOfItems = o.InvoiceItemModifierMappings
                                            .Where(mapping => o.Invoices.Any(invoice => mapping.InvoiceId == invoice.InvoiceId))
                                            .Count(),
                        Amount = o.PaymentDetails.FirstOrDefault()?.TotalPrice ?? 0m
                    }).ToList()
                };

                if (customerHistory == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CUSTOMER);
                    result.Status = ResponseStatus.Error;
                }
                else
                {
                    result.Data = customerHistory;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.CUSTOMER);
                    result.Status = ResponseStatus.Success;
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return result;
            }
        }
        private async Task<byte[]> GetFilteredCustomerDataAsync(PaginationDetails paginationDetails)
        {
            try
            {
                paginationDetails.PageSize = paginationDetails.TotalRecords;
                List<CustomerDetails> customersList = await _customerRepo.GetCustomersAsync(paginationDetails);
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", Constants.CUSTOMER_DETAILS_EXPORT_FILENAME);
                FileInfo fileInfo = new(templatePath);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException(Constants.TEMPLATE_NOT_FOUND);
                }
                using ExcelPackage package = new(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                worksheet.Cells["C2:F3"].Value = "";
                worksheet.Cells["J2:M3"].Value = paginationDetails.SearchQuery;
                worksheet.Cells["C5:F6"].Value = paginationDetails.DateRange;
                worksheet.Cells["J5:M6"].Value = paginationDetails.TotalRecords;
                worksheet.Cells[1, 1, 9, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1, 9, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                int startRow = 10;

                foreach (CustomerDetails customer in customersList)
                {
                    worksheet.Cells[startRow, 1].Value = customer.CustomerId;
                    worksheet.Cells[startRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 2, startRow, 4].Merge = true;
                    worksheet.Cells[startRow, 2, startRow, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 2].Value = customer.CustomerName;
                    worksheet.Cells[startRow, 5, startRow, 8].Merge = true;
                    worksheet.Cells[startRow, 5, startRow, 8].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 5].Value = customer.CustomerEmail;
                    worksheet.Cells[startRow, 9, startRow, 11].Merge = true;
                    worksheet.Cells[startRow, 9, startRow, 11].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 9].Value = customer.LastOrder.ToString();
                    worksheet.Cells[startRow, 12, startRow, 14].Merge = true;
                    worksheet.Cells[startRow, 12, startRow, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 12].Value = customer.CustomerPhone;
                    worksheet.Cells[startRow, 15, startRow, 16].Merge = true;
                    worksheet.Cells[startRow, 15, startRow, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 15].Value = customer.TotalOrders;

                    worksheet.Cells[startRow, 1, startRow, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow, 1, startRow, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    startRow++;
                }
                return package.GetAsByteArray();
            }
            catch
            {
                throw;
            }


        }

        public async Task<ResponseResult> UpdateCustomerAsync(CustomerDetails customer)
        {
            try
            {
                Customer? existingCustomer = await _customerRepo.GetCustomerDetailsById(customer.CustomerId);
                if (existingCustomer == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CUSTOMER);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                existingCustomer.CustName = customer.CustomerName;
                existingCustomer.EmailId = customer.CustomerEmail;
                existingCustomer.PhoneNumber = customer.CustomerPhone;
                existingCustomer.Modifyat = DateTime.Now;

                result = await _customerRepo.UpdateCustomerAsync(existingCustomer);
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
