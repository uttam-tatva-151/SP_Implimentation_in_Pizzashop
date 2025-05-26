$(document).ready(function () {
    let countryId = $('#CountryDropdown').val();
    let stateId = $('#StateDropdown').val();
    let cityId = $('#CityDropdown').val();

    // Load the State dropdown based on StateId
    if (stateId) {
        $.ajax({
            url: '/Profile/GetStates',
            type: 'GET',
            data: { countryId: countryId },
            dataType: 'json',
            success: function (response) {
                $('#StateDropdown').html('<option value="">Select State</option>');
                if (response.length > 0) {
                    $.each(response, function (i, state) {
                        let  selected = state.stateId == stateId ? 'selected' : '';
                        $('#StateDropdown').append(`<option value="${state.stateId}" ${selected}>${state.stateName}</option>`);
                    });
                }
            }
        });
    }

    // Load the City dropdown based on CityId
    if (cityId) {
        $.ajax({
            url: '/Profile/GetCities',
            type: 'GET',
            data: { stateId: stateId },
            dataType: 'json',
            success: function (response) {
                $('#CityDropdown').html('<option value="">Select City</option>');
                if (response.length > 0) {
                    $.each(response, function (i, city) {
                        let selected = city.cityId == cityId ? 'selected' : '';
                        $('#CityDropdown').append(`<option value="${city.cityId}" ${selected}>${city.cityName}</option>`);
                    });
                }
            }
        });
    }

   
//
});
 // Fetch States when Country changes
 $(document).on("change","#CountryDropdown",function () {
    let newCountryId = $(this).val();
    $('#StateDropdown').html('<option value="">Loading...</option>');
    $('#CityDropdown').html('<option value="">Select City</option>');
    console.log(newCountryId);
    if (newCountryId) {
        $.ajax({
            url: '/Profile/GetStates',
            type: 'GET',
            data: { countryId: newCountryId },
            dataType: 'json',
            success: function (response) {
                $('#StateDropdown').html('<option value="">Select State</option>');
                if (response.length > 0) {
                    $.each(response, function (i, state) {
                        $('#StateDropdown').append(`<option value="${state.stateId}">${state.stateName}</option>`);
                    });
                }
            }
        });
    }
});

// Fetch Cities when State changes
$(document).on("change","#StateDropdown",function () {
    let newStateId = $(this).val();
    $('#CityDropdown').html('<option value="">Loading...</option>');

    if (newStateId) {
        $.ajax({
            url: '/Profile/GetCities',
            type: 'GET',
            data: { stateId: newStateId },
            dataType: 'json',
            success: function (response) {
                $('#CityDropdown').html('<option value="">Select City</option>');
                if (response.length > 0) {
                    $.each(response, function (i, city) {
                        $('#CityDropdown').append(`<option value="${city.cityId}">${city.cityName}</option>`);
                    });
                }
            }
        });
    }
});
    function displayFileNameWithAnimation(input) {
        //console.log(input.files);
        const fileNameSpan = $('#uploadedFileName');
        if (input.files && input.files[0]) {
            const fileName = input.files[0].name;
            fileNameSpan.stop(true, true).hide().text(fileName).fadeIn(500);
        } else {
            fileNameSpan.fadeOut(300);
        }
    }
