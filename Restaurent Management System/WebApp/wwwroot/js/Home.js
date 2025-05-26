let revenueChart;
let customerGrowthChart;

$(document).ready(function () {
  // Load initial data
  loadDashboardData("All time");

  // Set up date filter change handler
  $("#DashBoardDateFilter").change(function () {
    const timeRange = $(this).val();
    loadDashboardData(timeRange);
  });
});

function loadDashboardData(timeRange) {
  showLoading(true);

  $.ajax({
    url: "/DashBoard/GetDashboardData",
    type: "GET",
    data: { timeRange: timeRange },
    success: function (data) {
      console.log("Successfully loaded dashboard data:", data);
      updateDashboard(data);
      showLoading(false);
    },
    error: function (xhr, status, error) {
      showLoading(false);
      showToaster("Error loading dashboard data. Please try again.","Error");
    },
  });
}

function updateDashboard(data) {
  // Update tiles
  $("#totalSales").text("₹" + data.totalSales.toFixed(2));
  $("#totalOrders").text(data.totalOrders);
  $("#avgOrderValue").text("₹" + data.avgOrderValue.toFixed(2));
  $("#waitingListCount").text(data.waitingListCount);
  $("#newCustomersCount").text(data.newCustomersCount);

  // For demo
  $("#avgWaitingTime").text("15 mins");

  // Update top selling items
  updateItemsList("#topSellingItems tbody", data.topSellingItems, true);

  // Update least selling items
  updateItemsList("#leastSellingItems tbody", data.leastSellingItems, false);

  // Update charts
  updateRevenueChart(data.revenueTrend);

  updateCustomerGrowthChart(data.customerGrowth);
}
function generateDateRange(rangeType) {
  const today = new Date();
  let startDate;

  if (rangeType === "Last 7 days") {
    startDate = new Date(today);
    startDate.setDate(today.getDate() - 6);
  } else if (rangeType === "Last 30 days") {
    startDate = new Date(today);
    startDate.setDate(today.getDate() - 29);
  } else if (rangeType === "This month") {
    startDate = new Date(today.getFullYear(), today.getMonth(), 1);
  } else {
    // For "All time" or custom ranges, you'll need to pass earliest date from DB
    return null;
  }

  const dates = [];
  let currentDate = new Date(startDate);
  while (currentDate <= today) {
    dates.push(new Date(currentDate));
    currentDate.setDate(currentDate.getDate() + 1);
  }
  return dates;
}
function formatDateLabel(date, rangeType) {
  const day = date.getDate();
  const month = date.toLocaleString("default", { month: "short" });
  const year = date.getFullYear();
  if (rangeType === "Last 7 days" || rangeType === "Last 30 days")
    return `${month} ${day}`; // e.g., Apr 24 } else if
  rangeType === "This month";
  return `${day}`; // e.g., 1, 2, 3... } else { return ` ${day} ${month} `; // e.g., 30
}
  function mapDataToRange(dateRange, rawData, valueField = "revenue") {
    const map = new Map(
      rawData.map((d) => [new Date(d.date).toDateString(), d[valueField]])
    );

    return dateRange.map((date) => {
      const key = date.toDateString();
      return map.has(key) ? map.get(key) : 0;
    });
  }

  function updateRevenueChart(revenueTrend) {
    const ctx = $("#revenueChart");
    if (revenueChart) revenueChart.destroy();

    const selectedRange = $("#DashBoardDateFilter").val();
    const dateRange = generateDateRange(selectedRange) || [
      ...new Set(revenueTrend.map((d) => new Date(d.date))),
    ];

    const labels = dateRange.map((date) =>
      formatDateLabel(date, selectedRange)
    );
    const revenueData = mapDataToRange(dateRange, revenueTrend, "revenue");

    revenueChart = new Chart(ctx, {
      type: "line",
      data: {
        labels: labels,
        datasets: [
          {
            label: "Revenue",
            data: revenueData,
            backgroundColor: "rgba(54, 162, 235, 0.2)",
            borderColor: "#007bff",
            borderWidth: 2,
            fill: true,
            tension: 0.4,
          },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            display: true,
            position: "top",
          },
        },
        scales: {
          y: {
            beginAtZero: true,
          },
        },
      },
    });
  }

  function updateCustomerGrowthChart(customerGrowth) {
    const ctx = $("#customerGrowthChart");
    if (customerGrowthChart) customerGrowthChart.destroy();

    const selectedRange = $("#DashBoardDateFilter").val();
    const dateRange = generateDateRange(selectedRange) || [
      ...new Set(customerGrowth.map((d) => new Date(d.date))),
    ];

    const labels = dateRange.map((date) =>
      formatDateLabel(date, selectedRange)
    );
    const customerData = mapDataToRange(
      dateRange,
      customerGrowth,
      "customerCount"
    ).map((v) => Math.round(v)); // Ensure whole numbers (no decimals)

    customerGrowthChart = new Chart(ctx, {
      type: "line",
      data: {
        labels: labels,
        datasets: [
          {
            label: "Customer Growth",
            data: customerData,
            backgroundColor: "rgba(54, 162, 235, 0.2)",
            borderColor: "#007bff",
            borderWidth: 2,
            fill: true,
            tension: 0.4,
          },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            display: true,
            position: "top",
          },
          tooltip: {
            callbacks: {
              label: function (context) {
                return `${context.parsed.y} customers`;
              },
            },
          },
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              precision: 0,
              stepSize: 1,
            },
          },
        },
      },
    });
  }

  function updateItemsList(selector, items, isTopSelling) {
    const $tbody = $(selector);
    $tbody.empty();

    items.forEach((item, index) => {
      const rank = index + 1;
      $tbody.append(`
    <tr>
        <td class="border-bottom d-flex align-items-center">
            #${rank} <img src="/images/dining-menu.png" alt="${item.itemName}" class="ms-2 pe-0">
            <div class="row d-flex">
                <span class="ms-2 pe-0">${item.itemName}</span>
                <span class="ms-2 pe-0"><i class="fa-solid fa-bell-concierge"></i>${item.orderCount} orders</span>
            </div>
        </td>
    </tr>
    `);
    });
  }

  function showLoading(show) {
    if (show) {
      $(".loading-spinner").show();
      $("#dashboardContent").css("opacity", "0.5");
    } else {
      $(".loading-spinner").hide();
      $("#dashboardContent").css("opacity", "1");
    }
  }

  // This function will be used later for SignalR partial updates
  function updateDashboardComponent(componentId, newData) {
    // console.log('Updating dashboard component:', componentId, 'with data:', newData);
    switch (componentId) {
      case "totalSales":
        $("#totalSales").text("₹" + newData.toFixed(2));
        break;
      case "totalOrders":
        $("#totalOrders").text(newData);
        break;
      case "avgOrderValue":
        $("#avgOrderValue").text("₹" + newData.toFixed(2));
        break;
      case "waitingListCount":
        $("#waitingListCount").text(newData);
        break;
      case "newCustomersCount":
        $("#newCustomersCount").text(newData);
        break;
      case "topSellingItems":
        updateItemsList("#topSellingItems tbody", newData, true);
        break;
      case "leastSellingItems":
        updateItemsList("#leastSellingItems tbody", newData, false);
        break;
      case "revenueTrend":
        updateRevenueChart(newData);
        break;
      case "customerGrowth":
        updateCustomerGrowthChart(newData);
        break;
      default:
        console.warn("Unknown component ID:", componentId);
    }
  }

