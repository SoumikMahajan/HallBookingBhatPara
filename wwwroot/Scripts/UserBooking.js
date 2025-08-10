$(function () {
    let _QueryParameter = window.location.pathname.split("/");
    let _ActionName = _QueryParameter[2].toLocaleLowerCase();

    // #region :: Dashboard
    if (_ActionName === "userhallbooking") {
        // Set minimum date to today
        const today = new Date().toISOString().split('T')[0];
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
            const hallType = $('#hallType option:selected').val();
            const startDate = $('#startDate').val();
            const endDate = $('#endDate').val();

            if (hallType == '0' || hallType === undefined ) {
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
            getHallAvailableSearchResult(hallType, startDate, endDate);
           
            //setTimeout(() => {
            //    //const mockResults = generateMockResults(hallType);
            //    //displayResults(mockResults);            
                
            //}, 1500);
        }

        function getHallAvailableSearchResult(hallType, startDate, endDate) {
            $.ajax({
                url: '/UserBooking/HallAvailableSearchResult',
                data: { hallType: hallType, startDate: startDate, endDate: endDate },
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
    }
});