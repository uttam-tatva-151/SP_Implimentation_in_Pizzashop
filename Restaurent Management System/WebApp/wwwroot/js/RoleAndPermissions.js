$(document).ready(function () {
    $(".can-view").on("change", function () {
        let row = $(this).closest("tr");
        let editCheckbox = row.find(".can-edit");
        let deleteCheckbox = row.find(".can-delete");

        if ($(this).is(":checked")) {
            editCheckbox.prop("disabled", false); // Enable "Can Edit"
            deleteCheckbox.prop("disabled", false); // Enable "Can Delete"
        } else {
            editCheckbox.prop("checked", false).prop("disabled", true); // Disable and uncheck "Can Edit"
            deleteCheckbox.prop("checked", false).prop("disabled", true); // Disable and uncheck "Can Delete"
        }
    });

    // Handle row-wise enable/disable
    $(".row-selectorforpermission").on("change", function () {
        let row = $(this).closest("tr");
        let checkboxes = row.find(".row-checkboxforpermission");

        if ($(this).is(":checked")) {
            checkboxes.each(function () {
                let checkbox = $(this);
                if (checkbox.hasClass("can-edit") || checkbox.hasClass("can-delete")) {
                    // Check if "Can View" is checked before enabling "Can Edit" or "Can Delete"
                    let canViewCheckbox = checkbox.closest("tr").find(".can-view");
                    if (canViewCheckbox.is(":checked")) {
                        checkbox.prop("disabled", false); // Enable "Can Edit" or "Can Delete" if "Can View" is checked
                    } else {
                        checkbox.prop("checked", false).prop("disabled", true); // Disable and uncheck if "Can View" is not checked
                    }
                } else {
                    checkbox.prop("disabled", false); // Enable other checkboxes
                }
            });
        } else {
            checkboxes.prop("checked", false).prop("disabled", true); // Disable and uncheck all checkboxes in the row
        }
    });

    // Select/Deselect all checkboxes
    $("#selectAll").on("change", function () {
        let isChecked = $(this).is(":checked");
        $(".row-selectorforpermission").prop("checked", isChecked).trigger("change");
    });
});

