//#region Customer Pagination
let paginationModelForCustomerPage = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "name",
  sortOrder: "asc",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
  customerStatus: "",
  dateRange: "",
};
const debouncedFetchCustomer = debounce(fetchCustomers, 300);

function updatePaginationInfo(pagination) {
  paginationModelForCustomerPage.totalRecords = pagination.totalRecords;
  $("#totalRecordsForCustomerPage").val(pagination.totalRecords);
  $("#fromDateForCustomerPageHiddenValue").val(pagination.fromDate);
  $("#toDateForCustomerPageHiddenValue").val(pagination.toDate);
  $("#paginationInfoForCustomerPage").text(
    `Showing ${
      (pagination.pageNumber - 1) * pagination.pageSize + 1
    } - ${Math.min(
      pagination.pageNumber * pagination.pageSize,
      pagination.totalRecords
    )} of ${pagination.totalRecords}`
  );
}
function fetchCustomers() {
  $.ajax({
    url: "/Customers/Customer",
    type: "GET",
    contentType: "application/json",
    data: paginationModelForCustomerPage,
    beforeSend: function () {
      $("#customerList").css("opacity", "0.5"); // Show fade effect
    },
    success: function (response) {
      $("#customerList").html(response.partialView);
      $("#customerList").css("opacity", "1"); // Show fade effect
      updatePaginationInfo(response.paginationDetails);
    },
    error: function (error) {
      showToaster(error, "Error");
    },
  });
}
// Sorting
$(document).on("click", ".sortForCustomerList", function () {
  paginationModelForCustomerPage.sortColumn = $(this).data("sortby");
  paginationModelForCustomerPage.sortOrder =
    paginationModelForCustomerPage.sortOrder === "asc" ? "desc" : "asc";
  debouncedFetchCustomer();
});

// Search Functionality
$(document).on("input", "#SearchCustomer", function () {
  let searchValue = $(this).val().trim();
  if (searchValue.length <= 100) {
    paginationModelForCustomerPage.searchQuery = searchValue;
    paginationModelForCustomerPage.pageNumber = 1;
    debouncedFetchCustomer();
  }
  if ($(this).val() == 0) {
    paginationModelForCustomerPage.searchQuery = "";
    paginationModelForCustomerPage.pageNumber = 1;
    debouncedFetchCustomer();
  }
});

// Date Filter
$(document).on("click", "#searchDateRangeForCustomerPage", function () {
  paginationModelForCustomerPage.fromDate = $("#fromDateForCustomerPage").val();
  paginationModelForCustomerPage.toDate = $("#toDateForCustomerPage").val();
  paginationModelForCustomerPage.pageNumber = 1; // Reset to first page
  debouncedFetchCustomer();
  $("#customDateRangeModal").modal("hide");
});

$(document).on("change", "#dateRangeForCustomerPage", function () {
  if ($(this).val() === "CustomRange") {
    $("#customDateRangeBtnForCustomerPage")
      .removeAttr("disabled")
      .removeClass("opacity-50 pe-none d-none")
      .css("cursor", "pointer")
      .attr("data-bs-toggle", "modal")
      .attr("data-bs-target", "#customDateRangeModal")
      .show();
    $("#customDateRangeModal").modal("show");
    paginationModelForCustomerPage.dateRange = $(this).val();
  } else {
    paginationModelForCustomerPage.fromDate = "";
    paginationModelForCustomerPage.toDate = "";
    paginationModelForCustomerPage.dateRange = $(this).val();
    paginationModelForCustomerPage.pageNumber = 1; // Reset to first page

    $("#customDateRangeBtnForCustomerPage")
      .attr("disabled", true)
      .addClass("opacity-50 pe-none d-none")
      .css("cursor", "not-allowed")
      .removeAttr("data-bs-toggle")
      .removeAttr("data-bs-target")
      .hide();
    debouncedFetchCustomer();
  }
});
$(document).on("click", "#customDateRangeBtnForCustomerPage", function () {
  $("#customDateRangeModal").modal("show");
});

$(document).on("change", "#sortAccordionForCustomerPage", function () {
  // //console.log("hello from sort");
  paginationModelForCustomerPage.sortColumn = $(this).val();
  paginationModelForCustomerPage.sortOrder = $(
    "input[name='sortOrder']:checked"
  ).val();
  debouncedFetchCustomer();
});
$(document).on("click", ".btn-checkForCustomerOrder", function () {
  paginationModelForCustomerPage.sortOrder = $(this).val();
  paginationModelForCustomerPage.sortColumn = $(
    "#sortAccordionForCustomerPage"
  ).val();
  debouncedFetchCustomer();
});

// Clear Date Filter
$(document).on("click", "#clearDateRangeForCustomerPage", function () {
  $("#fromDateForCustomerPage").val("");
  $("#toDateForCustomerPage").val("");
  paginationModelForCustomerPage.fromDate = "";
  paginationModelForCustomerPage.toDate = "";
  paginationModelForCustomerPage.pageNumber = 1; // Reset to first page
  debouncedFetchCustomer();
});
// Change Items Per Page
$(document).on("change", "#CustomerPerPage", function () {
  paginationModelForCustomerPage.pageSize = $(this).val();
  paginationModelForCustomerPage.pageNumber = 1;
  debouncedFetchCustomer();
});

// Pagination
$(document).on("click", "#prevPageForCustomerPage", function () {
  if (paginationModelForCustomerPage.pageNumber > 1) {
    paginationModelForCustomerPage.pageNumber--;
    debouncedFetchCustomer();
  }
});
$(document).on("click", "#nextPageForCustomerPage", function () {
  if (
    paginationModelForCustomerPage.pageNumber *
      paginationModelForCustomerPage.pageSize <
    paginationModelForCustomerPage.totalRecords
  ) {
    paginationModelForCustomerPage.pageNumber++;
    debouncedFetchCustomer();
  }
});
$(document).on("click", "#nextPageForCustomerPage", function () {
  if (
    paginationModelForCustomerPage.pageNumber *
      paginationModelForCustomerPage.pageSize <
    paginationModelForCustomerPage.totalRecords
  ) {
    paginationModelForCustomerPage.pageNumber++;
    debouncedFetchCustomer();
  }
});

$(document).on("click", ".customerDetailsRow", function () {
  let customerId = $(this).data("customer-id");
  $.ajax({
    url: `/Customers/GetCustomerHistory/`,
    type: "GET",
    data: {
      customerId: customerId,
    },
    success: function (response) {
      $("#showCustomerHistory").html(response);
      $("#CustomerHistoryModal").modal("show");
    },
    error: function (error) {
      showToaster(error, "Error");
    },
  });
});
$(document).ready(function () {
  paginationModelForCustomerPage.totalRecords = $(
    "#TotalRecordsForCustomerPage"
  ).val();
  console.log(paginationModelForCustomerPage);
  updatePaginationInfo(paginationModelForCustomerPage);
});
//#endregion
