let favoritesItem = false;
let categoryId = 0;
let searchQuery = "";
let orderItems = [];
const itemModifierGroupRelationVM = {
  itemId: 0,
  itemName: "",
  uniqueGroupId: "",
  specialInstructions: "",
  unitPrice: 0.0,
  groups: [
    {
      modifierGroupId: 0,
      modifierGroupName: "",
      modifiers: [
        {
          modifierId: 0,
          modifierName: "",
          modifierPrice: 0.0,
        },
      ],
      minRequired: 0,
      maxRequired: 0,
    },
  ],
};
const customer = {
  customerId: 0,
  customerName: "",
  customerPhone: "",
  noOfPerson: 1,
  customerEmail: "",
  totalOrders: 0,
  lastOrder: new Date(),
  editorId: 0,
};
let favItemsDebounceTimer;
const editorId = $("#sidebarAtOrderApp").attr("data-editor-id");
const urlParams = new URLSearchParams(window.location.search);
const orderId = atob(urlParams.get("orderId"));
const isActiveOrder = orderId > 0 && orderId;
//--------------------------------------------------------------- Document Ready--------------------------------------------//
$(document).ready(function () {
  if (isActiveOrder) {
    GetOrderDetails(orderId);
  } else {
    $("#orderPlaceSection").empty();
  }

  getMenuItems(categoryId);
  $("#orderAppMenu li#allItems").addClass("activeCategoryAtOrderApp");

  $("#orderAppMenu li").on("click", function () {
    if ($(this).attr("id") === "favoriteItems") {
      favoritesItem = true;
    } else {
      favoritesItem = false;
    }
    $("#orderAppMenu li").removeClass("activeCategoryAtOrderApp");
    $(this).addClass("activeCategoryAtOrderApp");
  });
  const windowWidth = $(window).width();

  if (windowWidth <= 1060 && isActiveOrder) {
    isMobileView = true;
    $("#openPopupBtn").removeClass("d-none");
    $("#openPopupBtn").trigger("click");
  }
});

//--------------------------------------------------------------- Event For fav icon--------------------------------------------//
$(document).on("click", ".favoriteIcon", function (event) {
  event.stopPropagation(); // Prevent event bubbling
  event.preventDefault();
  const $icon = $(this);
  const isLiked = $icon.hasClass("liked");

  if (isLiked) {
    $icon.removeClass("liked bi-heart-fill").addClass("bi-heart");
    $icon.attr("data-status", "false");
  } else {
    $icon.addClass("liked bi-heart-fill").removeClass("bi-heart");
    $icon.attr("data-status", "true");
  }
  let itemId = $icon.attr("data-item-id");
  if (itemId) {
    if ($(this).attr("data-status") === "true") {
      AddToFavorites(itemId);
    } else {
      RemoveFromFavorites(itemId);
    }
  }
});
//--------------------------------------------------------------- Event For Search--------------------------------------------//
$(document).on("keyup", "#searchInputAtOrderApp", function () {
  searchQuery = $(this).val().trim().toLowerCase();
  if ($("#favoriteItems").hasClass("activeCategoryAtOrderApp")) {
    favoritesItem = true;
  } else {
    favoritesItem = false;
  }
  if (searchQuery.length > 3) {
    getMenuItemsWithFilters(favoritesItem, categoryId, searchQuery);
  }
});
//--------------------------------------------------------------- Click on Fav items--------------------------------------------//
$(document).on("click", "#favoriteItems", function () {
  favoritesItem = true;
  categoryId = 0;
  searchQuery = $("#searchInputAtOrderApp").val().trim().toLowerCase();
  getMenuItemsWithFilters(favoritesItem, categoryId, searchQuery);
});
function AddToFavorites(itemId) {
  $.ajax({
    url: "/OrderAppMenu/AddToFavorites",
    type: "POST",
    data: { itemId, editorId },
    success: function (response) {
      showToaster(response.message, response.status);
    },
    error: function () {
      showToaster("Error At ajax call", "Error");
    },
  });
}
function RemoveFromFavorites(itemId) {
  $.ajax({
    url: "/OrderAppMenu/RemoveFromFavorites",
    type: "POST",
    data: { itemId, editorId },
    success: function (response) {
      showToaster(response.message, response.status);
    },
    error: function () {
      showToaster("Error At ajax call", "Error");
    },
  });
}
//---------------------------------------------------------Get Menu Items----------------------------------------------------------//
function getMenuItems(categoryId) {
  $.ajax({
    url: "/OrderAppMenu/GetMenuItems",
    type: "POST",
    data: { favoritesItem, categoryId, searchQuery },
    success: function (response) {
      if (response.partialView != null) {
        $("#orderAppMenuItemListPartialView").fadeOut(300, function () {
          $(this).html(response.partialView);
          $(this).fadeIn(400);
        });
      } else {
        showToaster(response.message, response.status);
      }
    },
    error: function () {
      showToaster("Error At ajax call", "Error");
    },
  });
}
function getMenuItemsWithFilters(favoritesItem, categoryId, searchQuery) {
  $.ajax({
    url: "/OrderAppMenu/GetMenuItems",
    type: "POST",
    data: { favoritesItem, categoryId, searchQuery },
    success: function (response) {
      if (response.partialView != null) {
        $("#orderAppMenuItemListPartialView").fadeOut(300, function () {
          $(this).html(response.partialView);
          $(this).fadeIn(400);
        });
      } else {
        showToaster(response.message, response.status);
      }
    },
    error: function (error) {
      showToaster("Error At ajax call", "Error");
    },
  });
}
//--------------------------------------------------------Get All Items----------------------------------------------------------//
$(document).on("click", "#orderAppMenu li#allItems", function () {
  getMenuItems(0);
});
//--------------------------------------------------------Get Items per Category----------------------------------------------------------//
$(document).on("click", ".itemCategoryAtOrderApp", function () {
  categoryId = $(this).data("category-id");
  getMenuItems(categoryId);
});

//--------------------------------------------------------Get Item - Modifier Relations----------------------------------------------------------//
$(document).on("click", ".menuItem", function () {
  let itemId = $(this).data("item-id");
  if (itemId) {
    $.ajax({
      url: "/OrderAppMenu/GetMenuItemMapping",
      type: "Get",
      data: { itemId },
      success: function (response) {
        if (response.message == null) {
          $("#modalPlaceholder").html(response);
          if ($("#ItemModifierRelationModal #emptyModal").length > 0) {
            $("#addToLocalListBtn").trigger("click"); // Trigger the "Add" button
            showToaster("Item Don't have any modifier group", "Success");
          } else {
            $("#ItemModifierRelationModal").modal("show");
            if (!(orderId > 0 && orderId)) {
              $("#modalFooterForMapping").empty();
            }
          }
        } else {
          showToaster(response.message, response.status);
        }
      },
      error: function () {
        showToaster("Error At ajax call", "Error");
      },
    });
  } else {
    showToaster("Item ID not found", "Error");
  }
});
$("#openPopupBtn").on("click", function () {
  $("#customPopup").css("display", "flex");
});

$("#closePopupBtn").on("click", function () {
  $("#customPopup").css("display", "none");
});
let isMobileView = null;

$(window).on("resize", function () {
  const isNowMobile = $(window).width() <= 1060;
  let customPopup = $("#customPopup");
  let orderPlacePopupBody = $("#orderPlacePopupBody");
  let orderPlaceSection = $("#orderPlaceSection");
  let openPopupBtn = $("#openPopupBtn");
  if (isMobileView === isNowMobile) return;

  if (isActiveOrder) {
    if (isNowMobile) {
      orderPlacePopupBody.html(orderPlaceSection.html());
      openPopupBtn.removeClass("d-none");
      orderPlaceSection.hide();
      $("#openPopupBtn").trigger("click");
      if (customPopup.is(":hidden")) {
        orderPlacePopupBody.empty();
      }
    } else {
      if (isMobileView != null)
        orderPlaceSection.html(orderPlacePopupBody.html());
      orderPlaceSection.show();
      openPopupBtn.addClass("d-none");
      if (customPopup.is(":visible")) {
        customPopup.hide();
      }
    }
  }

  isMobileView = isNowMobile;
});
$(document).on('click', '#categoryMenuBtn', function() {
  $('#sidebarAtOrderApp').addClass('sidebar-overlay active').removeClass('d-none');
  $('#sidebarOrderBackdrop').addClass('active');
});

// Hide sidebar overlay and backdrop when clicking backdrop
$(document).on('click', '#sidebarOrderBackdrop', function() {
  $('#sidebarAtOrderApp').removeClass('active');
  $('#sidebarOrderBackdrop').removeClass('active');
  setTimeout(function() {
    $('#sidebarAtOrderApp').addClass('d-none').removeClass('sidebar-overlay');
  }, 300); // match the transition duration
});
function handleOrderSidebarOnResize() {
  if ($(window).width() < 992) {
    $('#sidebarAtOrderApp').addClass('d-none').removeClass('sidebar-overlay active');
    $('#sidebarOrderBackdrop').removeClass('active');
  } else {
    $('#sidebarAtOrderApp').removeClass('d-none sidebar-overlay active');
    $('#sidebarOrderBackdrop').removeClass('active');
  }
}
$(window).on('resize', handleOrderSidebarOnResize);
$(function() { handleOrderSidebarOnResize(); });
// ----------------------------------------------------------Function to load the partial view ----------------------------------------------//
function GetOrderDetails(orderId) {
  $.ajax({
    url: "/OrderAppMenu/OrderDetails",
    type: "GET",
    data: { orderId: orderId },
    success: function (response) {
      if (response.message == null) {
        if ($(window).width() > 1060) {
          $("#orderPlaceSection").html(response);
        } else {
          $("#orderPlacePopupBody").html(response);
        }
        setUniqueIdsForOrderList();
        updatePriceSection(orderId);
        checkOrderStatus();
        setInvoiceButton();
      } else {
        showToaster(response.message, response.status);
      }
    },
    error: function () {
      showToaster("An error occurred while getting order details", "Error");
    },
  });
}

//--------------------------------------------------------Check Order Status----------------------------------------------------------//
// Function to check the order status and update the UI accordingly
function checkOrderStatus() {
  const orderStatus = $("#saveBtnForPlacedOrder").attr("data-status");
  if (orderStatus === "Completed") {
    markOrderSectionComplete();
  } else if (orderStatus === "Cancelled") {
    markOrderSectionCancel();
  }
}
function setInvoiceButton() {
  const invoiceButton = $("#generateInvoiceBtn");
  let orderStatus = $("#saveBtnForPlacedOrder").data("status");

  if (orderStatus === "Completed" || orderStatus === "Cancelled") {
    // Enable the button
    invoiceButton.attr("asp-controller", "Orders");
    invoiceButton.attr("asp-action", "DownloadInvoice");
    invoiceButton.attr("asp-route-orderId", orderId);
    invoiceButton.prop("disabled", false);
  } else {
    invoiceButton.prop("disabled", true);
  }
}
$(document).on("click", "#generateInvoiceBtn", function () {
  const invoiceUrl = $(this).attr("asp-route-orderid"); // Get the order ID from the button
  if (invoiceUrl > 0 && invoiceUrl) {
    const redirectUrl = `/Orders/DownloadInvoice?orderId=${invoiceUrl}`;
    window.location.href = redirectUrl;
  }
});
// --------------------------------------------------------Manupulate Modifier Bades UI----------------------------------------------------------//
$(document).on("click", ".option-button", function () {
  $(this).toggleClass("active");
});

//------------------------------------------------------------- "Add to Order" button -----------------------------------------------------//
function setUniqueIdsForOrderList() {
  $("#orderListSectionInOrderApp")
    .find(".itemInList")
    .each(function () {
      const itemDetails = $(this).attr("data-item-details");
      const itemModifierMapping = JSON.parse(itemDetails);
      const uniqueKey = generateUniqueKey(itemModifierMapping);
      $(this).attr("data-item-unique-key", uniqueKey);
      $(this)
        .find("td .inputbtns .decrement-quantity")
        .attr("data-item-unique-key", uniqueKey);
      $(this)
        .find("td .inputbtns .quantity-input")
        .attr("data-item-unique-key", uniqueKey);
      $(this)
        .find("td .inputbtns .increment-quantity")
        .attr("data-item-unique-key", uniqueKey);
      $(this).find(".totalPrice").attr("data-item-unique-key", uniqueKey);
      $(this)
        .find('button[data-bs-toggle="collapse"]')
        .attr("data-bs-target", `#itemAtOrderList-${uniqueKey}`);
      $(this).find(".collapse").attr("id", `itemAtOrderList-${uniqueKey}`);
    });
}

$(document).on("click", "#addToLocalListBtn", function () {
  let isValid = true;
  // Clone the global variable to avoid modifying the original structure
  let item = JSON.parse(JSON.stringify(itemModifierGroupRelationVM));
  const itemInfo = $("#ItemModifierRelationModalLabel");

  item.itemId = parseInt(itemInfo.attr("data-item-id"), 10);
  item.itemName = itemInfo.attr("data-item-name");
  item.unitPrice = parseFloat(itemInfo.attr("data-item-price"));

  // Clear the groups array
  item.groups = [];

  $(".modifierGroupContainer").each(function () {
    const groupContainer = $(this);

    let group = {
      modifierGroupId: parseInt(
        groupContainer.attr("data-modifier-group-id"),
        10
      ),
      modifierGroupName: groupContainer.attr("data-modifier-group-name"),
      modifiers: [],
      minRequired: parseInt(groupContainer.attr("data-min-required"), 10),
      maxRequired: parseInt(groupContainer.attr("data-max-required"), 10),
    };

    // Collect selected modifiers within the group
    groupContainer.find(".modifierContainer").each(function () {
      const modifierContainer = $(this);

      // Assuming active modifiers have a class "active"
      if (modifierContainer.hasClass("active")) {
        let modifier = {
          modifierId: parseInt(modifierContainer.attr("data-modifier-id"), 10),
          modifierName: modifierContainer.attr("data-modifier-name"),
          modifierPrice: parseFloat(
            modifierContainer.attr("data-modifier-price")
          ),
        };

        group.modifiers.push(modifier);
      }
    });

    // Validation: Check if the number of selected modifiers is within the range
    if (group.modifiers.length < group.minRequired) {
      isValid = false;
      showToaster(
        `In "${group.modifierGroupName}" group, you must select atleast ${group.minRequired} modifiers.`
      );
      return false; // Exit the loop early
    }
    if (group.modifiers.length > group.maxRequired) {
      isValid = false;
      showToaster(
        `In "${group.modifierGroupName}" group, you can select atmost ${group.minRequired} modifiers.`
      );
      return false; // Exit the loop early
    }

    // Add the group to the item's groups if valid
    if (group.modifiers.length > 0) {
      item.groups.push(group);
    }
  });

  if (!isValid) {
    return;
  }
  addOrUpdateItemInOrderList(item);
});
function extractItemIdFromUniqueId(uniqueId) {
  const itemId = uniqueId.split("-")[0];
  return parseInt(itemId, 10);
}
function addOrUpdateItemInOrderList(item) {
  let itemExists = false;
  const itemModifierMapping = getItemAndModifiers(item);
  const uniqueKey = generateUniqueKey(itemModifierMapping);
  $("#orderListSectionInOrderApp .itemInList").each(function () {
    let existingUniqueId = $(this).attr("data-item-unique-key");
    if (existingUniqueId === uniqueKey) {
      $(this)
        .find(`.increment-quantity[data-item-unique-key="${existingUniqueId}"]`)
        .trigger("click");
      $("#ItemModifierRelationModal").modal("hide");
      itemExists = true;
      return;
    }
  });

  if (!itemExists) {
    renderOrderList(item);
  }
  updatePriceSection(orderId);
}

//--------------------------------------------------------Helpers to show items in orderApp -----------------------------------------------//
function renderOrderList(orderItem) {
  let tableBodyHtml = createHTMLForItemRow(orderItem);

  $("#ItemModifierRelationModal").modal("hide");
  $("#orderListSectionInOrderApp").append(tableBodyHtml);
  $("#orderListSectionInOrderApp").fadeIn(400);
  const itemModifierMapping = getItemAndModifiers(orderItem);
  const uniqueKey = generateUniqueKey(itemModifierMapping);
  updateItemPrice(uniqueKey);
  updatePriceSection(orderId);
}
// Helper Function: Generate a unique key using itemId and modifiers
function generateUniqueKey(item) {
  const hash = generate5NumberHash(item.modifiers);
  return `${item.itemId}-${hash}`;
}
// Helper Function: Generate a 5-digit hash from modifiers
function generate5NumberHash(modifiers) {
  modifiers = Array.isArray(modifiers) ? modifiers.sort() : [];
  const jsonString = JSON.stringify(modifiers);
  let hash = 0;
  for (let i = 0; i < jsonString.length; i++) {
    hash += jsonString.charCodeAt(i);
  }
  return (hash % 100000).toString().padStart(5, "0");
}

function createHTMLForItemRow(item) {
  // Extract item and modifiers
  const itemModifierMapping = getItemAndModifiers(item);
  const modifiersForPriceCalculation = itemModifierMapping.modifiers || [];

  // Generate a unique key for the item based on itemId and modifiers
  const uniqueKey = generateUniqueKey(itemModifierMapping);
  // Calculate total price of modifiers
  const modifiersSum = calculateModifiersSum(modifiersForPriceCalculation);

  // Generate HTML for the full row
  return `
    <tr data-item-unique-key="${uniqueKey}" data-item-details='${JSON.stringify(
    itemModifierMapping
  )}' class="itemInList">
      <!-- Item Name and Modifiers -->
      <td>
        ${generateItemNameHtml(item, modifiersForPriceCalculation, uniqueKey)}
      </td>
      <!-- Quantity Controls -->
      <td class="text-center">
        ${generateQuantityControlsHtml(
          item,
          modifiersForPriceCalculation,
          uniqueKey
        )}
      </td>
      <!-- Price Details -->
      <td class="text-end totalPrice" data-item-unique-key="${uniqueKey}">
        ${generatePriceDetailsHtml(item.unitPrice, modifiersSum)}
      </td>
      <!-- Remove Button -->
      <td class="text-center">
        ${generateRemoveButtonHtml(uniqueKey)}
      </td>
    </tr>
  `;
}

// Helper Function: Calculate total price of modifiers
function calculateModifiersSum(modifiers) {
  return modifiers.reduce(
    (sum, modifier) => sum + (modifier.modifierPrice || 0),
    0
  );
}

// Helper Function: Generate HTML for item name and modifiers
function generateItemNameHtml(item, modifiers, uniqueKey) {
  const modifiersHtml = generateModifiersHtml(modifiers);
  return `
    <button class="btn btn-sm p-0 me-2" data-bs-toggle="collapse" data-bs-target="#itemAtOrderList-${uniqueKey}">
      <i class="fas fa-chevron-down"></i>
    </button>
    <span class="orderItemAtOrderApp savedItem" data-bs-toggle="modal" data-bs-target="#commentModal" data-item-unique-key="${uniqueKey}">
      ${item.itemName}
    </span>
    <div class="collapse mt-1" id="itemAtOrderList-${uniqueKey}">
      ${modifiersHtml}
    </div>
  `;
}

// Helper Function: Generate HTML for modifiers
function generateModifiersHtml(modifiers) {
  if (modifiers.length === 0) {
    return `<div class="text-muted small ms-4">No modifiers available</div>`;
  }

  return modifiers
    .map(
      (modifier) => `
    <div class="text-muted small ms-4" data-modifier-name="${modifier.modifierName}">
      • ${modifier.modifierName}
      <span class="float-end" data-modifier-unitprice="${modifier.modifierPrice}">₹${modifier.modifierPrice}</span>
    </div>
  `
    )
    .join("");
}

// Helper Function: Generate HTML for quantity controls
function generateQuantityControlsHtml(
  item,
  modifiersForPriceCalculation,
  uniqueKey
) {
  return `
    <div class="input-group input-group-sm justify-content-center" style="width: 100px;">
      <button class="btn btn-outline-secondary btn-sm decrement-quantity" data-item-unique-key="${uniqueKey}">-</button>
      <input type="number" class="form-control text-center quantity-input" 
             data-item-modifiers='${JSON.stringify(
               modifiersForPriceCalculation
             )}' 
             data-item-base-price="${item.unitPrice}" 
             value="${item.quantity || 1}" 
             min="${item.quantity || 1}" 
             data-item-unique-key="${uniqueKey}">
      <button class="btn btn-outline-secondary btn-sm increment-quantity" data-item-unique-key="${uniqueKey}">+</button>
    </div>
  `;
}

// Helper Function: Generate HTML for price details
function generatePriceDetailsHtml(itemPrice, modifiersSum) {
  return `
    <div class="d-flex flex-column">
      <span class="fw-bold">₹${(itemPrice + modifiersSum).toFixed(2)}</span>
      <span class="text-muted">₹${modifiersSum}</span>
    </div>
  `;
}

// Helper Function: Generate HTML for the remove button
function generateRemoveButtonHtml(uniqueKey) {
  return `
    <button class="btn btn-sm btn-outline-danger remove-item" data-item-unique-key="${uniqueKey}">
      <i class="bi bi-trash3 text-black"></i>
    </button>
  `;
}

function getItemAndModifiers(itemModifierGroupRelationVM) {
  // Extract item-level details
  const itemDetails = {
    itemId: itemModifierGroupRelationVM.itemId,
    itemName: itemModifierGroupRelationVM.itemName,
    unitPrice: itemModifierGroupRelationVM.unitPrice,
    modifiers: [],
  };

  // Collect all modifiers from all groups
  itemModifierGroupRelationVM.groups.forEach((group) => {
    group.modifiers.forEach((modifier) => {
      itemDetails.modifiers.push({
        modifierId: modifier.modifierId,
        modifierName: modifier.modifierName,
        modifierPrice: modifier.modifierPrice,
      });
    });
  });
  return itemDetails;
}
function getModifiersPrice(uniqueKey) {
  let totalModifiersPrice = 0;

  // Select the collapse container for the given itemId
  const modifiersContainer = $(`#itemAtOrderList-${uniqueKey}`);
  modifiersContainer.find("[data-modifier-unitprice]").each(function () {
    const unitPrice = parseFloat($(this).attr("data-modifier-unitprice")) || 0;
    totalModifiersPrice += unitPrice;
  });

  return totalModifiersPrice;
}
$(document).on("click", ".remove-item", function () {
  $(this).closest("tr").remove();
  updatePriceSection(orderId);
});

//#region Helpers to increment decrement items in orderApp

$(document).on(
  "click change",
  ".decrement-quantity, .increment-quantity, .quantity-input",
  function () {
    const isButton =
      $(this).hasClass("decrement-quantity") ||
      $(this).hasClass("increment-quantity");
    const uniqueKey = $(this).attr("data-item-unique-key");
    const inputField = $(
      `.quantity-input[data-item-unique-key="${uniqueKey}"]`
    ); // Target the input field
    const trElement = $(`.itemInList[data-item-unique-key="${uniqueKey}"]`); // Target the row
    const minValue = parseInt(inputField.attr("min"), 10) || 1;
    let currentValue = parseInt(inputField.val(), 10) || minValue;

    if (isButton) {
      // Handle button clicks for increment or decrement
      if ($(this).hasClass("decrement-quantity") && currentValue > minValue) {
        currentValue -= 1;
      } else if ($(this).hasClass("increment-quantity")) {
        currentValue += 1;
      }
      inputField.val(currentValue);
    } else {
      const inputValue = parseInt(inputField.val(), 10);
      currentValue =
        isNaN(inputValue) || inputValue < minValue ? minValue : inputValue;
      inputField.val(currentValue);
    }

    // If the current value equals the min value, trigger the decrement button and remove item
    if (currentValue === minValue) {
      const decrementButton = $(
        `.decrement-quantity[data-item-unique-key="${uniqueKey}"]`
      );
      decrementButton.trigger("click");

      // Trigger the remove-item button automatically
      const removeButton = $(
        `.remove-item[data-item-unique-key="${uniqueKey}"]`
      );
      removeButton.trigger("click");
    }

    // Update the model in the data-item-details attribute
    const currentItemDetails = JSON.parse(trElement.attr("data-item-details"));
    currentItemDetails.quantity = currentValue;
    trElement.attr("data-item-details", JSON.stringify(currentItemDetails));
    updateItemPrice(uniqueKey);
  }
);

function updateItemPrice(uniqueKey) {
  // Select quantity input and total price cell
  const quantityInput = $(
    `.quantity-input[data-item-unique-key="${uniqueKey}"]`
  );
  const totalPriceCell = $(`.totalPrice[data-item-unique-key="${uniqueKey}"]`);

  const modifiersPrice = getModifiersPrice(uniqueKey);

  // Create an item object for calculations
  const item = {
    uniqueKey: uniqueKey,
    itemPrice: parseFloat(quantityInput.attr("data-item-base-price")) || 0, // Base price
    quantity: parseInt(quantityInput.val()) || 1, // Current quantity
    modifiersPrice: modifiersPrice,
  };
  const totalPriceHtml = `
    <div class="d-flex flex-column">
      <span class="fw-bold">₹${(
        (item.itemPrice + modifiersPrice) *
        item.quantity
      ).toFixed(2)}</span>
      <span class="text-muted">₹${modifiersPrice.toFixed(2)}</span>
    </div>
  `;

  totalPriceCell.html(totalPriceHtml); // Update the cell's HTML
}

//#endregion

function updatePriceSection(orderId) {
  const items = getAppendedItems();
  const subTotalElement = $(`.priceSection #subTotal-${orderId}`);
  let subTotal = 0;
  console.log(items);
  // Loop through each item to calculate subtotal
  items.forEach((item) => {
    // let itemModifierMapping = item;
    const uniqueKey = generateUniqueKey(item);
    const modifiersPrice = item.modifiers
      ? item.modifiers.reduce(
          (sum, modifier) => sum + (modifier.modifierPrice || 0),
          0
        )
      : 0;
    const inputField = $(
      `.quantity-input[data-item-unique-key="${uniqueKey}"]`
    );

    const quantity = parseInt(inputField.val(), 10) || 1;
    subTotal += (item.unitPrice + modifiersPrice) * quantity;
  });

  // Update subtotal in the UI
  subTotalElement.attr("data-sub-total", subTotal.toFixed(2));
  subTotalElement.find("span:last").text(`₹${subTotal.toFixed(2)}`);

  // Calculate and update taxes
  let totalTax = 0;
  $(".taxisAtOrderAppMenu").each(function () {
    const taxValue = parseFloat($(this).attr("data-tax-value")) || 0;
    const taxType = $(this).attr("data-tax-type");

    let taxAmount = 0;
    if (taxType === "Percentage") {
      taxAmount = (subTotal * taxValue) / 100;
    } else if (taxType === "Flat Amount") {
      taxAmount = taxValue;
    }

    totalTax += taxAmount;

    // Update the tax amount in the UI
    $(this)
      .find("span:last")
      .text(`₹${taxAmount.toFixed(2)}`);
  });

  // Calculate and update the total amount
  const totalAmount = subTotal + totalTax;
  const totalAmountElement = $(`#totalAmount-${orderId}`);
  totalAmountElement.data("total-amount", totalAmount.toFixed(2));
  totalAmountElement.find("span:last").text(`₹${totalAmount.toFixed(2)}`);
}
function getAppendedItems() {
  const items = [];
  $("#orderListSectionInOrderApp")
    .find(".itemInList")
    .each(function () {
      const itemData = $(this).attr("data-item-details");
      let item = JSON.parse(itemData);
      items.push(item);
    });
  return items;
}
// Event listener for incrementing or decrementing quantities
$(document).on(
  "click",
  ".increment-quantity, .decrement-quantity",
  function () {
    updatePriceSection(orderId);
  }
);
// Event listener for manual quantity input changes
$(document).on("change", ".quantity-input", function () {
  updatePriceSection(orderId);
});

// Event listener for the "Place Order" button

$(document).on("click", "#saveBtnForPlacedOrder", function () {
  // Initialize an object to store the order data
  const orderData = {
    orderId: $(".priceSection").data("order-id"),
    subTotal: $(`#subTotal-${$(".priceSection").data("order-id")}`).data(
      "sub-total"
    ),
    totalAmountToPay: $(
      `#totalAmount-${$(".priceSection").data("order-id")}`
    ).data("total-amount"),
    paymentMethod: $('input[name="paymentMethod"]:checked').attr("id"),
    orderItems: [],
  };

  $("#orderListSectionInOrderApp .itemInList").each(function () {
    const itemData = $(this).attr("data-item-details");

    let item = JSON.parse(itemData);
    const uniqueKey = $(this).attr("data-item-unique-key");
    const quantity = $(`.quantity-input[data-item-unique-key="${uniqueKey}"]`)
      .val()
      .trim();
    (item.uniqueGroupId = uniqueKey),
      (item.quantity = parseInt(quantity, 10) || 1);
    orderData.orderItems.push(item);
  });

  $.ajax({
    url: "/OrderAppMenu/UpdateOrder",
    type: "POST",
    data: { order: orderData },
    success: function (response) {
      if (response.status === "Success" || response.status === 2) {
        showToaster(response.message, response.status);
        setTimeout(function () {
          window.location.reload(); // Reload after 3 seconds
        }, 1000);
      } else {
        showToaster(response.message, response.status);
      }
    },
    error: function () {
      showToaster("An error occur during ajax call", "Error");
    },
  });
});
$(document).on("click", ".statusBtnsOfBill", function () {
  let orderStatus = "InProgress";
  let modal = $(this).closest(".modal");
  let modalId = modal.attr("id");
  if (modalId === "completeConfirmationModal") {
    $("#customerReviewModal").modal("show");
  } else if (modalId === "CancelConfirmationModal") {
    orderStatus = "Cancelled";
    UpdateOrderStatus(orderStatus);
  }
});

function UpdateOrderStatus(orderStatus) {
  $.ajax({
    url: "/OrderAppMenu/UpdateOrderStatus",
    type: "POST",
    data: { orderStatus: orderStatus, orderId: orderId, editorId: editorId },
    success: function (response) {
      showToaster(response.message, response.status);
      window.location.reload();
      $("#saveBtnForPlacedOrder").prop("disabled", true);
    },
    error: function () {
      showToaster("An error occured at ajax call", "Error");
    },
  });
}
$(document).on("click", ".closeReviewModal", function () {
  UpdateOrderStatus("Completed");
});
$(document).on("click", "#saveReview", function () {
  const customerId = $("#customerIdReview").val();
  const foodRating = $("#foodRating").val();
  const serviceRating = $("#serviceRating").val();
  const ambienceRating = $("#ambienceRating").val();
  const comment = $("#reviewComment").val();

  const reviewData = {
    customerId: customerId,
    orderId: orderId,
    foodRating: foodRating,
    serviceRating: serviceRating,
    ambienceRating: ambienceRating,
    comment: comment,
  };

  $.ajax({
    url: "/OrderAppMenu/SaveCustomerReview",
    type: "POST",
    data: { reviewData },
    success: function (response) {
      showToaster(response.message, response.status);
    },
    error: function () {
      showToaster("An error occured at ajax call", "Error");
    },
  });
});
$(document).on("click", ".star-rating-static .star", function () {
  const $this = $(this);
  const starValue = $this.data("value");
  const $parentDiv = $this.closest(".star-rating-static");
  const category = $parentDiv.data("category");

  $parentDiv.find(".star").removeClass("highlighted");
  $parentDiv.find(".star").each(function () {
    if ($(this).data("value") <= starValue) {
      $(this).addClass("highlighted");
    }
  });

  $(`#${category}Rating`).val(starValue);
});
/*------------------------- QR View ------------------------------*/
$(function () {
  $(document).on("shown.bs.modal", "#qrModal", function () {
    // Extract orderId from the current URL
    const currentURL = window.location.href;
    const urlParams = new URLSearchParams(currentURL.split("?")[1]);
    const encodedOrderId = urlParams.get("orderId");

    if (!encodedOrderId) {
      console.error("Order ID is missing or invalid in the URL.");
      return;
    }

    let orderId;
    try {
      orderId = atob(encodedOrderId); // Decode the Base64-encoded orderId
    } catch (error) {
      console.error("Failed to decode the Base64-encoded Order ID:", error);
      return;
    }

    // Construct the links dynamically
    const invoiceLink = `http://localhost:5253/Orders/DownloadInvoice?orderId=${orderId}`;
    const customerMenuLink = "http://localhost:5253/OrderAppMenu/CustomersMenu";
    const managerMenuLink = currentURL;

    // Show loader and hide QR codes initially
    $(".qr-loader").show();
    $("#staticQR").addClass("d-none");
    $("#pdfQR").addClass("d-none");
    $("#menuQR").addClass("d-none");

    // Function to generate and display QR codes
    const generateQRCode = (selector, data) => {
      const qrLoader = $(selector).siblings(".qr-loader");
      $(selector)
        .attr(
          "src",
          "https://api.qrserver.com/v1/create-qr-code/?data=" +
            encodeURIComponent(data) +
            "&size=200x200"
        )
        .off("load")
        .on("load", function () {
          $(this).removeClass("d-none");
          qrLoader.hide();
        })
        .on("error", function () {
          console.error(`Failed to load QR code for ${data}`);
          qrLoader.hide();
        });
    };

    generateQRCode("#staticQR", invoiceLink); // Invoice QR Code
    generateQRCode("#menuQR", customerMenuLink); // Customer Menu QR Code
    generateQRCode("#pdfQR", managerMenuLink); // Manager Menu QR Code

    // Click handlers for downloading or opening links
    $("#staticQR")
      .off("click")
      .on("click", function () {
        $("<a>")
          .attr("href", invoiceLink)
          .attr("download", "Pizzashop_bill.pdf")
          .get(0)
          .click();
      });

    $("#menuQR")
      .off("click")
      .on("click", function () {
        window.open(customerMenuLink, "_blank");
      });

    $("#pdfQR")
      .off("click")
      .on("click", function () {
        window.open(managerMenuLink, "_blank");
      });
  });
});
/*------------------------- Comment Modal ------------------------------*/
$(document).on("input", "#orderComment", function () {
  const charCount = $("#charCount");
  charCount.text($(this).val().length);
});

$(document).on("click", ".specialInstructions", function () {
  const trElement = $(this).closest("tr[data-item-unique-key]");
  const uniqueId = trElement.attr("data-item-unique-key");

  $("#commentModalLabel").text("Special Instruction");
  $("#orderComment").val("");
  $("#saveCommentBtn").attr("id", "saveSpecialInstructions");
  $("#orderComment").attr("data-triggeredButton", uniqueId);
});
$(document).on("click", "#OrderCommentBtn", function () {
  $("#commentModalLabel").text("Order Wise Comment");
  $("#saveSpecialInstructions").attr("id", "saveCommentBtn");
  $("#orderComment").val("");
});

$(document).on("click", "#saveCommentBtn", function () {
  const comment = $("#orderComment").val();
  $("#orderWiseComment").val(comment);
});
$(document).on("click", "#saveSpecialInstructions", function () {
  const specialInstructions = $("#orderComment").val();
  const buttonUniqueId = $("#orderComment").attr("data-triggeredbutton");
  const row = $(`tr[data-item-unique-key=${buttonUniqueId}]`);
  let item = JSON.parse(row.attr("data-item-details"));
  item.specialInstructions = specialInstructions;
  row.attr("data-item-details", JSON.stringify(item));
  $("#commentModal").modal("hide");
});

/*------------------------- Cutomer Details ------------------------------*/
$(function () {
  $("#EditCustomerModal").on("shown.bs.modal", function () {
    const editCustomerModalBtn = $("#EditCustomerModalBtn");
    const customerData = editCustomerModalBtn.attr("data-customer-info");
    let customer = JSON.parse(customerData);

    $("#EditCustomerName").attr("data-customer-id", customer.customerId);
    $("#EditCustomerName").val(customer.customerName);
    $("#EditCustomerPhone").val(customer.customerPhone);
    $("#EditCustomerEmail").val(customer.customerEmail);
    $("#EditCustomerNoOfPersons").val(customer.noOfPerson);
  });
});

$(document).on("click", "#updateSectionBtn", function (e) {
  e.preventDefault();
  let form = $("#EditCustomerForm");
  if (!form.valid()) {
    return;
  }

  customer.customerId = $("#EditCustomerName").attr("data-customer-id");
  customer.customerEmail = $("#EditCustomerEmail").val();
  customer.customerName = $("#EditCustomerName").val();
  customer.noOfPerson = $("#EditCustomerNoOfPersons").val();
  customer.customerPhone = $("#EditCustomerPhone").val();
  customer.editorId = editorId;

  $.ajax({
    url: "/Customers/UpdateCustomer",
    type: "POST",
    data: { customer: customer },
    success: function (response) {
      showToaster(response.message, response.status);
      $("#EditCustomerModal").modal("hide");
    },
    error: function () {
      showToaster("An error occured at ajax call", "Error");
    },
  });
});

/*------------------------- Order Manage ------------------------------*/

function markOrderSectionComplete() {
  const $card = $(".order-details");

  // Avoid reapplying if already stamped
  if ($card.find(".completed-overlay").length) return;

  // Disable all inputs, textareas, selects, and buttons except the target button
  $card
    .find("input, textarea, select, button")
    .not(".btn-show")
    .prop("disabled", true);
  $card.find("[data-bs-toggle]").not(".btn-show").removeAttr("data-bs-toggle");

  const overlayHtml = `
      <div class="bill-overlay completed-overlay d-flex justify-content-center align-items-center">
          <div class="stamp-text stamp-text-completed">✔ COMPLETED</div>
      </div>
  `;
  showOnlyInvoiceQR();
  setInvoiceButton();
  $card.css("position", "relative").append(overlayHtml);
}
function markOrderSectionCancel() {
  const $card = $(".order-details");
  if ($card.find(".cancelled-overlay").length) return;
  $card
    .find("input, textarea, select, button")
    .not(".btn-show")
    .prop("disabled", true);
  $card.find("[data-bs-toggle]").not(".btn-show").removeAttr("data-bs-toggle");

  const overlayHtml = `
      <div class="bill-overlay cancelled-overlay d-flex justify-content-center align-items-center">
          <div class="stamp-text stamp-text-cancelled">✘ CANCELLED</div>
      </div>
  `;
  showOnlyInvoiceQR();
  setInvoiceButton();
  $card.css("position", "relative").append(overlayHtml);
}

function showOnlyInvoiceQR() {
  // Get the carousel element
  const $carousel = $("#qrCarousel");

  // Hide all slides except the "Invoice" slide
  $carousel.find(".carousel-item").each(function () {
    const $slide = $(this);
    const isInvoiceSlide = $slide.find("h6").text().trim() === "Invoice";

    if (isInvoiceSlide) {
      $slide.addClass("active");
    } else {
      $slide.removeClass("active").addClass("d-none");
    }
  });

  // Disable carousel navigation buttons
  $carousel
    .find(".carousel-control-prev, .carousel-control-next")
    .addClass("d-none");
}
