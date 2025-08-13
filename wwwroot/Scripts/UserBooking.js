$(function () {
    let _QueryParameter = window.location.pathname.split("/");
    let _ActionName = _QueryParameter[2].toLocaleLowerCase();

    // #region :: Dashboard
    if (_ActionName === "userhallbooking") {
        // Set minimum date to today
        const today = new Date().toISOString().split('T')[0];
        $('#startDate').val(today);
        $('#endDate').val(today);
        $('#startDate, #endDate').attr('min', today);

        // Update end date minimum when start date changes
        $('#startDate').on('change', function () {
            const startDate = this.value;
            $('#endDate').attr('min', startDate);

            // Optional: If endDate is already selected and now invalid, clear it
            if ($('#endDate').val() && $('#endDate').val() < startDate) {
                $('#endDate').val('');
            }
        });
        // When end date changes, update the max for start date
        $('#endDate').on('change', function () {
            const endDate = this.value;
            $('#startDate').attr('max', endDate);

            // Optional: If startDate is already selected and now invalid, clear it
            if ($('#startDate').val() && $('#startDate').val() > endDate) {
                $('#startDate').val('');
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

            if (catType == '0' || catType === undefined ) {
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
    // #endregion :: Dashboard

    if (_ActionName === "halldetailsbooking") {
        
        var allowedDatesStr = $("#allowedDatesHidden").val();

        var allowedDates = [];
        try {
            allowedDates = JSON.parse(allowedDatesStr);
        } catch (e) {
            console.error("Error parsing allowed dates:", e);
        }

        if (allowedDates.length > 0) {
            flatpickr("#eventDate", {
                dateFormat: "Y-m-d",
                enable: allowedDates,
                minDate: allowedDates[0],
                maxDate: allowedDates[allowedDates.length - 1]
            });
        }
        
        $('#phone, #alternatePhone').on('input', function () {
            let value = $(this).val().replace(/\D/g, '');
            if (value.length > 10) {
                value = value.substring(0, 10);
            }
            $(this).val(value);
        });

        // Form validation and submission
        $('#bookingForm').on('submit', function (e) {
            e.preventDefault();

            let isValid = true;
            let errorMessages = [];
            // Remove previous validation classes
            $('.form-control, .form-select, .form-check-input').removeClass('is-invalid');

            const hiddenCatId = $("#hiddenCatId").val();
            const hiddenHallId = $("#hiddenHallId").val();
            const hiddenHallAvailId = $("#HiddenHallAvailId").val();
            if (hiddenCatId === '0' || hiddenHallId === '0' || hiddenHallAvailId === '0') {
                return;
            }            

            const fullName = $("#fullName").val().trim();
            if (fullName === '') {                
                $('#fullName').addClass('is-invalid');
                errorMessages.push('Please enter Full Name!');               
                isValid = false;
            }

            // Email validation
            const email = $('#email').val().trim();
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (email === '' || !emailRegex.test(email)) {
                $('#email').addClass('is-invalid');
                errorMessages.push('Please enter a valid email address!');  
                isValid = false;
            }

            // Phone number validation (10 digits)
            const phone = $('#phone').val().trim().replace(/\D/g, '');
            if (phone === '' || phone.length !== 10) {
                $('#phone').addClass('is-invalid');
                errorMessages.push('Phone number must be 10 digits!');  
                isValid = false;
            }

            // Alternate phone number validation (10 digits)
            const Alterphone = $('#alternatePhone').val().trim().replace(/\D/g, '');
            if (Alterphone === '' || Alterphone.length !== 10) {
                $('#alternatePhone').addClass('is-invalid');
                errorMessages.push('Phone number must be 10 digits!');  
                isValid = false;
            }

            //Event name
            const eventName = $("#eventName").val().trim();
            if (eventName === '') {
                $('#eventName').addClass('is-invalid');
                errorMessages.push('Please Enter Event Name!');
                isValid = false;
            }

            //Event type
            const eventType = $("#eventType option:selected").val()
            if (eventType === '0' || eventType === '' || eventType === undefined) {
                $('#eventType').addClass('is-invalid');
                errorMessages.push('Please Select Event Type!');  
                isValid = false;
            }

            //Event date
            const eventDate = $("#eventDate").val()
            if (eventDate === '') {
                $('#eventDate').addClass('is-invalid');
                errorMessages.push('Please Select Event Date!');  
                isValid = false;
            }

            //Terms

            const terms = $("#agreeTerms").is(":checked");
            if (!terms) {
                $('#agreeTerms').addClass('is-invalid');
                errorMessages.push('Please Check Terms!');  
                isValid = false;
            }

            if (isValid) {
                // Show loading state
                const submitBtn = $('.btn-primary-custom');
                const originalText = submitBtn.html();
                submitBtn.html('<i class="fas fa-spinner fa-spin"></i> Processing...').prop('disabled', true);

                // Collect form data
                const formData = {
                    hallAvailId: HallAvailId,
                    catId: hiddenCatId,
                    hallId: HallId,
                    fullName: fullName,
                    email: email,
                    phone: phone,
                    alternatePhone: Alterphone,
                    address: $('#address').val().trim(),
                    eventName: eventName,
                    eventType: eventType,
                    eventDate: eventDate,                    
                    eventDescription: $('#eventDescription').val(),                    
                    totalAmount: $('#totalAmount').text()
                };

                let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

                $.ajax({
                    url: '/Admin/BookUserConfirmedHall',
                    type: 'POST',
                    data: formData,
                    contentType: false,
                    processData: false,
                    dataType: 'json',
                    beforeSend: function (xhr) {
                        $(".loader").css("display", "flex");
                        xhr.setRequestHeader("RequestVerificationToken", antiForgeryToken);
                    },
                    success: function (response) {
                        $(".loader").css("display", "none");
                        if (response.isSuccess) {
                            notify(true, 'Booking Submitted Successfully! Redirecting to payment page...', true);
                            
                        } else {
                            notify(false, response.errorMessages, false);
                        }

                    }
                });
               
                setTimeout(function () {
                    // Success notification
                    showNotification('success', 'Booking Submitted Successfully!', 'Redirecting to payment page...');

                    // In real application, you would submit to server:
                    // $.post('/api/bookings', formData)
                    //   .done(function(response) {
                    //     window.location.href = '/payment/' + response.bookingId;
                    //   })
                    //   .fail(function(xhr) {
                    //     showNotification('error', 'Booking Failed', xhr.responseJSON.message);
                    //   });

                    // For demo, redirect after 2 seconds
                    setTimeout(function () {
                        // window.location.href = 'payment.html';
                        console.log('Form Data:', formData);
                    }, 2000);

                }, 1500);

                //// Collect selected services
                //$('input[type="checkbox"]:checked').each(function () {
                //    if ($(this).attr('id') !== 'agreeTerms') {
                //        formData.additionalServices.push({
                //            service: $(this).next('label').text().trim(),
                //            price: $(this).val()
                //        });
                //    }
                //});

                //// Simulate API call
                //setTimeout(function () {
                //    // Success notification
                //    showNotification('success', 'Booking Submitted Successfully!', 'Redirecting to payment page...');

                //    // In real application, you would submit to server:
                //    // $.post('/api/bookings', formData)
                //    //   .done(function(response) {
                //    //     window.location.href = '/payment/' + response.bookingId;
                //    //   })
                //    //   .fail(function(xhr) {
                //    //     showNotification('error', 'Booking Failed', xhr.responseJSON.message);
                //    //   });

                //    // For demo, redirect after 2 seconds
                //    setTimeout(function () {
                //        // window.location.href = 'payment.html';
                //        console.log('Form Data:', formData);
                //    }, 2000);

                //}, 1500);

            } else {                
                notify(false, 'Please fix the following errors:' + errorMessages.join(', '), false);

                // Scroll to first error
                const firstError = $('.is-invalid').first();
                if (firstError.length) {
                    $('html, body').animate({
                        scrollTop: firstError.offset().top - 100
                    }, 500);
                }
            }
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
});