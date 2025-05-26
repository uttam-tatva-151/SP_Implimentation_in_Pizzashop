//#region User Pagination
let paginationModelForUserPage = {
  pageSize: 5,
  pageNumber: 1,
  sortColumn: "name",
  sortOrder: "asc",
  searchQuery: "",
  fromDate: "",
  toDate: "",
  totalRecords: 0,
  userStatus: "",
  dateRange: "",
};
const debouncedFetchUsers = debounce(fetchUsers, 300);
function updatePaginationInfoForUserPage(pagination) {
  paginationModelForUserPage.totalRecords = pagination.totalRecords;
  $("#totalRecordsForCustomerPage").val(pagination.totalRecords);
  $("#paginationInfoForUserPage").text(
    `Showing ${
      (pagination.pageNumber - 1) * pagination.pageSize + 1
    } - ${Math.min(
      pagination.pageNumber * pagination.pageSize,
      pagination.totalRecords
    )} of ${pagination.totalRecords}`
  );
}
function fetchUsers() {
  $.ajax({
    url: "/User/UserList",
    type: "GET",
    contentType: "application/json",
    data: paginationModelForUserPage,
    success: function (response) {
      $("#userListContainer").html(response.partialView);
      updatePaginationInfoForUserPage(response.paginationDetails);
    },
    error: function (error) {
      showToaster(error, "Error");
    },
  });
}

$(document).on("click", ".sortForUserList", function () {
  // //console.log("hello from sort");
  paginationModelForUserPage.sortColumn = $(this).data("sortby");
  paginationModelForUserPage.sortOrder =
    paginationModelForUserPage.sortOrder === "asc" ? "desc" : "asc";
  debouncedFetchUsers();
});
$(document).on("change", "#sortAccordion", function () {
  // //console.log("hello from sort");
  paginationModelForUserPage.sortColumn = $(this).val();
  paginationModelForUserPage.sortOrder = $(
    "input[name='sortOrder']:checked"
  ).val();
  debouncedFetchUsers();
});
$(document).on("click", ".btn-checkForUserOrder", function () {
  paginationModelForUserPage.sortOrder = $(this).val();
  paginationModelForUserPage.sortColumn = $("#sortAccordion").val();
  debouncedFetchUsers();
});

// Search Functionality
$(document).on("input", "#searchUser", function () {
  let searchValue = $(this).val().trim();
  if (searchValue.length > 1 && searchValue.length <= 100) {
    paginationModelForUserPage.searchQuery = searchValue;
    paginationModelForUserPage.pageNumber = 1;
    debouncedFetchUsers();
  }
  if ($(this).val().length == 0) {
    paginationModelForUserPage.searchQuery = "";
    paginationModelForUserPage.pageNumber = 1;
    debouncedFetchUsers();
  }
});

// Change Items Per Page
$(document).on("change", "#UserPerPage", function () {
  paginationModelForUserPage.pageSize = $(this).val();
  paginationModelForUserPage.pageNumber = 1;
  debouncedFetchUsers();
});

// Pagination
$(document).on("click", "#prevPageForUsersPage", function () {
  // //console.log("hello from prev");
  if (paginationModelForUserPage.pageNumber > 1) {
    paginationModelForUserPage.pageNumber--;
    debouncedFetchUsers();
  }
});
$(document).on("click", "#nextPageForUsersPage", function () {
  if (
    paginationModelForUserPage.pageNumber *
      paginationModelForUserPage.pageSize <
    paginationModelForUserPage.totalRecords
  ) {
    paginationModelForUserPage.pageNumber++;
    debouncedFetchUsers();
  }
});

//#endregion

$(document).ready(function () {
  paginationModelForUserPage.totalRecords = $("#totalRecordsForUserPage").val();
  $(".deleteUserBtn").on("click", function () {
    let userId = $(this).attr("data-user-id");
    $("#userIdForDelete").val(userId);
  });
  updatePaginationInfoForUserPage(paginationModelForUserPage);

  let countryId = $("#CountryDropdown").val();
  let stateId = $("#StateDropdown").val();
  let cityId = $("#CityDropdown").val();

  // Load the State dropdown based on StateId
  if (stateId) {
    $.ajax({
      url: "/Profile/GetStates",
      type: "GET",
      data: { countryId: countryId },
      dataType: "json",
      success: function (response) {
        $("#StateDropdown").html('<option value="">Select State</option>');
        if (response.length > 0) {
          $.each(response, function (i, state) {
            let selected = state.stateId == stateId ? "selected" : "";
            $("#StateDropdown").append(
              `<option value="${state.stateId}" ${selected}>${state.stateName}</option>`
            );
          });
        }
      },
    });
  }

  // Load the City dropdown based on CityId
  if (cityId) {
    $.ajax({
      url: "/Profile/GetCities",
      type: "GET",
      data: { stateId: stateId },
      dataType: "json",
      success: function (response) {
        $("#CityDropdown").html('<option value="">Select City</option>');
        if (response.length > 0) {
          $.each(response, function (i, city) {
            let selected = city.cityId == cityId ? "selected" : "";
            $("#CityDropdown").append(
              `<option value="${city.cityId}" ${selected}>${city.cityName}</option>`
            );
          });
        }
      },
    });
  }

  // Fetch States when Country changes
  $("#CountryDropdown").change(function () {
    let newCountryId = $(this).val();
    $("#StateDropdown").html('<option value="">Loading...</option>');
    $("#CityDropdown").html('<option value="">Select City</option>');

    if (newCountryId) {
      $.ajax({
        url: "/Profile/GetStates",
        type: "GET",
        data: { countryId: newCountryId },
        dataType: "json",
        success: function (response) {
          $("#StateDropdown").html('<option value="">Select State</option>');
          if (response.length > 0) {
            $.each(response, function (i, state) {
              $("#StateDropdown").append(
                `<option value="${state.stateId}">${state.stateName}</option>`
              );
            });
          }
        },
      });
    }
  });

  // Fetch Cities when State changes
  $("#StateDropdown").change(function () {
    let newStateId = $(this).val();
    $("#CityDropdown").html('<option value="">Loading...</option>');

    if (newStateId) {
      $.ajax({
        url: "/Profile/GetCities",
        type: "GET",
        data: { stateId: newStateId },
        dataType: "json",
        success: function (response) {
          $("#CityDropdown").html('<option value="">Select City</option>');
          if (response.length > 0) {
            $.each(response, function (i, city) {
              $("#CityDropdown").append(
                `<option value="${city.cityId}">${city.cityName}</option>`
              );
            });
          }
        },
      });
    }
  });
});

$(document).ready(function () {
  $("#fileInput").change(function () {
    const file = this.files[0]; // Get the selected file
    const extensionOfFile = file.name.split(".").pop().toLowerCase();

    if (!file && ["jpg", "jpeg", "png"].includes(extensionOfFile)) {
      $("#fileInput").val("");
      $("#imagePreview").hide();
      showToaster(
        "Please select a file with jpg, jpeg, or png format.",
        "Error"
      );
    } else if (file && file.size > 5 * 1024 * 1024) {
      $("#fileInput").val("");
      $("#imagePreview").hide();
      showToaster("File Size not more than 5 MB", "Error");
    } else {
      $("#fileDetails").removeClass("d-none");
      $("#fileDetails").siblings().addClass("d-none");
      $("#fileName").text(file.name);

      const reader = new FileReader();
      reader.onload = function (e) {
        $("#imagePreview").attr("src", e.target.result).show();
      };

      reader.readAsDataURL(file);
    }
  });

  $("#removeImageButton").click(function () {
    $("#fileInput").val("");
    $("#fileDetails").addClass("d-none");
    $("#fileDetails").siblings().removeClass("d-none");
    $("#imagePreview").hide();
  });
});
