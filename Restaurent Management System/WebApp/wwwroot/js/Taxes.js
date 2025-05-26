const taxDetails = {
  TaxId : 0,
  TaxName : "",
  TaxType : "",
  Isenabled : true,
  Isdefault : true,
  TaxValue : 0,
  EditorId : 0,
}

const debouncedFetchTax = debounce(fetchTax, 300);
const editorId =$("#editorIdAtTaxPage").val();
function AssignValueForDeleteTax(id) {
  document.getElementById("taxIdForDelete").value = id;
}

function AssignValueForEditTax(taxId, name, type, isEnabled, isDefault, rate,editorId) {
  $("#taxIdForEdit").val(taxId);
  $("#UpdateTaxNameForEdit").val(name);
  $("#UpdateTaxTypeForEdit").val(type);
  $("#RateForEdit").val(rate);
  $("#checkBoxForIsEnabledTaxForEdit").prop(
    "checked",
    isEnabled == "True" ? true : false
  ); // Convert to boolean
  $("#checkBoxForDefaultTaxForEdit").prop(
    "checked",
    isDefault == "True" ? true : false
  );
  $("#editorIdForEdit").val(editorId);
  $("#EditTaxModal").modal("show"); // Show the modal
}

const paginationModelForTaxes = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "id",
  sortOrder: "asc",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
  orderStatus: "",
  dateRange: "",
};
function updatePaginationInfoForTaxes(pagination) {
  paginationModelForTaxes.totalRecords = pagination.totalRecords;
  $("#paginationInfoForTaxesPage").text(
    `Showing ${
      (pagination.pageNumber - 1) * pagination.pageSize + 1
    } - ${Math.min(
      pagination.pageNumber * pagination.pageSize,
      pagination.totalRecords
    )} of ${pagination.totalRecords}`
  );
}
function fetchTax() {
  $.ajax({
    url: "/Taxes/Taxes",
    type: "POST",
    data: paginationModelForTaxes,
    success: function (response) {
      $("#taxesList").html(response.partialView);
      updatePaginationInfoForTaxes(response.paginationDetails);
    },
    error: function () {
      showToaster('An Error occurs in ajax call','Error')
    },
  });
}

// Search Functionality
$(document).on("input", "#SearchTax", function () {
  let searchValue = $(this).val().trim();
  if (searchValue.length < 100 && searchValue.length >3) {
  paginationModelForTaxes.searchQuery = searchValue;
  paginationModelForTaxes.pageNumber = 1;
  debouncedFetchTax();
  }else
  if($(this).val().length == 0){
    paginationModelForTaxes.searchQuery = "";
    paginationModelForTaxes.pageNumber = 1;
    debouncedFetchTax();
  }
});

// Change Items Per Page
$(document).on("change", "#TaxesPerPage", function () {
  paginationModelForTaxes.pageSize = $(this).val();
  paginationModelForTaxes.pageNumber = 1;
  debouncedFetchTax();
});

// Pagination
$(document).on("click", "#prevPageForTaxes", function () {
  if (paginationModelForTaxes.pageNumber > 1) {
    paginationModelForTaxes.pageNumber--;
    debouncedFetchTax();
  }
});
$(document).on("click", "#nextPageForTaxes", function () {
  if (
    paginationModelForTaxes.pageNumber * paginationModelForTaxes.pageSize <
    paginationModelForTaxes.totalRecords
  ) {
    paginationModelForTaxes.pageNumber++;
    debouncedFetchTax();
  }
});

$(document).ready(function () {
  paginationModelForTaxes.totalRecords = $("#totalRecordsForTaxPage").val();
  updatePaginationInfoForTaxes(paginationModelForTaxes);

  $("#AddTaxModal").on("hidden.bs.modal", function () {
    let form = $("#AddNewTaxForm");
    clearFormAtHide(form);
  });
  $("#EditTaxModal").on("hidden.bs.modal", function () {
    let form = $("#EditTaxForm");
    clearFormAtHide(form);
  });


});


//#region Taxes CRUD 

    /* ------------------------------- Add New Tax ------------------------------ */
    $(document).on("click", "#saveNewTaxes", function (event) {
      event.preventDefault();
      let form = $("#AddNewTaxForm");
      if (!form.valid()) {
        return;
      }
      taxDetails.TaxValue = $("#newTaxRate").val();
      taxDetails.TaxName = $("#newTaxName").val()?.trim();
      taxDetails.TaxType = $("#newTaxType").val();
      taxDetails.EditorId = editorId;
      taxDetails.Isenabled = $("#checkBoxForIsEnabledForNewTax").is(":checked");
      taxDetails.Isdefault = $("#checkBoxForDefaultNewTax").is(":checked");

      $.ajax({
        url: "/Taxes/AddNewTax",
        type: "POST",
        data: { taxDetails: taxDetails },
        success: function (response) {
          showToaster(response.message, response.status);
          paginationModelForTaxes.pageNumber = 1;
          paginationModelForTaxes.pageSize = 5;
          $("#taxesList").html(response.partialView);
          updatePaginationInfoForTaxes(response.pagination);
          $("#AddTaxModal").modal('hide');
        },
        error: function () {
          showToaster("An error occurred while Adding Taxes.", "Error");
          $("#AddTaxModal").modal('hide');
        },
      });
    });

    /* ------------------------------- Edit New Tax ------------------------------ */
    $(document).on("click", "#updateTaxesBtn", function (event) {
      event.preventDefault();
      let form = $("#EditTaxForm");
      if (!form.valid()) {
        return;
      }
      taxDetails.TaxId = $("#taxIdForEdit").val();
      taxDetails.TaxValue = $("#RateForEdit").val();
      taxDetails.TaxName = $("#UpdateTaxNameForEdit").val()?.trim();
      taxDetails.TaxType = $("#UpdateTaxTypeForEdit").val();
      taxDetails.EditorId = editorId;
      taxDetails.Isenabled = $("#checkBoxForIsEnabledTaxForEdit").is(":checked");
      taxDetails.Isdefault = $("#checkBoxForDefaultTaxForEdit").is(":checked");

      $.ajax({
        url: "/Taxes/UpdateTax",
        type: "POST",
        data: { taxDetails: taxDetails },
        success: function (response) {
          showToaster(response.message, response.status);
          $("#taxesList").html(response.partialView);
          paginationModelForTaxes.pageNumber = 1;
          paginationModelForTaxes.pageSize = 5;
          updatePaginationInfoForTaxes(response.pagination);
          $("#EditTaxModal").modal('hide');
        },
        error: function () {
          showToaster("An error occurred while Adding Taxes.", "Error");
          $("#EditTaxModal").modal('hide');
        },
      });
    });

    /* ------------------------------- Add New Tax ------------------------------ */
    $(document).on("click", "#deleteTaxBtn", function () {
     
      taxDetails.TaxId = $("#taxIdForDelete").val();
      taxDetails.TaxValue = 0;
      taxDetails.TaxName = "";
      taxDetails.TaxType = "";
      taxDetails.EditorId = editorId;
      taxDetails.Isenabled = true;
      taxDetails.Isdefault = true;

      $.ajax({
        url: "/Taxes/DeleteTax",
        type: "POST",
        data: { taxDetails: taxDetails },
        success: function (response) {
          showToaster(response.message, response.status);
          $("#taxesList").html(response.partialView);
          updatePaginationInfoForTaxes(response.pagination);
          $("#DeleteModalForTax").modal('hide');
        },
        error: function () {
          showToaster("An error occurred while Adding Taxes.", "Error");
          $("#DeleteModalForTax").modal('hide');
        },
      });
    });

//#endregion


