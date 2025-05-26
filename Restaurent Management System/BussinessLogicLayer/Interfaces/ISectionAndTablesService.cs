using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;

namespace PMSServices.Interfaces
{
    public interface ISectionAndTablesService
    {
        Task<ResponseResult> GetDefaultAreaDeatils(PaginationDetails paginationDetails);
        Task<ResponseResult> GetTables(int sectionId, PaginationDetails paginationDetails);
        Task<ResponseResult> AddSection(SectionDetails section);
        Task<ResponseResult> EditSection(SectionDetails section);
        Task<ResponseResult> DeleteSection(int sectionId,int editorId);
        Task<ResponseResult> AddTable(TableDetails newTable);
        Task<ResponseResult> MassDeleteTableAsync(int[] tableIds, int editorId);
        Task<ResponseResult> DeleteTable(int tableId,int editorId);
        Task<ResponseResult> UpdateTable(TableDetails updateTable);
    }
}
