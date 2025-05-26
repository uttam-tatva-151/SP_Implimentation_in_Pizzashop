using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class OrderAppMenuService : IOrderAppMenuService
    {
        private readonly IOrdersService _ordersService;
        private readonly IFavoriteItemRepo _favoriteItemRepo;
        private readonly IInvoiceItemMappingRepo _invoiceItemMapping;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IItemRepo _itemRepo;
        private readonly ITaxesRepo _taxRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IModifierRepo _modifierRepo;
        private readonly IFeedbackRepo _feedbackRepo;
        private readonly ITableRepo _tableRepo;

        public OrderAppMenuService(IOrderRepo orderRepo, IFeedbackRepo feedbackRepo, ITableRepo tableRepo, IModifierRepo modifierRepo, ITaxesRepo taxRepo, IFavoriteItemRepo favoriteItemRepo, IItemRepo itemRepo, IOrdersService ordersService, ICategoryRepo categoryRepo, IInvoiceItemMappingRepo invoiceItemMapping)
        {
            _ordersService = ordersService;
            _orderRepo = orderRepo;
            _modifierRepo = modifierRepo;
            _favoriteItemRepo = favoriteItemRepo;
            _tableRepo = tableRepo;
            _itemRepo = itemRepo;
            _taxRepo = taxRepo;
            _feedbackRepo = feedbackRepo;
            _categoryRepo = categoryRepo;
            _invoiceItemMapping = invoiceItemMapping;


        }

        ResponseResult result = new();
        public async Task<ResponseResult> GetCategoryList()
        {
            try
            {
                List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                if (categories != null)
                {
                    List<CategoryDetails> categoryList = ConvertCategoryToCategoryDetailsViewModel(categories);
                    result.Data = categoryList;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.CATEGORY_LIST);
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CATEGORY_LIST);
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

        public async Task<ResponseResult> GetDefaultMenu()
        {
            try
            {
                List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                List<Item> items = await _itemRepo.GetAllItemsAsync();

                List<CategoryDetails> categoryList = ConvertCategoryToCategoryDetailsViewModel(categories);
                List<ItemDetails> itemsList = ConvertItemToItemDetailsViewModel(items);

                MenuDetails menu = new()
                {
                    categories = categoryList,
                    items = itemsList
                };
                result.Data = menu;
                result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.MENU);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> GetMenuItems(bool favoritesItem, int categoryId, string searchQuery)
        {
            try
            {
                List<Item> items = new();
                List<ItemDetails> itemList = new();
                PaginationDetails paginationDetails = new()
                {
                    PageSize = 0,
                    SearchQuery = searchQuery
                };
                if (favoritesItem == false)
                {
                    items = await _itemRepo.GetItemsByCategoryId(categoryId, paginationDetails);
                }
                else
                {
                    List<FavoritesItem> favoritesItems = await _favoriteItemRepo.GetFavoriteItems(searchQuery);
                    items = ConvertFavoriteItemToItemDetailsViewModel(favoritesItems);
                }
                if (items != null)
                {
                    itemList = ConvertItemToItemDetailsViewModel(items);
                    result.Data = itemList;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.ITEM_LIST);
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ITEM_LIST);
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

        public async Task<ResponseResult> GetOrderDetailsAsync(int orderId)
        {
            result = await _ordersService.GetOrderDetailsAsync(orderId);
            OrderExportDetails orderDetails = result.Data as OrderExportDetails ?? new();

            // Check if tax details are null or only contain "Other"
            if (orderDetails.taxDetails == null || !orderDetails.taxDetails.Any() ||
                (orderDetails.taxDetails.Count == 1 && orderDetails.taxDetails.FirstOrDefault()?.TaxName == "Other"))
            {

                List<Taxis> defaultTaxes = await _taxRepo.GetDefaultTaxesAsync();
                orderDetails.taxDetails = ConvertToTaxesHelper(defaultTaxes);
                orderDetails.TotalAmountToPay = CalculateTotalAmount(orderDetails.taxDetails, 0);
            }
            result.Data = orderDetails;
            return result;

        }

        public async Task<ResponseResult> GetMenuItemMapping(int itemId)
        {
            try
            {
                Item? item = await GetItemByIdAsync(itemId);

                if (item != null)
                {
                    ItemModifierGroupRelationVM itemModifierGroupRelationVM = BuildItemModifierGroupRelationVM(item);
                    result.Data = itemModifierGroupRelationVM;
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.ITEM_LIST);
                    result.Status = ResponseStatus.Success;
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ITEM_LIST);
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

        public async Task<ResponseResult> UpdateOrder(OrderExportDetails order)
        {
            try
            {
                if (order == null || order.OrderItems == null || !order.OrderItems.Any())
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ORDER_LIST);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                OrderDetail? existingOrder = await _orderRepo.GetOrderDetailsByOrderIdAsync(order.OrderId);
                if (existingOrder == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ORDER_LIST);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }

                int invoiceId = existingOrder.Order.Invoices.First().InvoiceId;
                order.EditorId = 9;
                result = await UpdateItemListToOrder(
                    order.OrderItems,
                    existingOrder.Order.InvoiceItemModifierMappings,
                    order.OrderId,
                    order.EditorId,
                    invoiceId
                );

                if (result.Status != ResponseStatus.Success)
                {
                    return result;
                }
                (decimal subtotal, decimal total) = await CalculateTotalAmountToUpdateOrderAsync(order.OrderId, invoiceId);
                existingOrder.Modifiedby = order.EditorId;
                existingOrder.Modifiedat = DateTime.Now;
                existingOrder.Order.Status = "InProgress";
                existingOrder.Payment.ActualPrice = subtotal;
                existingOrder.Payment.TotalPrice = total;
                existingOrder.Order.ExtraComments = order.OrderInstruction;
                result = await _orderRepo.UpdateOrderAsync(existingOrder);

                if (result.Status == ResponseStatus.Success)
                {
                    result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.ORDER);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            return result;
        }

        public async Task<ResponseResult> UpdateOrderStatus(string orderStatus, int orderId, int editorId)
        {
            try
            {
                OrderDetail? order = await _orderRepo.GetOrderDetailsByOrderIdAsync(orderId);
                if (order != null)
                {
                    order.Order.Status = orderStatus;
                    order.Modifiedat = DateTime.Now;
                    order.Modifiedby = editorId;
                    order.Order.Modifyat = DateTime.Now;
                    order.Order.Modifyby = editorId;
                    result = await _orderRepo.UpdateOrderAsync(order);
                    if (result.Status == ResponseStatus.Success)
                    {
                        int[] tableIds = order.TableId;
                        List<Table> tableList = await _tableRepo.GetTableListFromTableIdsAsync(tableIds);
                        foreach (Table table in tableList)
                        {
                            table.Status = "Available";
                        }
                        result = await _tableRepo.MassUpdateTablesAsync(tableList);
                        if (result.Status == ResponseStatus.Success)
                        {
                            result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.ORDER);
                        }
                    }
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ORDER);
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

        #region Private Helper Methods

        private static decimal CalculateTotalAmount(List<OrderExportDetails.TaxDetailsHelperModel> taxDetails, decimal SubTotal)
        {
            decimal totalTax = 0;

            foreach (OrderExportDetails.TaxDetailsHelperModel tax in taxDetails)
            {
                if (tax.TaxType == "Percentage")
                {
                    totalTax += SubTotal * tax.TaxValue / 100;
                }
                else if (tax.TaxType == "Flat Amount")
                {
                    totalTax += tax.TaxValue;
                }
            }

            return totalTax + SubTotal;
        }
        private static List<OrderExportDetails.TaxDetailsHelperModel> ConvertToTaxesHelper(List<Taxis> defaultTaxes)
        {
            if (defaultTaxes == null || !defaultTaxes.Any())
            {
                return new List<OrderExportDetails.TaxDetailsHelperModel>();
            }

            return defaultTaxes.Select(tax => new OrderExportDetails.TaxDetailsHelperModel
            {
                TaxName = tax.TaxName,
                TaxValue = tax.TaxValue,
                TaxType = tax.TaxType,
            }).ToList();
        }

        private async Task<(decimal Subtotal, decimal Total)> CalculateTotalAmountToUpdateOrderAsync(int orderId, int invoiceId)
        {
            decimal subtotal = 0;
            decimal totalTaxAmount = 0;
            decimal itemTax = 0;
            decimal ItemPrice=0;
            decimal modifierPrice=0;
            // Get item and modifier mappings
            List<InvoiceItemModifierMapping> itemMappings = await _invoiceItemMapping.GetItemsForInvoiceAsync(orderId);
            int quantity = itemMappings.FirstOrDefault()?.ItemQuantity??0;
            foreach (InvoiceItemModifierMapping mapping in itemMappings)
            {
                ItemPrice = mapping.ItemPrice;
                modifierPrice = mapping.ModifierPrice ?? 0M;
                subtotal += (ItemPrice + modifierPrice) * mapping.ItemQuantity;
                itemTax += (ItemPrice * mapping.ItemTaxPercentage / 100) ?? 0M;
            }
            subtotal -= (itemMappings.Count - 1)*ItemPrice*quantity;
            // Get tax mappings and calculate tax on subtotal
            List<InvoiceTaxesMapping> taxMappings = await _taxRepo.GetTaxMappingsByInvoiceIdAsync(invoiceId);

            foreach (InvoiceTaxesMapping tax in taxMappings)
            {
                if (tax.TaxType == "Percentage")
                {
                    decimal taxAmount = subtotal * tax.InvoiceTaxValue / 100;
                    totalTaxAmount += taxAmount;
                }
                else if (tax.TaxType == "Flat Amount")
                {
                    totalTaxAmount += tax.InvoiceTaxValue;
                }
            }

            totalTaxAmount += itemTax;
            decimal total = subtotal + totalTaxAmount;

            return (subtotal, total);
        }

        private async Task<ResponseResult> UpdateItemListToOrder(List<OrderExportDetails.OrderItemHelperModel> orderItems, ICollection<InvoiceItemModifierMapping> existingMappings, int orderId, int userId, int invoiceId)
        {

            try
            {
                ResponseResult deleteResult = await DeleteRemovedMappingsAsync(orderItems, existingMappings, invoiceId);
                if (deleteResult.Status != ResponseStatus.Success)
                {
                    return deleteResult;
                }
                foreach (OrderExportDetails.OrderItemHelperModel orderItem in orderItems)
                {
                    ResponseResult itemResult = await ProcessOrderItem(orderItem, existingMappings, orderId, userId, invoiceId);
                    if (itemResult.Status != ResponseStatus.Success)
                    {
                        return itemResult;
                    }
                }

                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Status = ResponseStatus.Error;
                result.Message = ex.Message;
            }

            return result;
        }

        private async Task<ResponseResult> DeleteRemovedMappingsAsync(List<OrderExportDetails.OrderItemHelperModel> orderItems, ICollection<InvoiceItemModifierMapping> existingMappings, int invoiceId)
        {
            List<InvoiceItemModifierMapping> mappingsToDelete = existingMappings
                .Where(existing =>
                    !orderItems.Any(o =>
                        o.ItemId == existing.ItemId &&
                        existing.InvoiceId == invoiceId))
                .ToList();

            if (mappingsToDelete.Any())
            {
                ResponseResult deleteResult = await _invoiceItemMapping.DeleteMappingsAsync(mappingsToDelete);
                return deleteResult;
            }
            else
            {
                result.Status = ResponseStatus.Success;
                result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.MAPPING_RELATIONS);
                return result;
            }
        }
        private static string GenerateUniqueKey(OrderExportDetails.OrderItemHelperModel orderItem)
        {
            int itemId = orderItem.ItemId;
            List<string> modifiers = orderItem.Modifiers?
                .OrderBy(m => m.ModifierId)
                .Select(m => $"{m.ModifierId}")
                .ToList() ?? new List<string>();
            string combinedKey = $"{itemId}-{string.Join(",", modifiers)}";
            int numericHash = Math.Abs(combinedKey.GetHashCode());
            return $"{itemId}-{numericHash.ToString("D5")}";
        }
        private async Task<ResponseResult> ProcessOrderItem(OrderExportDetails.OrderItemHelperModel orderItem, ICollection<InvoiceItemModifierMapping> existingMappings, int orderId, int userId, int invoiceId)
        {
            List<InvoiceItemModifierMapping> existingMapping = FindExistingMapping(orderItem, existingMappings);

            if (existingMapping.Any())
            {
                return await UpdateExistingMapping(existingMapping, orderItem.Quantity, orderItem.SpecialInstructions ?? string.Empty);
            }

            if (orderItem.Modifiers != null && orderItem.Modifiers.Any(m => !string.IsNullOrWhiteSpace(m.ModifierName)))
            {
                return await AddMappingsWithModifiers(orderItem, orderId, userId, invoiceId);
            }
            else
            {
                return await AddMappingWithoutModifier(orderItem, orderId, userId, invoiceId);
            }
        }

        private static List<InvoiceItemModifierMapping> FindExistingMapping(OrderExportDetails.OrderItemHelperModel orderItem, ICollection<InvoiceItemModifierMapping> existingMappings)
        {

            return existingMappings.Where(mapping => mapping.ItemId == orderItem.ItemId &&
                                                (IsWithoutModifiers(orderItem, mapping) || IsWithModifiers(orderItem, mapping))).ToList();
        }
        // Check if both the order item and mapping have no modifiers
        private static bool IsWithoutModifiers(OrderExportDetails.OrderItemHelperModel orderItem, InvoiceItemModifierMapping mapping)
        {
            bool orderItemWithoutModifiers = orderItem.Modifiers == null || !orderItem.Modifiers.Any(m => m.ModifierId != 0);
            bool mappingWithoutModifiers = mapping.ModifierId == null || mapping.ModifierId == 0;

            return orderItemWithoutModifiers && mappingWithoutModifiers;
        }

        // Check if the order item and mapping have matching modifiers
        private static bool IsWithModifiers(OrderExportDetails.OrderItemHelperModel orderItem, InvoiceItemModifierMapping mapping)
        {
            return orderItem.Modifiers != null && mapping.Modifier != null &&
                   orderItem.Modifiers.Any(modifier =>
                       modifier.ModifierId == mapping.ModifierId &&
                       (modifier.ModiferQuantity == mapping.ModifiersQuantity || mapping.ModifiersQuantity == null) &&
                       (modifier.ModifierPrice == mapping.ModifierPrice || mapping.ModifierPrice == null));
        }
        private async Task<ResponseResult> UpdateExistingMapping(List<InvoiceItemModifierMapping> mappings, int newQuantity, string specialInstructions)
        {
            foreach (InvoiceItemModifierMapping map in mappings)
            {
                map.ItemQuantity = newQuantity;
                map.SpecialInstructions = specialInstructions;
            }
            result = await _invoiceItemMapping.UpdateMultipleItemMappingsAsync(mappings);
            return result;
        }

        private async Task<ResponseResult> AddMappingsWithModifiers(OrderExportDetails.OrderItemHelperModel orderItem, int orderId, int userId, int invoiceId)
        {
            if (orderItem.Modifiers != null)
                foreach (OrderExportDetails.OrderItemHelperModel.OrderModiferHelperModel modifier in orderItem.Modifiers)
                {
                    Modifier? modifierEntity = await _modifierRepo.GetModifierByNameAsync(modifier.ModifierName);
                    if (modifierEntity == null)
                    {
                        result.Status = ResponseStatus.NotFound;
                        result.Message = MessageHelper.GetNotFoundMessage(Constants.MODIFIER);
                        return result;
                    }
                    string UniqueGroupId = GenerateUniqueKey(orderItem);
                    InvoiceItemModifierMapping newMapping = new()
                    {
                        OrderId = orderId,
                        InvoiceId = invoiceId,
                        ItemId = orderItem.ItemId,
                        GroupListId = UniqueGroupId,
                        ModifierId = modifierEntity.MId,
                        ItemQuantity = orderItem.Quantity,
                        ItemPrice = orderItem.UnitPrice,
                        ModifiersQuantity = 1,
                        ModifierPrice = modifierEntity.UnitPrice,
                        Createby = userId,
                        Createat = DateTime.Now
                    };

                    result = await _invoiceItemMapping.AddMappingAsync(newMapping);
                    if (result.Status != ResponseStatus.Success)
                    {
                        return result;
                    }
                }

            result.Status = ResponseStatus.Success;
            return result;
        }

        private async Task<ResponseResult> AddMappingWithoutModifier(OrderExportDetails.OrderItemHelperModel orderItem, int orderId, int userId, int invoiceId)
        {
            Item? item = await _itemRepo.GetItemById(orderItem.ItemId);
            if (item == null)
            {
                result.Status = ResponseStatus.NotFound;
                result.Message = MessageHelper.GetNotFoundMessage(Constants.ITEM);
                return result;
            }
            string UniqueGroupId = GenerateUniqueKey(orderItem);
            InvoiceItemModifierMapping newMapping = new()
            {
                OrderId = orderId,
                InvoiceId = invoiceId,
                ItemId = orderItem.ItemId,
                ItemQuantity = orderItem.Quantity,
                ItemPrice = orderItem.UnitPrice,
                Createby = userId,
                GroupListId = UniqueGroupId,
                Createat = DateTime.Now
            };

            result = await _invoiceItemMapping.AddMappingAsync(newMapping);
            return result;
        }

        private async Task<decimal> CalculateTotalAmountToUpdateOrderAsync(List<OrderExportDetails.OrderItemHelperModel> orderItems, decimal subTotal)
        {
            List<int> itemIds = orderItems.Select(item => item.ItemId).ToList();
            Dictionary<int, decimal> defaultTaxes = await _itemRepo.GetDefaultTaxesForItemsAsync(itemIds);
            decimal totalTax = 0m;
            foreach (OrderExportDetails.OrderItemHelperModel orderItem in orderItems)
            {
                // Check if the item has a default tax
                if (defaultTaxes.TryGetValue(orderItem.ItemId, out decimal taxPercentage))
                {
                    decimal itemTax = orderItem.UnitPrice * orderItem.Quantity * (taxPercentage / 100);
                    totalTax += itemTax;
                }
            }
            return subTotal + totalTax;
        }

        private async Task<Item?> GetItemByIdAsync(int itemId)
        {
            // Fetch the item from the repository
            return await _itemRepo.GetItemById(itemId);
        }

        private static ItemModifierGroupRelationVM BuildItemModifierGroupRelationVM(Item item)
        {
            // Initialize the main ViewModel
            ItemModifierGroupRelationVM itemModifierGroupRelationVM = new()
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                ItemPrice = item.UnitPrice,
                Groups = new List<ItemModifierGroupRelationVM.ModifierGroupHelper>()
            };

            // Populate Modifier Groups
            foreach (ItemModifierGroupsMapping group in item.ItemModifierGroupsMappings)
            {
                ItemModifierGroupRelationVM.ModifierGroupHelper groupHelper = BuildModifierGroupHelper(group);
                itemModifierGroupRelationVM.Groups.Add(groupHelper);
            }

            return itemModifierGroupRelationVM;
        }

        private static ItemModifierGroupRelationVM.ModifierGroupHelper BuildModifierGroupHelper(ItemModifierGroupsMapping group)
        {
            // Initialize the ModifierGroupHelper
            ItemModifierGroupRelationVM.ModifierGroupHelper groupHelper = new()
            {
                ModifierGroupId = group.MgId,
                ModifierGroupName = group.Mg.MgName,
                MinRequired = group.MinModifiers,
                MaxRequired = group.MaxModifiers,
                Modifiers = new List<ItemModifierGroupRelationVM.ModifierGroupHelper.ModifiersHelper>()
            };

            // Populate Modifiers
            foreach (ModifierModifierGroupRelation modifier in group.Mg.ModifierModifierGroupRelations)
            {
                ItemModifierGroupRelationVM.ModifierGroupHelper.ModifiersHelper modifiersHelper = BuildModifiersHelper(modifier);
                groupHelper.Modifiers.Add(modifiersHelper);
            }

            return groupHelper;
        }

        private static ItemModifierGroupRelationVM.ModifierGroupHelper.ModifiersHelper BuildModifiersHelper(ModifierModifierGroupRelation modifierRelation)
        {
            // Initialize the ModifiersHelper
            return new ItemModifierGroupRelationVM.ModifierGroupHelper.ModifiersHelper
            {
                ModifierId = modifierRelation.ModifierId,
                ModifierName = modifierRelation.Modifier.MName,
                UnitPrice = modifierRelation.Modifier.UnitPrice,
                Quantity = modifierRelation.Modifier.Quantity, // Default quantity
                UnitType = modifierRelation.Modifier.UnitType
            };
        }

        private static List<Item> ConvertFavoriteItemToItemDetailsViewModel(List<FavoritesItem> favoritesItems)
        {
            List<Item> items = new();

            foreach (FavoritesItem favorite in favoritesItems)
            {
                if (favorite.Item != null)
                {
                    items.Add(favorite.Item);
                }
            }
            return items;
        }
        private static List<ItemDetails> ConvertItemToItemDetailsViewModel(List<Item> items)
        {
            List<ItemDetails> itemdetails = new();
            foreach (Item item in items)
            {
                if (item.IsAvailable == true)
                {
                    ItemDetails temp = new()
                    {
                        categoryId = item.CategoryId,
                        id = item.ItemId,
                        itemName = item.ItemName,
                        itemType = item.ItemType,
                        unitPrice = item.UnitPrice,
                        Description = item.Description ?? string.Empty,
                        quantity = item.Quantity,
                        unitType = item.UnitType,
                        IsFavorite = (item.FavoritesItems.Count() > 0) ? true : false // if this item is in the favorite item table then it hit true either false
                    };
                    if (item.PhotoData != null)
                    {
                        temp.photo = ConvertToBase64Image(item.PhotoData);
                    }
                    itemdetails.Add(temp);
                }
                else
                {
                    continue;
                }
            }
            return itemdetails;
        }
        private static string? ConvertToBase64Image(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            string base64String = Convert.ToBase64String(imageData);
            return $"{Constants.IMAGE_FORMATE},{base64String}";
        }
        private static List<CategoryDetails> ConvertCategoryToCategoryDetailsViewModel(List<Category> categories)
        {
            try
            {
                List<CategoryDetails> categoryList = new();

                foreach (Category category in categories)
                {
                    CategoryDetails categoryDetails = new()
                    {
                        id = category.CategoryId,
                        categoryName = category.CategoryName
                    };
                    categoryList.Add(categoryDetails);
                }
                return categoryList;
            }
            catch
            {
                return new();
            }
        }

        public async Task<ResponseResult> AddToFavorites(int itemId, int editorId)
        {
            try
            {
                Item? item = await _itemRepo.GetItemById(itemId);
                if (item != null)
                {
                    FavoritesItem favoriteItem = new()
                    {
                        ItemId = item.ItemId,
                        Item = item,
                        UserId = editorId,
                        Createdat = DateTime.Now,
                        Isactive = true,
                    };
                    result = await _favoriteItemRepo.AddToFavoritesAsync(favoriteItem);
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ITEM_LIST);
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

        public async Task<ResponseResult> RemoveFromFavorites(int itemId, int editorId)
        {
            try
            {
                FavoritesItem? favoritesItem = await _favoriteItemRepo.GetFavoriteItemById(itemId, editorId);
                if (favoritesItem != null)
                {
                    favoritesItem.Isactive = false;
                    result = await _favoriteItemRepo.UpdateAtFavoritesAsync(favoritesItem);
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ITEM_LIST);
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

        public async Task<ResponseResult> AddCustomerFeedback(CustomerReviewViewModel reviewData)
        {
            try
            {
                FeedbackForm feedback = new()
                {
                    AmbianceRating = reviewData.AmbienceRating,
                    FoodRating = reviewData.FoodRating,
                    ServiceRating = reviewData.ServiceRating,
                    FeedbackDescription = reviewData.Comment,
                };
                feedback.OrderDetails.First().OrderId = reviewData.OrderId;
                result = await _feedbackRepo.AddCustomerFeedbackAsync(feedback);

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
