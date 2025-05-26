const permissionLists = [];

$(document).ready(function () {
  $("body").css("visibility", "visible");

  // Fetch permissions and update buttons
  const checkAndUpdatePermissions = function () {
    fetchPermissions()
      .then(function (permissions) {
        permissions.permissionDetails.forEach(function (detail) {
          if (!permissionLists.some((p) => p.permissionId === detail.permissionId)) {
            permissionLists.push(detail);
          }
        });
        updateButtons(permissions);
        const pageName = getActivePageFromSidebar();
        if (pageName) {
          setButtonsBasedOnRole(pageName);
        }
      })
      .catch(function (error) {
        showToaster(error, "Error");
      });
  };

  checkAndUpdatePermissions();

  // Observe DOM changes for partial views
  const observer = new MutationObserver(function (mutationsList) {
    let hasRelevantChange = false;

    mutationsList.forEach((mutation) => {
      if (mutation.addedNodes.length > 0 || mutation.removedNodes.length > 0) {
        hasRelevantChange = true;
      }
    });

    if (hasRelevantChange) {
      const pageName = getActivePageFromSidebar();
      if (pageName) {
        setButtonsBasedOnRole(pageName);
      }
    }
  });

  observer.observe(document.body, {
    childList: true,
    subtree: true,
  });
});

// Get the active page module from the sidebar
function getActivePageFromSidebar() {
  let module = null;
  $("li[data-module]").each(function () {
    if ($(this).find("a").hasClass("activepage")) {
      module = $(this).data("module");
      return false;
    }
  });
  return module;
}

// Fetch permissions via an AJAX request
function fetchPermissions() {
  return $.ajax({
    url: "/RoleAndPermissions/GetALlPermissions",
    method: "GET",
    contentType: "application/json",
  })
    .then((response) => response)
    .catch(() => {
      console.error("Failed to fetch permissions");
      return Promise.reject("Failed to fetch permissions");
    });
}

// Update buttons based on permissions
function updateButtons(permissions) {
  $("li[data-module]").each(function () {
    const module = $(this).data("module");
    const permission = permissions.permissionDetails.find(
      (p) => p.moduleName === module
    );

    if (!permission || !permission.canView) {
      $(this)
        .addClass("disabled")
        .find("a")
        .attr({
          tabindex: "-1",
          title: "You don't have permission for this action",
        })
        .removeAttr("asp-action asp-controller data-title");
    } else {
      $(this).removeClass("disabled").find("a").removeAttr("tabindex title");
    }
  });
}

// Set buttons based on the user role permissions
function setButtonsBasedOnRole(moduleName) {
  if (permissionLists.length > 0) {
    const permission = permissionLists.find((p) => p.moduleName === moduleName);
    if (permission) {
      updateButtonsForCRUD(permission);
    } else {
      console.error("Permission for the module not found.");
    }
  }
}

// Update CRUD buttons based on permissions
function updateButtonsForCRUD(permission) {
  $(".crudAction[data-action]").each(function () {
    let target = $(this).data("action");
    let action = target.split("-")[0];
    let section = target.split("-")[1];

    if (
      (action === "Create" || action === "Update") &&
      !permission.canCreateandedit
    ) {
      $(this)
        .addClass("buttonLocked")
        .attr({
          tabindex: "-1",
          title: `You are not allowed to ${action} ${section}`,
        })
        .removeAttr("data-bs-target data-bs-toggle onClick asp-action asp-controller href");
    } else if (action === "Delete" && !permission.canDelete) {
      $(this)
        .addClass("buttonLocked")
        .attr({
          tabindex: "-1",
          title: `You are not allowed to ${action} ${section}`,
        })
        .removeAttr("data-bs-target data-bs-toggle onClick asp-action asp-controller href");
    }
  });
}


// Debounce utility function
function debounce(func, delay) {
  let timeoutId;
  return function (...args) {
      clearTimeout(timeoutId);
      timeoutId = setTimeout(() => func.apply(this, args), delay);
  };
}


let loaderStartTime = null;
const MIN_LOADER_DURATION = 300;

// Show the loader
function showLoader() {
  loaderStartTime = Date.now();
  $("body").addClass("loading");
}

// Hide the loader after the minimum duration
function hideLoader() {
  const elapsed = Date.now() - loaderStartTime;
  const delay = Math.max(MIN_LOADER_DURATION - elapsed, 0);

  setTimeout(() => {
    $("body").removeClass("loading");
  }, delay);
}

showLoader();

$(window).on("load", () => {
  hideLoader();
});

$(document).ready(() => {
  $(document).ajaxStart(function () {
    showLoader();
  });

  $(document).ajaxStop(function () {
    hideLoader();
  });

  $(document).ajaxError(function () {
    hideLoader();
  });

  let role = $("#OrderAppLogo").data("role");
  let currentPage = window.location.pathname;

  if (role === "chef") {
    if (
      !currentPage.includes("Profile") &&
      !currentPage.includes("Password") &&
      !currentPage.includes("Login")
    ) {
      redirectToKOTPage();
    }
  }

  let isChef = $("#KOTHeading").data("role");
  if (isChef === "True") {
    setChefDashboard();
  }
});

// Show toaster notifications
function showToaster(message, status) {
  const commonOptions = {
    positionClass: "toast-top-right",
    timeOut: 5000,
    extendedTimeOut: 2000,
    closeButton: true,
    progressBar: true,
    newestOnTop: true,
    preventDuplicates: true,
    showDuration: 500,
    hideDuration: 500,
    showMethod: "fadeIn",
    hideMethod: "fadeOut",
    tapToDismiss: true,
    escapeHtml: false,
    closeHtml: '<button><i class="fa fa-times"></i></button>',
  };

  if (status == 1 || status === "Error") {
    toastr.error(message, "", commonOptions);
  } else if (status == 2 || status === "Success") {
    toastr.success(message, "", commonOptions);
  } else {
    toastr.warning(message, "", commonOptions);
  }
}

// Redirect to the KOT page
function redirectToKOTPage() {
  window.location.href = `/KOT/KOT`;
}

// Set up the chef dashboard
function setChefDashboard() {
  const $kotBtnsDiv = $(".btn-group .kotBtns");
  $kotBtnsDiv.hide();
  $kotBtnsDiv.find("a[asp-controller][asp-for]").remove();
}