using PMSCore.Beans;
using PMSCore.DTOs;
using PMSCore.ViewModel;

namespace PMSServices.Utilities.Mappers;

public static class ItemDetailsMapper
{
    public static ItemDetails ItemDetailsDTOToViewModel(ItemDetailsDTO dto)
    {
        if (dto == null) return null!;

        return new ItemDetails
        {
            id = dto.Id,
            categoryId = dto.CategoryId,
            itemName = dto.ItemName,
            itemType = dto.ItemType,
            unitPrice = dto.UnitPrice,
            Description = dto.Description,
            quantity = dto.Quantity,
            unitType = dto.UnitType,
            IsFavorite = dto.IsFavorite,
            photo = string.IsNullOrEmpty(dto.Photo) ? null : $"{Constants.IMAGE_FORMATE},{dto.Photo}",
            isAvailable = true
        };
    }

    public static ItemDetailsDTO ItemDetailsViewModelToDTO(ItemDetails model)
    {
        if (model == null) return null!;

        return new ItemDetailsDTO
        {
            Id = model.id,
            CategoryId = model.categoryId,
            ItemName = model.itemName,
            ItemType = model.itemType,
            UnitPrice = model.unitPrice,
            Description = model.Description ?? string.Empty,
            Quantity = model.quantity,
            UnitType = model.unitType,
            IsFavorite = model.IsFavorite,
            Photo = model.photo ?? string.Empty
        };
    }

    public static List<ItemDetails> ItemDetailsDTOListToViewModelList(IEnumerable<ItemDetailsDTO> dtos)
    {
        if (dtos == null) return new List<ItemDetails>();
        return dtos.Select(ItemDetailsDTOToViewModel).ToList();
    }

    public static List<ItemDetailsDTO> ItemDetailsViewModelListToDTOList(IEnumerable<ItemDetails> models)
    {
        if (models == null) return new List<ItemDetailsDTO>();
        return models.Select(ItemDetailsViewModelToDTO).ToList();
    }
}
