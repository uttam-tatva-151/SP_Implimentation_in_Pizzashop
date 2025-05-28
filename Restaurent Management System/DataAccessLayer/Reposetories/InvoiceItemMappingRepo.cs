using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class InvoiceItemMappingRepo : IInvoiceItemMappingRepo
    {
        private readonly AppDbContext _appDbContext;
        public InvoiceItemMappingRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        readonly ResponseResult result = new();
        public async Task<List<InvoiceItemModifierMapping>> GetItemsForInvoiceAsync(int orderId)
        {
            return await _appDbContext.InvoiceItemModifierMappings.Include(i => i.Item).AsNoTracking().Where(i => i.OrderId == orderId).ToListAsync();
        }

        public async Task<ResponseResult> UpdateMultipleItemMappingsAsync(List<InvoiceItemModifierMapping> invoiceItemModifierMappings)
        {
            try
            {
                _appDbContext.InvoiceItemModifierMappings.UpdateRange(invoiceItemModifierMappings);
                await _appDbContext.SaveChangesAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateInvoiceItemModifierMappingsAsync(List<UpdateInvoiceItemModifierMappingDTO> mappings)
        {
            try
            {
                DbConnection connection = _appDbContext.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                NpgsqlConnection npgsqlConn = (NpgsqlConnection)connection;

                // the command
                using NpgsqlCommand query = new("CALL public.update_invoice_item_modifier_mappings(@updates)", npgsqlConn);

                // the parameter as an array of DTOs
                query.Parameters.AddWithValue("updates", mappings.ToArray());

                // Execute the command
                await query.ExecuteNonQueryAsync();

                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }


        public async Task<ResponseResult> AddInvoiceItemModifierMappingsAsync(List<AddInvoiceItemModifierMappingInputDTO> mappings)
        {
            try
            {
                DbConnection connection = _appDbContext.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                NpgsqlConnection npgsqlConn = (NpgsqlConnection)connection;

                // the command
                using NpgsqlCommand query = new("CALL public.add_invoice_item_modifier_mappings(@updates)", npgsqlConn);

                // the parameter as an array of DTOs
                query.Parameters.AddWithValue("updates", mappings.ToArray());

                // Execute the command
                await query.ExecuteNonQueryAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> DeleteInvoiceItemModifierMappingsAsync(List<UpdateInvoiceItemModifierMappingDTO> mappingsToDelete)
        {
            try
            {
                DbConnection connection = _appDbContext.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                NpgsqlConnection npgsqlConn = (NpgsqlConnection)connection;
                using NpgsqlCommand query = new("CALL public.delete_invoice_item_modifier_mappings(@deletes)", npgsqlConn);
                query.Parameters.AddWithValue("deletes", mappingsToDelete.ToArray());

                await query.ExecuteNonQueryAsync();

                result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.MAPPING_RELATIONS);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
    }
}
