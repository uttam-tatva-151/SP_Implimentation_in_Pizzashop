/* -------------------------------------------------------------------------- */
/*                                 Sidebar Js                                 */
/* -------------------------------------------------------------------------- */
const defaultIcons = {

  dashboard: 'dashboard.png',
  users: 'users.png',
  roles_permissions: 'roles-permissions.png',
  menu: 'menu.png',
  section_table: 'dinner-table.png',
  taxes: 'money-bag.png',
  orders: 'clipboard.png',
  customers: 'client.png',

}
const activeIcons = {

  dashboard: 'dashboard_active.png',
  users: 'users_active.png',
  roles_permissions: 'roles-permissions_active.png',
  menu: 'menu_active.png',
  section_table: 'dinner-table_active.png',
  taxes: 'money-bag_active.png',
  orders: 'clipboard_active.png',
  customers: 'client_active.png',

};
/* ------------------------------ Sidebar hover ----------------------------- */
$(document).ready(function () {
  const currentUrl = window.location.pathname.toLowerCase();

  $('#sidebar a').each(function () {
    let linkUrl = $(this).attr('href').toLowerCase();
    let img = $(this).find('img');
    let src = img.attr('src');
    let currentPage = $(this).data('page-name');

    if (currentUrl.includes(linkUrl.split('/')[1])) {
      $(this).addClass('activepage');
      img.attr('src', src.replace(defaultIcons[currentPage], activeIcons[currentPage]));
    }
  });

});

/* ----------------------------- Sidebar Toggle ----------------------------- */
function handleSidebarOnResize() {
  if ($(window).width() < 992) {
    // Hide sidebar on mobile, show logo
    $("#sidebarColumn").addClass("d-none").removeClass("sidebar-overlay active");
    $("#sidebarBackdrop").removeClass("active");
    $("#navbarLogo").removeClass("invisible");
  } else {
    $("#sidebarColumn").removeClass("d-none sidebar-overlay active");
    $("#sidebarBackdrop").removeClass("active");
    $("#navbarLogo").addClass("invisible");
  }
}

// On resize
$(window).on('resize', handleSidebarOnResize);

// On initial load
$(function() {
  handleSidebarOnResize();
});

// Hide sidebar (mobile), show logo
$(document).on('click', '#pizzashopLogo', function() {
  if ($(window).width() < 992) {
  $("#sidebarColumn").addClass("d-none").removeClass("sidebar-overlay active");
  $("#sidebarBackdrop").removeClass("active");
  $("#navbarLogo").removeClass("invisible");
}
});

// Show sidebar as overlay (mobile)
$(document).on('click', '#navbarLogo', function() {
  
    $("#sidebarColumn").addClass("sidebar-overlay active").removeClass("d-none");
    $("#sidebarBackdrop").addClass("active");
    $("#navbarLogo").addClass("invisible");
});

// Hide overlay sidebar and backdrop (mobile)
$(document).on('click', '#sidebarBackdrop', function() {
  $("#sidebarColumn").removeClass("active");
  $("#sidebarBackdrop").removeClass("active");
  $("#navbarLogo").removeClass("invisible");
  setTimeout(function() {
    $("#sidebarColumn").addClass("d-none").removeClass("sidebar-overlay");
  }, 300); // matches CSS transition
});
/* -------------------------------------------------------------------------- */
/*                           For Password Eye Button                          */
/* -------------------------------------------------------------------------- */

function showPassword(sectionId, imageId) {
  let id1 = document.getElementById(sectionId);
  let id2 = document.getElementById(imageId);

  if (id1.type === "password") {
    id1.type = "text";
    id2.src = "../images/icons/shared-vision.png"
  } else {
    id1.type = "password";
    id2.src = "../images/icons/eye.png"
  }
}

/* -------------------------------------------------------------------------- */
/*                   Email Auto Populate To the Forgot Page                   */
/* -------------------------------------------------------------------------- */

function populateForgotPasswordLink() {
  let email = document.getElementById("loginEmail").value.trim();
  window.location.href = "/Login/ForgotPassword?forgotAction=" + btoa(email);
}


/* --------------------------------------------------- */
/*                  For Toast Message                  */
/* --------------------------------------------------- */
$(document).ready(function () {
  if (toastMessage.trim() !== "") { // Ensure there's a message
    showToaster(toastMessage, toastStatus); // This toastmessage and status declare into Shared Layout
  }
});




$(document).on("change", ".select-all-checkbox", function () {
  let table = $(this).closest(".selectable-table");
  let rowCheckboxes = table.find(".row-checkbox");

  rowCheckboxes.prop("checked", this.checked);
  updateSelectAllState(table);
});

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


/* ---------------------------- Clear Modal Forms --------------------------- */

function clearFormAtHide(form){
  // Clear validation messages
  form.find(".field-validation-error").each(function () {
   $(this).text("");
 });

 // Remove validation classes (invalid styles)
 form.find(".is-invalid").removeClass("is-invalid");
 form.find(".is-valid").removeClass("is-valid");

 // Optionally reset the form fields
 form[0].reset(); // Reset all form fields
}

/* ======================== */
/*         Signal R         */
/* ======================== */
// // SignalR connection setup
// const connection = new signalR.HubConnectionBuilder()
//   .withUrl("/signalrhub")
//   .build();

// // Start the SignalR connection
// connection.start()
//   .then(() => {
//     //console.log("SignalR connected");

//     // Dynamically join the group when the user is on the "UserPage" section
//     if (window.location.pathname.includes("/User/UserList")) {
//       connection.invoke("JoinGroup", "UserList")
//         .then(() => //console.log("Joined group: UserList"))
//         .catch(err => console.error("Error joining group:", err));
//     }

//   })
//   .catch(err => console.error("SignalR connection failed:", err));


// connection.on("UserDataChanged", (action, userData) => {
//   // Publish a custom event for user data changes
//   const event = new CustomEvent("UserDataChanged", {
//     detail: { action, userData },
//   });
//   document.dispatchEvent(event);
// });

// // Leave the group when navigating away from the section
// window.addEventListener("beforeunload", () => {
//   if (window.location.pathname.includes("/User/UserList")) {
//     connection.invoke("LeaveGroup", "UserList")
//       .then(() => //console.log("Left group: UserList"))
//       .catch(err => console.error("Error leaving group:", err));
//   }

// });




/* -------------------------------------------------------------------------- */
/*                          For Permissions                                   */
/* -------------------------------------------------------------------------- */

