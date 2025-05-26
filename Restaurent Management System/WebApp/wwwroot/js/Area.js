//#region Global Variables
const sectionDetails = {
  sectionId: 0,
  sectionName: "",
  Description: "",
  editorId: 0,
};
const tableDetails = {
  tableId: 0,
  tableName: "",
  capacity: 0,
  sectionId: 0,
  status: "Available",
  editorId: 0,
};
const editorId = $("#editorIdAtTableAndSections").val();
// const sectionDataScript = $("#sectionDataScript");
// const tableDataScript = $("#tableDataScript");
let sectionId = 0;

//#endregion

//#region Helper Functions
function highlightSection(id) {
  // Remove highlight from all categories
  $(".section-area").removeClass("active-category");
  $(".showBtnsAtSections").addClass("d-none");

  // Add highlight to the selected section
  $(`#section-${id}`).addClass("active-category");
  $(`#sectionBtns-${id}`).removeClass("d-none");
  sectionId = id;
}
function highlightFirstSection() {
  let firstSection = $("#sectionList .section-area:first");
  if (firstSection.length) {
    let sectionId = firstSection.data("section-id");
    getTablesbySectionId(sectionId);
  }
}
function clearSearchField() {
  $("#searchforTablesList").val("");
  paginationModelForAreaPage.searchQuery = "";
}
/**
 * error - The error status object
 * modalId - The ID of the modal to hide
 */
function handleAjaxError(error, modalId) {
  if (error.status === 403) {
    showToaster("You don't have permission for this functionality", "Error");
  } else {
    showToaster("An error occurred while updating section.", "Error");
  }
  $(`#${modalId}`).modal("hide");
}
/**
 * response - The success response object
 * containerId - The ID of the container to update
 * modalId - The ID of the modal to hide
 */
function handleAjaxSuccess(response, containerId, modalId) {
  showToaster(response.message, response.status);
  $(`#${containerId}`).html(response.partialView);
  $(`#${modalId}`).modal("hide");
}
//#endregion

//#region Section

/*-------------------------------------Add Section ---------------------------------- */
$(document).on("click", "#addSectionBtn", function (event) {
  event.preventDefault();
  const form = $("#AddSectionForm");
  if (!form.valid()) {
    return;
  }
  sectionDetails.sectionName = $("#sectionName").val()?.trim();
  sectionDetails.Description = $("#sectionDescription").val()?.trim() || null;
  sectionDetails.sectionId = 0;
  sectionDetails.editorId = editorId;

  $.ajax({
    url: "/SectionAndTables/AddSection",
    type: "POST",
    data: { section: sectionDetails },
    success: (response) => handleAjaxSuccess(response, "sectionList", "addSectionModal"),
    error: (error) => handleAjaxError(error, "addSectionModal"),

  });
});

/*-------------------------------------Edit Section ---------------------------------- */
$(document).on("click", "#updateSectionBtn", function (event) {
  event.preventDefault();
  const form = $("#EditSectionForm");
  if (!form.valid()) {
    return;
  }
  sectionDetails.sectionId = $("#sectionIdForEdit").val();
  sectionDetails.sectionName = $("#EditSectionName").val()?.trim();
  sectionDetails.Description =
    $("#EditSectionDescription").val()?.trim() || null;
  sectionDetails.editorId = editorId;

  $.ajax({
    url: "/SectionAndTables/EditSection",
    type: "POST",
    data: { section: sectionDetails },
    success:  (response) => handleAjaxSuccess(response, "sectionList", "EditSectionModal"),
    error: (error) => handleAjaxError(error, "EditSectionModal"),
  });
});

/*-------------------------------------Delete Section ---------------------------------- */
$(document).on("click", "#deleteSectionBtn", function (event) {
  event.preventDefault();
  sectionDetails.sectionId = $("#sectionIdForDelete").val();
  sectionDetails.editorId = editorId;
  $.ajax({
    url: "/SectionAndTables/DeleteSection",
    type: "POST",
    data: { section: sectionDetails },
    success: function (response) {
      highlightFirstSection();
      handleAjaxSuccess(response, "sectionList", "deleteSectionModal");
    },
    error: (error) => handleAjaxError(error, "deleteSectionModal"),
  });
});
//#endregion

//#region  Tables
/*---------------------------------------Get Tables by Section Id ---------------------------------- */
function getTablesbySectionId(sectionId) {
  highlightSection(sectionId);
  $.ajax({
    url: "/SectionAndTables/GetTables",
    type: "POST",
    data: {
      sectionId: sectionId,
      paginationDetails: paginationModelForAreaPage,
    }, 
    success: function (response) {
      $("#AreaTables").html(response.partialView);
      updatePaginationInfoForAreaPage(response.pagination);
      // showToaster(response.message, response.status);
    },
    error: (error) => handleAjaxError(error, "addTableModal"),
  });
}

/*-------------------------------------Add New Table ---------------------------------- */
$(document).on("click", "#saveNewTableBtn", function (event) {
  event.preventDefault();
  const form = $("#AddTableForm");
  if (!form.valid()) {
    return;
  }
  tableDetails.tableName = $("#tableNameForAddNewTable").val()?.trim();
  tableDetails.capacity = $("#tableCapacityForNewTable").val();
  tableDetails.sectionId = $("#sectionIdForNewTable").val();
  tableDetails.editorId = editorId;
  tableDetails.status = $("#tableStatusForNewTable").val()?.trim();


  $.ajax({
    url: "/SectionAndTables/AddNewTable",
    type: "POST",
    data: { table: tableDetails },
    success: function (response) {
      updatePaginationInfoForAreaPage(response.pagination);
      handleAjaxSuccess(response, "AreaTables", "addTableModal");
    },
    error: (error) => handleAjaxError(error, "addTableModal"),
  });
});
/*-------------------------------------Edit Table ---------------------------------- */
$(document).on("click", "#updateTableBtn", function (event) {
  event.preventDefault();
  const form = $("#EditTableForm");
  if (!form.valid()) {
    return;
  }
  tableDetails.tableId = $("#tableIdForUpdate").val();
  tableDetails.tableName = $("#EditTableName").val()?.trim();
  tableDetails.capacity = $("#EditTableCapacity").val();
  tableDetails.sectionId = $("#sectionIdForUpdateTable").val();
  tableDetails.editorId = editorId;
  tableDetails.status = $("#tableStatusForUpdateTable").val()?.trim();

  $.ajax({
    url: "/SectionAndTables/UpdateTable",
    type: "POST",
    data: { table: tableDetails },
    success: function (response) {
      updatePaginationInfoForAreaPage(response.pagination);
      clearSearchField();
      handleAjaxSuccess(response, "AreaTables", "editTableModal");
    },
    error: (error) => handleAjaxError(error, "editTableModal"),
  });
});
/*-------------------------------------Delete Table ---------------------------------- */
$(document).on("click", "#deleteTableBtn", function () {
  tableDetails.tableId = $("#tableIdForDelete").val();
  tableDetails.tableName = "";
  tableDetails.capacity = 0;
  tableDetails.sectionId = 0;
  tableDetails.editorId = editorId;
  tableDetails.status = "";

  $.ajax({
    url: "/SectionAndTables/DeleteTable",
    type: "POST",
    data: { table: tableDetails },
    success: function (response) {
      updatePaginationInfoForAreaPage(response.pagination);
      clearSearchField();
      handleAjaxSuccess(response, "AreaTables", "deleteTableModal");
    },
    error: (error) => handleAjaxError(error, "deleteTableModal"),
  });
});
/*-------------------------------------Mass Delete Table ---------------------------------- */
$(document).on("click", "#deleteSelectedTables", function () {
  let selectedIds = [];
  $(".row-checkbox:checked").each(function () {
    selectedIds.push($(this).data("table-id"));
  });
  if (selectedIds.length > 0) {
    $.ajax({
      url: "/SectionAndTables/MassDeleteTable",
      type: "POST",
      data: { tableIds: selectedIds, editorId: editorId },
      success: function (response) {
        showToaster(response.message, response.status);
        $("#AreaTables").html(response.partialView);
        updatePaginationInfoForAreaPage(response.pagination);
        clearSearchField();
        $("#massDeleteTableModal").modal("hide");
      },
      error: function (status) {
        if (status.status == 403)
          showToaster(
            "You don't have permission for this functionality",
            "Error"
          );
        else showToaster("An error occurred while updating section.", "Error");
        $("#massDeleteTableModal").modal("hide");
      },
    });
  } else {
    showToaster("Please select at least one table to delete.", "Not Found");
    $("#massDeleteTableModal").modal("hide");
  }
});
//#endregion

//#region Table Pagination
const paginationModelForAreaPage = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "id",
  sortOrder: "asc", 
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
  tableStatus: "",
  dateRange: "",
};

function updatePaginationInfoForAreaPage(pagination) {
  paginationModelForAreaPage.totalRecords = pagination.totalRecords;

  $("#paginationInfoForAreaPage").text(
    `Showing ${
      (paginationModelForAreaPage.pageNumber - 1) *
        paginationModelForAreaPage.pageSize +
      1
    } - ${Math.min(
      paginationModelForAreaPage.pageNumber *
        paginationModelForAreaPage.pageSize,
      paginationModelForAreaPage.totalRecords
    )} of ${paginationModelForAreaPage.totalRecords}`
  );
}

function fetchTables() {
  //console.log("Here in fetchTables............");
  let sectionId =
    $("#sectionList .section-area.active-category").data("section-id") || null;
  //console.log(sectionId);
  getTablesbySectionId(sectionId);
}

// Search Functionality
$(document).on("keyup", "#searchforTablesList", function () {
  paginationModelForAreaPage.searchQuery = $(this).val();
  paginationModelForAreaPage.pageNumber = 1;
  fetchTables();
});

// Change Items Per Page
$(document).on("change", "#TablePerPage", function () {
  paginationModelForAreaPage.pageSize = $(this).val();
  paginationModelForAreaPage.pageNumber = 1;
  fetchTables();
});

// Pagination
$(document).on("click", "#prevPageforTablesList", function () {
  if (paginationModelForAreaPage.pageNumber > 1) {
    paginationModelForAreaPage.pageNumber--;
    fetchTables();
  }
});
$(document).on("click", "#nextPageforTablesList", function () {
  if (
    paginationModelForAreaPage.pageNumber *
      paginationModelForAreaPage.pageSize <
    paginationModelForAreaPage.totalRecords
  ) {
    paginationModelForAreaPage.pageNumber++;
    fetchTables();
  }
});

//#endregion

//#region  Fill Data To Process

/*-------------------------------------Fill data fields for Edit & Delete Section ---------------------------------- */
$("#sectionList").on("click", ".editSectionBtn", function () {
  let sections = JSON.parse(sectionDataScript.textContent || "{}");

  const sectionId = $(this).attr("data-section-id");
  const sectionData = sections.find((section) => section.sectionId == sectionId);

  $("#sectionIdForEdit").val(sectionData.sectionId);
  $("#EditSectionName").val(sectionData.sectionName);
  $("#EditSectionDescription").val(sectionData.description);
});
$("#sectionList").on("click", ".deleteSectionBtn", function () {
  const sectionId = $(this).attr("data-section-id");
  $("#sectionIdForDelete").val(sectionId);
});

/*-------------------------------------Fill data fields for Edit & Delete Table ---------------------------------- */

$("#AreaTables").on("click", ".editTableBtn", function () {
  const tables = JSON.parse(tableDataScript.textContent || "{}");

  const tableId = $(this).attr("data-table-id");
  const tableData = tables.find((table) => table.tableId == tableId);

  $("#tableIdForUpdate").val(tableData.tableId);
  $("#EditTableName").val(tableData.tableName);
  $("#EditTableCapacity").val(tableData.capacity);
  $("#sectionIdForUpdateTable").val(tableData.sectionId);
  $("#tableStatusForUpdateTable").val(tableData.status);
});
$("#AreaTables").on("click", ".deleteTableBtn", function () {
  const tableId = $(this).attr("data-table-id");
  $("#tableIdForDelete").val(tableId);
});


//#endregion


$(document).ready(function () {
  paginationModelForAreaPage.totalRecords = $(
    "#AreaTables .selectable-row"
  ).length;
  updatePaginationInfoForAreaPage(paginationModelForAreaPage);
  highlightFirstSection();

  $("#addSectionModal").on("hidden.bs.modal", function () {
    const form = $("#AddSectionForm");
    clearFormAtHide(form);
  });
  $("#EditSectionModal").on("hidden.bs.modal", function () {
    const form = $("#EditSectionForm");
    clearFormAtHide(form);
  });
  $("#addTableModal").on("hidden.bs.modal", function () {
    const form = $("#AddTableForm");
    clearFormAtHide(form);
  });
  $("#editTableModal").on("hidden.bs.modal", function () {
    const form = $("#EditTableForm");
    clearFormAtHide(form);
  });

  $("#addTableModal").on("shown.bs.modal", function () {
    $("#sectionIdForNewTable").val(sectionId);
  });
});
