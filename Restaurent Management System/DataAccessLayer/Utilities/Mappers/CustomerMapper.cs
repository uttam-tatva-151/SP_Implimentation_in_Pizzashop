using PMSCore.DTOs;

namespace PMSData.Utilities.Mappers;

public static class CustomerMapper
{
    public static Customer DTOToEntity(CustomerDTO dto)
    {
        if (dto == null) return null!;

        return new Customer
        {
            CustId = dto.CustId,
            CustName = dto.CustName,
            EmailId = dto.EmailId,
            PhoneNumber = dto.PhoneNumber,
            TotalOrders = dto.TotalOrders,
            Createat = dto.Createat,
            Modifyat = dto.Modifyat,
            Iscontinued = dto.Iscontinued,
        };
    }
}
