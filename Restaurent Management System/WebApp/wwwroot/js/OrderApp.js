//#region  Global Variables and Models
let KOTStatus = "InProgress";

const waitingToken = {
  tokenId: 0,
  createAt: "",
  customerName: "",
  email: "",
  phoneNumber: "",
  noOfPersons: 0,
  sectionId: 0,
  editorId: 0,
  customerId: 0,
  tableIds: [],
};
const customerDetails = {
  CustomerId: 0,
  CustomerName: "",
  CustomerPhone: "",
  CustomerEmail: "",
  NoOfPerson: 0,
};
let assignTableList = {
  tableId: 0,
  tableName: "",
  tableCapacity: 0,
};
//#endregion

//#region Function on Document Load
$(document).ready(function () {
  const currentUrl = window.location.pathname.toLowerCase();
  $(".kotBtns a").each(function () {
    const linkUrl = $(this).attr("href").toLowerCase();
    $(this).toggleClass("activePageInOrderApp", currentUrl.includes(linkUrl));
  });
  $(".orderAppDropDown a").each(function () {
    const linkUrl = $(this).attr("href").toLowerCase();
    $(this).toggleClass("orderAppDropdownItem", currentUrl.includes(linkUrl));
  });

  $("#noOfPersonForAssigningTable").on("input", function () {
    const maxCapacity = parseInt($(this).attr("max"));
    const currentValue = parseInt($(this).val());
    if (currentValue > maxCapacity) {
      $(this).val(maxCapacity);
      showToaster(
        "Number of persons exceeds the total capacity of selected tables.",
        "NotFound"
      );
    }
  });
});
//#endregion
/* ------------------------------ Waiting Token ----------------------------- */

//#region Waiting List Section
function clearWaitingTokenModal() {
  $("#waitingTokenEditorId").val(0);
  $("#waitingTokenCustomerId").val(0);
  $("#waitingTokenCustomerName").val("Guest");
  $("#waitingTokenCustomerEmail").val("");
  $("#waitingTokenCustomerPhone").val("");
  $("#waitingTokenNumberOfPersons").val(1);
}
$(document).on("click", "#waitingTokenBtn", function () {
  clearWaitingTokenModal();
  let sectionId = $(this).data("section-id");
  $("#waitingtokenSectionForAddWaitingToken").val(sectionId);
});
$(document).on("click", "#AddNewTokenBtn", function (event) {
  event.preventDefault();
  let form = $("#waitingTokenForm");
  if (!form.valid()) return;
  waitingToken.editorId = $("#editorIdForAssignToken").val();
  waitingToken.customerId = $("#waitingTokenCustomerId").val();
  waitingToken.customerName = $("#waitingTokenCustomerName").val();
  waitingToken.email = $("#waitingTokenCustomerEmail").val();
  waitingToken.phoneNumber = $("#waitingTokenCustomerPhone").val();
  waitingToken.noOfPersons = $("#waitingTokenNumberOfPersons").val();
  waitingToken.sectionId = $("#waitingtokenSectionForAddWaitingToken").val();

  $.ajax({
    url: "/WaitingList/AddNewToken",
    type: "POST",
    data: { token: waitingToken },
    success: function (response) {
      showToaster(response.message, response.status);
      $("#waitingTokenModal").modal("hide");
    },
    error: function () {
      showToaster("An error occurred while updating section.", "Error");
      $("#waitingTokenModal").modal("hide");
    },
  });
  // $("#waitingTokenModal").modal("show");;
});
$(document).on("input", "#waitingTokenCustomerEmail", function () {
  let emailId = $(this).val();
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  if (emailRegex.test(emailId)) {
    $.ajax({
      url: "/WaitingList/GetCustomerDetails",
      type: "Get",
      data: { emailId: emailId },
      success: function (response) {
        $("#waitingTokenCustomerId").val(response.customerDetails.customerId);
        $("#waitingTokenCustomerName").val(
          response.customerDetails.customerName
        );
        $("#waitingTokenCustomerPhone").val(
          response.customerDetails.customerPhone
        );
      },
      error: function () {
        showToaster("An error occurred while updating section.", "Error");
      },
    });
  }
});

$(document).on("input", "#emailIdForAssigningTable", function () {
  let emailId = $(this).val();
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  if (emailRegex.test(emailId)) {
    $.ajax({
      url: "/WaitingList/GetCustomerDetails",
      type: "Get",
      data: { emailId: emailId },
      success: function (response) {
        $("#waitingTokenCustomerIdForAssignToken").val(
          response.customerDetails.customerId
        );
        $("#customerNameForAssigningTable").attr(
          "data-customer-id",
          response.customerDetails.customerId
        );
        $("#customerNameForAssigningTable").val(
          response.customerDetails.customerName
        );
        $("#phoneNumberForAssigningTable").val(
          response.customerDetails.customerPhone
        );
      },
      error: function () {
        showToaster("An error occurred while updating section.", "Error");
      },
    });
  }
});
//#endregion

function updateTimers() {
  $(".live-timer").each(function () {
    const orderTime = $(this).data("order-time");
    const orderDate = new Date(orderTime);
    const now = new Date();
    const elapsed = Math.floor((now - orderDate) / 1000);

    const days = Math.floor(elapsed / (24 * 3600));
    const hours = Math.floor((elapsed % (24 * 3600)) / 3600);
    const minutes = Math.floor((elapsed % 3600) / 60);
    const seconds = elapsed % 60;

    const timeString = [
      days > 0 ? `${days}d` : "",
      hours > 0 || days > 0 ? `${hours}h` : "",
      minutes > 0 || hours > 0 || days > 0 ? `${minutes}m` : "",
      `${seconds}s`,
    ].join(" ");

    $(this).text(timeString.trim());

    // Add or remove the 'text-danger' class based on elapsed time
    if (elapsed >= 24 * 3600) {
      // More than or equal to 24 hours
      $(this).addClass("text-danger");
      $(this).removeClass("text-dark");
    } else {
      // Less than 24 hours
      $(this).removeClass("text-danger");
      $(this).addClass("text-dark");
    }
  });
}

// Update timers every second
setInterval(updateTimers, 1000);

/* ---------------------------------- Table View --------------------------------- */

//#region Table View Section
$(document).on("click", ".tableSectionAtOrderApp", function () {
  let accordianSectionId = $(this).data("section-id");

  $.ajax({
    url: "/OrderApp/GetSectionView",
    type: "Get",
    data: { sectionId: accordianSectionId },
    success: function (response) {
      // showToaster(response.message, response.status);
      $("#tableViewList-" + accordianSectionId).html(response);
    },
    error: function () {
      showToaster("An error occurred while updating section.", "Error");
    },
  });
});

$(document).on("click", ".assignTable", function () {
  let data = $(this).attr("data-table-details");
  let tableDetails = JSON.parse(data);

  if ($(this).hasClass("customBgLight")) {
    $(this).removeClass("customBgLight").addClass("customBgAssign");
    assignTableList.push({
      tableId: tableDetails.tableId,
      tableName: tableDetails.tableName,
      tableCapacity: tableDetails.capacity,
    });
    updateAssignButton(tableDetails.sectionId);
  } else if ($(this).hasClass("customBgAssign")) {
    $(this).removeClass("customBgAssign").addClass("customBgLight");
    assignTableList = assignTableList.filter(
      (table) => table.tableId !== tableDetails.tableId
    );
    updateAssignButton(tableDetails.sectionId);
  }
});
function clearAssigningTableForm() {
  // Clear the values of the input fields
  $("#emailIdForAssigningTable").val("").prop("disabled", false);
  $("#phoneNumberForAssigningTable").val("").prop("disabled", false);
  $("#noOfPersonForAssigningTable").val("").prop("disabled", false);
  $("#customerNameForAssigningTable").val("").prop("disabled", false);

  // Optionally, clear any data attributes if necessary
  $("#customerNameForAssigningTable")
    .removeData("token-id")
    .removeData("customer-id");
}
// Function to update the visibility of the assign button based on the list
function updateAssignButton(sectionId) {
  if (assignTableList.length > 0) {
    $(`#assignBtn-${sectionId}`).removeClass("d-none");
  } else {
    $(`#assignBtn-${sectionId}`).addClass("d-none");
  }
}

// Event listener for changing sections or floors
$(document).on("click", ".tableSectionAtOrderApp", function () {
  assignTableList = [];
  updateAssignButton($(this).attr("data-section-id"));
});

$(document).on("click", ".assignBtn", function () {
  clearAssigningTableForm();
  let sectionId = $(this).attr("data-section-id");
  if (sectionId > 0) {
    $.ajax({
      url: "/OrderApp/GetWaitingList",
      type: "Get",
      data: { sectionId: sectionId },
      success: function (response) {
        if (response.message != null) {
          showToaster(response.message, response.status);
        } else {
          $("#waitiningListAtAssignTable").html(response);

          // === Disable radio buttons based on person count ===
          $("tr .waitngTokenDetails").each(function () {
            // console.log("hello");
            let tokenDetails = $(this).attr("data-token-details");
            let token = JSON.parse(tokenDetails);
            if (token && token.noOfPersons > totalCapacity) {
              $(this).prop("disabled", true);
              $(this).closest("tr").addClass("text-muted");
            } else {
              $(this).prop("disabled", false);
              $(this).closest("tr").removeClass("text-muted");
            }
          });
        }
      },
      error: function () {
        showToaster("An error occurred while updating waitingList.", "Error");
      },
    });

    let tableNames = assignTableList.map((table) => table.tableName);
    let tableNamesString = tableNames.join(", ");
    let totalCapacity = assignTableList.reduce(
      (total, table) => total + table.tableCapacity,
      0
    );
    // console.log(totalCapacity);
    let HtmlTextForTableName = "Selected tables :- " + tableNamesString;
    let TableCapacity = "Total Capacity :- " + totalCapacity;
    $("#tableIdsForAssignToCustomer").text(HtmlTextForTableName);
    $("#tablesCapacityForAssignToCustomer").text(TableCapacity);

    $("#sectionIdForAssigningTable").val(sectionId);
    $("#noOfPersonForAssigningTable").attr("max", totalCapacity);
  } else {
    showToaster("An error occurred while updating sectionId.", "Error");
  }
});

$(document).on("click", ".waitngTokenDetails", function () {
  let data = $(this).attr("data-token-details");
  let tokenDetails = JSON.parse(data);
  // console.log(tokenDetails);
  $("#emailIdForAssigningTable").val(tokenDetails.email).prop("disabled", true);
  $("#phoneNumberForAssigningTable")
    .val(tokenDetails.phoneNumber)
    .prop("disabled", true);
  $("#noOfPersonForAssigningTable")
    .val(tokenDetails.noOfPersons)
    .prop("disabled", true);
  $("#customerNameForAssigningTable")
    .val(tokenDetails.customerName)
    .prop("disabled", true);
  // Set data attributes (token-id and customer-id) on the element
  $("#customerNameForAssigningTable").data("token-id", tokenDetails.tokenId);
  $("#customerNameForAssigningTable").data(
    "customer-id",
    tokenDetails.customerId
  );
});

/* ------------------------------ Assign Table ------------------------------ */

$(document).on("click", "#assignTableBtn", function () {
  let form = $("#assignTableFormAtTableView");
  if (!form.valid()) return;
  waitingToken.tokenId = $("#customerNameForAssigningTable").data("token-id");
  waitingToken.customerId = $("#customerNameForAssigningTable").data(
    "customer-id"
  );
  waitingToken.customerName = $("#customerNameForAssigningTable").val();
  waitingToken.email = $("#emailIdForAssigningTable").val();
  waitingToken.phoneNumber = $("#phoneNumberForAssigningTable").val();
  waitingToken.noOfPersons = $("#noOfPersonForAssigningTable").val();
  waitingToken.sectionId = $("#sectionIdForAssigningTable").val();
  waitingToken.editorId = $("#waitingTokenEditorId").val();
  waitingToken.tableIds = assignTableList.map((table) => table.tableId);

  $.ajax({
    url: "/OrderApp/AssignTable",
    type: "POST",
    data: { tokenDetails: waitingToken },
    success: function (response) {
      redirectToOrderMenuPage(response.orderId);
      showToaster(response.message, response.status);
    },
    error: function () {
      showToaster("An error occurred while updating waitingList.", "Error");
    },
  });
});
$(document).on("click", ".runningOrder", function () {
  if ($(".customBgAssign").length>0) {
    showToaster("This table already assigned", "Error");
    return;
  }else{
    let orderId = $(this).attr("data-order-id");
    redirectToOrderMenuPage(orderId);
  }
});
function redirectToOrderMenuPage(orderId) {
  window.location.href = `/OrderAppMenu/OrderAppMenu?orderId=${btoa(orderId)}`;
}
//#endregion
