using PMSCore.DTOs;
using PMSCore.ViewModel;

namespace PMSServices.Utilities.Mappers;

public class KOTMapper
{
    public static KOTVM DTOToViewModel(IEnumerable<KOTDTO> kotEntities)
    {
        if (kotEntities == null || !kotEntities.Any()) return null!;

        KOTDTO first = kotEntities.First();
        return new KOTVM
        {
            OrderId = first.OrderId,
            tableName = first.TableNames,
            orderStatus = first.OrderStatus,
            sectionName = first.SectionName,
            orderComments = first.ExtraComments,
            orderAt = first.OrderAt,
            kotItems = GroupKotItemsFromDtos(kotEntities)
        };
    }
    public static List<KOTDTO> ViewModelToDTO(KOTVM kotViewModel)
    {
        if (kotViewModel == null || kotViewModel.kotItems == null) return new();

        List<KOTDTO> kotEntities = new();

        foreach (KOTVM.KOTItemsVM item in kotViewModel.kotItems)
        {
            // If no modifiers, still create a DTO for the item
            if (item.modifiers == null || !item.modifiers.Any())
            {
                kotEntities.Add(CreateDTOFromVM(kotViewModel, item, null));
            }
            else
            {
                foreach (KOTVM.KOTItemsVM.KOTModifiersVM mod in item.modifiers)
                {
                    kotEntities.Add(CreateDTOFromVM(kotViewModel, item, mod));
                }
            }
        }
        return kotEntities;
    }
    public static List<KOTVM> ToViewModelList(IEnumerable<KOTDTO> kotEntities)
    {
        if (kotEntities == null) return new List<KOTVM>();
        return kotEntities
                .GroupBy(d => d.OrderId)
                .Select(orderGroup => DTOToViewModel(orderGroup))
                .ToList();
    }

    public static List<KOTDTO> ToDTOList(IEnumerable<KOTVM> kotViewModelList)
    {
        if (kotViewModelList == null) return new List<KOTDTO>();
        return kotViewModelList.SelectMany(vm => ViewModelToDTO(vm)).ToList();
    }

    // =======================
    // --- Private Helpers ---
    // =======================
    #region Helpers
    private static List<KOTVM.KOTItemsVM> GroupKotItemsFromDtos(IEnumerable<KOTDTO> kotDtos)
    {
        return kotDtos
            .GroupBy(dto => dto.GroupListId)
            .Select(itemGroup =>
            {
                KOTDTO first = itemGroup.First();
                return new KOTVM.KOTItemsVM
                {
                    categoryId = first.CategoryId,
                    categoryName = first.CategoryName,
                    itemId = first.ItemId,
                    itemName = first.ItemName,
                    quantity = first.ItemQuantity,
                    preparedItems = first.PreparedItems,
                    itemComments = first.SpecialInstructions,
                    isPrepared = first.OrderStatus == "Ready",
                    UniqueGroupId = first.GroupListId,
                    modifiers = itemGroup
                        .Where(x => x.ModifierId.HasValue)
                        .Select(x => new KOTVM.KOTItemsVM.KOTModifiersVM
                        {
                            modifierId = x.ModifierId ?? 0,
                            modifierName = x.ModifierName
                        }).ToList(),
                    
                };
            }).ToList();
    }

    private static KOTDTO CreateDTOFromVM(KOTVM kotViewModel,
                                           KOTVM.KOTItemsVM itemVm,
                                           KOTVM.KOTItemsVM.KOTModifiersVM? modVm)
    {
        return new KOTDTO
        {
            OrderId = kotViewModel.OrderId,
            OrderStatus = kotViewModel.orderStatus,
            OrderAt = kotViewModel.orderAt,
            ExtraComments = kotViewModel.orderComments,
            GroupListId = itemVm.UniqueGroupId,
            ItemId = itemVm.itemId,
            ItemName = itemVm.itemName,
            CategoryId = itemVm.categoryId,
            CategoryName = itemVm.categoryName,
            ItemQuantity = itemVm.quantity,
            PreparedItems = itemVm.preparedItems,
            SpecialInstructions = itemVm.itemComments,
            ModifierId = modVm?.modifierId,
            ModifierName = modVm?.modifierName ?? string.Empty,
        };
    }
}
    #endregion