$(function () {
    let _QueryParameter = window.location.pathname.split("/");
    let _ActionName = _QueryParameter[2].toLocaleLowerCase();

    // #region :: User Hall Search
    if (_ActionName === "userhallbooking") {
        const today = flatpickr.formatDate(new Date(), "Y-m-d");

        let startPicker = flatpickr("#startDate", {
            dateFormat: "Y-m-d",
            minDate: today,
            defaultDate: today,
            onChange: function (selectedDates, dateStr) {
                if (selectedDates.length > 0) {
                    endPicker.set('minDate', dateStr); // update end date min
                    if (endPicker.input.value && endPicker.input.value < dateStr) {
                        endPicker.clear(); // clear invalid end date
                    }
                }
            }
        });

        let endPicker = flatpickr("#endDate", {
            dateFormat: "Y-m-d",
            minDate: today,
            defaultDate: today,
            onChange: function (selectedDates, dateStr) {
                if (selectedDates.length > 0) {
                    startPicker.set('maxDate', dateStr); // update start date max
                    if (startPicker.input.value && startPicker.input.value > dateStr) {
                        startPicker.clear(); // clear invalid start date
                    }
                }
            }
        });

        // Form submission
        $('#searchForm').on('submit', function (e) {
            e.preventDefault();
            searchHalls();
        });
        function searchHalls() {
            const catType = $('#hallType option:selected').val();
            const startDate = $('#startDate').val();
            const endDate = $('#endDate').val();

            if (catType == '0' || catType === undefined) {
                notify(false, 'Please select hall type', false);
                $("#hallType").addClass("is-invalid");
                return;
            }
            else {
                $("#hallType").removeClass("is-invalid");
            }

            if (startDate == '') {
                notify(false, 'Please select start date', false);
                $("#startDate").addClass("is-invalid");
                return;
            }
            else {
                $("#startDate").removeClass("is-invalid");
            }

            if (endDate == '') {
                notify(false, 'Please select end date', false);
                $("#endDate").addClass("is-invalid");
                return;
            }
            else {
                $("#endDate").removeClass("is-invalid");
            }

            if (new Date(startDate) > new Date(endDate)) {
                notify(false, 'End date must be after start date', false);
                return;
            }

            $(".loader").css("display", "flex");
            getHallAvailableSearchResult(catType, startDate, endDate);

            //setTimeout(() => {
            //    //const mockResults = generateMockResults(hallType);
            //    //displayResults(mockResults);            

            //}, 1500);
        }

        function getHallAvailableSearchResult(catType, startDate, endDate) {
            $.ajax({
                url: '/UserBooking/HallAvailableSearchResult',
                data: { catType: catType, startDate: startDate, endDate: endDate },
                type: 'GET',
                dataType: 'HTML',
                success: function (response) {
                    if (response != '') {
                        $("#partailSearchResult").empty();
                        $("#partailSearchResult").html(response);
                        // After results are displayed, scroll to show results section properly
                        setTimeout(() => {
                            // Calculate scroll position to show search form at top and results below
                            const searchHero = $('.search-hero');
                            const searchHeroHeight = searchHero.outerHeight();

                            $('html, body').animate({
                                scrollTop: searchHeroHeight - 50 // Small offset to show part of search form
                            }, 800, 'swing');
                        }, 100);
                    }

                },
                complete: function () {
                    $(".loader").css("display", "none");
                },
            });
        }

        function generateMockResults(hallType) {
            const halls = {
                marriage: [
                    {
                        name: "Royal Marriage Palace",
                        city: "Bhatpara",
                        capacity: "500 guests",
                        price: "₹25000",
                        amenities: ["Air Conditioning", "Parking", "Catering", "Decoration"]
                    },
                    {
                        name: "Grand Wedding Hall",
                        city: "North 24 Parganas",
                        capacity: "300 guests",
                        price: "₹18000",
                        amenities: ["Sound System", "Lighting", "Parking", "Kitchen"]
                    }
                ],
                auditorium: [
                    {
                        name: "Municipal Auditorium",
                        city: "Bhatpara",
                        capacity: "800 seats",
                        price: "₹15000",
                        amenities: ["Stage", "Sound System", "Lighting", "AC"]
                    },
                    {
                        name: "Cultural Center Hall",
                        city: "Bhatpara",
                        capacity: "400 seats",
                        price: "₹12000",
                        amenities: ["Projector", "Microphone", "Green Room", "Parking"]
                    }
                ],
                seminar: [
                    {
                        name: "Conference Hall A",
                        city: "Bhatpara",
                        capacity: "100 people",
                        price: "₹8000",
                        amenities: ["Projector", "WiFi", "AC", "Whiteboard"]
                    },
                    {
                        name: "Training Center",
                        city: "Bhatpara",
                        capacity: "50 people",
                        price: "₹5000",
                        amenities: ["Audio Visual", "Internet", "Refreshments", "Parking"]
                    }
                ],
                community: [
                    {
                        name: "Community Hall",
                        city: "Bhatpara",
                        capacity: "200 people",
                        price: "₹10000",
                        amenities: ["Kitchen", "Parking", "Sound System", "Tables"]
                    }
                ]
            };

            return halls[hallType] || [];
        }

        function displayResults(halls) {
            $(".loader").css("display", "none");

            if (halls.length === 0) {
                $('#resultsContainer').html(`
                    <div class="col-12">
                        <div class="no-results">
                            <i class="fas fa-search"></i>
                            <h3>No Halls Found</h3>
                            <p>Sorry, no halls are available for your selected criteria. Please try different dates or hall type.</p>
                        </div>
                    </div>
                `);
                $('#resultsHeader').show();
                return;
            }

            $('#resultsHeader').show();

            const resultsHtml = halls.map(hall => `
                <div class="hall-card">
                    <div class="hall-card-header">
                        <h4 class="hall-title">${hall.name}</h4>
                        <span class="hall-type">${$('#hallType option:selected').text()}</span>
                    </div>
                    <div class="hall-card-body">
                        <div class="hall-info">
                            <div class="info-item">
                                <div class="info-icon"><i class="fas fa-map-marker-alt"></i></div>
                                <span>City: ${hall.city}</span>
                            </div>
                            <div class="info-item">
                                <div class="info-icon"><i class="fas fa-users"></i></div>
                                <span>Capacity: ${hall.capacity}</span>
                            </div>
                            <div class="info-item">
                                <div class="info-icon"><i class="fas fa-star"></i></div>
                                <span>Amenities: ${hall.amenities.slice(0, 2).join(', ')}</span>
                            </div>
                        </div>
                
                        <div class="price-section">
                            <div>
                                <div class="price">${hall.price}</div>
                                <div class="price-label">per day</div>
                            </div>
                            <button class="book-btn" onclick="bookHall('${hall.name}')">
                                <i class="fas fa-calendar-plus"></i> Book Now
                            </button>
                        </div>
                    </div>
                </div>
            `).join('');

            $('#resultsContainer').html(resultsHtml);
        }



    }
    // #endregion :: User Hall Search

    //#region :: User Booking Form
    if (_ActionName === "halldetailsbooking") {       
        var allowedDatesStr = $("#allowedDatesHidden").val();
        var allowedDates = [];
        var selectedEventDates = [];
        var singleDatePicker = null;
        var multipleDatePicker = null;

        try {
            allowedDates = JSON.parse(allowedDatesStr);
        } catch (e) {
            console.error("Error parsing allowed dates:", e);
        }

        // Event Duration Type Selection Handler
        $('#eventDurationType').on('change', function () {
            const durationType = $(this).val();

            // Reset all date selections
            resetDateSelections();

            if (durationType === 'single') {
                showSingleDateSelection();
            } else if (durationType === 'multiple') {
                showMultipleDateSelection();
            } else {
                hideDateSelections();
            }
        });

        // Show single date selection
        function showSingleDateSelection() {
            $('#singleDateContainer').slideDown(300);
            $('#multipleDatesContainer').slideUp(300);
            $('#selectedDatesDisplay').slideDown(300);

            // Initialize single date picker if not already done
            if (!singleDatePicker && allowedDates.length > 0) {
                singleDatePicker = flatpickr("#eventDate", {
                    dateFormat: "Y-m-d",
                    enable: allowedDates,
                    minDate: allowedDates[0],
                    maxDate: allowedDates[allowedDates.length - 1],
                    onChange: function (selectedDates, dateStr, instance) {
                        if (dateStr) {
                            selectedEventDates = [dateStr];
                        } else {
                            selectedEventDates = [];
                        }
                        updateSelectedDatesDisplay();
                    }
                });
            }
        }

        // Show multiple date selection
        function showMultipleDateSelection() {
            $('#singleDateContainer').slideUp(300);
            $('#multipleDatesContainer').slideDown(300);
            $('#selectedDatesDisplay').slideDown(300);

            // Initialize multiple date picker if not already done
            if (!multipleDatePicker && allowedDates.length > 0) {
                multipleDatePicker = flatpickr("#eventDatesRange", {
                    mode: "range",
                    dateFormat: "Y-m-d",
                    enable: allowedDates,
                    minDate: allowedDates[0],
                    maxDate: allowedDates[allowedDates.length - 1],
                    onChange: function (selectedDates, dateStr, instance) {
                        if (selectedDates.length === 2) {
                            generateDateRange(selectedDates[0], selectedDates[1]);
                        } else {
                            selectedEventDates = [];
                            updateSelectedDatesDisplay();
                        }
                    }
                });
            }
        }

        // Hide all date selections
        function hideDateSelections() {
            $('#singleDateContainer').slideUp(300);
            $('#multipleDatesContainer').slideUp(300);
            $('#selectedDatesDisplay').slideUp(300);
        }

        // Generate date range between start and end dates
        function generateDateRange(startDate, endDate) {
            selectedEventDates = [];
            let currentDate = new Date(startDate);
            const endDateTime = new Date(endDate);

            while (currentDate <= endDateTime) {
                const dateStr = flatpickr.formatDate(currentDate, "Y-m-d");
                if (allowedDates.includes(dateStr)) {
                    selectedEventDates.push(dateStr);
                }
                currentDate.setDate(currentDate.getDate() + 1);
            }

            updateSelectedDatesDisplay();
        }

        // Update selected dates display for multiple days
        function updateSelectedDatesDisplay() {
            const selectedDatesList = $('#selectedDatesList');
            const totalSelectedDays = $('#totalSelectedDays');
            const durationType = $('#eventDurationType').val();

            selectedDatesList.empty();

            if (selectedEventDates.length > 0) {
                if (durationType === 'single') {
                    // For single date, show the date with clear all option
                    const date = selectedEventDates[0];
                    const formattedDate = new Date(date).toLocaleDateString('en-US', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric'
                    });

                    const badge = $(`
                        <span class="date-badge single-date-badge">
                            <i class="fas fa-calendar-day"></i> ${formattedDate}
                            <button type="button" class="clear-all-dates" title="Clear date">×</button>
                        </span>
                    `);

                    selectedDatesList.append(badge);
                }
                else {
                    const startDate = new Date(selectedEventDates[0]);
                    const endDate = new Date(selectedEventDates[selectedEventDates.length - 1]);

                    const startFormatted = startDate.toLocaleDateString('en-US', {
                        weekday: 'short',
                        month: 'short',
                        day: 'numeric'
                    });
                    const endFormatted = endDate.toLocaleDateString('en-US', {
                        weekday: 'short',
                        month: 'short',
                        day: 'numeric',
                        year: 'numeric'
                    });

                    const badge = $(`
                        <span class="date-badge range-date-badge">
                            <i class="fas fa-calendar-week"></i> ${startFormatted} - ${endFormatted}
                            <button type="button" class="clear-all-dates" title="Clear date range">×</button>
                        </span>
                    `);

                    selectedDatesList.append(badge);
                }
                
            } else {
                selectedDatesList.append('<span class="text-muted">No dates selected</span>');
            }

            totalSelectedDays.text(selectedEventDates.length);
            
        }

        // Clear all selected dates
        $(document).on('click', '.clear-all-dates', function () {
            const durationType = $('#eventDurationType').val();

            if (confirm(`Are you sure you want to clear ${durationType === 'single' ? 'the selected date' : 'the selected date range'}?`)) {
                selectedEventDates = [];
                updateSelectedDatesDisplay();

                // Clear the appropriate picker
                if (durationType === 'single' && singleDatePicker) {
                    singleDatePicker.clear();
                } else if (durationType === 'multiple' && multipleDatePicker) {
                    multipleDatePicker.clear();
                }
            }
        });

        // Get selected dates for form submission
        function getSelectedEventDates() {
            return {
                durationType: $('#eventDurationType').val(),
                selectedDates: selectedEventDates,
                totalDays: selectedEventDates.length
            };
        }

        // Reset all date selections
        function resetDateSelections() {
            selectedEventDates = [];

            // Clear single date picker
            if (singleDatePicker) {
                singleDatePicker.clear();
            }

            // Clear multiple date picker
            if (multipleDatePicker) {
                multipleDatePicker.clear();
            }

            // Clear display
            updateSelectedDatesDisplay();
            $('#eventDate').val('');
            $('#eventDatesRange').val('');
        }       

        $('#phone, #alternatePhone').on('input', function () {
            let value = $(this).val().replace(/\D/g, '');
            if (value.length > 10) {
                value = value.substring(0, 10);
            }
            $(this).val(value);
        });

        // 🔹 Main Form Handler
        $('#bookingForm').on('submit', function (e) {
            e.preventDefault();

            const validation = validateBookingForm();

            if (validation.isValid) {
                const submitBtn = $('.btn-primary-custom');
                const originalText = submitBtn.html();
                submitBtn.html('<i class="fas fa-spinner fa-spin"></i> Processing...')
                    .prop('disabled', true);

                const formData = {
                    catId: $("#hiddenCatId").val(),
                    hallId: $("#hiddenHallId").val(),
                    hallAvailId: $("#HiddenHallAvailId").val(),
                    rate: $("#hiddenRate").val(),
                    securityMoney: $("#hiddenSecurityMoney").val(),
                    initial_payable_amount: $("#hiddenInitialPayableRate").val(),
                    fullName: $("#fullName").val().trim(),
                    phone: $("#phone").val().trim(),
                    alternatePhone: $("#alternatePhone").val().trim(),
                    email: $("#email").val().trim(),
                    address: $('#address').val().trim(),
                    eventType: $("#eventType").val(),
                    eventDate: selectedEventDates.join('^'),
                };

                //console.log(selectedEventDates);
                //return;

                let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

                submitBookingForm(formData, antiForgeryToken);

            } else {
                const firstError = $('.is-invalid').first();
                if (firstError.length) {
                    $('html, body').animate({
                        scrollTop: firstError.offset().top - 100
                    }, 500);
                }
            }
        });

        // 🔹 Separate Validation Function
        function validateBookingForm() {
            let isValid = true;
            let errorMessages = [];

            // Reset errors
            $('.form-control, .form-select, .form-check-input').removeClass('is-invalid');

            const hiddenCatId = $("#hiddenCatId").val();
            const hiddenHallId = $("#hiddenHallId").val();
            const hiddenHallAvailId = $("#HiddenHallAvailId").val();
            if (hiddenCatId === '0' || hiddenHallId === '0' || hiddenHallAvailId === '0') {
                return { isValid: false, errorMessages };
            }

            const fullName = $("#fullName").val().trim();
            if (fullName === '') {
                $('#fullName').addClass('is-invalid');
                errorMessages.push('Please enter Full Name!');
                isValid = false;
            }

            const email = $('#email').val().trim();
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (email === '' || !emailRegex.test(email)) {
                $('#email').addClass('is-invalid');
                errorMessages.push('Please enter a valid email address!');
                isValid = false;
            }

            const phone = $('#phone').val().trim().replace(/\D/g, '');
            if (phone === '' || phone.length !== 10) {
                $('#phone').addClass('is-invalid');
                errorMessages.push('Phone number must be 10 digits!');
                isValid = false;
            }

            const Alterphone = $('#alternatePhone').val().trim().replace(/\D/g, '');
            if (Alterphone === '' || Alterphone.length !== 10) {
                $('#alternatePhone').addClass('is-invalid');
                errorMessages.push('Alternate phone number must be 10 digits!');
                isValid = false;
            }

            const eventType = $("#eventType option:selected").val();
            if (eventType === '0' || eventType === '' || eventType === undefined) {
                $('#eventType').addClass('is-invalid');
                errorMessages.push('Please Select Event Type!');
                isValid = false;
            }

            const eventDurationType = $("#eventDurationType option:selected").val();
            if (eventDurationType === '0' || eventDurationType === '' || eventDurationType === undefined) {
                $('#eventDurationType').addClass('is-invalid');
                errorMessages.push('Please Select Event Duration!');
                isValid = false;
            }

            if (eventDurationType === 'single' && selectedEventDates.length === 0) {
                $('#eventDate').addClass('is-invalid');
                errorMessages.push('Please Select Date!');
                isValid = false;
            }
            if (eventDurationType === 'multiple' && selectedEventDates.length === 0) {
                $('#eventDatesRange').addClass('is-invalid');
                errorMessages.push('Please Select Multiple Date!');
                isValid = false;
            }

            const terms = $("#agreeTerms").is(":checked");
            if (!terms) {
                $('#agreeTerms').addClass('is-invalid');
                errorMessages.push('Please Check Terms!');
                isValid = false;
            }

            return { isValid, errorMessages };
        }

        // 🔹 Separate AJAX Function
        function submitBookingForm(formData, antiForgeryToken) {
            $.ajax({
                url: '/UserBooking/BookUserConfirmedHall',
                type: 'POST',
                data: formData,
                dataType: 'json',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    xhr.setRequestHeader("RequestVerificationToken", antiForgeryToken);
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    if (response.isSuccess) {
                        notify(true, 'Booking Submitted Successfully! Redirecting to payment page...', true);
                        setTimeout(() => {
                            window.location.href = "/UserBooking/UserHallBooking";
                        }, 2000);
                    } else {
                        notify(false, response.errorMessages, false);
                    }
                }
            });
        }


        $("input[name='paymentType']").on("change", function () {
            var selectedPaymentType = $(this).val();

            //var sendValue = selectedPaymentType;
            //if (selectedPaymentType === "1") {
            //    sendValue = "2";
            //}

            const AvailId = $("#HiddenHallAvailId").val();

            $.ajax({
                url: '/UserBooking/GetPaymentSummeryDetails',
                data: { selectedPaymentType: selectedPaymentType, AvailId: AvailId },
                type: 'GET',
                dataType: 'HTML',
                success: function (response) {
                    if (response != '') {
                        $("#partialPaymentSummery").empty();
                        $("#partialPaymentSummery").html(response);                        
                    }

                },
                complete: function () {
                    $(".loader").css("display", "none");
                },
            });
        });

        // Form change tracking
        //let formChanged = false;
        //let formSubmitted = false;

        //// Track form changes
        //$('#bookingForm input, #bookingForm select, #bookingForm textarea').on('input change', function () {
        //    formChanged = true;
        //});

        //// Prevent page leave if form has changes
        //$(window).on('beforeunload', function (e) {
        //    if (formChanged && !formSubmitted) {
        //        const message = 'Are you sure you want to leave? All entered data will be lost.';
        //        e.returnValue = message; // For older browsers
        //        return message; // For modern browsers
        //    }
        //});

        //// Handle browser back/forward buttons
        //window.addEventListener('popstate', function (e) {
        //    if (formChanged && !formSubmitted) {
        //        if (confirm('Are you sure you want to go back? All entered data will be lost.')) {
        //            formChanged = false; // Allow navigation
        //            history.back();
        //        } else {
        //            // Push the current state back to prevent navigation
        //            history.pushState(null, null, window.location.pathname);
        //        }
        //    }
        //});

        //// Add initial history state
        //history.pushState(null, null, window.location.pathname);

        // Back button functionality
        $('.btn-secondary-custom').on('click', function () {
            if (!formChanged || confirm('Are you sure you want to go back? All entered data will be lost.')) {
                //formChanged = false; // Allow navigation
                window.history.back();
            }
        });

        // Handle all navigation links on the page
        //$(document).on('click', 'a[href]:not([href^="#"]):not([target="_blank"])', function (e) {
        //    if (formChanged && !formSubmitted) {
        //        if (!confirm('Are you sure you want to leave? All entered data will be lost.')) {
        //            e.preventDefault();
        //            return false;
        //        } else {
        //            formChanged = false; // Allow navigation
        //        }
        //    }
        //});

        // Handle form submission to mark as submitted
        //$('#bookingForm').on('submit', function () {
        //    formSubmitted = true; // Mark form as submitted to avoid confirmation
        //});

        // Auto-resize textarea
        $('textarea').on('input', function () {
            this.style.height = 'auto';
            this.style.height = (this.scrollHeight) + 'px';
        });
    }

    //#endregion :: User Booking Form

    // #region :: Booking list
    if (_ActionName === "bookinglist") {
        
    }
    // #endregion :: Booking list
});