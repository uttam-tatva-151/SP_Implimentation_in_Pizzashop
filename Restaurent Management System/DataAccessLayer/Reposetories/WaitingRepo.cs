using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSData.Interfaces;

namespace PMSData.Reposetories
{
    public class WaitingRepo : IWaitingRepo
    {
        private readonly AppDbContext _appDbContext;

        public WaitingRepo(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        readonly ResponseResult result = new();
        public async Task<ResponseResult> AddWaitingTokenAsync(WaitingTokenDTO token)
        {
            try
            {
                DbConnection connection = _appDbContext.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                NpgsqlConnection npgsqlConn = (NpgsqlConnection)connection;

                // the command
                using var query = new NpgsqlCommand("CALL public.add_waiting_token(@token)", npgsqlConn);

                // the parameter of DTO
                query.Parameters.AddWithValue("token", token);

                // Execute the command
                await query.ExecuteNonQueryAsync();
                result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.WAITING_TOKEN);
                result.Status = ResponseStatus.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<List<WaitingTokenDTO>> GetWaitingTokensBySectionAsync(int sectionId)
        {
            DbConnection connection = _appDbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            string sql = "SELECT * FROM get_waiting_token_dto_by_section(@sectionId)";
            IEnumerable<WaitingTokenDTO> result = await Dapper.SqlMapper.QueryAsync<WaitingTokenDTO>(
                connection, sql, new { sectionId });

            return result.ToList();
        }
        public async Task<WaitingTokenDTO?> GetWaitingTokenByIdAsync(int tokenId)
        {
             DbConnection connection = _appDbContext.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            string sql = "SELECT * FROM get_waiting_token_dto_by_tokenid(@tokenId)";
            IEnumerable<WaitingTokenDTO> result = await Dapper.SqlMapper.QueryAsync<WaitingTokenDTO>(
                connection, sql, new { tokenId });

            return result.FirstOrDefault();
        }

        public async Task<ResponseResult> UpdateWaitingToken(WaitingTokenDTO waitingToken)
        {
            try
            {
                DbConnection connection = _appDbContext.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                    await connection.OpenAsync();

                NpgsqlConnection npgsqlConn = (NpgsqlConnection)connection;

                // the command
                using var query = new NpgsqlCommand("CALL public.update_waiting_token(@waiting_token)", npgsqlConn);

                // the parameter of DTO
                query.Parameters.AddWithValue("waiting_token", waitingToken);

                // Execute the command
                await query.ExecuteNonQueryAsync();
                result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.WAITING_TOKEN);
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
