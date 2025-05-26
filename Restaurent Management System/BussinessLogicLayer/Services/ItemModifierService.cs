using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class ItemModifierService : IItemModifierService
    {
        private readonly IItemModifierRepo _itemModifierRepo;

        public ItemModifierService(IItemModifierRepo itemModifierRepo)
        {
            _itemModifierRepo = itemModifierRepo;
        }

        ResponseResult result = new();
        public async Task<ResponseResult> AddMultipleEntriesOfItemModifier(List<ItemModifierGroupRelation>? itemModifiers)
        {
            try
            {
                if (itemModifiers == null)
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.MAPPING_RELATIONS);
                    result.Status = ResponseStatus.NotFound;
                    return result;
                }
                List<ItemModifierGroupsMapping> MappingList = ConvertToMappingList(itemModifiers);
                result = await _itemModifierRepo.AddMultipleEntriesAsync(MappingList);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        private static List<ItemModifierGroupsMapping> ConvertToMappingList(List<ItemModifierGroupRelation> relations)
        {

            return relations.Select(relation => new ItemModifierGroupsMapping
            {
                ImId = relation.IMGid,
                ItemId = relation.ItemId,
                MgId = relation.MgId,
                MinModifiers = relation.MinModifiers,
                MaxModifiers = relation.MaxModifiers
            }).ToList();
        }
        // public async Task<ResponseResult> UpdateItemModifiers(List<ItemModifierGroupRelation> itemModifierGroupRelations, int itemId)
        // {
        //     try
        //     {
        //         //Take a list of existing relations
        //         List<int> existingRelationIds = await _itemModifierRepo.GetRelationsByItemIdAsync(itemId);
        //         List<int> newRelationIds = new();



        //         foreach (ItemModifierGroupRelation relation in itemModifierGroupRelations)
        //         {
        //             if (relation.IMGid == 0) continue;
        //             newRelationIds.Add(relation.IMGid);
        //         }

        //         List<int> removeRelations = existingRelationIds.Except(newRelationIds).ToList();


        //         result = await _itemModifierRepo.RemoveMultipleEntriesAsync(removeRelations);
        //         if (result.Status == ResponseStatus.Success)
        //         {
        //             List<ItemModifierGroupsMapping> MappingList = ConvertToMappingList(itemModifierGroupRelations);
        //             result = await _itemModifierRepo.AddMultipleEntriesAsync(MappingList);
        //         }
        //         else
        //         {
        //             result.Status = ResponseStatus.Error;
        //             result.Message = MessageHelper.GetErrorMessageForDeleteOperation(Constants.MAPPING_RELATIONS);
        //         }


        //     }
        //     catch (Exception ex)
        //     {
        //         result.Message = ex.Message;
        //         result.Status = ResponseStatus.Error;
        //     }
        //     return result;
        // }

        public async Task<ResponseResult> UpdateItemModifiers(List<ItemModifierGroupRelation> newRelations, int itemId)
        {

            try
            {
                // 1. Fetch existing mappings from DB
                List<ItemModifierGroupsMapping> existingMappings = await _itemModifierRepo.GetMappingsByItemIdAsync(itemId);

                // 2. Build dictionaries for fast lookup
                Dictionary<int, ItemModifierGroupRelation> newMap = newRelations
                    .ToDictionary(r => r.IMGid == 0 ? r.MgId : r.IMGid,r => r);
                Dictionary<int, ItemModifierGroupsMapping> existingMap = existingMappings
                    .ToDictionary(m => m.ImId, m => m);

                // 3. Find mappings to add, update, and remove
                List<ItemModifierGroupsMapping> mappingNeedToAdd = GetMappingsmappingNeedToAdd(newMap, existingMap, itemId);
                List<ItemModifierGroupsMapping> mappingNeedToUpdate = GetMappingsmappingNeedToUpdate(newMap, existingMap);
                List<int> mappingNeedToRemove = GetMappingsmappingNeedToRemove(newMap, existingMap);

                // 4. Remove
                if (mappingNeedToRemove.Any())
                {
                    result = await _itemModifierRepo.RemoveMultipleEntriesAsync(mappingNeedToRemove);
                    if (result.Status != ResponseStatus.Success)
                    {
                        result.Status = ResponseStatus.Error;
                        result.Message = MessageHelper.GetErrorMessageForDeleteOperation(Constants.MAPPING_RELATIONS);
                        return result;
                    }
                }

                // 5. Update
                if (mappingNeedToUpdate.Any())
                {
                    result = await _itemModifierRepo.UpdateMultipleEntriesAsync(mappingNeedToUpdate);
                    if (result.Status != ResponseStatus.Success)
                    {
                        result.Status = ResponseStatus.Error;
                        result.Message = MessageHelper.GetErrorMessageForUpdateOperation(Constants.MAPPING_RELATIONS);
                        return result;
                    }
                }

                // 6. Add
                if (mappingNeedToAdd.Any())
                {
                    result = await _itemModifierRepo.AddMultipleEntriesAsync(mappingNeedToAdd);
                    if (result.Status != ResponseStatus.Success)
                    {
                        result.Status = ResponseStatus.Error;
                        result.Message = MessageHelper.GetErrorMessageForAddOperation(Constants.MAPPING_RELATIONS);
                        return result;
                    }
                }

                result.Status = ResponseStatus.Success;
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MAPPING_RELATIONS);
            }
            catch (Exception ex)
            {
                result.Status = ResponseStatus.Error;
                result.Message = ex.Message;
            }
            return result;
        }

        // Helper: Find mappings to add
        private static List<ItemModifierGroupsMapping> GetMappingsmappingNeedToAdd(
            Dictionary<int, ItemModifierGroupRelation> newMap,
            Dictionary<int, ItemModifierGroupsMapping> existingMap,
            int itemId)
        {
            List<ItemModifierGroupsMapping> mappingNeedToAdd = new();
            foreach (KeyValuePair<int, ItemModifierGroupRelation> keyValuePairOfMapping in newMap)
            {
                if (!existingMap.ContainsKey(keyValuePairOfMapping.Value.IMGid))
                    mappingNeedToAdd.Add(ConvertToMapping(keyValuePairOfMapping.Value, itemId));
            }
            return mappingNeedToAdd;
        }

        // Helper: Find mappings to update (exists in both, but details changed)
        private static List<ItemModifierGroupsMapping> GetMappingsmappingNeedToUpdate(
            Dictionary<int, ItemModifierGroupRelation> newMap,
            Dictionary<int, ItemModifierGroupsMapping> existingMap)
        {
            List<ItemModifierGroupsMapping> mappingNeedToUpdate = new();
            foreach (KeyValuePair<int, ItemModifierGroupRelation> keyValuePairOfMapping in newMap)
            {
                ItemModifierGroupsMapping updatedMapping = new();
                if (existingMap.TryGetValue(keyValuePairOfMapping.Key, out ItemModifierGroupsMapping? existing))
                {
                    if (existing.MinModifiers != keyValuePairOfMapping.Value.MinModifiers || existing.MaxModifiers != keyValuePairOfMapping.Value.MaxModifiers){
                        updatedMapping.MinModifiers = keyValuePairOfMapping.Value.MinModifiers;
                        updatedMapping.MaxModifiers = keyValuePairOfMapping.Value.MaxModifiers;
                        updatedMapping.ImId = existing.ImId;
                        updatedMapping.ItemId = existing.ItemId;
                        updatedMapping.MgId = existing.MgId;
                        updatedMapping.Item = existing.Item;
                        updatedMapping.Mg = existing.Mg;
                        mappingNeedToUpdate.Add(updatedMapping);
                    }
                        
                }
            }
            return mappingNeedToUpdate;
        }

        // Helper: Find mappings to remove
        private static List<int> GetMappingsmappingNeedToRemove(
            Dictionary<int, ItemModifierGroupRelation> newMap,
            Dictionary<int, ItemModifierGroupsMapping> existingMap)
        {
            List<int> mappingNeedToRemove = new();
            foreach (KeyValuePair<int, ItemModifierGroupsMapping> keyValuePairOfMapping in existingMap)
            {
                if (!newMap.ContainsKey(keyValuePairOfMapping.Key))
                    mappingNeedToRemove.Add(keyValuePairOfMapping.Key); // IMGid
            }
            return mappingNeedToRemove;
        }
        public async Task<List<ItemModifierGroupRelation>> GetItemModifierDetails(int itemId)
        {
            List<ItemModifierGroupsMapping> MappingList = await _itemModifierRepo.GetItemModifierDetails(itemId);
            return ConvertToRelationList(MappingList);
        }
        public static List<ItemModifierGroupRelation> ConvertToRelationList(List<ItemModifierGroupsMapping> mappings)
        {
            return mappings.Select(mapping => new ItemModifierGroupRelation
            {
                IMGid = mapping.ImId,
                ItemId = mapping.ItemId,
                MgId = mapping.MgId,
                MinModifiers = mapping.MinModifiers,
                MaxModifiers = mapping.MaxModifiers
            }).ToList();
        }
        private static ItemModifierGroupsMapping ConvertToMapping(ItemModifierGroupRelation relation, int itemId)
        {
            return new ItemModifierGroupsMapping
            {
                ImId = relation.IMGid,
                ItemId = itemId,
                MgId = relation.MgId,
                MinModifiers = relation.MinModifiers,
                MaxModifiers = relation.MaxModifiers
            };
        }
    }

}
