const editorId = $("#KOTHeading").attr('data-editor-id');
function getKOTs(sortOrder, sortBy) {
  $.ajax({
    type: "GET",
    url: "/KOT/Kot",
    data: { status: sortOrder, categoryId: sortBy },
    success: function (response) {
      $("#kotsGrid").html(response);
      clearEmptyBox();
    },
    error: function (error) {
      showToaster("Failed to load KOTs. Please try again.", "Error");
    },
  });
}

$(document).on("click", ".sortKOTs, .sortKOTsByCategory", function () {
  const sortOrder =
    $(this).data("show-kots") || $(".activeBtnInOrderApp").data("show-kots");
  KOTStatus = sortOrder;
  const sortBy = $(".activeTabAtKOTPage").data("category-id");
  getKOTs(sortOrder, sortBy);
});

$(document).on("click", ".kotCard", function () {
  try {
    let data = $(this).attr("data-kot-details");
    let tokenDetail = JSON.parse(data);

    $("#UpdateKOTModalLabel").text(`Order Details: #${tokenDetail.orderId}`);
    $("#itemsToMarkReady").empty();

    if (
      Array.isArray(tokenDetail.kotItems) &&
      tokenDetail.kotItems.length > 0
    ) {
      $("#itemsToMarkPrepared").empty();
      tokenDetail.kotItems.forEach((item) => {
        CreateItemElementAtKOTModal(item);
      });
      if (tokenDetail.kotItems[0].isPrepared == false) {
        $("#updateModalBtn").text("Mark as prepared");
        KOTStatus = "InProgress";
      } else {
        $("#updateModalBtn").text("Mark as InProgress");
        KOTStatus = "Ready";
      }
    }
    $("#UpdateKOTModal").modal("show");
  } catch {
    showToaster("Failed to load KOT details. Please try again.", "Error");
  }
});

function clearEmptyBox() {
  let parentSelector = ".kotCard";
  let childSelector = ".itemsInKot";
  $(parentSelector).each(function () {
    if (!$(this).has(childSelector).length) {
      $(this).hide(); // Hide the parent element if it doesn't have the child
    }
  });
}

$(document).on("click", "#updateModalBtn", function () {
  
  let selectedItems = [];
  if (KOTStatus == "InProgress") {
    $(".item-checkbox:checked").each(function () {
      debugger;
      let itemRaw = $(this).data("item");
      let item = null;
      if (typeof itemRaw === "string") {
        item = JSON.parse(itemRaw);
      }else{
        item = itemRaw;
      }
      let preparedItems = $(this).closest("tr").find("input.input-box").val();
      selectedItems.push({
        ...item,
        preparedItems: item.preparedItems + parseInt(preparedItems),
      });
    });
  } else {
    $(".item-checkbox:checked").each(function () {
      let item = $(this).data("item");
      let preparedItems = $(this).closest("tr").find("input.input-box").val();
      selectedItems.push({
        ...item,
        preparedItems: item.preparedItems - parseInt(preparedItems),
      });
    });
  }
  const orderId = parseInt($("#UpdateKOTModalLabel").text().split("#")[1], 10);
  if (selectedItems.length > 0) {
    $.ajax({
      type: "POST",
      url: "/KOT/UpdateKOT",
      data: { kotItems: selectedItems, orderId: orderId , editorId : editorId},
      success: function (response) {
        $("#UpdateKOTModal").modal("hide");
        getKOTs(
          $(".activeBtnInOrderApp").data("show-kots"),
          $(".activeTabAtKOTPage").data("category-id")
        );

        showToaster(response.message, response.status);
      },
      error: function () {
        showToaster("Failed to update KOT. Please try again.", "Error");
      },
    });
  } else {
    showToaster("Please select at least one item.", "Error");
  }
});

function CreateItemElementAtKOTModal(item) {
  debugger
  const maxQuantity = item.isPrepared
    ? item.preparedItems
    : item.quantity - (item.preparedItems || 0);
  const itemData = JSON.stringify(item).replace(/'/g, '&#39;');;
  const modifierRows = (item.modifiers || [])
    .map((modifier) => `<li class="text-start">${modifier.modifierName}</li>`)
    .join("");
  //console.log("Hello"+itemData)
  $("#itemsToMarkPrepared").append(`
        <tr class="d-flex justify-content-between align-items-center">
            <td>
                <input type="checkbox" class="item-checkbox" data-item='${itemData}'>
                <span class="mx-2">${item.itemName}</span>
                <ul class="mx-4 p-0 fs-6">${modifierRows}</ul>
            </td>
            <td align="right">
                <div class="quantity d-flex align-items-center border border-blue rounded mt-2 p-1">
                    <button class="minus btn px-2 py-1" aria-label="Decrease">
                        <i class="bi bi-dash"></i>
                    </button>
                    <input type="number" class="input-box form-control text-center mx-2 border-none" value="0" min="1" max="${maxQuantity}" style="width: 60px;">
                    <button class="plus btn px-2 py-1" aria-label="Increase">
                        <i class="bi bi-plus"></i>
                    </button>
                </div>
            </td>
        </tr>
    `);
}
$(document).on("click", ".minus", function () {
  const input = $(this).siblings("input.input-box"); // Get the sibling input box
  let currentValue = parseInt(input.val(), 10); // Get the current value
  const minValue = parseInt(input.attr("min"), 10); // Get the minimum value

  if (currentValue > minValue) {
    input.val(currentValue - 1); // Decrease the value
  }
});

$(document).on("click", ".plus", function () {
  const input = $(this).siblings("input.input-box"); // Get the sibling input box
  let currentValue = parseInt(input.val(), 10); // Get the current value
  const maxValue = parseInt(input.attr("max"), 10); // Get the maximum value

  if (currentValue < maxValue) {
    input.val(currentValue + 1); // Increase the value
  }
});

$(".nav-link").click(function (e) {
  e.preventDefault();
  $(".nav-link").removeClass("activeTabAtKOTPage");
  $(this).addClass("activeTabAtKOTPage");
  $("#categoryNameAtKotPage").text($(this).text());
});

$(".sortKOTs").click(function (e) {
  e.preventDefault();
  $(".sortKOTs").removeClass("activeBtnInOrderApp");
  $(this).addClass("activeBtnInOrderApp");
});

$(".scrollBtns").click(function () {
  const direction = $(this).data("scroll-direction");
  const scrollAmount = 450; // Amount to scroll in pixels
  const currentScroll = $("#kotsGrid").scrollLeft();

  $("#kotsGrid").animate(
    {
      scrollLeft:
        direction === "forword"
          ? currentScroll + scrollAmount
          : currentScroll - scrollAmount,
    },
    "slow"
  );
});

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
  });
}

// Update timers every second
setInterval(updateTimers, 1000);
