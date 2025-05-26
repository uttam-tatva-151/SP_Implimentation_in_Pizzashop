//#region Order Pagination
let paginationModel = {
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
const debouncedFetchOrder = debounce(fetchOrders, 300);

function updatePaginationInfo(pagination) {
  paginationModel.totalRecords = pagination.totalRecords;
  $("#paginationInfo").text(
    `Showing ${
      (pagination.pageNumber - 1) * pagination.pageSize + 1
    } - ${Math.min(
      pagination.pageNumber * pagination.pageSize,
      pagination.totalRecords
    )} of ${pagination.totalRecords}`
  );
}
function fetchOrders() {
  $.ajax({
    url: "/Orders/Order",
    type: "GET",
    contentType: "application/json",
    data: paginationModel,
    success: function (response) {
      $("#orderList").html(response.partialView);
      updatePaginationInfo(response.paginationDetails);
    },
    error: function () {
      showToaster("Error during fetch Order","Error");
    },
  });
}
// Sorting
$(document).on("click", ".sort", function () {
  paginationModel.sortColumn = $(this).data("sortby");
  paginationModel.sortOrder =
    paginationModel.sortOrder === "asc" ? "desc" : "asc";
  debouncedFetchOrder();
});

// Search Functionality
$(document).on("input", "#SearchUser", function () {
let searchValue = $(this).val().trim();
  if(searchValue.length>2){
    paginationModel.searchQuery = searchValue;
    paginationModel.pageNumber = 1;
    debouncedFetchOrder();
  }
  if(searchValue.length==0){
    paginationModel.searchQuery = "";
    paginationModel.pageNumber = 1;
    debouncedFetchOrder();
  }
});

// Date Filter
$(document).on("click", "#searchDateRange", function () {
  paginationModel.fromDate = $("#fromDate").val();
  paginationModel.toDate = $("#toDate").val();
  paginationModel.pageNumber = 1;  
  if(paginationModel.fromDate>paginationModel.toDate){
    showToaster("Please enter appropriate Date Range","Warning");
  }else{

    debouncedFetchOrder();
  }
});

$(document).on("change", "#orderStatus", function () {
  paginationModel.orderStatus = $(this).val();
  paginationModel.pageNumber = 1;  
  debouncedFetchOrder();
});
$(document).on("change", "#dateRange", function () {
  paginationModel.dateRange = $(this).val();
  paginationModel.pageNumber = 1;  
  debouncedFetchOrder();
});
// Clear Date Filter
$(document).on("click", "#clearDateRange", function () {
  $("#fromDate").val("");
  $("#toDate").val("");
  paginationModel.fromDate = "";
  paginationModel.toDate = "";
  paginationModel.pageNumber = 1;  
  debouncedFetchOrder();
});
// Change Items Per Page
$(document).on("change", "#OrderPerPage", function () {
  paginationModel.pageSize = $(this).val();
  paginationModel.pageNumber = 1;
  debouncedFetchOrder();
});

// Pagination
$(document).on("click", "#prevPage", function () {
  if (paginationModel.pageNumber > 1) {
    paginationModel.pageNumber--;
    debouncedFetchOrder();
  }
});
$(document).on("click", "#nextPage", function () {
  if (
    paginationModel.pageNumber * paginationModel.pageSize <
    paginationModel.totalRecords
  ) {
    paginationModel.pageNumber++;
    debouncedFetchOrder();
  }
});

$(document).ready(function () {
  paginationModel.totalRecords = $("#totalRecordsForOrderPage").val();
  updatePaginationInfo(paginationModel);
});

//#endregion

//#region View-order-details

$(document).on("click",".viewOrderDetails", function () {
  //console.log("Hello From OrdesDetails")
  let orderId = $(this).data("order-id");

  $.ajax({
    url: '/Orders/OrderDetails',
    type: "GET",
    data: { orderId: orderId },
    success: function () {
        showToaster("Order Details Show successfully!!!","Success");
    },
    error: function () {
        showToaster("Error during fetch Order Details!!!","Error");
    },
  });
});

//#endregion
