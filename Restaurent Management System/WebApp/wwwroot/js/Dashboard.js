const paginationModel = $("#dashboardFilter").attr("data-pagination");
const pagination = JSON.parse(paginationModel);

function getDashboardAnalytics() {
  $.ajax({
    url: "/Home/Index",
    type: "POST",
    data: { paginationDetails: pagination },
    success: function (response) {
      $("#dashboardPartialSection").html(response.partialView);
      setChart();
      showToaster(response.message, response.status);
    },
    error: function () {
      showToaster("Error loading dashboard data.", "Error");
    },
  });
}

$(document).on("change", "#dashboardFilter", function () {
  let selectedValue = $(this).val();

  if (selectedValue === "CustomRange") {
    $("#customRangeBtn")
      .removeAttr("disabled")
      .removeClass("opacity-50 pe-none d-none")
      .css("cursor", "pointer")
      .attr("data-bs-toggle", "modal")
      .attr("data-bs-target", "#customDateRangeModal")
      .show();
    $("#customDateRangeModal").modal("show");
  } else {
    pagination.toDate = "";
    pagination.fromDate = "";
    pagination.dateRange = selectedValue;
    getDashboardAnalytics();

    $("#customRangeBtn")
      .attr("disabled", true)
      .addClass("opacity-50 pe-none d-none")
      .css("cursor", "not-allowed")
      .removeAttr("data-bs-toggle")
      .removeAttr("data-bs-target")
      .hide();
  }
});
$(document).ready(function () {
  setChart();
});
$(document).on("hidden.bs.modal", "#customDateRangeModal", function () {
  $("#fromDateValidation").text("");
  $("#toDateValidation").text("");
});
$(document).on("click", "#customDateRangeBtn", function () {
  $("#customDateRangeModal").modal("show");
});
$(document).on("click", "#searchDateRangeForDashboardPage", function () {
  
  pagination.toDate = $("#toDateForDashboardPage").val();
  pagination.fromDate = $("#fromDateForDashboardPage").val();
  if (
    $("#toDateForDashboardPage").val() === "" &&
    $("#fromDateForDashboardPage").val() === ""
  ) {
    showToaster("Any one date is required to search!", "NotFound");
    return;
  } else if (
    $("#toDateForDashboardPage").val() != "" &&
    $("#fromDateForDashboardPage").val() != "" &&
    pagination.toDate < pagination.fromDate
  ) {
    showToaster("Please enter appropriate Date Range", "Warning");
    return;
  }
  pagination.dateRange = "All";
  getDashboardAnalytics();
  $("#customDateRangeModal").modal("hide");
});
$(document).on("click", "#clearDateRangeForDashboardPage", function () {
  pagination.toDate = $("#toDateForDashboardPage").val();
  pagination.fromDate = $("#fromDateForDashboardPage").val();
  $("#toDateForDashboardPage").val(" ");
  $("#fromDateForDashboardPage").val(" ");
});

let revenueChartType = "line";
let customerGrothChartType = "line";
let revenueChartInstance = null;
let customerChartInstance = null;

$(document).on("change", 'input[name="revenueChartType"]', function () {
  revenueChartType = this.value;
  setChart();
});
$(document).on("change", 'input[name="customerGrowthChartType"]', function () {
  customerGrothChartType = this.value;
  setChart();
});

function setChart() {
  const timePeriod = "Time Period";
  const revenueData = JSON.parse($("#revenueChart").attr("data-chart-details"));
  const customerData = JSON.parse(
    $("#customerChart").attr("data-chart-details")
  );

  // Destroy previous revenue chart if exists
  if (revenueChartInstance) {
    revenueChartInstance.destroy();
  }
  // Destroy previous customer chart if exists
  if (customerChartInstance) {
    customerChartInstance.destroy();
  }
  // Revenue Chart
  const revenueCtx = $("#revenueChart")[0].getContext("2d");
  revenueChartInstance = new Chart(revenueCtx, {
    type: revenueChartType,
    data: {
      labels: revenueData.map((item) => item.range),
      datasets: [
        {
          label: "Revenue",
          data: revenueData.map((item) => item.revenue),
          borderColor: "rgba(75, 192, 192, 1)",
          backgroundColor: "rgba(75, 192, 192, 0.2)",
          borderWidth: 2,
          tension: 0.5,
          fill: true,
        },
      ],
    },
    options: {
      responsive: true,
      plugins: {
        legend: { display: true, position: "top" },
        tooltip: {
          callbacks: {
            label: function (tooltipItem) {
              return `Revenue: ₹${tooltipItem.raw.toLocaleString()}`;
            },
          },
        },
      },
      scales: {
        x: {
          title: { display: true, text: timePeriod },
          grid: { display: true, color: "rgba(200, 200, 200, 0.2)" },
        },
        y: {
          title: { display: true, text: "Revenue (₹)" },
          beginAtZero: true,
          grid: { display: true, color: "rgba(200, 200, 200, 0.2)" },
        },
      },

      animation: { duration: 1000, easing: "easeInOutQuad" },
    },
  });

  // Customer Growth Chart
  const growthCtx = $("#customerChart")[0].getContext("2d");
  customerChartInstance = new Chart(growthCtx, {
    type: customerGrothChartType,
    data: {
      labels: customerData.map((item) => item.range),
      datasets: [
        {
          label: "New Customers",
          data: customerData.map((item) => item.newCustomers),
          backgroundColor: "rgba(54, 162, 235, 0.6)",
          borderColor: "rgba(54, 162, 235, 1)",
          borderWidth: 1,
        },
      ],
    },
    options: {
      responsive: true,
      plugins: {
        legend: { display: true, position: "top" },
        tooltip: {
          callbacks: {
            label: function (tooltipItem) {
              return `New Customers: ${tooltipItem.raw.toLocaleString()}`;
            },
          },
        },
      },
      scales: {
        x: {
          title: { display: true, text: timePeriod },
          grid: { display: true, color: "rgba(200, 200, 200, 0.2)" },
        },
        y: {
          title: { display: true, text: "Number of Customers" },
          beginAtZero: true,
          grid: { display: true, color: "rgba(200, 200, 200, 0.2)" },
          ticks: {
            stepSize: 1,
            callback: function(value) {
              if (Number.isInteger(value)) {
                return value;
              }
              return null; // Skip non-integer labels to avoid decimal points
            },
          }
        },
      },
      animation: { duration: 1000, easing: "easeInOutQuad" },
    },
  });
}
