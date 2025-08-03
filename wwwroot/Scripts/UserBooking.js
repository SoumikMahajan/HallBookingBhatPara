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
            $('#endDate').attr('min', this.value);
        });

        // Form submission
        $('#searchForm').on('submit', function (e) {
            e.preventDefault();
            searchHalls();
        });
        function searchHalls() {
            const hallType = $('#hallType').val();
            const startDate = $('#startDate').val();
            const endDate = $('#endDate').val();

            if (!hallType || !startDate || !endDate) {
                alert('Please fill in all fields');
                return;
            }

            if (new Date(startDate) > new Date(endDate)) {
                alert('End date must be after start date');
                return;
            }

            // Show loading
            $(".loader").css("display", "flex");
            $('#resultsHeader').hide();
            $('#resultsContainer').empty();

            $('html, body').animate({
                scrollTop: $('.results-section').offset().top - 80
            }, 800);

            // Simulate API call with setTimeout
            setTimeout(() => {
                const mockResults = generateMockResults(hallType);
                displayResults(mockResults);

                // After results are displayed, scroll to show results section properly
                setTimeout(() => {
                    // Calculate scroll position to show search form at top and results below
                    const searchHero = $('.search-hero');
                    const searchHeroHeight = searchHero.outerHeight();

                    $('html, body').animate({
                        scrollTop: searchHeroHeight - 50 // Small offset to show part of search form
                    }, 800, 'swing');
                }, 100);

            }, 1500);
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
});