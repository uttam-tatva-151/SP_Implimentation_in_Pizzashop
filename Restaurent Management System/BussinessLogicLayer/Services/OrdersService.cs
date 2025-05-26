using OfficeOpenXml;
using OfficeOpenXml.Style;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using SelectPdf;
using PMSData.Interfaces;
using PMSServices.Interfaces;
using PMSCore.Beans.ENUM;

namespace PMSServices.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IInvoiceItemMappingRepo _invoiceItemMapping;
        private readonly IInvoiceRepo _invoiceRepo;
        private readonly IInvoiceTaxesMappingRepo _invoiceTaxesMapping;
        private readonly ITableRepo _tableRepo;
        private readonly ISectionRepo _sectionRepo;
        private readonly ITaxesRepo _taxRepo;
        private readonly IModifierRepo _modifierRepo;

        public OrdersService(IOrderRepo orderRepo, ITableRepo tableRepo, ISectionRepo sectionRepo, ITaxesRepo taxRepo, IModifierRepo modifierRepo, IInvoiceItemMappingRepo invoiceItemMapping, IInvoiceRepo invoiceRepo, IInvoiceTaxesMappingRepo invoiceTaxesMapping)
        {
            _orderRepo = orderRepo;
            _invoiceRepo = invoiceRepo;
            _tableRepo = tableRepo;
            _sectionRepo = sectionRepo;
            _taxRepo = taxRepo;
            _modifierRepo = modifierRepo;
            _invoiceItemMapping = invoiceItemMapping;
            _invoiceTaxesMapping = invoiceTaxesMapping;
        }

        readonly ResponseResult result = new();
        public async Task<ResponseResult> GetOrderList(PaginationDetails paginationDetails)
        {
            try
            {
                List<OrderDetail> orders = await _orderRepo.GetOrderListAsync(paginationDetails);
                List<OrderDetailsVM> orderList = ConvertToViewModel(orders);
                result.Data = orderList;

                if (result.Data == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ORDER_LIST);
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.ORDER_LIST);
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

        public (byte[], string) CreatePdf(string invoiceId, string partialView)
        {
            try
            {
                // Convert the HTML string to a PDF
                HtmlToPdf converter = new();
                PdfDocument pdf = converter.ConvertHtmlString(partialView);

                using MemoryStream stream = new();
                pdf.Save(stream);
                pdf.Close();

                // Generate a file name for the PDF
                string fileName = $"Invoice_{invoiceId}.pdf";

                // Return the PDF as a byte array and the file name
                return (stream.ToArray(), fileName);
            }
            catch
            {
                throw new Exception(Constants.EXPORT_FILE_GENERATION_ERROR);
            }
        }
        public async Task<ResponseResult> ExportOrderList(string orderSearch, string status, string dateRange)
        {
            try
            {
                PaginationDetails paginationDetails = new()
                {
                    PageSize = 0,
                    SearchQuery = orderSearch,
                    OrderStatus = Enum.Parse<OrderStatus>(status),
                    DateRange = Enum.Parse<TimePeriod>(dateRange)
                };
                List<OrderDetail> orders = await _orderRepo.GetOrderListAsync(paginationDetails);
                List<OrderDetailsVM> orderList = ConvertToViewModel(orders);
                result.Data = GenerateExcelFile(orderSearch, status, dateRange, paginationDetails.TotalRecords, orderList);
                if (result.Data == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ORDER);
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    result.Message = Constants.EXPORT_FILE_GENERATION_SUCCESS;
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
        private static List<OrderDetailsVM> ConvertToViewModel(List<OrderDetail> orders)
        {
            List<OrderDetailsVM> orderList = new();
            foreach (OrderDetail orderDetail in orders)
            {
                short OrderFeedback = 0;
                if (orderDetail.Feedback != null)
                {
                    OrderFeedback = (short)(((orderDetail.Feedback.FoodRating ?? 0) + (orderDetail.Feedback.AmbianceRating ?? 0) + (orderDetail.Feedback.ServiceRating ?? 0) )/ 3);
                }
                OrderDetailsVM orderDetailVM = new()
                {
                    OrderId = orderDetail.OrderId,
                    OrderDate = DateOnly.FromDateTime(orderDetail.Createdat),
                    CustomerName = orderDetail.Order.Customer != null ? orderDetail.Order.Customer.CustName : string.Empty,
                    OrderStatus = orderDetail.Order.Status,
                    PaymentType = orderDetail.Payment != null ? orderDetail.Payment.PaymentMethod : string.Empty,
                    Rating = OrderFeedback,
                    TotalAmount = orderDetail.Payment != null ? orderDetail.Payment.ActualPrice : 0
                };
                orderList.Add(orderDetailVM);
            }
            return orderList;
        }
        private static byte[] GenerateExcelFile(string orderSearch, string status, string dateRange, int noOfRecords, List<OrderDetailsVM> orders)
        {
            try
            {
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", Constants.ORDER_SALES_DATA_FORMAT_FILE);
                FileInfo fileInfo = new(templatePath);
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException(Constants.TEMPLATE_NOT_FOUND);
                }
                using ExcelPackage package = new(fileInfo);
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                worksheet.Cells["C2:F3"].Value = status;
                worksheet.Cells["J2:M3"].Value = orderSearch;
                worksheet.Cells["C5:F6"].Value = dateRange;
                worksheet.Cells["J5:M6"].Value = noOfRecords;
                worksheet.Cells[1, 1, 8, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1, 8, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                int startRow = 9;

                foreach (OrderDetailsVM order in orders)
                {
                    worksheet.Cells[startRow, 1].Value = order.OrderId;
                    worksheet.Cells[startRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 2, startRow, 4].Merge = true;
                    worksheet.Cells[startRow, 2, startRow, 4].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 2].Value = order.OrderDate.ToString();
                    worksheet.Cells[startRow, 5, startRow, 7].Merge = true;
                    worksheet.Cells[startRow, 5, startRow, 7].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 5].Value = order.CustomerName;
                    worksheet.Cells[startRow, 8, startRow, 10].Merge = true;
                    worksheet.Cells[startRow, 8, startRow, 10].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 8].Value = order.OrderStatus;
                    worksheet.Cells[startRow, 11, startRow, 12].Merge = true;
                    worksheet.Cells[startRow, 11, startRow, 12].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 11].Value = order.PaymentType;
                    worksheet.Cells[startRow, 13, startRow, 14].Merge = true;
                    worksheet.Cells[startRow, 13, startRow, 14].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 13].Value = order.Rating;
                    worksheet.Cells[startRow, 15, startRow, 16].Merge = true;
                    worksheet.Cells[startRow, 15, startRow, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    worksheet.Cells[startRow, 15].Value = order.TotalAmount;

                    worksheet.Cells[startRow, 1, startRow, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[startRow, 1, startRow, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    startRow++;
                }
                return package.GetAsByteArray();
            }
            catch
            {
                throw;
            }
        }

        public async Task<ResponseResult> GetOrderDetailsAsync(int orderId)
        {
            try
            {
                OrderDetail? orderDetails = await _orderRepo.GetOrderDetailsByOrderIdAsync(orderId);
                if (orderDetails == null)
                {
                    result.Message = MessageHelper.GetNotFoundMessage(Constants.ORDER);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                List<InvoiceItemModifierMapping> listofItems = await _invoiceItemMapping.GetItemsForInvoiceAsync(orderId);
                Invoice invoiceDetails = await _invoiceRepo.GetInvoiceNumberAsync(orderId);
                List<InvoiceTaxesMapping> listOfTaxes = await _invoiceTaxesMapping.GetTaxesListForInvoiceAsync(invoiceDetails.InvoiceId);

                OrderExportDetails invoice = await CreateInvoice(orderDetails, listofItems, invoiceDetails, listOfTaxes);

                result.Data = invoice;
                if (result.Data == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ORDER);
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.ORDER);
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

        //Helper Methods
        private async Task<OrderExportDetails> CreateInvoice(OrderDetail orderDetails, List<InvoiceItemModifierMapping> listofItems, Invoice invoiceDetails, List<InvoiceTaxesMapping> listOfTaxes)
        {
            OrderExportDetails invoice = new()
            {
                OrderId = orderDetails.OrderId,
                InvoiceNo = invoiceDetails.InvoiceNumber,
                Status = orderDetails.Order.Status,
                PaidOn = orderDetails.Payment.Createat,
                PlacedOn = orderDetails.Createdat,
                ModifiedOn = orderDetails.Modifiedat ?? orderDetails.Createdat
            };
            TimeSpan duration = invoice.ModifiedOn - invoice.PlacedOn;
            TimeSpan normalizedDuration = TimeSpan.FromHours(duration.TotalHours % 24);
            invoice.OrderDuration = TimeOnly.FromTimeSpan(normalizedDuration);
            (invoice.Tables, invoice.Section) = await GetTableBasedOrdersDetails(orderDetails.TableId);
            invoice.PaymentMethod = orderDetails.Payment.PaymentMethod;
            invoice.taxDetails = await GetTaxDetails(listOfTaxes, listofItems);
            invoice.OrderItems = await GetOrderItems(listofItems);
            invoice.SubTotal = orderDetails.Payment.ActualPrice;
            invoice.TotalAmountToPay = CalculateTotalAmount(invoice.taxDetails, invoice.SubTotal);
            invoice.CustomerInfo = GetCustomerInfo(orderDetails.Order.Customer, orderDetails.Order.NuOfPersons);

            return invoice;
        }

        private static CustomerDetails GetCustomerInfo(Customer customer, int noOfPersons)
        {
            CustomerDetails customerDetails = new()
            {
                CustomerId = customer.CustId,
                CustomerName = customer.CustName,
                NoOfPerson = noOfPersons,
                CustomerPhone = customer.PhoneNumber,
                CustomerEmail = customer.EmailId ?? "_"
            };

            return customerDetails;
        }


        private static decimal CalculateTotalAmount(List<OrderExportDetails.TaxDetailsHelperModel> taxDetails, decimal SubTotal)
        {
            decimal totalTax = taxDetails.Sum(t => t.TaxValue);
            return totalTax + SubTotal;
        }


        private async Task<List<OrderExportDetails.OrderItemHelperModel>> GetOrderItems(List<InvoiceItemModifierMapping> listofItems)
        {
            List<OrderExportDetails.OrderItemHelperModel> orderItems = new();

            IEnumerable<IGrouping<string?, InvoiceItemModifierMapping>> groupedItems = listofItems.GroupBy(i => i.GroupListId); // Grouping by item_id

            foreach (IGrouping<string?, InvoiceItemModifierMapping> group in groupedItems)
            {
                InvoiceItemModifierMapping firstItem = group.First(); // Get first entry to retrieve item details

                OrderExportDetails.OrderItemHelperModel orderItem = new()
                {
                    ItemName = firstItem.Item.ItemName, // Fetch item name from DB 
                    ItemId = firstItem.ItemId,
                    Quantity = firstItem.ItemQuantity,
                    SpecialInstructions = firstItem.SpecialInstructions ?? string.Empty,
                    UnitPrice = firstItem.ItemPrice,
                    TaxPerItem = firstItem.ItemTaxPercentage ?? 0M,
                    UniqueGroupId = firstItem.GroupListId ?? string.Empty,
                    PreparedItems = firstItem.PreparedItems,
                    TotalPrice = firstItem.ItemQuantity * firstItem.ItemPrice,
                    Modifiers = new List<OrderExportDetails.OrderItemHelperModel.OrderModiferHelperModel>()
                };
                if (group.Any())
                {

                    foreach (InvoiceItemModifierMapping row in group)
                    {
                        if (row.ModifierId != 0)
                        {

                            OrderExportDetails.OrderItemHelperModel.OrderModiferHelperModel modifierModel = new()
                            {
                                ModifierId = row.ModifierId ?? 0,
                                ModifierName = await _modifierRepo.GetModifierNameByIdAsync(row.ModifierId ?? 0) ?? string.Empty, // Fetch modifier name
                                ModiferQuantity = row.ModifiersQuantity ?? 0,
                                ModifierPrice = row.ModifierPrice ?? 0m,
                                ModifierTotalPrice = row.ModifierPrice * row.ModifiersQuantity ?? 0
                            };
                            orderItem.Modifiers.Add(modifierModel);
                        }

                    }


                }
                orderItems.Add(orderItem);
            }

            return orderItems;
        }



        private async Task<List<OrderExportDetails.TaxDetailsHelperModel>> GetTaxDetails(List<InvoiceTaxesMapping> listOfTaxes, List<InvoiceItemModifierMapping> listofItems)
        {
            List<OrderExportDetails.TaxDetailsHelperModel> taxDetails = new();
            foreach (InvoiceTaxesMapping taxmapping in listOfTaxes)
            {
                OrderExportDetails.TaxDetailsHelperModel taxDetail = new();
                Taxis taxis = await _taxRepo.GetTaxByIdAsync(taxmapping.TaxId);
                taxDetail.TaxName = taxis.TaxName;
                taxDetail.TaxValue = taxmapping.InvoiceTaxValue;
                taxDetail.TaxType = taxis.TaxType;
                taxDetails.Add(taxDetail);
            }
            OrderExportDetails.TaxDetailsHelperModel taxDetailFormPerItems = CalculateTaxForItems(listofItems);
            taxDetails.Add(taxDetailFormPerItems);
            return taxDetails;
        }

        private static OrderExportDetails.TaxDetailsHelperModel CalculateTaxForItems(List<InvoiceItemModifierMapping> listofItems)
        {
            if (listofItems == null || !listofItems.Any())
            {
                return new OrderExportDetails.TaxDetailsHelperModel
                {
                    TaxName = "Other",
                    TaxValue = 0m,
                    TaxType = "Flat Amount"
                };
            }

            decimal totalTax = listofItems.Sum(item =>
            {
                decimal itemTotalPrice = item.ItemPrice * item.ItemQuantity;
                decimal taxAmount = item.ItemTaxPercentage.HasValue
                    ? (itemTotalPrice * item.ItemTaxPercentage.Value / 100)
                    : 0m;
                return taxAmount;
            });

            return new OrderExportDetails.TaxDetailsHelperModel
            {
                TaxName = "Other",
                TaxValue = totalTax,
                TaxType = "Flat Amount"
            };
        }


        public async Task<(string, string)> GetTableBasedOrdersDetails(int[] tableIds)
        {
            string TablesName = string.Empty;
            int sectionId = 0;
            List<Table> tablesDetails = await _tableRepo.GetTableListFromTableIdsAsync(tableIds);
            Section sectionDetails = await _sectionRepo.GetSectionAsync(sectionId) ?? new();
            foreach (Table table in tablesDetails)
            {
                TablesName += table.TableName + " , ";
                sectionId = table.SectionId;
            }
            TablesName = TablesName.TrimEnd(new char[] { ' ', ',' });
            return (TablesName, sectionDetails.SectionName);
        }



    }
}
