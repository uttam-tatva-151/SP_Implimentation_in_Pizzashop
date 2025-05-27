using PMSCore.DTOs;
using PMSCore.ViewModel;

namespace PMSServices.Utilities.Mappers;

public class WaitingTokenMapper
{
    public static waitingTokenVM WaitingTokenDTOToViewModel(WaitingTokenDTO waitingTokenDTO)
    {
        if (waitingTokenDTO == null) return null!;

        return new waitingTokenVM
        {
            TokenId = waitingTokenDTO.TokenId,
            CustomerId = waitingTokenDTO.CustomerId,
            CreateAt = waitingTokenDTO.Createat,
            CustomerName = waitingTokenDTO.CustName,
            Email = waitingTokenDTO.EmailId,
            PhoneNumber = waitingTokenDTO.PhoneNumber,
            NoOfPersons = waitingTokenDTO.NoOfPerson,
            SectionId = waitingTokenDTO.SectionId,
        };
    }

    public static WaitingTokenDTO WaitingTokenViewModelToDTO(waitingTokenVM waitingTokenVM)
    {
        if (waitingTokenVM == null) return null!;

        return new WaitingTokenDTO
        {
            TokenId = waitingTokenVM.TokenId,
            CustomerId = waitingTokenVM.CustomerId,
            Createat = waitingTokenVM.CreateAt ?? DateTime.Now, 
            CustName = waitingTokenVM.CustomerName,
            EmailId = waitingTokenVM.Email,
            PhoneNumber = waitingTokenVM.PhoneNumber,
            NoOfPerson = waitingTokenVM.NoOfPersons,
            SectionId = waitingTokenVM.SectionId,
            TableIds = waitingTokenVM.TableIds?.ToList() ?? new List<int>(),
            Modifyat = DateTime.Now,
            Createby = waitingTokenVM.EditorId,
            Modifyby = waitingTokenVM.EditorId,
            IsActive = true,
        };
    }

    public static List<waitingTokenVM> WaitingTokensDTOListToViewModelList(IEnumerable<WaitingTokenDTO> waitingTokenDTOs)
    {
        if (waitingTokenDTOs == null) return new List<waitingTokenVM>();
        return waitingTokenDTOs.Select(WaitingTokenDTOToViewModel).ToList();
    }

    public static List<WaitingTokenDTO> WaitingTokensViewModelListToDTOList(IEnumerable<waitingTokenVM> waitingTokenVMs)
    {
        if (waitingTokenVMs == null) return new List<WaitingTokenDTO>();
        return waitingTokenVMs.Select(WaitingTokenViewModelToDTO).ToList();
    }
}
