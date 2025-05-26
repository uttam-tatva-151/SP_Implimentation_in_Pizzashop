const waitingTokenVM = {
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

const EditorId = $("#pageHead").attr("data-user-id");

$(document).ready(function () {
  $("#nav-watingList-tab-0").addClass("activeTabAtKOTPage");
  getWaitingList(0);
});

$(document).on("click", ".sortwatingListBysection", function (event) {
  event.preventDefault();
  let sectionId = $(this).attr("data-section-id");
  highlightSection(sectionId);
  getWaitingList(sectionId);
});
function getWaitingList(sectionId) {
  $.ajax({
    url: "/WaitingList/GetWaitingList",
    type: "Get",
    data: { sectionId: sectionId },
    success: function (response) {
      if (response.message != null) {
        showToaster(response.message, response.status);
      } else {
        $("#watingListGrid").html(response);
      }
    },
    error: function () {
      showToaster("An error occurred while updating waitingList.", "Error");
    },
  });
}

function highlightSection(sectionId) {
  $(".sortwatingListBysection").removeClass("activeTabAtKOTPage");
  // const targetedSection = `${sectionId}`;
  $("#nav-watingList-tab-" + sectionId).addClass("activeTabAtKOTPage");
}

$(document).on("click", "#AssignNewTokenBtn", function (event) {
  event.preventDefault();
  let form = $("#waitingTokenForm");
  if (!form.valid()) return;
  // clearWaitingToken();
  waitingTokenVM.editorId = EditorId;
  waitingTokenVM.customerId = $("#waitingTokenCustomerId").val();
  waitingTokenVM.customerName = $("#waitingTokenCustomerName").val();
  waitingTokenVM.email = $("#waitingTokenCustomerEmail").val();
  waitingTokenVM.phoneNumber = $("#waitingTokenCustomerPhone").val();
  waitingTokenVM.noOfPersons = $("#waitingTokenNumberOfPersons").val();
  waitingTokenVM.sectionId = $("#waitingtokenSectionForAssignWaitingToken").val();
  console.log(waitingTokenVM);
  $.ajax({
    url: "/WaitingList/AddNewToken",
    type: "POST",
    data: { token: waitingTokenVM },
    success: function (response) {
      // $("#watingListGrid").html(response.partialView);
      window.location.reload();
      showToaster(response.message, response.status);
      // $("#waitingTokenModal").modal("hide");
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
$(document).on("click", ".editTokenBtn", function () {
  const tokenData = $(this).attr("data-token-details");
  let token = JSON.parse(tokenData);
  
  // Populate the edit modal fields with the token details
  $("#waitingTokenCustomerName").attr("data-token-id", token.tokenId);
  $("#waitingTokenCustomerId").val(token.customerId);
  $("#waitingTokenCustomerName").val(token.customerName);
  $("#waitingTokenCustomerEmail").val(token.email);
  $("#waitingTokenCustomerPhone").val(token.phoneNumber);
  $("#waitingTokenNumberOfPersons").val(token.noOfPersons);
  $("#waitingtokenSectionForAssignWaitingToken").val(token.sectionId);
  $("#waitingtokenSectionForAssignWaitingToken").attr(
    "data-token-section-id",
    token.sectionId
  );
  $("#AssignNewTokenBtn").attr("id", "UpdateTokenBtn");
  // Show the modal
  $("#waitingTokenModal").modal("show");
});

// Submit Edit Token Form
$(document).on("click", "#UpdateTokenBtn", function () {
  let form = $("#waitingTokenForm");
  if (!form.valid()) return;

  waitingTokenVM.tokenId = $("#waitingTokenCustomerName").attr("data-token-id");
  waitingTokenVM.editorId = EditorId;
  waitingTokenVM.customerId = $("#waitingTokenCustomerId").val();
  waitingTokenVM.customerName = $("#waitingTokenCustomerName").val();
  waitingTokenVM.email = $("#waitingTokenCustomerEmail").val();
  waitingTokenVM.phoneNumber = $("#waitingTokenCustomerPhone").val();
  waitingTokenVM.noOfPersons = $("#waitingTokenNumberOfPersons").val();
  waitingTokenVM.sectionId = $("#waitingtokenSectionForAssignWaitingToken").val();
  // let sectionId = $("#waitingtokenSectionForAddWaitingToken").attr(
  //   "data-token-section-id"
  // );
  console.log($("#waitingTokenCustomerName").attr("data-token-id"))
  $.ajax({
    url: "/WaitingList/UpdateToken",
    type: "POST",
    data: { token: waitingTokenVM },
    success: function (response) {
      // $("#watingListGrid").html(response.partialView); // Update the grid
      window.location.reload();
      showToaster(response.message, response.status);
      // $("#UpdateTokenBtn").attr("id", "AddNewTokenBtn");
      // $("#waitingTokenModal").modal("hide");
      window.reload();
    },
    error: function () {
      showToaster("An error occurred while editing the token.", "Error");
      $("#UpdateTokenBtn").attr("id", "AddNewTokenBtn");
      $("#waitingTokenModal").modal("hide");
    },
  });
});
function calculateAndSetHeader(fromSection, toSection) {
  const fromBadge = $(`#nav-watingList-tab-${fromSection} .badge`);
  const toBadge = $(`#nav-watingList-tab-${toSection} .badge`);

  if (fromBadge.length) {
    let fromCount = parseInt(fromBadge.text().trim(), 10);
    if (!isNaN(fromCount) && fromCount > 0) {
      fromBadge.text(fromCount - 1);
    }
  }

  if (toBadge.length) {
    let toCount = parseInt(toBadge.text().trim(), 10);
    if (!isNaN(toCount)) {
      toBadge.text(toCount + 1);
    }
  }
}
function calculateTotalBadgeCount() {
  let totalCount = 0;
  $(".badge").each(function () {
    let count = parseInt($(this).text().trim(), 10);
    if (!isNaN(count)) {
      totalCount += count;
    }
  });
  return totalCount;
}
function calculateAndSetHeaderAtDeleteOrAssign(sectionId) {
  const badge = $(`#nav-watingList-tab-0 .badge`);
  const sectionBadge = $(`#nav-watingList-tab-${sectionId} .badge`);
  if (badge.length) {
    let badgeCount = calculateTotalBadgeCount();
    badge.text(badgeCount - 1);
  }
  if (sectionBadge.length) {
    let badgeCount = parseInt(badge.text().trim(), 10);
    if (!isNaN(badgeCount) && badgeCount > 0) {
      sectionBadge.text(badgeCount - 1);
    }
  }
}
// Delete Token
$(document).on("click", ".deleteTokenBtn", function () {
  let tokenId = $(this).data("token-id");
  $("#deleteWaitingTokenBtn").attr("data-token-id", tokenId);
  $("#deleteTokenModal").modal("show");
});

$(document).on("click", "#deleteWaitingTokenBtn", function (event) {
  event.preventDefault();
  let tokenId = $(this).attr("data-token-id");
  let sectionId = $(this).attr("data-token-section-id");
  $.ajax({
    url: "/WaitingList/DeleteToken",
    type: "POST",
    data: { tokenId: tokenId, editorId: EditorId },
    success: function (response) {
      $("#watingListGrid").html(response.partialView);
      // calculateAndSetHeaderAtDeleteOrAssign(sectionId);
      // $("#deleteTokenModal").modal("hide");
      window.location.reload();
      showToaster(response.message, response.status);
    },
    error: function () {
      showToaster("An error occurred while deleting the token.", "Error");
    },
  });
});

/*----------------------------------Assign Table----------------------------------*/

$(document).on("click", ".assignWaititngTokenBtn", function () {
  let tokenDetails = $(this).attr("data-token-details");
  let token = JSON.parse(tokenDetails);
  $("#assignTableToWaitingTokenBtn").attr("data-token-details",tokenDetails);
  const sectionDropdown = $("#sectionIdForAssignTable");
  sectionDropdown.prop("disabled", true);

  $("#sectionIdForAssignTable").val(token.sectionId);
  $("#sectionIdForAssignTable").attr("data-token-id", token.tokenId);

  changeTableDropdown(token.sectionId);
  $("#assignTableModal").modal("show");
  suggestTablesForRequirement();
});

$(document).on("change", "#sectionIdForAssignTable", function () {
  const selectedSectionId = $(this).val();
  changeTableDropdown(selectedSectionId);
});

function changeTableDropdown(selectedSectionId) {
  const tableDropdown = $("#tableIdsForAssignTable");
  tableDropdown.empty().append('<option value="">Select Table</option>');

  $("#selectedCount").text('0');
  if (selectedSectionId) {
    let tableSection = $("#sectionIdForAssignTable-" + selectedSectionId).attr(
      "data-table-list"
    );
$("#tableIdsForAssignTable").attr('data-tables',tableSection);
    const tableDetails = JSON.parse(tableSection);
    tableDetails.forEach((table) => {
      const listItem = `
        <li class="custom-dropdown-item px-2 w-100" data-table-capacity="${table.capacity}">
          <div class="form-check w-100" >
            <input class="form-check-input assign-table-checkbox "
                   type="checkbox" 
                   id="assignTable_${table.tableId}" 
                   value="${table.tableId}" 
                   name="TableId">
            <label class="form-check-label " 
                   for="assignTable_${table.tableId}">
              ${table.tableName} - Capacity(${table.capacity})
            </label>
          </div>
        </li>
      `;
  
      tableDropdown.append(listItem); // Append the generated HTML to the container
    });
  }
}
$(document).on("change", ".assign-table-checkbox", function () {
  let selectedCount = $(".assign-table-checkbox:checked").length;
  $("#selectedCount").text(selectedCount); // Update the count in the dropdown button
});

function suggestTables(requirement, tables) {
  // Sort tables by capacity in descending order for optimal selection
  tables.sort((a, b) => b.capacity - a.capacity);

  // Variables to track selected tables and their total capacity
  let selectedTables = [];
  let totalCapacity = 0;

  // Loop through the sorted tables to select the optimal combination
  for (const table of tables) {
    if (totalCapacity >= requirement) break; // Stop if requirement is met
    selectedTables.push(table);
    totalCapacity += table.capacity; // Add table capacity to the total
  }

  // Check if the requirement is met
  if (totalCapacity >= requirement) {
    return {
      suggestedTables: selectedTables,
      totalCapacity: totalCapacity,
      status: "success",
    };
  } else {
    return {
      suggestedTables: [],
      totalCapacity: totalCapacity,
      status: "failure",
    };
  }
}
function suggestTablesForRequirement(){
  let tokenDetails = $("#assignTableToWaitingTokenBtn").attr("data-token-details");
  let token = JSON.parse(tokenDetails);
  let tablesData = $("#tableIdsForAssignTable").attr('data-tables');
  let tables = JSON.parse(tablesData);
  let noOfPersons = token.noOfPersons;

  const suggestion = suggestTables(noOfPersons, tables);
  if (suggestion.status === "success") {
    console.log(
      `Suggested tables: ${suggestion.suggestedTables
        .map((table) => table.tableId)
        .join(", ")} (Total Capacity: ${suggestion.totalCapacity})`
    );
  } else {
    console.log("No suitable combination of tables meets the requirement.");
  }
}






$(document).on("click", "#assignTableToWaitingTokenBtn", function () {
  let tokenDetails = $(this).attr("data-token-details");
  let token = JSON.parse(tokenDetails);
  let sectionId = $("#sectionIdForAssignTable").val();
    let selectedTableIds = $(".assign-table-checkbox:checked")
    .map(function () {
      return parseInt(this.value, 10); 
    })
    .get();
  $("#hiddenFieldForTableIds").val(selectedTableIds.join(",")); // Save to hidden field for validation
    if(selectedTableIds.length>0){
      waitingTokenVM.tokenId = token.tokenId || 0;
    waitingTokenVM.createAt = token.createAt || ""; 
    waitingTokenVM.customerName = token.customerName || "";
    waitingTokenVM.email = token.email || ""; 
    waitingTokenVM.phoneNumber = token.phoneNumber || ""; 
    waitingTokenVM.noOfPersons = token.noOfPersons || 0;
    waitingTokenVM.sectionId = sectionId || 0;
    waitingTokenVM.editorId = EditorId || 0;
    waitingTokenVM.customerId = token.customerId || 0;
    waitingTokenVM.tableIds = selectedTableIds;


  $.ajax({
    url: "/OrderApp/AssignTable",
    type: "POST",
    data: { tokenDetails: waitingTokenVM },
    success: function (response) {
      redirectToOrderMenuPage(response.orderId);
      showToaster(response.message, response.status);
    },
    error: function () {
      showToaster("An error occurred while updating waitingList.", "Error");
    },
  });
    }else{
      showToaster("Select atleast one table for assign it", "Warning");
    }
    
});

function redirectToOrderMenuPage(orderId) {
  window.location.href = `/OrderAppMenu/OrderAppMenu?orderId=${btoa(orderId)}`;
}
