$(function () {
    let _QueryParameter = window.location.pathname.split("/");
    let _ActionName = _QueryParameter[2].toLocaleLowerCase();

    // #region :: Login
    if (_ActionName === "login") {
        // Password toggle functionality

        $(".togglePassword").on("click", function (e) {
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
                        //notify(true, 'Login successful! Redirecting...', true);

                        if (response.result && response.result.redirectUrl) {
                            window.location.href = response.result.redirectUrl;
                        } else {
                            window.location.href = '/Home/Index';
                        }
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

        $(".togglePassword").on("click", function (e) {
            const passwordInput = $('#password');
            const toggleIcon = $(this).find('i');

            if (passwordInput.attr('type') === 'password') {
                passwordInput.attr('type', 'text');
                toggleIcon.removeClass('fa-eye').addClass('fa-eye-slash');
            } else {
                passwordInput.attr('type', 'password');
                toggleIcon.removeClass('fa-eye-slash').addClass('fa-eye');
            }
        });


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
        

        // On form submit
        $('#singleRegisterForm').on('submit', function (e) {
            e.preventDefault();

            if (validateRegistrationForm()) {
                submitRegistrationForm();
            }
        });
        
        function validateRegistrationForm() {
            let hasError = false;

            const firstName = $('#firstName').val()?.trim() || "";
            const lastName = $('#lastName').val()?.trim() || "";
            const email = $('#email').val()?.trim() || "";
            const phone = $('#phone').val()?.trim() || "";
            const gender = $('#gender').val();
            const dob = $('#dob').val()?.trim() || "";
            const address = $('#address').val()?.trim() || "";
            const city = $('#city').val()?.trim() || "";
            const pincode = $('#pincode').val()?.trim() || "";

            const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            const phonePattern = /^\d{10}$/;
            const pinPattern = /^\d{6}$/;

            if (firstName === '') {
                showError("#firstName", "Enter a valid First Name.");
                hasError = true;
            } else {
                showValid('#firstName');
            }

            if (lastName === '') {
                showError("#lastName", "Enter a valid Last Name.");
                hasError = true;
            } else {
                showValid('#lastName');
            }

            if (!emailPattern.test(email)) {
                showError("#email", "Enter a valid email address.");
                hasError = true;
            } else {
                showValid('#email');
            }

            if (!phonePattern.test(phone)) {
                showError("#phone", "Enter a 10-digit mobile number.");
                hasError = true;
            } else {
                showValid('#phone');
            }

            if (gender === '') {
                showError("#gender", "Please select your gender.");
                hasError = true;
            } else {
                showValid('#gender');
            }

            if (!dob) {
                showError("#dob", "Please enter your birthdate.");
                hasError = true;
            } else {
                showValid('#dob');
            }

            if (!address) {
                showError("#address", "Enter your full address.");
                hasError = true;
            } else {
                showValid('#address');
            }

            if (!city) {
                showError("#city", "Enter your city.");
                hasError = true;
            } else {
                showValid('#city');
            }

            if (!pinPattern.test(pincode)) {
                showError("#pincode", "Enter a 6-digit PIN code.");
                hasError = true;
            } else {
                showValid('#pincode');
            }

            return !hasError;
        }

        function submitRegistrationForm() {
            const formData = new FormData();

            formData.append("FirstName", $('#firstName').val()?.trim() || "");
            formData.append("LastName", $('#lastName').val()?.trim() || "");
            formData.append("Email", $('#email').val()?.trim() || "");
            formData.append("Phone", $('#phone').val()?.trim() || "");
            formData.append("Gender", $('#gender').val());
            formData.append("DOB", $('#dob').val()?.trim() || "");
            formData.append("Address", $('#address').val()?.trim() || "");
            formData.append("City", $('#city').val()?.trim() || "");
            formData.append("Pincode", $('#pincode').val()?.trim() || "");
            formData.append("Password", $('#password').val());
            formData.append("BasePassword", $('#password').val());

            const fileInput = $('#profilePic')[0];
            if (fileInput?.files?.length > 0) {
                formData.append("ProfilePic", fileInput.files[0]);
            }

            $.ajax({
                url: '/User/Registration',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    $('.btn-register').prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Registering...');
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    if (response.isSuccess) {
                        notify(true, response.result, true);                        
                        setTimeout(function () {
                            window.location.href = '/User/Login';
                        }, 1500);
                    }
                    else {
                        notify(false, response.errorMessages, false);
                    }
                },
                complete: function () {
                    $('.btn-register').prop('disabled', false).html('<i class="fas fa-user-plus me-2"></i>Create Account');
                }
            });
        }
    }
    // #endregion :: Registrion

});