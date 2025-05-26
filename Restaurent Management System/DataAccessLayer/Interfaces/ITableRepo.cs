using PMSCore.Beans;
using PMSCore.ViewModel;

namespace PMSData.Interfaces
{
    public interface ITableRepo
    {
        Task<ResponseResult> AddTableAsync(Table newTable);
        Task<List<Table>> GetTablesBySectionId(int id,PaginationDetails paginationDetails);
        Task<ResponseResult> UpdateTableAsync(Table updateTable);
        Task<Table?> GetTableAsync(int tableId);
        Task<ResponseResult> MassUpdateTablesAsync(List<Table> tableList);
        Task<List<Table>> GetTableListBySectionIdAsync(int sectionId);
        Task<List<Table>> GetTableListFromTableIdsAsync(int[] tableIds);
    }
}
