using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PMSCore.Beans;
using PMSCore.ViewModel;
using PMSData;
using PMSServices.Interfaces;
using PMSWebApp.Attributes;
using PMSWebApp.Extensions;
using PMSWebApp.Utilities;

namespace PMSWebApp.Controllers
{
    public class SectionAndTablesController : Controller
    {
        private readonly ISectionAndTablesService _sectionAndTablesService;
        public SectionAndTablesController(ISectionAndTablesService sectionAndTablesService)
        {
            _sectionAndTablesService = sectionAndTablesService;
        }
        ResponseResult result = new();


        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> SectionAndTables(PaginationDetails paginationDetails)
        {
            try
            {
                paginationDetails.PageSize = 2;
                result = await _sectionAndTablesService.GetDefaultAreaDeatils(paginationDetails);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            AreaDetails areaDetails = (AreaDetails)result.Data;
            ToasterHelper.SetToastMessage(TempData, result.Message, result.Status);
            TempData[Constants.LAYOUT_VARIABLE_NAME] = Constants.MAIN_LAYOUT;
            return View(areaDetails);
        }

        #region Section

        [HttpPost]
        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> AddSection(SectionDetails section)
        {
            try
            {
                result = await _sectionAndTablesService.AddSection(section);
                List<SectionDetails> sectionList = result.Data as List<SectionDetails> ?? new List<SectionDetails>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_SECTION_LIST_GRID, sectionList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        [HttpPost]
        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteSection(SectionDetails section)
        {
            try
            {
                result = await _sectionAndTablesService.DeleteSection(section.SectionId, section.editorId);
                List<SectionDetails> sectionList = result.Data as List<SectionDetails> ?? new List<SectionDetails>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_SECTION_LIST_GRID, sectionList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }

        }
        [HttpPost]
        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> EditSection(SectionDetails section)
        {

            try
            {
                result = await _sectionAndTablesService.EditSection(section);
                List<SectionDetails> sectionList = result.Data as List<SectionDetails> ?? new List<SectionDetails>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_SECTION_LIST_GRID, sectionList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }

        #endregion

        #region Tables

        [HttpPost]
        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.VIEW_PERMISSION)]
        public async Task<IActionResult> GetTables(int sectionId, PaginationDetails paginationDetails)
        {
            try
            {
                result = await _sectionAndTablesService.GetTables(sectionId, paginationDetails);
                List<TableDetails> tableList = result.Data as List<TableDetails> ?? new List<TableDetails>();
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TABLES_LIST_GRID, tableList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = paginationDetails });

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        [HttpPost]
        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> AddNewTable(TableDetails table)
        {
            try
            {

                result = await _sectionAndTablesService.AddTable(table);
                (List<TableDetails> tableList, PaginationDetails pagination) = ((List<TableDetails>, PaginationDetails))result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TABLES_LIST_GRID, tableList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = pagination });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        [HttpPost]
        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> DeleteTable(TableDetails table)
        {
            try
            {
                result = await _sectionAndTablesService.DeleteTable(table.TableId, table.editorId);
                (List<TableDetails> tableList, PaginationDetails pagination) = ((List<TableDetails>, PaginationDetails))result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TABLES_LIST_GRID, tableList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = pagination });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        [HttpPost]
        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.CREATE_AND_EDIT_PERMISSION)]
        public async Task<IActionResult> UpdateTable(TableDetails table)
        {
            try
            {
                result = await _sectionAndTablesService.UpdateTable(table);
                (List<TableDetails> tableList, PaginationDetails pagination) = ((List<TableDetails>, PaginationDetails))result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TABLES_LIST_GRID, tableList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = pagination });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }
        }
        [HttpPost]
        [AuthorizePermission(Constants.TABLE_AND_SECTION_MODULE, Constants.DELETE_PERMISSION)]
        public async Task<IActionResult> MassDeleteTable(int[] tableIds, int editorId)
        {
            try
            {
                result = await _sectionAndTablesService.MassDeleteTableAsync(tableIds, editorId);
                (List<TableDetails> tableList, PaginationDetails pagination) = ((List<TableDetails>, PaginationDetails))result.Data;
                string partialView = await this.RenderPartialViewToString(Constants.PARTIAL_TABLES_LIST_GRID, tableList);
                return Json(new { partialView = partialView, message = result.Message, status = result.Status, pagination = pagination });
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
                return Json(new { message = result.Message, status = result.Status });
            }

        }

        #endregion

    }

}
