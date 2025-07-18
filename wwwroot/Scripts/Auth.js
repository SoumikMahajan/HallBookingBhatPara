$(function () {
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
    $("#loginForm").on("submit", function (e)  {
        e.preventDefault();

        const email = $('#floatingEmail').val();
        const password = $('#passwordInput').val();

        // Hide previous alerts
        $('.alert').hide();

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

        // Simulate API call
        setTimeout(function () {
            // Reset button state
            $('.login-text').show();
            $('.loading').hide();
            $('.btn-login').prop('disabled', false);

            // Simulate successful login
            if (email === 'admin@gmail.com' && password === 'admin') {
                showAlert('success', 'Login successful! Redirecting to dashboard...');
                setTimeout(function () {
                    // Redirect to dashboard (you can change this URL)
                    window.location.href = '/Home/Index';
                }, 1500);
            } else {
                showAlert('error', 'Invalid email or password. Please try again.');
            }
        }, 2000);
    });

    function isValidEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    }

    // Function to show alerts
    function showAlert(type, message) {
        const alertElement = type === 'error' ? $('#errorAlert') : $('#successAlert');
        const messageElement = type === 'error' ? $('#errorMessage') : $('#successMessage');

        messageElement.text(message);
        alertElement.fadeIn();

        // Auto hide after 5 seconds
        setTimeout(function () {
            alertElement.fadeOut();
        }, 5000);
    }

    // Input animations
    $('.form-control').on('focus', function () {
        $(this).parent().addClass('focused');
    });

    $('.form-control').on('blur', function () {
        if ($(this).val() === '') {
            $(this).parent().removeClass('focused');
        }
    });

    // Forgot password link
    $('.forgot-link').on('click',function (e) {
        e.preventDefault();
        showAlert('success', 'Password reset link will be sent to your email.');
    });

    // Create account link
    $('.register-link a').on('click', function (e) {
        e.preventDefault();
        showAlert('success', 'Registration page will be available soon.');
    });

    // Add floating label effect
    $('.form-floating .form-control').each(function () {
        if ($(this).val() !== '') {
            $(this).addClass('has-value');
        }
    });

    $('.form-floating .form-control').on('blur', function () {
        if ($(this).val() !== '') {
            $(this).addClass('has-value');
        } else {
            $(this).removeClass('has-value');
        }
    });
});