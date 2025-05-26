//#region  View Models
let categoryDetails = {
  id: 0,
  categoryName: "",
  description: "",
  editorId: 0,
};
let modifierGroup = {
  groupId: 0,
  groupName: "",
  description: "",
  editorId: 0,
  modifierIds: [],
};
let ModifierVM = {
  ModifierId: 0, // Default value
  ModifierGroupId: [],
  ModifierName: "",
  Description: null,
  UnitPrice: 0.0,
  Quantity: 1,
  UnitType: "",
  EditorId: 0,
};
const debouncedFetchItem = debounce(getItems, 300);
const debouncedFetchModifiers = debounce(getModifiersList, 300);
const categoryIdOfFirstchild = $("#categoryList .category-item:first").data(
  "category-id"
);
const groupIdOfFirstchild = $("#modifierGroupList .modifier-group:first").data(
  "group-id"
);
const editorId = $("#editorIdAtMenuPage").val();
//#endregion
//#region  Validations
/* -------------------------- Validation for modals ------------------------- */
function validateCategoryDetails(categoryDetails) {
  if (categoryDetails.categoryName.trim() === "") {
    $("#validationTextForCategoryNameForAddCategory").text(
      "Please Enter Category Name"
    );
    $("#validationTextForCategoryNameForAddCategory").removeClass("d-none");
    $("#categoryNameForAddCategory").focus();
    return false;
  }
  return true;
}
function validateCategoryDetailsForEdit(categoryDetails) {
  if (categoryDetails.categoryName.trim() === "") {
    $("#validationTextForCategoryNameForEditCategory").text(
      "Please Enter Category Name"
    );
    $("#validationTextForCategoryNameForEditCategory").removeClass("d-none");
    $("#categoryNameForAddCategory").focus();
    return false;
  }
  return true;
}
function validateModifierGroupDetails(modifierGroup) {
  if (modifierGroup.groupName.trim() === "") {
    $("#validationTextForModifierGroupNameForAddGroup").text(
      "Please Enter Modifier Group Name"
    );
    $("#validationTextForModifierGroupNameForAddGroup").removeClass("d-none");
    $("#addModifierGroupName").focus();
    return false;
  }
  return true;
}
function validateModifierGroupDetailsForEdit(modifierGroup) {
  if (modifierGroup.groupName.trim() === "") {
    $("#validationTextForModifierGroupNameForEditGroup").text(
      "Please Enter Modifier Group Name"
    );
    $("#validationTextForModifierGroupNameForEditGroup").removeClass("d-none");
    $("#editModifierGroupDescription").focus();
    return false;
  }
  return true;
}
function hideValidationTextForCategoryNameAtAddCategory() {
  $("#validationTextForCategoryNameForAddCategory").addClass("d-none");
  $("#validationTextForCategoryNameForAddCategory").text("");
}
function hideValidationTextForCategoryNameAtEditCategory() {
  $("#validationTextForCategoryNameForEditCategory").addClass("d-none");
  $("#validationTextForCategoryNameForEditCategory").text("");
}
function hideValidationTextForModifierGroupNameAtAddGroup() {
  $("#validationTextForModifierGroupNameForAddGroup").addClass("d-none");
  $("#validationTextForModifierGroupNameForAddGroup").text("");
}
function hideValidationTextForModifierGroupNameAtEditGroup() {
  $("#validationTextForModifierGroupNameForEditGroup").addClass("d-none");
  $("#validationTextForModifierGroupNameForEditGroup").text("");
}

//#endregion
//#region  Category Section
function highlightCategory(categoryId) {
  // Remove active class from all category items
  $(".category-item").removeClass("active-category");

  // Hide all category action buttons
  $(".showBtns").addClass("d-none");

  // Add active class to the selected category
  $(`#category-${categoryId}`).addClass("active-category");

  // Show action buttons for the selected category
  $(`#categoryBtns-${categoryId}`).removeClass("d-none");
}
// ===============================
// Category Management Functions
// ===============================
function AssignValueForCategory(categoryId) {
  $("#categoryIdForDelete").val(categoryId);
}
$(document).on("click", "#EditItemBtnForMobileView", function () {
  let categoryId = $(".active-category").data("category-id");
  let categoryName = $(".active-category").data("category-name");
  let categoryDescription = $(".active-category").data("category-description");
  FillFormForEditCategory(categoryId, categoryName, categoryDescription);
});
$(document).on("click", "#deleteCategoryBtnForMobileView", function () {
  let categoryId = $(".active-category").data("category-id");

  AssignValueForCategory(categoryId);
});
/* ------------------------------ Add Category ------------------------------ */
$(document).on("click", "#addCategoryBtn", function () {
  categoryDetails.categoryName = $("#categoryNameForAddCategory").val();
  categoryDetails.description = $("#categoryDescriptionForAddCategory").val();
  categoryDetails.editorId = $("#editorIdForAddCategory").val();
  if (validateCategoryDetails(categoryDetails)) {
    $.ajax({
      url: "/Menu/AddCategory",
      type: "POST",
      data: { newCategory: categoryDetails },
      success: function (response) {
        showToaster(response.message, response.status);
        $("#addCategoryModal").modal("hide");
        $("#categorySection").html(response.data);
      },
      error: function () {
        showToaster("Error Occur On Ajax Call", "Error");
      },
    });
  }
});

/* ------------------------------ Edit Category ----------------------------- */
function FillFormForEditCategory(
  categoryId,
  categoryName,
  categoryDescription
) {
  $("#categoryIdForEditCategory").val(categoryId);
  $("#categoryNameForEditCategory").val(categoryName);
  $("#categoryDescriptionForEditCategory").val(categoryDescription);
  categoryDetails.id = categoryId;
  categoryDetails.categoryName = categoryName;
  categoryDetails.description = categoryDescription;
}
function clearEditCategoryForm() {
  FillFormForEditCategory(
    categoryDetails.id,
    categoryDetails.categoryName,
    categoryDetails.description
  );
}
$(document).on("click", "#editCategoryBtn", function () {
  categoryDetails.id = $("#categoryIdForEditCategory").val();
  categoryDetails.categoryName = $("#categoryNameForEditCategory").val();
  categoryDetails.description = $("#categoryDescriptionForEditCategory").val();
  categoryDetails.editorId = $("#editorIdForEditCategory").val();
  if (validateCategoryDetailsForEdit(categoryDetails)) {
    $.ajax({
      url: "/Menu/EditCategory",
      type: "POST",
      data: { updateCategory: categoryDetails },
      success: function (response) {
        showToaster(response.message, response.status);
        $("#EditCategoryModal").modal("hide");
        $("#categorySection").html(response.partialView);
      },
      error: function () {
        showToaster("Error Occur On Ajax Call", "Error");
      },
    });
  }
});

/* ------------------------------ Delete Category ----------------------------- */
$(document).on("click", "#deleteCategoryBtn", function () {
  let categoryId = $("#categoryIdForDelete").val();
  let editorId = $("#editorIdForDeleteCategory").val();
  $.ajax({
    url: "/Menu/DeleteCategory",
    type: "POST",
    data: { categoryId: categoryId, editorId: editorId },
    success: function (response) {
      showToaster(response.message, response.status);
      $("#deleteCategoryModal").modal("hide");
      $("#categorySection").html(response.partialView);
    },
    error: function () {
      showToaster("Error Occur On Ajax Call", "Error");
    },
  });
});

//#endregion

//#region  Pagination For Item Table

let paginationForItemTable = {
  pageSize: 3,
  pageNumber: 1,
  sortColumn: "",
  sortOrder: "",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
};

function updatePaginationInfoForItemPage() {
  let paginationDetailsElement = $("#paginationDetailsForItemsPage");

  paginationForItemTable.pageSize = paginationDetailsElement.data("page-size");
  paginationForItemTable.pageNumber =
    paginationDetailsElement.data("page-number");
  paginationForItemTable.sortColumn =
    paginationDetailsElement.data("sort-column");
  paginationForItemTable.sortOrder =
    paginationDetailsElement.data("sort-order");
  paginationForItemTable.searchQuery =
    paginationDetailsElement.data("search-query");
  paginationForItemTable.fromDate = paginationDetailsElement.data("from-date");
  paginationForItemTable.toDate = paginationDetailsElement.data("to-date");
  paginationForItemTable.totalRecords =
    paginationDetailsElement.data("total-records");

  $("#paginationInfoForItemTable").text(
    `Showing ${
      (paginationForItemTable.pageNumber - 1) *
        paginationForItemTable.pageSize +
      1
    } - ${Math.min(
      paginationForItemTable.pageNumber * paginationForItemTable.pageSize,
      paginationForItemTable.totalRecords
    )} of ${paginationForItemTable.totalRecords}`
  );
}

function getItems() {
  let categoryId =
    $(".category-item.active-category").data("category-id") || null;
  getItemsbyCategoryId(categoryId);
}

function getItemsbyCategoryId(categoryId) {
  $.ajax({
    url: "/Menu/GetItems",
    type: "POST",
    data: { id: categoryId, paginationDetails: paginationForItemTable },
    success: function (response) {
      $("#MenuItems").html(response);
      highlightCategory(categoryId);
      updatePaginationInfoForItemPage();
    },
    error: function () {
      showToaster("No Items Found in this Category", "NotFound");
    },
  });
}

// Search Functionality
$(document).on("input", "#searchItem", function () {
  let searchValue = $(this).val().trim();
  if(searchValue.length<=100 && searchValue.length >= 2 ){
    paginationForItemTable.searchQuery = searchValue;
    paginationForItemTable.pageNumber = 1;
    debouncedFetchItem();
  }
  if($(this).val().length == 0){
    paginationForItemTable.searchQuery = "";
    paginationForItemTable.pageNumber = 1;
    debouncedFetchItem();
  }
});

// Change Items Per Page
$(document).on("change", "#itemsPerPage", function () {
  paginationForItemTable.pageSize = parseInt($(this).val(), 10);
  paginationForItemTable.pageNumber = 1;
  debouncedFetchItem();
});

// Pagination - Previous Button
$(document).on("click", "#prevPageForItem", function () {
  if (paginationForItemTable.pageNumber > 1) {
    paginationForItemTable.pageNumber--;
    debouncedFetchItem();
  }
});

// Pagination - Next Button
$(document).on("click", "#nextPageForItem", function () {
  if (
    paginationForItemTable.pageNumber * paginationForItemTable.pageSize <
    paginationForItemTable.totalRecords
  ) {
    paginationForItemTable.pageNumber++;
    debouncedFetchItem();
  }
});
//#endregion

//#region Global Event Listener
// Event listener for individual row checkboxes
$(document).on("click", ".row-checkbox", function () {
  let table = $(this).closest(".selectable-table");
  let checkbox = $(this);
  checkbox.prop("checked", !checkbox.prop("checked")).trigger("change");
  updateSelectAllState(table);
});

// Click on row to toggle checkbox selection (excluding direct checkbox clicks)
$(document).on("click", ".selectable-row", function () {
  let checkbox = $(this).find(".row-checkbox");
  checkbox.prop("checked", !checkbox.prop("checked")).trigger("change");

  let table = $(this).closest(".selectable-table");
  updateSelectAllState(table);
});

// Function to update "Select All" checkbox state for the current table
function updateSelectAllState(table) {
  let totalCheckboxes = table.find(".row-checkbox").length;
  let checkedCheckboxes = table.find(".row-checkbox:checked").length;
  let selectAll = table.find(".select-all-checkbox");

  if (checkedCheckboxes === 0) {
    selectAll.prop("checked", false).prop("indeterminate", false);
  } else if (checkedCheckboxes === totalCheckboxes) {
    selectAll.prop("checked", true).prop("indeterminate", false);
  } else {
    selectAll.prop("checked", false).prop("indeterminate", true);
  }
}

//#endregion

//#region  Item Section

function AssignValue(itemId) {
  $("#itemIdForDelete").val(itemId);
}
/* ---------------------------------Mass Items Delete ----------------------------*/
$(document).on("click", "#deleteSelectedItems", function () {
  let selectedIds = [];
  $(".row-checkbox:checked").each(function () {
    selectedIds.push($(this).data("itemid"));
  });
  if (selectedIds.length > 0) {
    $.ajax({
      url: "/Menu/DeleteMultipleItems",
      type: "POST",
      data: { ids: selectedIds, editorId: editorId },
      success: function (response) {
        debouncedFetchItem();
        showToaster(response.message, response.status);
        $("#deleteItemsModal").modal("hide");
      },
      error: function (status) {
        if (status.status == 403)
          showToaster(
            "You don't have permission for this functionality",
            "Error"
          );
        else showToaster("An error occurred while updating section.", "Error");
        $("#deleteItemsModal").modal("hide");
      },
    });
  } else {
    showToaster("Please select at least one item to delete.", "Not Found");
    $("#deleteItemsModal").modal("hide");
  }
});

function openEditModal(itemId) {
  $.ajax({
    url: "/Menu/GetItemById",
    type: "GET",
    data: { itemId: itemId },
    success: function (response) {
      if (response.success) {
        let editModal = $("#EditItemModal");

        if (editModal.length) {
          $("#EditItemModal").modal("show");
        }

        $("#UpdateCategoryForEdit").val(response.data.categoryId);
        $("#ItemNameForEdit").val(response.data.name);
        $("#Item-TypeForEdit").val(response.data.itemType);
        $("#PriceRateForEdit").val(response.data.unitPrice);
        $("#ItemQuantityForEdit").val(response.data.quantity);
        $("#UpdateUnitForEdit").val(response.data.unitType);
        $("#AvailableForEdit").prop("checked", response.data.isAvailable);
        $("#DefaultTaxForEdit").prop("checked", response.data.defaultTax);
        $("#ItemTaxPercentageForEdit").val(response.data.taxPercentage);
        $("#ShortCdeForEdit").val(response.data.shortCode);
        $("#descriptionForEdit").val(response.data.description);
        $("#ItemIdForEdit").val(itemId);

        // Directly pass imDetails to the function
        let itemModifierRelation = [].concat(response.data.imDetails || []);
        itemModifierRelation.forEach((mapping) => {
          addModifierGroup({
            IMGid: mapping.imGid,
            ItemId: mapping.itemId,
            MgId: mapping.mgId,
            MinModifiers: mapping.minModifiers,
            MaxModifiers: mapping.maxModifiers,
          });
        });
      } else {
        showToaster("Error fetching item data.", "Error");
      }
    },
    error: function (status) {
      if (status.status == 403)
        showToaster(
          "You don't have permission for this functionality",
          "Error"
        );
      else showToaster("An error occurred while updating section.", "Error");
      $("#EditItemModal").modal("hide");
    },
  });
}

function addModifierGroup(modifier) {
  let itemId = modifier.ItemId;

  let itemModifierRelationId = modifier.IMGid;
  let modifierGroupId = modifier.MgId;
  let minModifiers = modifier.MinModifiers;
  let maxModifiers = modifier.MaxModifiers;

  if ($("#modifierGroupSection-" + modifierGroupId).length === 0) {
    $.ajax({
      url: "/Menu/GetModifiersByGroupId",
      type: "GET",
      data: { groupId: modifierGroupId },
      success: function (data) {
        if (!data || data.length === 0) return;
        let groupName =
          $("#attachModifierGroupforEditItem")
            .find(`option[value="${modifierGroupId}"]`)
            .text() || "Unknown Group";
        let tableRows = data.modifiers
          .map(
            (mod) =>
              `<tr><td><li>${mod.modifierName}</li></td><td class="text-end">${mod.unitPrice}</td></tr>`
          )
          .join("");

        let newModifiersHtml = `
          <div class="row px-4 modifier-group" id="modifierGroupSection-${modifierGroupId}">
              <div class="col-12 my-1 d-flex justify-content-between">
                  <h4 class="mx-4">${groupName}</h4>
                  <button type="button" class="btn-close remove-modifier-group" data-group-id="${modifierGroupId}"></button>
              </div>
              <div class="col-12 d-flex justify-content-center">
                  <input type="number" class="form-control rounded-pill mx-2 min-modifier" name="IMDetails[${modifierGroupId}].MinModifiers" value="${minModifiers}" placeholder="Min Modifiers" min="0">
                  <input type="number" class="form-control rounded-pill mx-2 max-modifier" name="IMDetails[${modifierGroupId}].MaxModifiers" value="${maxModifiers}" placeholder="Max Modifiers" min="0" max="${data.modifiers.length}">
              </div>
              <input type="hidden" name="IMDetails[${modifierGroupId}].IMGid" class="IMGid" value="${itemModifierRelationId}">
              <input type="hidden" name="IMDetails[${modifierGroupId}].ItemId" class="ItemId" value="${itemId}">
              <input type="hidden" name="IMDetails[${modifierGroupId}].MgId" class="MgId" value="${modifierGroupId}"> 
              <div class="col-12 d-flex justify-content-center">
                  <table class="mx-2 w-100">
                      <tbody>${tableRows}</tbody>
                  </table>
              </div>
          </div>`;
        if (itemId > 0) {
          $("#SelectedModifierGroups").append(newModifiersHtml);
        } else {
          $("#SelectedModifierGroupsForAddItem").append(newModifiersHtml);
        }
      },
      error: function (status) {
        if (status.status == 403)
          showToaster(
            "You don't have permission for this functionality",
            "Error"
          );
        else showToaster("Failed to load modifier details. ", "Error");
        $("#deleteItemsModal").modal("hide");
      },
    });
  } else {
    showToaster("This modifier group already exists!", "Warning");
  }
}

$(document).on("input", ".min-modifier, .max-modifier", function () {
  let minField = $(this).closest("div").find(".min-modifier");
  let maxField = $(this).closest("div").find(".max-modifier");
  let modifiersCount = parseInt(maxField.attr("max")) || 0;

  let minValue = parseInt(minField.val()) || 0;
  let maxValue = parseInt(maxField.val()) || 0;

  if (minValue > maxValue) {
    // Show toaster notification
    minField.val("0");
    maxField.val("0");
    showToaster(
      "Minimum Modifiers must be less than or equal to Maximum Modifiers.",
      "Warning"
    );
  }
  if (maxValue > modifiersCount) {
    maxField.val(modifiersCount);
    showToaster(
      "Maximum Modifiers cannot exceed the number of Modifiers (" +
        modifiersCount +
        ").",
      "Warning"
    );
  }
});
$(document).on("change", "#attachModifierGroupforEditItem", function () {
  let modifierGroupId = parseInt($(this).val(), 10);
  let itemId = parseInt($("#ItemIdForEdit").val(), 10);
  if (!isNaN(modifierGroupId)) {
    $(this).prop("disabled", true);
    let modifier = {
      IMGid: 0,
      ItemId: itemId,
      MgId: modifierGroupId,
      MinModifiers: 0,
      MaxModifiers: 0,
    };

    addModifierGroup(modifier);
    $(this).prop("disabled", false);
  }
});

$("#SelectedModifierGroups").on("click", ".remove-modifier-group", function () {
  $("#modifierGroupSection-" + $(this).data("group-id")).remove();
});

$(document).on("click", "#updateItemBtn", function (event) {
  event.preventDefault();
  let modifierGroups = [];
  $("#SelectedModifierGroups > .row").each(function () {
    let groupId = $(this).attr("id").split("-")[1];
    let imGid = $(this).find(".IMGid").val() || 0;
    let itemId = $(this).find(".ItemId").val() || 0;
    let minModifier = $(this).find(".min-modifier").val() || 0;
    let maxModifier = $(this).find(".max-modifier").val() || 0;

    modifierGroups.push({
      IMGid: imGid,
      ItemId: itemId,
      MgId: groupId,
      MinModifiers: minModifier,
      MaxModifiers: maxModifier,
    });
  });
  $("#ItemModifierRelationForEdit").val(JSON.stringify(modifierGroups));
  let formData1 = new FormData($("#editItemForm")[0]);
  let form = $("#editItemForm");
  if (!form.valid()) {
    return;
  }
  $.ajax({
    url: "/Menu/UpdateItem",
    type: "POST",
    data: formData1,
    dataType: "json",
    contentType: false, // Necessary for FormData
    processData: false, // Necessary for FormData
    success: function (response) {
      showToaster(response.message, response.status);
      $("#MenuItems").html(response.data);
      $("#EditItemModal").modal("hide");
    },
    error: function (status) {
      if (status.status == 403)
        showToaster(
          "You don't have permission for this functionality",
          "Error"
        );
      else showToaster("An error occurred while updating item.", "Error");
      $("#EditItemModal").modal("hide");
    },
  });
});

//#endregion

//#region  Modifier Group Section

function highlightModifierGroup(groupId) {
  $(".modifier-group").removeClass("active-category");
  $(".showBtns").addClass("d-none");
  $(`#modifierGroup-${groupId}`).addClass("active-category");
  $(`#modifierGroupBtns-${groupId}`).removeClass("d-none");
}
/* -----------------Add New Group -------------------------*/
$(document).on("click", "#saveModifierGroupBtn", function () {
  let modifierList = [];
  $("#badgesForNewGroup > .badge").each(function () {
    let groupId = $(this).attr("id").split("-")[1];
    modifierList.push(groupId);
  });
  $("#modifiersForAddNewGroup").val(JSON.stringify(modifierList));
  modifierGroup.editorId = $("#editorIdForAddGroup").val();
  modifierGroup.description =
    $("#addModifierGroupDescription").val()?.trim() || null;
  modifierGroup.groupName = $("#addModifierGroupName").val()?.trim();
  modifierGroup.groupId = 0;
  modifierGroup.modifierIds = modifierList;

  if (validateModifierGroupDetails(modifierGroup)) {
    $.ajax({
      url: "/Menu/AddModifierGroup",
      type: "POST",
      data: { modifierGroupDetails: modifierGroup },
      success: function (response) {
        showToaster(response.message, response.status);

        $("#modiferGroupSection").html(response.partialView);
        $("#addModifierGroupModal").hide();
        $(".modal-backdrop").remove();
        hideValidationTextForModifierGroupNameAtAddGroup();
      },
      error: function () {
        showToaster("An error occurred while updating GroupSection.", "Error");
        $("#addModifierGroupModal").hide();
        $(".modal-backdrop").remove();
        hideValidationTextForModifierGroupNameAtAddGroup();
      },
    });
  }
});
/* -----------------Edit Group -------------------------*/
$(document).on("click", "#updateModifierGroupBtn", function () {
  clearListFromAddGroupSection();
  let modifierList = [];
  $("#badgesForEditGroup > .badge").each(function () {
    let groupId = $(this).attr("id").split("-")[1];
    modifierList.push(groupId);
  });
  modifierGroup.editorId = editorId;
  modifierGroup.description =
    $("#editModifierGroupDescription").val()?.trim() || null;
  modifierGroup.groupName = $("#editModifierGroupName").val()?.trim();
  modifierGroup.groupId = $("#editModifierGroupId").val();
  modifierGroup.modifierIds = modifierList;

  if (validateModifierGroupDetails(modifierGroup)) {
    $.ajax({
      url: "/Menu/UpdateModifierGroup",
      type: "POST",
      data: { modifierGroupDetails: modifierGroup },
      success: function (response) {
        showToaster(response.message, response.status);

        $("#modiferGroupSection").html(response.partialView);
        $("#editModifierGroupModal").hide();
        prepareModifierPage();
        $(".modal-backdrop").remove();
        hideValidationTextForModifierGroupNameAtAddGroup();
      },
      error: function () {
        showToaster("An error occurred while updating GroupSection.", "Error");
        $("#editModifierGroupModal").hide();
        $(".modal-backdrop").remove();
        hideValidationTextForModifierGroupNameAtAddGroup();
      },
    });
  }

  function showModifierGroupSectionPartially(groupList) {
    $.ajax({
      url: "/Menu/ShowModifierGroup",
      type: "POST",
      data: { groupList: groupList },
      success: function (respose) {
        $("#modiferGroupSection").html(respose);
      },
      error: function (status) {
        if (status.status == 403)
          showToaster(
            "You don't have permission for this functionality",
            "Error"
          );
        else showToaster("Failed To load Updated Group List", 1);
        $("#editModifierGroupModal").modal("hide");
      },
    });
  }
});
/* -----------------Delete Group -------------------------*/
$(document).on("click", "#deleteModifierGroupBtn", function () {
  let modifierGroupId = $("#modifierGroupIdForDelete").val();
  $.ajax({
    url: "/Menu/DeleteModifierGroup",
    type: "POst",
    data: { modifierGroupId: modifierGroupId, editorId: editorId },
    success: function (result) {
      showToaster(result.message, result.status);
      $("#modiferGroupSection").html(result.partialView);
      prepareModifierPage();
      $(".modal-backdrop").remove();
    },
    error: function () {
      showToaster("An error occurred while updating section.", "Error");
      $("#deleteItemsModal").modal("hide");
      $(".modal-backdrop").remove();
    },
  });
});

function AssignValueForDeleteModifierGroup(modifierGroupId) {
  $("#modifierGroupIdForDelete").val(modifierGroupId);
}
// Event listener for "Select All" checkbox
$(document).on("change", ".select-all-checkbox", function () {
  let table = $(this).closest(".selectable-table");
  let rowCheckboxes = table.find(".row-checkbox");

  rowCheckboxes.prop("checked", this.checked);
  updateSelectAllState(table);
});

$(document).on("click", ".editModifierGroup", function () {
  let modifierGroupId = $(this).data("modifier-group-id");
  let modifierGroupName = $(this).data("modifier-group-name");
  let modifierGroupDescription = $(this).data("modifier-group-description");
  clearListFromUpdateGroupSection();

  $("#editModifierGroupId").val(modifierGroupId);
  $("#editModifierGroupName").val(modifierGroupName);
  $("#editModifierGroupDescription").val(modifierGroupDescription);

  $.ajax({
    url: "/Menu/GetModifiersByGroupId",
    type: "GET",
    data: { groupId: modifierGroupId },
    success: function (result) {
      let modifiers = [].concat(result.modifiers || []);
      modifiers.forEach((modifier) => {
        addModifierBadge(modifier, true);
      });
      showToaster(result.message, result.status);
    },
  });
});
function addModifierBadge(modifier, flag) {
  let modifierId = modifier.id;
  let modifierName = modifier.modifierName;
  if ($("#modifierBadge-" + modifierId).length === 0) {
    let newBadge = `
        <span id="modifierBadge-${modifierId}"
            class="badge rounded-pill bg-white text-dark d-inline-flex border border-secondary align-items-center p-2 m-2 shadow-sm"
            style="transition: box-shadow 0.2s, transform 0.2s;"
            onmouseover="this.style.boxShadow='0 0.5rem 1rem rgba(0,0,0,0.15)'; this.style.transform='scale(1.03)'"
            onmouseout="this.style.boxShadow='0 0.125rem 0.25rem rgba(0,0,0,0.075)'; this.style.transform='scale(1)'">
            ${modifierName}
        <button type="button" class="btn-close ms-2" style="cursor:pointer;" onclick="removeModifierBadge(${modifierId})" aria-label="Remove"></button>
        </span>
      `;
    if (flag) {
      $("#badgesForEditGroup").append(newBadge);
    } else {
      $("#badgesForNewGroup").append(newBadge);
    }
  }
}
function removeModifierBadge(modifierId) {
  $("#modifierBadge-" + modifierId).remove();
}
$(function() {
  let count = $('#modifierCountForAdd');
  let badges = $('#badgesForNewGroup');

  function updateCount() {
      count.text(`Currently added modifiers :- ${badges.children().length}`);
  }
  updateCount();

  // Listen for child changes
  new MutationObserver(updateCount).observe(badges[0], {childList: true});
});


// $(document).on('click', '.existingModiferModalBtn', function () {
//   let badges = $(this).closest('.my-2').nextAll('.modifierBadges').first();
  
//   let addedIds = [];
//   badges.children('span[id^="modifierBadge-"]').each(function () {
//       let id = $(this).attr('id').replace('modifierBadge-', '');
//       addedIds.push(id);
//   });

//   // For each checkbox in the modal list
//   $('#ExsitingModifiersModal input[type="checkbox"]').each(function () {
//       let modifierId = $(this).attr('id');
//       if (addedIds.includes(modifierId)) {
//           $(this).prop('checked', true).prop('disabled', true);
//       } else {
//           $(this).prop('checked', false).prop('disabled', false);
//       }
//   });
// });

function clearListFromAddItems() {
  $("#SelectedModifierGroupsForAddItem").children().remove();
}
function clearListFromAddGroupSection() {
  $("#badgesForNewGroup").children().remove();
}
function clearListFromUpdateGroupSection() {
  $("#badgesForEditGroup").empty();
}

$(document).on("click", ".remove-modifier-group", function () {
  let groupId = $(this).data("group-id");
  $("#modifierGroupSection-" + groupId).remove();
});

function highlightModifiers(modifierGroupId) {
  $(".category-item").removeClass("active-category");
  $(".showBtns").addClass("d-none");

  $(`#modifierGroup-${modifierGroupId}`).addClass("active-category");
  $(`#modifierGroupBtns-${modifierGroupId}`).removeClass("d-none");
}

//#endregion

$(document).on("click", ".modifierDeleteBtn", function () {
  let modifierId = $(this).attr("data-modifier-id");
  $("#deleteModifierBtn").data("modifier-id", modifierId);
});

//#region PAgination For Modifier Section
let paginationForModifierTable = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "",
  sortOrder: "",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
};

function updatePaginationInfoForModifierPage() {
  let paginationDetailsElement = $("#paginationDetailsForModifiersPage");

  paginationForModifierTable.pageSize =
    paginationDetailsElement.data("page-size");
  paginationForModifierTable.pageNumber =
    paginationDetailsElement.data("page-number");
  paginationForModifierTable.sortColumn =
    paginationDetailsElement.data("sort-column");
  paginationForModifierTable.sortOrder =
    paginationDetailsElement.data("sort-order");
  paginationForModifierTable.searchQuery =
    paginationDetailsElement.data("search-query");
  paginationForModifierTable.fromDate =
    paginationDetailsElement.data("from-date");
  paginationForModifierTable.toDate = paginationDetailsElement.data("to-date");
  paginationForModifierTable.totalRecords =
    paginationDetailsElement.data("total-records");
  $("#paginationInfoForModifierTable").text(
    `Showing ${
      (paginationForModifierTable.pageNumber - 1) *
        paginationForModifierTable.pageSize +
      1
    } - ${Math.min(
      paginationForModifierTable.pageNumber *
        paginationForModifierTable.pageSize,
      paginationForModifierTable.totalRecords
    )} of ${paginationForModifierTable.totalRecords}`
  );
}

function getModifiersList() {
  let modiferGroupId =
    $(".modifier-group.active-category").data("group-id") || null;
  getModifiersByGroupId(modiferGroupId);
}

function getModifiersByGroupId(modiferGroupId) {
  $.ajax({
    url: "/Menu/GetModifiersList",
    type: "POST",
    data: {
      modifierGroupId: modiferGroupId,
      paginationDetails: paginationForModifierTable,
    },
    success: function (response) {
      $("#MenuModifiers").html(response);
      highlightModifierGroup(modiferGroupId);
      updatePaginationInfoForModifierPage();
    },
    error: function () {
      showToaster("No Modifiers Found in this Category", "Error");
    },
  });
}
$(document).on("click", "#deleteSelectedModifiers", function () {
  let selectedIds = [];
  $(".row-checkbox:checked").each(function () {
    selectedIds.push($(this).data("moidierid"));
  });

  if (selectedIds.length > 0) {
    $.ajax({
      url: "/Menu/DeleteMultipleModifiers",
      type: "POST",
      data: { ids: selectedIds },
      success: function (response) {
        debouncedFetchItem();
        showToaster(response.message, response.status);
        $("#deleteMultipleModifiersModal").modal("hide");
      },
      error: function (error) {
        showToaster(error, "Error");
      },
    });
  } else {
    showToaster("Please select at least one item to delete.", "Warning");
  }
});

// Search Functionality
$(document).on("input", "#searchModifier", function () {
  let searchValue = $(this).val().trim();
  if(searchValue.length <= 100 && searchValue.length >=2){
    paginationForModifierTable.searchQuery = searchValue;
    paginationForModifierTable.pageNumber = 1;
    debouncedFetchModifiers();
  }
  if($(this).val() == 0){
    paginationForModifierTable.searchQuery = "";
    paginationForModifierTable.pageNumber = 1;
    debouncedFetchModifiers();
  }
});

// Change Modifiers Per Page
$(document).on("change", "#modifiersListPerPage", function () {
  paginationForModifierTable.pageSize = parseInt($(this).val(), 10);
  paginationForModifierTable.pageNumber = 1;
  debouncedFetchModifiers();
});

// Pagination - Previous Button
$(document).on("click", "#prevPageForModifier", function () {
  if (paginationForModifierTable.pageNumber > 1) {
    paginationForModifierTable.pageNumber--;
    debouncedFetchModifiers();
  }
});

// Pagination - Next Button
$(document).on("click", "#nextPageForModifier", function () {
  if (
    paginationForModifierTable.pageNumber *
      paginationForModifierTable.pageSize <
    paginationForModifierTable.totalRecords
  ) {
    paginationForModifierTable.pageNumber++;
    debouncedFetchModifiers();
  }
});
//#endregion

//#region All Ready Existing Modifier's Modal
let paginationModelforEM = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "",
  sortOrder: "",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
};

// Extract pagination details from the hidden element
function updatePaginationInfo() {
  let paginationDetailsElement = $("#paginationDetailsForExistingModifiers");

  paginationModelforEM.pageSize = paginationDetailsElement.data("page-size");
  paginationModelforEM.pageNumber =
    paginationDetailsElement.data("page-number");
  paginationModelforEM.sortColumn =
    paginationDetailsElement.data("sort-column");
  paginationModelforEM.sortOrder = paginationDetailsElement.data("sort-order");
  paginationModelforEM.searchQuery =
    paginationDetailsElement.data("search-query");
  paginationModelforEM.fromDate = paginationDetailsElement.data("from-date");
  paginationModelforEM.toDate = paginationDetailsElement.data("to-date");
  paginationModelforEM.totalRecords =
    paginationDetailsElement.data("total-records");
  $("#paginationInfoAtExistingModifierModal").text(
    `Showing ${
      (paginationModelforEM.pageNumber - 1) * paginationModelforEM.pageSize + 1
    } - ${Math.min(
      paginationModelforEM.pageNumber * paginationModelforEM.pageSize,
      paginationModelforEM.totalRecords
    )} of ${paginationModelforEM.totalRecords}`
  );
}

function GetAllExistingModifers() {
  $.ajax({
    url: "/Menu/GetAllExistingModifers",
    type: "GET",
    data: paginationModelforEM,
    success: function (response) {
      let existingModal = $("#AllModifiersModal");
      if (existingModal) {
        $("#ExsitingModifiersModal .modal-content .modal-body #modifiersTableBody").html(response);
        updatePaginationInfo();
        // Show modal
        if (!existingModal.classList.contains("show")) {
          let myModal = new bootstrap.Modal(existingModal, {
            backdrop: "static",
            keyboard: false,
          });
          myModal.show();
        }
      } else {
        showToaster("Modal with ID 'AllModifiersModal' not found!", "Error");
      }
    },
    error: function () {
      showToaster("Failed to fetch existing modifiers. Please try again.","Error");
    },
  });
}

$(document).on("click", ".existingModiferModalBtn", function () {
  GetAllExistingModifers();
});
// Search Functionality
$(document).on("input", "#modifierSearchBox", function () {
  paginationModelforEM.searchQuery = $(this).val();
  paginationModelforEM.pageNumber = 1;
  GetAllExistingModifers();
});

// Change Items Per Page
$(document).on("change", "#modifiersPerPage", function () {
  paginationModelforEM.pageSize = $(this).val();
  paginationModelforEM.pageNumber = 1;
  GetAllExistingModifers();
});

// Pagination
$(document).on("click", "#prevModifierPage", function () {
  if (paginationModelforEM.pageNumber > 1) {
    paginationModelforEM.pageNumber--;
    GetAllExistingModifers();
  }
});

$(document).on("click", "#nextModifierPage", function () {
  if (
    paginationModelforEM.pageNumber * paginationModelforEM.pageSize <
    paginationModelforEM.totalRecords
  ) {
    paginationModelforEM.pageNumber++;
    GetAllExistingModifers();
  }
});

function populateModifiers() {
  let openModal = $(".modal:visible");
  let callFrom = openModal.attr("id");
  let checkedCheckboxes = $(".row-checkbox:checked");
  let selectedModifiers = [];
  checkedCheckboxes.forEach((modifier) => {
    selectedModifiers.push([
      modifier.getAttribute("data-modifierId"),
      modifier.getAttribute("data-modifierName"),
    ]);
  });

  selectedModifiers.forEach((modifier) => {
    if (document.getElementById("modifierBadge-" + modifier[0]) === null) {
      let newBadge = `
         <span id="modifierBadge-${modifier[0]}" 
            class="badge rounded-pill bg-white text-blue d-inline-flex border border-primary align-items-center p-2 m-2 shadow-sm"
            style="transition: box-shadow 0.2s, transform 0.2s;"
            onmouseover="this.style.boxShadow='0 0.5rem 1rem rgba(11, 147, 189, 0.15)'; this.style.transform='scale(1.03)'"
            onmouseout="this.style.boxShadow='0 0.125rem 0.25rem rgba(29, 140, 204, 0.07)'; this.style.transform='scale(1)'">
           ${modifier[1]}
           <button type="button" class="btn-close" style="cursor:pointer;" onclick="removeModifierBadge(${modifier[0]})" ></button>
         </span>`;
      if (callFrom == "addModifierGroupModal") {
        $("#badgesForNewGroup").append(newBadge);
      } else {
        $("#badgesForEditGroup").append(newBadge);
      }
    } else {
      showToaster(
        "Some of them Are allready addModifierModal into this group!!!",
        "Warning"
      );
    }
  });
}

function removeModifierBadge(modifierId) {
  $("#modifierBadge-" + modifierId).remove();
}

$(function() {
  let count = $('#modifierCountForEdit');
  let badges = $('#badgesForEditGroup');

  function updateCount() {
      count.text(`Currently added modifiers :- ${badges.children().length}`);
  }
  updateCount();

  // Listen for child changes
  new MutationObserver(updateCount).observe(badges[0], {childList: true});
});
//#endregion

//#region  Modifier section
// Update selected count on checkbox change
$(document).on("change", ".modifier-group-checkbox", function () {
  let selectedCount = $(".modifier-group-checkbox:checked").length;
  $("#selectedCount").text(selectedCount); // Update the count in the dropdown button
  $("#editSelectedCount").text(selectedCount);
});

/* -----------------Add New Modifier -------------------------*/
$(document).on("click", "#modifierSaveBtn", function (event) {
  event.preventDefault();

  let selectedGroupIds = $(".modifier-group-checkbox:checked")
    .map(function () {
      return parseInt(this.value, 10);
    })
    .get();
  $("#ModifierGroupHiddenField").val(selectedGroupIds.join(",")); // Save to hidden field for validation

  let form = $("#addModifierForm");
  if (!form.valid()) {
    return;
  }

  // Update the global ModifierVM object
  ModifierVM.ModifierGroupId = selectedGroupIds;
  ModifierVM.ModifierName = $("#newModifierName").val();
  ModifierVM.Description = $("#newModifierDescription").val() || null;
  ModifierVM.UnitPrice = parseFloat($("#newModifierRate").val());
  ModifierVM.Quantity = parseInt($("#newModifierQuantity").val());
  ModifierVM.UnitType = $("#newModifierUnitType").val();
  ModifierVM.EditorId = editorId;

  $.ajax({
    url: "/Menu/AddModifier",
    type: "POST",
    data: { newModifier: ModifierVM },
    success: function (response) {
      showToaster(response.message, response.status);
      $("#MenuModifiers").html(response.partialView);
      $("#AddModifierModal").modal("hide");
    },
    error: function (status) {
      if (status.status == 403)
        showToaster(
          "You don't have permission for this functionality",
          "Error"
        );
      else showToaster("An error occurred while updating section.", "Error");
      $("#AddModifierModal").modal("hide");
    },
  });
});
/* -----------------Edit Modifier -------------------------*/

$(document).on("click", ".EditModifierBtn", function () {
  let modifierId = $(this).data("modifier-id");
  $.ajax({
    url: "/Menu/GetModifierById",
    type: "GET",
    data: { modifierId: modifierId },
    success: function (response) {
      if (response.data) {
        $("#editModifierName")
          .val(response.data.modifierName) // Set the name field value
          .data("modifier-id", response.data.modifierId); // Add modifier ID as a data attribute

        $("#EditModifierGroupHiddenField").val(
          JSON.stringify(response.data.modifierGroupId)
        );
        $("#editModifierRate").val(response.data.unitPrice);
        $("#editModifierQuantity").val(response.data.quantity);
        $("#editModifierUnitType").val(response.data.unitType);
        $("#editModifierDescription").val(response.data.description);
        $("#EditModifierModal").modal("show");
      } else {
        showToaster(response.message, response.status);
      }
    },
    error: function (status) {
      if (status.status == 403)
        showToaster(
          "You don't have permission for this functionality",
          "Error"
        );
      else showToaster("An error occurred while updating section.", "Error");
      $("#EditModifierModal").modal("hide");
    },
  });
});

$(document).on("click", "#modifierUpdateBtn", function (event) {
  event.preventDefault();

  // Get selected Modifier Group IDs
  let selectedGroupIds = $(".modifier-group-checkbox:checked")
    .map(function () {
      return parseInt(this.value, 10); // Parse as integer
    })
    .get();
  $("#EditModifierGroupHiddenField").val(selectedGroupIds.join(",")); // Save to hidden field for validation

  let form = $("#editModifierForm");
  if (!form.valid()) {
    return;
  }

  // Update the global ModifierVM object
  ModifierVM.ModifierId = $("#editModifierName").data("modifier-id"); 
  ModifierVM.ModifierGroupId = selectedGroupIds;
  ModifierVM.ModifierName = $("#editModifierName").val();
  ModifierVM.Description = $("#editModifierDescription").val() || null; 
  ModifierVM.UnitPrice = parseFloat($("#editModifierRate").val()); 
  ModifierVM.Quantity = parseInt($("#editModifierQuantity").val()); 
  ModifierVM.UnitType = $("#editModifierUnitType").val();
  ModifierVM.EditorId = editorId; 

  $.ajax({
    url: "/Menu/UpdateModifier",
    type: "POST",
    data: { editModifier: ModifierVM }, 
    success: function (response) {
      showToaster(response.message, response.status);
      $("#MenuModifiers").html(response.partialView);
      $("#EditModifierModal").modal("hide");
    },
    error: function (status) {
      if (status.status == 403)
        showToaster(
          "You don't have permission for this functionality",
          "Error"
        );
      else showToaster("An error occurred while updating section.", "Error");
      $("#EditModifierModal").modal("hide");
    },
  });
});

/* -----------------Delete Modifier -------------------------*/
$(document).on("click", "#deleteModiferBtn", function () {
  let modifierId = $("#deleteModiferBtn").attr("data-modifier-id");
  $.ajax({
    url: "/Menu/DeleteModifierById",
    type: "POST",
    data: { modifierId: modifierId, editorId: editorId },
    success: function (response) {
      showToaster(response.message, response.status);
      $("#MenuModifiers").html(response.partialView);
      $("#deleteModifierModal").modal("hide");
    },
    error: function (status) {
      if (status.status == 403)
        showToaster(
          "You don't have permission for this functionality",
          "Error"
        );
      else showToaster("An error occurred while updating section.", "Error");
      $("#deleteModifierModal").modal("hide");
    },
  });
});

/* -----------------Multiple Delete Modifier -------------------------*/

$(document).on("click", "#deleteSelectedModifiers", function () {
  let selectedIds = [];
  $(".row-checkbox:checked").each(function () {
    selectedIds.push($(this).data("modifier-id"));
  });
  if (selectedIds.length > 0) {
    $.ajax({
      url: "/Menu/DeleteMultipleModifiers",
      type: "POST",
      data: { modifierIds: selectedModifierIds, editorId: editorId },
      success: function (response) {
        showToaster(response.message, response.status);
        $("#MenuModifiers").html(response.partialView);
        $("#deleteMultipleModifiersModal").modal("hide");
      },
      error: function (status) {
        if (status.status == 403)
          showToaster(
            "You don't have permission for this functionality",
            "Error"
          );
        else
          showToaster("An error occurred while deleting modifiers.", "Error");
        $("#deleteMultipleModifiersModal").modal("hide");
      },
    });
  } else {
    showToaster("Please select at least one item to delete.", "Not Found");
    $("#deleteItemsModal").modal("hide");
  }
});
//#endregion

function prepareModifierPage() {
  highlightModifierGroup(groupIdOfFirstchild);
  debouncedFetchModifiers();
}
//#region Function on Document Load

$(document).ready(function () {
  $("#nav-items-tab").on("click", function () {
    highlightCategory(categoryIdOfFirstchild);
  });
  highlightCategory(categoryIdOfFirstchild);
  updatePaginationInfoForItemPage();
  $("#fileInputAtAddItem").change(function () {
    const file = this.files[0];
    if (file) {
      $("#fileDetailsAtAddItem").removeClass("d-none");
      $("#fileNameAtAddItem").text(file.name);

      if (file.type.startsWith("image/")) {
        const reader = new FileReader();
        reader.onload = function (e) {
          $("#imagePreviewAtAddItem").attr("src", e.target.result).show();
        };
        reader.readAsDataURL(file);
      } else {
        $("#imagePreviewAtAddItem").hide();
      }
    } else {
      $("#fileDetailsAtAddItem").addClass("d-none");
    }
  });

  $("#removeImageButtonAtEditItem").click(function () {
    $("#fileInputAtEditItem").val("");
    $("#fileDetailsAtEditItem").addClass("d-none");
    $("#imagePreviewAtEditItem").hide();
  });

  $("#fileInputAtEditItem").change(function () {
    const file = this.files[0];
    if (file) {
      $("#fileDetailsAtEditItem").removeClass("d-none");
      $("#fileNameAtEditItem").text(file.name);

      if (file.type.startsWith("image/")) {
        const reader = new FileReader();
        reader.onload = function (e) {
          $("#imagePreviewAtEditItem").attr("src", e.target.result).show();
        };
        reader.readAsDataURL(file);
      } else {
        $("#imagePreviewAtEditItem").hide();
      }
    } else {
      $("#fileDetailsAtEditItem").addClass("d-none");
    }
  });

  $("#removeImageButtonAtEditItem").click(function () {
    $("#fileInputAtEditItem").val("");
    $("#fileDetailsAtEditItem").addClass("d-none");
    $("#imagePreviewAtEditItem").hide();
  });

  $("#EditItemModal").on("hidden.bs.modal", function () {
    $("#SelectedModifierGroups").children().remove();
    let form = $("#editItemForm");

    // Clear validation messages
    form.find(".field-validation-error").each(function () {
      $(this).text("");
    });

    // Remove validation classes (invalid styles)
    form.find(".is-invalid").removeClass("is-invalid");
    form.find(".is-valid").removeClass("is-valid");

    form[0].reset(); // Reset all form fields

    $("#fileDetailsAtEditItem").addClass("d-none");
    $("#imagePreviewAtEditItem").hide();
    $("#fileInputAtEditItem").val("");

    // Clear the hidden IMDetails input field if needed
    $("#IMDetails").val("");
  });
  $('#AddItemModal').on('hidden.bs.modal', function () {
    let  form = $('#itemForm');

    form.find('.field-validation-error').each(function () {
        $(this).text('');
    });

    form.find('.is-invalid').removeClass('is-invalid');
    form.find('.is-valid').removeClass('is-valid');

    form[0].reset(); 
    $('#fileDetails').addClass('d-none');
    $('#imagePreview').hide();
    $('#fileInput').val('');

    $('#IMDetails').val('');
});
  $("#AddModifierModal").on("show.bs.modal", function (event) {
    // Fetch group IDs from the hidden field or backend variable
    $("#EditModifierGroupHiddenField").val("");
    $(".modifier-group-checkbox").prop("checked", false);
  });
  $("#EditModifierModal").on("show.bs.modal", function (event) {
    // Fetch group IDs from the hidden field or backend variable
    let hiddenFieldValue = $("#EditModifierGroupHiddenField").val();
    let groupIds = [];

    groupIds = JSON.parse(hiddenFieldValue || "[]");
    // Uncheck all checkboxes first
    $(".modifier-group-checkbox").prop("checked", false);

    // Loop through `groupIds` and check the corresponding checkboxes
    groupIds.forEach((id) => {
      $(`#editModifierGroup_${id}`).prop("checked", true);
    });

    // Update the selected count
    $("#editSelectedCount").text(groupIds.length);
  });
  $("#EditCategoryModal").on("hidden.bs.modal", function () {
    hideValidationTextForCategoryNameAtEditCategory();
  });

  $("#AddCategoryModal").on("hidden.bs.modal", function () {
    hideValidationTextForCategoryNameAtAddCategory();
  });
  $("#addModifierGroupModal").on("hidden.bs.modal", function () {
    hideValidationTextForModifierGroupNameAtAddGroup();
  });
  $("#editModifierGroupModal").on("hidden.bs.modal", function () {
    hideValidationTextForModifierGroupNameAtEditGroup();
  });
});
let modifierGroupDropdown = $('#attachModifierGroup');

  $(document).on("change", "#attachModifierGroup", function () {
      let  modifierGroupId = $(this).val();

      if (modifierGroupId) {

          addModifierGroup({
              IMGid: 0,
              ItemId: 0,
              MgId: modifierGroupId,
              MinModifiers: 0,
              MaxModifiers: 0
          }); 
      }
  });

  $(document).on("click", "#saveItemBtn", function (event) {
      event.preventDefault();   

      let modifierGroups = [];
      $("#SelectedModifierGroupsForAddItem > .row").each(function () {
          let groupId = $(this).attr("id").split("-")[1];
          let minModifier = $(this).find(".min-modifier").val() || 0;
          let maxModifier = $(this).find(".max-modifier").val() || 0;

          modifierGroups.push({ ItemId: 0, MgId: groupId, MinModifiers: minModifier, MaxModifiers: maxModifier });
      });
      let jsonString = JSON.stringify(modifierGroups);
      $("#IMDetails").val(jsonString);
      let form = $('#itemForm');
      if (!form.valid()) {
          return;
      }
      let formData = new FormData($("#itemForm")[0]);
      $.ajax({
          url: '/Menu/AddItem',
          type: "POST",
          data: formData,
          dataType: "json",
          contentType: false,
          processData: false, 
          success: function (response) {
                  $('#AddItemModal').modal('hide');
              showToaster(response.message, response.status);
          },
          error: function (xhr) {
              let errorResponse = JSON.parse(xhr.responseText);
              showToaster(errorResponse.message, errorResponse.status);
          }
      });
  });

  
//#endregion
