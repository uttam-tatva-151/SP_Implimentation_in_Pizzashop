using Microsoft.EntityFrameworkCore;
using PMSCore.Beans;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class TableRepo : ITableRepo
    {

        private readonly AppDbContext _appDbContext;
        public TableRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();
        public async Task<ResponseResult> AddTableAsync(Table newTable)
        {
            try
            {
                _appDbContext.Tables.Add(newTable);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.TABLE);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateTableAsync(Table updateTable)
        {
            try
            {
                _appDbContext.Tables.Update(updateTable);
                await _appDbContext.SaveChangesAsync();
                result.Message =  MessageHelper.GetSuccessMessageForUpdateOperation(Constants.TABLE);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<List<Table>> GetTablesBySectionId(int id, PaginationDetails paginationDetails)
        {
            try
            {
                IQueryable<Table> query = _appDbContext.Tables
                                 .Where(a => a.SectionId == id && a.Iscontinued == true);
                if (!string.IsNullOrEmpty(paginationDetails.SearchQuery))
                {
                    query = query.Where(o => o.TableName.Contains(paginationDetails.SearchQuery));
                }
                paginationDetails.TotalRecords = await query.CountAsync();
                return await query.Skip((paginationDetails.PageNumber - 1) * paginationDetails.PageSize)
                                .Take(paginationDetails.PageSize)
                                     .ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<Table?> GetTableAsync(int tableId)
        {
            try
            {
                return await _appDbContext.Tables.Where(t => t.TableId == tableId && t.Iscontinued==true).FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<ResponseResult> MassUpdateTablesAsync(List<Table> tableList)
        {
            try
            {
                foreach (Table table in tableList)
                {
                    _appDbContext.Tables.Update(table);
                }
                await _appDbContext.SaveChangesAsync();
                result.Message =  MessageHelper.GetSuccessMessageForUpdateOperation(Constants.TABLE);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<List<Table>> GetTableListBySectionIdAsync(int sectionId)
        {
            return await _appDbContext.Tables.Where(s => s.SectionId == sectionId && s.Iscontinued == true).ToListAsync();
        }

        public async Task<List<Table>> GetTableListFromTableIdsAsync(int[] tableIds)
        {
            return await _appDbContext.Tables.Where(table => tableIds.Contains(table.TableId)).ToListAsync();
        }
    }
}
