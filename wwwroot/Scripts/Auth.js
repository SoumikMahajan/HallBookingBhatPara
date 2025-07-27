$(function () {
    let _QueryParameter = window.location.pathname.split("/");
    let _ActionName = _QueryParameter[2].toLocaleLowerCase();

    // #region :: Login
    if (_ActionName === "login") {
        // Password toggle functionality

        $("#togglePassword").on("click", function (e) {
            const passwordInput = $('#passwordInput');
            const toggleIcon = $(this).find('i');

            if (passwordInput.attr('type') === 'password') {
                passwordInput.attr('type', 'text');
                toggleIcon.removeClass('fa-eye').addClass('fa-eye-slash');
            } else {
                passwordInput.attr('type', 'password');
                toggleIcon.removeClass('fa-eye-slash').addClass('fa-eye');
            }
        });

        // Form submission with animation
        $("#loginForm").on("submit", function (e) {
            e.preventDefault();

            // Hide previous alerts
            $('.alert').hide();
            const email = $('#floatingEmail').val();
            const password = $('#passwordInput').val();

            // Basic validation
            if (!email || !password) {
                showAlert('error', 'Please fill in all required fields.');
                return;
            }

            if (!isValidEmail(email)) {
                showAlert('error', 'Please enter a valid email address.');
                return;
            }

            // Show loading state
            $('.login-text').hide();
            $('.loading').show();
            $('.btn-login').prop('disabled', true);

            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            // Get form data
            const formData = {
                Email: email,
                Password: password,
                RememberMe: $('#rememberMe').is(':checked')
            };

            $.ajax({
                url: '/User/Login',
                type: 'POST',
                data: formData,
                dataType: 'json',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    xhr.setRequestHeader("RequestVerificationToken", antiForgeryToken);
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    $('.login-text').show();
                    $('.loading').hide();
                    $('.btn-login').prop('disabled', false);
                    if (response.isSuccess) {
                        notify(true, 'Login successful! Redirecting...', true);

                        // Redirect after a short delay to show success message
                        setTimeout(function () {
                            if (response.result && response.result.redirectUrl) {
                                window.location.href = response.result.redirectUrl;
                            } else {
                                window.location.href = '/Home/Index';
                            }
                        }, 1500);
                    } else {
                        notify(false, 'Invalid email or password. Please try again.', false);
                    }

                }
            });
        });

        function isValidEmail(email) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(email);
        }

        // Forgot password link
        $('.forgot-link').on('click', function (e) {
            e.preventDefault();
            showAlert('success', 'Password reset link will be sent to your email.');
        });
    }
    // #endregion :: Dashboard


    // #region :: Registrion
    if (_ActionName === "registration") {
        const fileInput = $('#profilePic');
        const uploadArea = $('.upload-area');
        const previewImage = $('#previewImage');
        const fileNameDisplay = $('#fileNameDisplay');

        // Prevent default drag behaviors
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            uploadArea.on(eventName, function (e) {
                e.preventDefault();
                e.stopPropagation();
            });
        });

        // Highlight on drag
        uploadArea.on('dragover', function () {
            uploadArea.addClass('dragover');
        });

        // Remove highlight on drag leave
        uploadArea.on('dragleave drop', function () {
            uploadArea.removeClass('dragover');
        });

        // Handle file drop
        uploadArea.on('drop', function (e) {
            const files = e.originalEvent.dataTransfer.files;
            if (files.length > 0) {
                fileInput[0].files = files;
                handleFile(files[0]);
            }
        });

        // Handle manual file selection
        fileInput.on('change', function () {
            const file = this.files[0];
            if (file) {
                handleFile(file);
            }
        });

        // Preview and display file
        function handleFile(file) {
            const validTypes = ['image/jpeg', 'image/png', 'image/jpg'];
            const maxSize = 2 * 1024 * 1024; // 2MB

            if (!validTypes.includes(file.type)) {
                alert("Only JPG, JPEG, or PNG files are allowed.");
                fileInput.val('');
                previewImage.hide();
                fileNameDisplay.text("No file selected");
                return;
            }

            if (file.size > maxSize) {
                alert("File size must be under 2MB.");
                fileInput.val('');
                previewImage.hide();
                fileNameDisplay.text("No file selected");
                return;
            }

            fileNameDisplay.text(file.name);

            const reader = new FileReader();
            reader.onload = function (e) {
                previewImage.attr('src', e.target.result).show();
            };
            reader.readAsDataURL(file);
        }

        function showError(selector, message) {
            const $input = $(selector);
            const $parent = $input.closest(".form-floating, .input-group, .form-check");

            $input.addClass("is-invalid");
            $parent.find(".invalid-feedback").text(message).show();

            // Auto-clear after 2 seconds
            setTimeout(() => {
                $input.removeClass("is-invalid");
                $parent.find(".invalid-feedback").hide();
            }, 2000);
        }

        function showValid(input) {
            const $input = $(input);
            const $container = $input.closest('.form-floating, .input-group, .form-check');

            $input.removeClass('is-invalid');
            $container.find('.invalid-feedback').hide();
        }

        //// Field validation on blur
        //$('.form-control, .form-select').on('blur', function () {
        //    if ($(this).hasClass('is-invalid')) {
        //        $(this).removeClass('is-invalid');
        //        $(this).closest('.form-group').find('.invalid-feedback').hide();
        //    }
        //});

        // On form submit
        $('#singleRegisterForm').on('submit', function (e) {
            e.preventDefault();

            let hasError = false;

            const firstName = $('#firstname').val()?.trim() || "";
            const lastName = $('#lastname').val()?.trim() || "";
            const email = $('#email').val()?.trim() || "";
            const phone = $('#phone').val()?.trim() || "";
            const gender = $('#gender').val();
            const dob = $('#dob').val().trim();
            const address = $('#address').val()?.trim() || "";
            const city = $('#city').val()?.trim() || "";
            const pincode = $('#pincode').val()?.trim() || "";

            const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            const phonePattern = /^\d{10}$/;           
            const pinPattern = /^\d{6}$/;

            if (firstName == '') {
                showError("#firstname", "Enter a valid First Name (only letters, min 2 chars).");
                hasError = true;
            }
            else {
                showValid('#firstName');
            }
            if (lastName == '') {
                showError("#lastname", "Enter a valid Last Name (only letters, min 2 chars).");
                hasError = true;
            }
            else {
                showValid('#lastName');
            }
            if (!emailPattern.test(email)) {
                showError("#email", "Enter a valid email address.");
                hasError = true;
            }
            else {
                showValid('#email');
            }
            if (!phonePattern.test(phone)) {
                showError("#phone", "Enter a 10-digit mobile number.");
                hasError = true;
            }
            else {
                showValid('#email');
            }

            if (!gender) {
                showError(document.getElementById("gender"), "Please select your gender.");
                hasError = true;
            }
            else {
                showValid('#email');
            }
            if (!dob) {
                showError("#dob", "Please enter your birthdate.");
                hasError = true;
            }
            else {
                showValid('#email');
            }
            if (!address) {
                showError("#address", "Enter your full address.");
                hasError = true;
            }
            else {
                showValid('#email');
            }
            if (!city) {
                showError("#city", "Enter your city.");
                hasError = true;
            }
            else {

            }
            if (!pinPattern.test(pincode)) {
                showError("#pincode", "Enter a 6-digit PIN code.");
                hasError = true;
            }
            else {

            }

            if (!hasError) {
                const formData = new FormData(this);

                $.ajax({
                    url: '/your/api/register',
                    type: 'POST',
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        alert("Registration successful!");
                    },
                    error: function (err) {
                        alert("An error occurred.");
                    }
                });
            }
        });

        //$('#singleRegisterForm').on('submit', function (e) {
        //    e.preventDefault();

        //    const isValid =
        //        validateField('firstName', val => val !== '', 'Enter First Name.') &&
        //        validateField('lastName', val => val !== '', 'Enter Last Name.') &&
        //        validateField('email', val => emailPattern.test(val), 'Enter valid email.') &&
        //        validateField('phone', val => phonePattern.test(val), 'Enter 10-digit mobile number.') &&
        //        validateField('gender', val => val !== '', 'Select gender.') &&
        //        validateField('dob', val => val !== '', 'Enter DOB.') &&
        //        validateField('address', val => val !== '', 'Enter address.') &&
        //        validateField('city', val => val !== '', 'Enter city.') &&
        //        validateField('pincode', val => pincodePattern.test(val), 'Enter valid 6-digit PIN code.') &&
        //        validateField('password', val => passwordPattern.test(val), 'Password too weak.') &&
        //        validatePasswordMatch() &&
        //        $('#terms').is(':checked');

        //    if (!isValid) {
        //        return;
        //    }

        //    // Prepare form data for AJAX
        //    const formData = new FormData();
        //    formData.append("FirstName", $('#firstName').val());
        //    formData.append("LastName", $('#lastName').val());
        //    formData.append("Email", $('#email').val());
        //    formData.append("Phone", $('#phone').val());
        //    formData.append("Gender", $('#gender').val());
        //    formData.append("DOB", $('#dob').val());
        //    formData.append("Address", $('#address').val());
        //    formData.append("City", $('#city').val());
        //    formData.append("Pincode", $('#pincode').val());
        //    formData.append("Password", $('#password').val());
        //    formData.append("ConfirmPassword", $('#confirmPassword').val());

        //    const file = $('#profilePic')[0].files[0];
        //    if (file) formData.append("ProfilePic", file);

        //    // Replace this with your actual endpoint URL
        //    $.ajax({
        //        url: '/User/Register',
        //        type: 'POST',
        //        data: formData,
        //        contentType: false,
        //        processData: false,
        //        beforeSend: function () {
        //            $('.btn-register').prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Registering...');
        //        },
        //        success: function (response) {
        //            if (response.success) {
        //                alert('Registration successful!');
        //                window.location.href = '/User/Login';
        //            } else {
        //                alert(response.message || 'Registration failed!');
        //            }
        //        },
        //        error: function () {
        //            alert('Something went wrong. Please try again.');
        //        },
        //        complete: function () {
        //            $('.btn-register').prop('disabled', false).html('<i class="fas fa-user-plus me-2"></i>Create Account');
        //        }
        //    });
        //});
    }
    // #endregion :: Registrion

});