using Microsoft.AspNetCore.Http;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class MenuService : IMenuService
    {
        private readonly IItemRepo _itemRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IModifierRepo _modifierRepo;

        private readonly IItemModifierService _itemModifierService;

        public MenuService(IModifierRepo modifierRepo, ICategoryRepo categoryRepo, IItemRepo itemRepo, IItemModifierService itemModifierService)
        {
            _itemRepo = itemRepo;
            _categoryRepo = categoryRepo;
            _modifierRepo = modifierRepo;

            _itemModifierService = itemModifierService;
        }

        ResponseResult result = new();
        public async Task<ResponseResult> GetDefaultMenu(PaginationDetails paginationDetails)
        {
            try
            {
                if (paginationDetails == null || paginationDetails.PageNumber <= 0 || paginationDetails.PageSize <= 0)
                {
                    return new ResponseResult
                    {
                        Data = new MenuDetails(),
                        Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.PAGINATION),
                        Status = ResponseStatus.Error
                    };
                }
                List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                List<ModifiersGroup> modifierGroups = await _modifierRepo.GetAllModifierGroups();
                List<Modifier> modifiersForFirstGroup = await _modifierRepo.GetModifiersByModifierGroupId(modifierGroups[0].MgId, paginationDetails);
                List<ItemDetails> itemsForFirstCategory = await GetItemsByCategoryId(categories[0].CategoryId, paginationDetails);

                List<ModifierGropDetails> modifierGroupList = ConvertToModifierGroupList(modifierGroups);
                List<CategoryDetails> categoryList = ConvertToCategoryList(categories);
                List<ModifierDetails> modifiersList = ConvertToModifierList(modifiersForFirstGroup);
                MenuDetails defaultMenu = new()
                {
                    categories = categoryList,
                    items = itemsForFirstCategory,
                    modifierGrops = modifierGroupList,
                    modifiers = modifiersList
                };
                if (defaultMenu != null)
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.MENU);
                    result.Status = ResponseStatus.Success;
                    result.Data = defaultMenu;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        private static List<ModifierGropDetails> ConvertToModifierGroupList(List<ModifiersGroup> modifierGroups)
        {
            return modifierGroups.Select(group => new ModifierGropDetails
            {
                id = group.MgId,
                modifierGroupName = group.MgName,
                description = group.Description,
                editorId = group.Modifyby ?? group.Createby,
                modifiersIds = group.ModifierModifierGroupRelations
            .Select(relation => relation.ModifierId)
            .ToList()
            }).ToList();
        }


        #region  CRUD for Category
        public async Task<ResponseResult> AddCategory(CategoryDetails category)
        {
            try
            {

                Category? existingCategory = await _categoryRepo.GetCategoryByNameAsync(category.categoryName);
                if (existingCategory != null)
                {
                    result.Message = MessageHelper.GetWarningMessageForAllReadyEntityExists(Constants.CATEGORY);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                Category newCategory = new()
                {
                    CategoryName = category.categoryName,
                    Description = category.description,
                    Createby = category.editorId,
                    Createat = DateTime.Now,
                    Isactive = true
                };

                result = await _categoryRepo.AddCategory(newCategory);
                if (result.Status == ResponseStatus.Success)
                {
                    result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.CATEGORY);
                    List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                    List<CategoryDetails> categoryList = ConvertToCategoryList(categories);
                    result.Data = categoryList;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> EditCategory(CategoryDetails category)
        {
            try
            {
                Category? existingCategory = await _categoryRepo.GetCategoryByIdAsync(category.id);
                if (existingCategory == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CATEGORY);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                existingCategory.CategoryName = category.categoryName;
                existingCategory.Description = category.description;
                existingCategory.Modifyby = category.editorId;
                existingCategory.Modifyat = DateTime.Now;
                result = await _categoryRepo.UpdateCategory(existingCategory);
                if (result.Status == ResponseStatus.Success)
                {
                    result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.CATEGORY);
                    List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                    List<CategoryDetails> categoryList = ConvertToCategoryList(categories);
                    result.Data = categoryList;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> DeleteCategory(int id, int editorId)
        {
            try
            {
                Category existingCategory = await _categoryRepo.GetCategoryByIdAsync(id);
                if (existingCategory == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.CATEGORY);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                existingCategory.Isactive = false;
                existingCategory.Modifyby = editorId;
                existingCategory.Modifyat = DateTime.Now;
                result = await _categoryRepo.UpdateCategory(existingCategory);
                if (result.Status == ResponseStatus.Success)
                {
                    await _itemRepo.DeleteAllItemsByCategoryIdAsync(id, editorId);
                    result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.CATEGORY);
                    List<Category> categories = await _categoryRepo.GetAllCategoriesAsync();
                    List<CategoryDetails> categoryList = ConvertToCategoryList(categories);
                    result.Data = categoryList;
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


        #region CRUD for Item
        public async Task<List<ItemDetails>> GetItemsByCategoryId(int id, PaginationDetails paginationDetails)
        {

            List<Item> itemList = await _itemRepo.GetItemsByCategoryId(id, paginationDetails);
            List<ItemDetails> itemDetailsList = new();
            foreach (Item item in itemList)
            {
                ItemDetails itemDetails = new()
                {
                    id = item.ItemId,
                    itemName = item.ItemName,
                    itemType = item.ItemType,
                    unitPrice = item.UnitPrice,
                    unitType = item.UnitType,
                    quantity = item.Quantity,
                    isAvailable = item.IsAvailable ?? false,
                };
                if (item.PhotoData != null)
                {
                    itemDetails.photo = ConvertToBase64Image(item.PhotoData);

                }

                itemDetailsList.Add(itemDetails);
            }
            return itemDetailsList;
        }
        public static string? ConvertToBase64Image(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            string base64String = Convert.ToBase64String(imageData);
            return $"{Constants.IMAGE_FORMATE},{base64String}";
        }
        public async Task<ResponseResult> GetItems(int id, PaginationDetails paginationDetails)
        {
            try
            {
                if (paginationDetails == null || paginationDetails.PageNumber <= 0 || paginationDetails.PageSize <= 0)
                {
                    return new ResponseResult
                    {
                        Data = new MenuDetails(),
                        Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.PAGINATION),
                        Status = ResponseStatus.Error
                    };
                }
                List<ItemDetails> items = await GetItemsByCategoryId(id, paginationDetails);
                if (items.Count > 0)
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.ITEM);
                    result.Status = ResponseStatus.Success;
                    result.Data = items;
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ITEM);
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
        public async Task<ResponseResult> AddItem(AddItem Item)
        {
            try
            {
                if (Item == null)
                {
                    result.Message = MessageHelper.GetErrorMessageForAddOperation(Constants.CATEGORY);
                    result.Status = ResponseStatus.Error;
                }
                else
                {
                    Item newItem = new()
                    {
                        ItemName = Item.Name,
                        Description = Item.Description,
                        CategoryId = Item.CategoryId,
                        ItemType = Item.ItemType,
                        UnitType = Item.UnitType,
                        UnitPrice = Item.UnitPrice,
                        Quantity = Item.quantity,
                        TaxPercentage = Item.TaxPercentage,
                        IsAvailable = Item.IsAvailable,
                        IsDefaultTax = Item.DefaultTax,
                        ShortCode = Item.ShortCode,
                        Createby = Item.EditorId,
                        Createat = DateTime.Now,
                        Isactive = true
                    };
                    if (Item.Photo != null)
                    {
                        newItem.PhotoData = ConvertImageToByteArray(Item.Photo);
                    }
                    result = await _itemRepo.AddItemAsync(newItem);
                    int itemId = (int)result.Data;
                    if (itemId == 0)
                    {
                        result.Message = MessageHelper.GetErrorMessageForAddOperation(Constants.ITEM);
                        result.Status = ResponseStatus.Error;
                        return result;
                    }
                    if (Item.IMDetails != null)
                    {
                        List<ItemModifierGroupRelation> itemModifiers = Item.IMDetails;
                        foreach (ItemModifierGroupRelation modifier in itemModifiers)
                        {
                            modifier.ItemId = itemId;
                        }
                        result = await _itemModifierService.AddMultipleEntriesOfItemModifier(itemModifiers);
                    }
                    else
                    {
                        //No Modifiers are requierd to add in new Item
                        result.Status = ResponseStatus.Success;
                    }
                    if (result.Status == ResponseStatus.Success)
                    {
                        result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.ITEM);
                        PaginationDetails paginationDetails = new();
                        List<ItemDetails> items = await GetItemsByCategoryId(newItem.CategoryId, paginationDetails);
                        result.Data = (items, paginationDetails);
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

        public async Task<ResponseResult> UpdateItem(AddItem UpdateItem)
        {
            try
            {
                Item? existingItem = await _itemRepo.GetItemById(UpdateItem.itemID);
                if (existingItem == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ITEM);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                existingItem.ItemName = UpdateItem.Name;
                existingItem.Description = UpdateItem.Description;
                existingItem.CategoryId = UpdateItem.CategoryId;
                existingItem.ItemType = UpdateItem.ItemType;
                existingItem.UnitType = UpdateItem.UnitType;
                existingItem.UnitPrice = UpdateItem.UnitPrice;
                existingItem.Quantity = UpdateItem.quantity;
                existingItem.TaxPercentage = UpdateItem.TaxPercentage;
                existingItem.IsAvailable = UpdateItem.IsAvailable;
                existingItem.IsDefaultTax = UpdateItem.DefaultTax;
                existingItem.ShortCode = UpdateItem.ShortCode;
                existingItem.Modifyby = UpdateItem.EditorId;
                existingItem.Modifyat = DateTime.Now;
                existingItem.Isactive = true;
                if (UpdateItem.Photo != null)
                {
                    existingItem.PhotoData = ConvertImageToByteArray(UpdateItem.Photo);
                }
                result = await _itemRepo.UpdateItemAsync(existingItem);

                if (result.Status == ResponseStatus.Success)
                {
                    if (UpdateItem.IMDetails != null)
                    {
                        result = await _itemModifierService.UpdateItemModifiers(UpdateItem.IMDetails, UpdateItem.itemID);
                    }
                    if (result.Status == ResponseStatus.Success)
                    {
                        result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.ITEM);
                        PaginationDetails paginationDetails = new();
                        result.Data = await _itemRepo.GetItemsByCategoryId(existingItem.CategoryId, paginationDetails);
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

        public async Task<ResponseResult> DeleteItem(int itemId, int editorId)
        {
            try
            {
                Item? existingItem = await _itemRepo.GetItemById(itemId);
                if (existingItem == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ITEM);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                existingItem.Isactive = false;
                existingItem.Modifyat = DateTime.Now;
                existingItem.Modifyby = editorId;
                result = await _itemRepo.UpdateItemAsync(existingItem);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> DeleteMultipleItems(int[] ids, int editorId)
        {
            try
            {
                List<Item> existingItems = await _itemRepo.GetItemListByIds(ids);
                if (existingItems == null || existingItems.Count == 0)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.ITEM);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                foreach (Item item in existingItems)
                {
                    item.Isactive = false;
                    item.Modifyby = editorId;
                    item.Modifyat = DateTime.Now;
                }
                return await _itemRepo.MassUpdateItemsAsync(existingItems);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return result;
            }
        }

        public async Task<AddItem> GetItemById(int itemId)
        {
            AddItem item = new();
            Item? itemInfo = await _itemRepo.GetItemById(itemId);
            List<ItemModifierGroupRelation> itemModifierDetails = await _itemModifierService.GetItemModifierDetails(itemId);
            if(itemInfo == null){
                return item;
            }
            item.itemID = itemInfo.ItemId;
            item.Name = itemInfo.ItemName;
            item.Description = itemInfo.Description ?? string.Empty;
            item.CategoryId = itemInfo.CategoryId;
            item.ItemType = itemInfo.ItemType;
            item.UnitType = itemInfo.UnitType;
            item.UnitPrice = itemInfo.UnitPrice;
            item.quantity = itemInfo.Quantity;
            item.TaxPercentage = itemInfo.TaxPercentage ?? 0;
            item.IsAvailable = itemInfo.IsAvailable ?? false;
            item.DefaultTax = itemInfo.IsDefaultTax ?? false;
            item.ShortCode = itemInfo.ShortCode ?? string.Empty;
            item.IMDetails = itemModifierDetails;

            return item;
        }


        #endregion



        #region CRUD for Modifier


        public async Task<ResponseResult> GetModifiers(int id, PaginationDetails paginationDetails)
        {
            try
            {
                if (paginationDetails == null || paginationDetails.PageNumber <= 0 || paginationDetails.PageSize <= 0)
                {
                    return new ResponseResult
                    {
                        Data = new MenuDetails(),
                        Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.PAGINATION),
                        Status = ResponseStatus.Error
                    };
                }
                List<Modifier> modifiers = await _modifierRepo.GetModifiersByModifierGroupId(id, paginationDetails);

                if (modifiers.Count > 0)
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.MODIFIER_LIST);
                    result.Status = ResponseStatus.Success;
                    result.Data = ConvertToModifierList(modifiers);
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MODIFIER_LIST);
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


        public async Task<ResponseResult> AddNewModifier(ModifierVM newModifer)
        {
            try
            {
                Modifier? modifier = new();
                modifier = await _modifierRepo.GetModifierByModifierIdAsync(newModifer.ModifierId);
                if (modifier == null)
                {
                    modifier = new Modifier
                    {
                        MName = newModifer.ModifierName,
                        UnitPrice = newModifer.UnitPrice,
                        Quantity = newModifer.Quantity,
                        UnitType = newModifer.UnitType.ToLower(),
                        Isactive = true,
                        Description = newModifer.Description,
                        Createat = DateTime.Now,
                        Createby = newModifer.EditorId,
                        ModifierModifierGroupRelations = new List<ModifierModifierGroupRelation>()
                    };
                    foreach (int groupId in newModifer.ModifierGroupId)
                    {
                        modifier.ModifierModifierGroupRelations.Add(new ModifierModifierGroupRelation
                        {
                            GroupId = groupId,
                        });
                    }
                    result = await _modifierRepo.AddModifierAsync(modifier);
                }
                else
                {
                    result.Message = MessageHelper.GetWarningMessageForAllReadyEntityExists(Constants.MODIFIER);
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


        public async Task<ResponseResult> EditModifier(ModifierVM editModifer)
        {
            try
            {
                Modifier? modifier = await _modifierRepo.GetModifierByModifierIdAsync(editModifer.ModifierId);

                if (modifier == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MODIFIER);
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    modifier.Modifyby = editModifer.EditorId;
                    modifier.Modifyat = DateTime.Now;
                    modifier.MId = editModifer.ModifierId;
                    modifier.MName = editModifer.ModifierName;
                    modifier.UnitPrice = editModifer.UnitPrice;
                    modifier.Quantity = editModifer.Quantity;
                    modifier.UnitType = editModifer.UnitType.ToLower();
                    modifier.Isactive = true;
                    modifier.Description = editModifer.Description;

                    await UpdateModifierMappingRelations(modifier.ModifierModifierGroupRelations, editModifer.ModifierGroupId, editModifer.ModifierId);
                    result = await _modifierRepo.UpdateModifierAsync(modifier);
                    result.Data = GetModifiers(editModifer.ModifierGroupId[0], new PaginationDetails()).Result.Data;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        private async Task UpdateModifierMappingRelations(ICollection<ModifierModifierGroupRelation> modifierModifierGroupRelations, int[] modifierGroupIds, int modifierId)
        {
            List<ModifierModifierGroupRelation> existingRelation = modifierModifierGroupRelations.ToList();
            List<ModifierModifierGroupRelation> removeRelations = modifierModifierGroupRelations
                                                                                            .Where(r => !modifierGroupIds.Contains(r.GroupId))
                                                                                            .ToList();
            List<ModifierModifierGroupRelation> addNewRelations = modifierGroupIds
                                                                            .Where(id => !existingRelation.Select(r => r.GroupId).Contains(id))
                                                                            .Select(id => new ModifierModifierGroupRelation
                                                                            {
                                                                                GroupId = id
                                                                            })
                                                                            .ToList();
            if (removeRelations.Any())
            {
                await _modifierRepo.RemoveModifierAndGroupRelationsAsync(removeRelations);
            }

            if (addNewRelations.Any())
            {
                await _modifierRepo.AddNewModifierAndGroupRelationsAsync(addNewRelations);
            }
        }


        public async Task<ResponseResult> DeleteModifier(int modifierId, int editorId)
        {
            try
            {
                Modifier? existingModifier = await _modifierRepo.GetModifierByModifierIdAsync(modifierId);
                if (existingModifier == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MODIFIER);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                existingModifier.Isactive = false;
                existingModifier.Modifyby = editorId;
                existingModifier.Modifyat = DateTime.Now;
                result = await _modifierRepo.UpdateModifierAsync(existingModifier);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> DeleteMultipleModifiers(int[] ids, int editorId)
        {
            try
            {
                List<Modifier> existingModifiers = await _modifierRepo.GetModifierListByIdsAsync(ids);
                if (existingModifiers == null || existingModifiers.Count == 0)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MODIFIER);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                foreach (Modifier modifier in existingModifiers)
                {
                    modifier.Isactive = false;
                    modifier.Modifyby = editorId;
                    modifier.Modifyat = DateTime.Now;
                }
                return await _modifierRepo.MassUpdateModifiersAsync(existingModifiers);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        #endregion

        #region CRUD for ModifierGroup


        public async Task<ResponseResult> AddModifierGroup(ModifierGroupVM newGroup)
        {
            List<ModifierGropDetails> modifierGroupList = new();
            try
            {
                if (newGroup == null)
                {
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.MODIFIER_GROUP);
                    result.Status = ResponseStatus.Error;
                }
                else
                {
                    ModifiersGroup modifiersGroup = new()
                    {
                        MgName = newGroup.groupName,
                        Description = newGroup.description,
                        Createby = newGroup.editorId,
                        Createat = DateTime.Now,
                        Isactive = true
                    };
                    result = await _modifierRepo.AddModifierGroupAsync(modifiersGroup);
                    if (result.Status == ResponseStatus.Success)
                    {
                        result = await UpdateModifierMappingRelations(newGroup.groupName, 0, newGroup.modifierIds);
                        if (result.Status == ResponseStatus.Success)
                        {
                            result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.MODIFIER_GROUP);
                        }
                        else
                        {
                            result.Message = MessageHelper.GetErrorMessageForAddOperation(Constants.MAPPING_RELATIONS);
                        }
                    }
                    else
                    {
                        result.Message = MessageHelper.GetErrorMessageForAddOperation(Constants.MODIFIER_GROUP);
                        result.Status = ResponseStatus.Error;
                    }
                    List<ModifiersGroup> modifierGroups = await _modifierRepo.GetAllModifierGroups();
                    modifierGroupList = ConvertToModifierGroupList(modifierGroups);
                    result.Data = modifierGroupList;
                }
            }
            catch (Exception ex)
            {
                result.Data = modifierGroupList;
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        private async Task<ResponseResult> UpdateModifierMappingRelations(string groupName, int groupId, List<int> modifierIds)
        {
            if (groupId == 0)
            {
                groupId = await _modifierRepo.GetModifierGroupIdByName(groupName);
            }
            if (groupId == 0)
            {
                result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MODIFIER_GROUP);
                result.Status = ResponseStatus.Error;
                return result;
            }

            List<ModifierModifierGroupRelation> existingMappings = await _modifierRepo.GetModifiersRelationsByGroupid(groupId);
            List<ModifierModifierGroupRelation> newMappings = new();
            foreach (int modifierId in modifierIds)
            {
                newMappings.Add(new ModifierModifierGroupRelation
                {
                    ModifierId = modifierId,
                    GroupId = groupId
                });
            }

            List<ModifierModifierGroupRelation> addNewRelations = newMappings.Except(existingMappings).ToList();
            List<ModifierModifierGroupRelation> removeRelations = existingMappings.Except(newMappings).ToList();
            List<ModifierModifierGroupRelation> newGroupMapping = new();
            result = await _modifierRepo.AddNewModifierAndGroupRelationsAsync(addNewRelations);
            if (result.Status == ResponseStatus.Error)
            {
                result.Message = MessageHelper.GetErrorMessageForAddOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Error;
                return result;
            }
            result = await _modifierRepo.RemoveModifierAndGroupRelationsAsync(removeRelations);
            if (result.Status == ResponseStatus.Error)
            {
                result.Message = MessageHelper.GetErrorMessageForDeleteOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Error;
                return result;
            }
            else
            {
                result.Message = MessageHelper.GetErrorMessageForUpdateOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Success;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateModifierGroup(ModifierGroupVM updateGroup)
        {
            List<ModifierGropDetails> modifierGroupList = new();
            try
            {
                if (updateGroup == null)
                {
                    result.Message = MessageHelper.GetWarningMessageForInvalidInput(Constants.MODIFIER_GROUP);
                    result.Status = ResponseStatus.Error;
                }
                else
                {
                    ModifiersGroup? existingGroup = await _modifierRepo.GetModifierGroupById(updateGroup.groupId);
                    if (existingGroup == null)
                    {
                        result.Message = MessageHelper.GetNotFoundMessage(Constants.MODIFIER_GROUP);
                        result.Status = ResponseStatus.NotFound;
                        return result;
                    }
                    existingGroup.MgName = updateGroup.groupName;
                    existingGroup.Description = updateGroup.description;
                    existingGroup.Modifyby = updateGroup.editorId;
                    existingGroup.Modifyat = DateTime.Now;
                    result = await _modifierRepo.UpdateModifierGroupAsync(existingGroup);
                    if (result.Status == ResponseStatus.Success)
                    {
                        result = await UpdateModifierMappingRelations(updateGroup.groupName, updateGroup.groupId, updateGroup.modifierIds);
                        if (result.Status == ResponseStatus.Success)
                        {
                            result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MODIFIER_GROUP);
                        }
                        else
                        {
                            result.Message = MessageHelper.GetErrorMessageForUpdateOperation(Constants.MAPPING_RELATIONS);
                        }
                    }
                    else
                    {
                        result.Message = MessageHelper.GetErrorMessageForUpdateOperation(Constants.MODIFIER_GROUP);
                    }
                }
                List<ModifiersGroup> modifierGroups = await _modifierRepo.GetAllModifierGroups();
                modifierGroupList = ConvertToModifierGroupList(modifierGroups);
                result.Data = modifierGroupList;
            }
            catch (Exception ex)
            {
                result.Data = modifierGroupList;
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> DeleteModifierGroup(int modifierGroupId, int editorId)
        {
            List<ModifierGropDetails> modifierGroupList = new();
            try
            {
                ModifiersGroup? existingModifierGroup = await _modifierRepo.GetModifierGroupById(modifierGroupId);
                if (existingModifierGroup == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MODIFIER_GROUP);
                    result.Status = ResponseStatus.Error;
                    return result;
                }
                existingModifierGroup.Isactive = false;
                existingModifierGroup.Modifyby = editorId;
                existingModifierGroup.Modifyat = DateTime.Now;
                result = await _modifierRepo.UpdateModifierGroupAsync(existingModifierGroup);

                if (result.Status == ResponseStatus.Success)
                {
                    List<ModifierModifierGroupRelation> existingMappings = await _modifierRepo.GetModifiersRelationsByGroupid(modifierGroupId);
                    result = await _modifierRepo.RemoveModifierAndGroupRelationsAsync(existingMappings);
                    if (result.Status == ResponseStatus.Success)
                        result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.MODIFIER_GROUP);

                }
                List<ModifiersGroup> modifierGroups = await _modifierRepo.GetAllModifierGroups();
                modifierGroupList = ConvertToModifierGroupList(modifierGroups);
                result.Data = modifierGroupList;

            }
            catch (Exception ex)
            {
                result.Data = modifierGroupList;
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        #endregion
        #region CRUD for ItemModifier
        #endregion

        #region CRUD for ItemModifierGroup
        #endregion


        public async Task<ResponseResult> GetModifierByModifierId(int modifierId)
        {
            try
            {
                Modifier? modifier = await _modifierRepo.GetModifierByModifierIdAsync(modifierId);
                if (modifier == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MODIFIER);
                    result.Status = ResponseStatus.Error;
                }
                else
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.MODIFIER);
                    result.Status = ResponseStatus.Success;
                    result.Data = ConvertToModifierVM(modifier);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }



        public async Task<List<ModifierDetails>> GetAllModifiers(PaginationDetails paginationDetails)
        {
            List<Modifier> modifiers = await _modifierRepo.GetAllModifiers(paginationDetails);

            return ConvertToModifierList(modifiers);
        }
        public async Task<List<ModifierDetails>> GetModifiersByGroupId(int groupId)
        {
            List<Modifier> modifiers = await _modifierRepo.GetModifiersByModifierGroupIdAsync(groupId);
            return ConvertToModifierList(modifiers);
        }



        //Helper methods
        public static byte[] ConvertImageToByteArray(IFormFile file)
        {
            using MemoryStream memoryStream = new();
            file.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        // Convert Entity Model to ViewModel
        private static List<CategoryDetails> ConvertToCategoryList(List<Category> categories)
        {
            List<CategoryDetails> categoryList = categories.Select(category => new CategoryDetails
            {
                id = category.CategoryId,
                categoryName = category.CategoryName,
                description = category.Description
            }).ToList();
            return categoryList;
        }

        private static ModifierDetails ConvertToModifierDetails(Modifier modifier)
        {
            return new ModifierDetails
            {
                id = modifier.MId,
                modifierName = modifier.MName,
                description = modifier.Description ?? string.Empty,
                unitPrice = modifier.UnitPrice,
                quantity = modifier.Quantity,
                unitType = modifier.UnitType,
            };
        }
        private static ModifierVM ConvertToModifierVM(Modifier modifier)
        {
            return new ModifierVM
            {
                ModifierId = modifier.MId,
                ModifierName = modifier.MName,
                Description = modifier.Description ?? string.Empty,
                UnitPrice = modifier.UnitPrice,
                Quantity = modifier.Quantity,
                UnitType = modifier.UnitType.ToUpper(),

                // Map ModifierGroupIds from ModifierModifierGroupRelations
                ModifierGroupId = modifier.ModifierModifierGroupRelations
                    .Select(relation => relation.GroupId)
                    .ToList().ToArray()
            };
        }
        private static List<ModifierDetails> ConvertToModifierList(List<Modifier> modifiers)
        {
            List<ModifierDetails> modifierList = modifiers.Select(modifier => new ModifierDetails
            {
                id = modifier.MId,
                modifierName = modifier.MName,
                description = modifier.Description ?? string.Empty,
                unitPrice = modifier.UnitPrice,
                quantity = modifier.Quantity,
                unitType = modifier.UnitType,
            }).ToList();
            return modifierList;
        }
    }
}
