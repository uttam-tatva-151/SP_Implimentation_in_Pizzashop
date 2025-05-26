using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;

namespace PMSServices.Services
{
    public class SectionAndTablesService : ISectionAndTablesService
    {
        private readonly ISectionRepo _sectionRepo;
        private readonly ITableRepo _tableRepo;
        private readonly ICommonServices _commonServices;

        public SectionAndTablesService(ISectionRepo sectionRepo, ITableRepo tableRepo, ICommonServices commonServices)
        {
            _sectionRepo = sectionRepo;
            _tableRepo = tableRepo;
            _commonServices = commonServices;
        }

        ResponseResult result = new();



        public async Task<ResponseResult> GetDefaultAreaDeatils(PaginationDetails paginationDetails)
        {
            try
            {
                List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                List<Table> tables = await _tableRepo.GetTablesBySectionId(sections[0].SectionId, paginationDetails);
                List<SectionDetails> sectionList = ConvertSectionToSectionDetailsViewModel(sections);
                List<TableDetails> tableList = ConvertTablesToTableDetailsViewModel(tables);
                AreaDetails defaultArea = new()
                {
                    sections = sectionList,
                    tables = tableList,
                };
                if (defaultArea != null)
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.AREA);
                    result.Status = ResponseStatus.Success;
                    result.Data = defaultArea;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        #region  CRUD for Section
        public async Task<ResponseResult> AddSection(SectionDetails section)
        {
            try
            {
                Section newSection = new()
                {
                    SectionName = section.SectionName,
                    Description = section.Description,
                    Createdat = DateTime.Now,
                    Createdby = section.editorId,
                    Isdeleted = false
                };
                result = await _sectionRepo.AddSectionAsync(newSection);
                if (result.Status == ResponseStatus.Success)
                {
                    List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                    if (sections != null)
                    {
                        List<SectionDetails> sectionList = ConvertSectionToSectionDetailsViewModel(sections);
                        result.Data = sectionList;
                        result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.SECTION);
                        result.Status = ResponseStatus.Success;
                    }
                    else
                    {
                        result.Message = MessageHelper.GetErrorMessageForAddOperation(Constants.SECTION);
                        result.Status = ResponseStatus.NotFound;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        public async Task<ResponseResult> DeleteSection(int sectionId, int editorId)
        {
            try
            {
                Section? existingSection = await _sectionRepo.GetSectionAsync(sectionId);
                if (existingSection != null)
                {
                    existingSection.Modifiedby = editorId;
                    existingSection.Modifiedat = DateTime.Now;
                    existingSection.Isdeleted = true;
                    result = await _sectionRepo.UpdateSectionAsync(existingSection);

                    if (result.Status == ResponseStatus.Success)
                    {
                        result = await DeleteTablesBySectionId(sectionId, editorId);
                        if (result.Status == ResponseStatus.Success)
                        {
                            result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.SECTION);
                        }
                        else
                        {
                            result.Message = MessageHelper.GetErrorMessageForDeleteOperation(Constants.SECTION);
                            result.Status = ResponseStatus.Error;
                        }
                        List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                        List<SectionDetails> sectionList = ConvertSectionToSectionDetailsViewModel(sections);
                        result.Data = sectionList;
                    }
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.SECTION);
                    result.Status = ResponseStatus.NotFound;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        private async Task<ResponseResult> DeleteTablesBySectionId(int sectionId, int editorId)
        {
            List<Table> tables = await _tableRepo.GetTableListBySectionIdAsync(sectionId);
            foreach (Table table in tables)
            {
                table.Modifyat = DateTime.Now;
                table.Modifyby = editorId;
                table.Iscontinued = false;
            }
            result = await _tableRepo.MassUpdateTablesAsync(tables);
            return result;
        }

        public async Task<ResponseResult> EditSection(SectionDetails section)
        {
            try
            {
                Section? existingSection = await _sectionRepo.GetSectionAsync(section.SectionId);
                if (existingSection != null)
                {
                    existingSection.SectionName = section.SectionName;
                    existingSection.Description = section.Description;
                    existingSection.Modifiedby = section.editorId;
                    existingSection.Modifiedat = DateTime.Now;
                    result = await _sectionRepo.UpdateSectionAsync(existingSection);
                    if (result.Status == ResponseStatus.Success)
                    {
                        List<Section> sections = await _sectionRepo.GetAllSectonsAsync();
                        if (sections != null)
                        {
                            List<SectionDetails> sectionList = ConvertSectionToSectionDetailsViewModel(sections);
                            result.Data = sectionList;
                            result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.SECTION);
                            result.Status = ResponseStatus.Success;
                        }
                        else
                        {
                            result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.SECTION);
                            result.Status = ResponseStatus.NotFound;
                        }
                    }
                }else{
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.SECTION);
                            result.Status = ResponseStatus.Error;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }

        #endregion

        #region  CRUD for Tables

        public async Task<ResponseResult> GetTables(int sectionId, PaginationDetails paginationDetails)
        {
            try
            {
                List<Table> tableList = await _tableRepo.GetTablesBySectionId(sectionId, paginationDetails);
                List<TableDetails> Tables = ConvertTablesToTableDetailsViewModel(tableList);
                if (Tables.Count > 0)
                {
                    result.Message = MessageHelper.GetSuccessMessageForReadOperation(Constants.TABLE);
                    result.Status = ResponseStatus.Success;
                    result.Data = Tables;
                }
                else
                {
                    result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.TABLE);
                    result.Status = ResponseStatus.NotFound;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> AddTable(TableDetails newTable)
        {
            try
            {
                Table table = new()
                {
                    TableName = newTable.TableName,
                    SectionId = newTable.SectionId,
                    Capacity = newTable.Capacity,
                    Status = newTable.Status,
                    Createby = newTable.editorId,
                    Createat = DateTime.Now,
                    Iscontinued = true
                };
                result = await _tableRepo.AddTableAsync(table);

                if (result.Status == ResponseStatus.Success)
                {
                    PaginationDetails paginationDetails = new()
                    {
                        PageSize = 2
                    };
                    List<Table> tables = await _tableRepo.GetTablesBySectionId(newTable.SectionId, paginationDetails);
                    if (tables == null)
                    {
                        result.Message = MessageHelper.GetErrorMessageForAddOperation(Constants.TABLE);
                        result.Status = ResponseStatus.NotFound;
                    }
                    else
                    {
                        List<TableDetails> tableList = ConvertTablesToTableDetailsViewModel(tables);
                        result.Message = MessageHelper.GetSuccessMessageForAddOperation(Constants.TABLE);
                        result.Status = ResponseStatus.Success;
                        result.Data = (tableList, paginationDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> DeleteTable(int tableId, int editorId)
        {
            try
            {
                Table? table = await _tableRepo.GetTableAsync(tableId);
                if(table == null){
                    result.Message = MessageHelper.GetNotFoundMessage(Constants.TABLE);
                    result.Status = ResponseStatus.NotFound;
                    return result; 
                }
                table.Modifyby = editorId;
                table.Modifyat = DateTime.Now;
                table.Iscontinued = false;
                result = await _tableRepo.UpdateTableAsync(table);

                if (result.Status == ResponseStatus.Success)
                {
                    PaginationDetails paginationDetails = new()
                    {
                        PageSize = 2
                    };
                    List<Table> tables = await _tableRepo.GetTablesBySectionId(table.SectionId, paginationDetails);
                    if (tables == null)
                    {
                        result.Message = MessageHelper.GetErrorMessageForDeleteOperation(Constants.TABLE);
                        result.Status = ResponseStatus.NotFound;
                    }
                    else
                    {
                        List<TableDetails> tableList = ConvertTablesToTableDetailsViewModel(tables);
                        result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.TABLE);
                        result.Status = ResponseStatus.Success;
                        result.Data = (tableList, paginationDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> UpdateTable(TableDetails updateTable)
        {
            try
            {
                Table? table = await _tableRepo.GetTableAsync(updateTable.TableId);
                if(table == null){
                    result.Message = MessageHelper.GetNotFoundMessage(Constants.TABLE);
                    result.Status = ResponseStatus.NotFound;
                    return result; 
                }
                table.TableName = updateTable.TableName;
                table.SectionId = updateTable.SectionId;
                table.Capacity = updateTable.Capacity;
                table.Status = updateTable.Status;
                table.Modifyby = updateTable.editorId;
                table.Modifyat = DateTime.Now;
                result = await _tableRepo.UpdateTableAsync(table);

                if (result.Status == ResponseStatus.Success)
                {
                    PaginationDetails paginationDetails = new()
                    {
                        PageSize = 2
                    };
                    List<Table> tables = await _tableRepo.GetTablesBySectionId(updateTable.SectionId, paginationDetails);
                    if (tables == null)
                    {
                        result.Message = MessageHelper.GetErrorMessageForUpdateOperation(Constants.TABLE);
                        result.Status = ResponseStatus.NotFound;
                    }
                    else
                    {
                        List<TableDetails> tableList = ConvertTablesToTableDetailsViewModel(tables);
                        result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.TABLE);
                        result.Status = ResponseStatus.Success;
                        result.Data = (tableList, paginationDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> MassDeleteTableAsync(int[] tableIds, int editorId)
        {
            try
            {
                List<Table> tables = await _tableRepo.GetTableListFromTableIdsAsync(tableIds);
                foreach (Table table in tables)
                {
                    table.Modifyat = DateTime.Now;
                    table.Modifyby = editorId;
                    table.Iscontinued = false;
                }
                result = await _tableRepo.MassUpdateTablesAsync(tables);
                if (result.Status == ResponseStatus.Success)
                {

                    PaginationDetails paginationDetails = new()
                    {
                        PageSize = 2
                    };
                    List<Table> updatedtables = await _tableRepo.GetTablesBySectionId(tables[0].SectionId, paginationDetails);
                    if (tables == null)
                    {
                        result.Message = MessageHelper.GetErrorMessageForDeleteOperation(Constants.TABLE);
                        result.Status = ResponseStatus.NotFound;
                    }
                    else
                    {
                        List<TableDetails> tableList = ConvertTablesToTableDetailsViewModel(tables);
                        result.Message = MessageHelper.GetSuccessMessageForDeleteOperation(Constants.TABLE);
                        result.Status = ResponseStatus.Success;
                        result.Data = (tableList, paginationDetails);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        #endregion

        #region Convert Entity to ViewModel 
        private static List<TableDetails> ConvertTablesToTableDetailsViewModel(List<Table> tables)
        {
            List<TableDetails> tableDetails = new();
            foreach (Table table in tables)
            {
                TableDetails temp = new()
                {
                    TableId = table.TableId,
                    TableName = table.TableName,
                    SectionId = table.SectionId,
                    Capacity = table.Capacity
                };
                temp.TableName = table.TableName;
                temp.Status = table.Status;
                tableDetails.Add(temp);
            }
            return tableDetails;
        }

        private static List<SectionDetails> ConvertSectionToSectionDetailsViewModel(List<Section> sections)
        {
            List<SectionDetails> sectiondetails = new();
            foreach (Section section in sections)
            {
                SectionDetails temp = new()
                {
                    SectionId = section.SectionId,
                    SectionName = section.SectionName,
                    Description = section.Description
                };
                sectiondetails.Add(temp);
            }
            return sectiondetails;
        }

        #endregion

    }
}
