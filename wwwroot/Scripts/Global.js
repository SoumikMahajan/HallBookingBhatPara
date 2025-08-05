const _BaseURL = window.location.origin;
const currentlocation = window.location.href;
const _QueryParameter = window.location.pathname.split('/');
const actionName = _QueryParameter[2] || '';
const currentPath = window.location.pathname.toLowerCase();


const styles = [
    'background: linear-gradient(#204d74, #7792a8)'
    , 'border: 1px solid #3E0E02'
    , 'color: white'
    , 'display: block'
    , 'text-shadow: 0 1px 0 rgba(0, 0, 0, 0.3)'
    , 'box-shadow: 0 1px 0 rgba(255, 255, 255, 0.4) inset, 0 5px 3px -5px rgba(0, 0, 0, 0.5), 0 -13px 5px -10px rgba(255, 255, 255, 0.4) inset'
    , 'line-height: 40px'
    , 'text-align: center'
    , 'font-weight: bold'
    , 'font-size: 20px'
].join(';');

console.log('%c Thank you for using Bhatpara Municipality Hall Booking.', styles);
console.log('%c This console is used by developers for development purpose, please avoid writing codes here to avoid malfunction of Website.', styles);

$(function () {
    $('.navbar-nav .nav-link').each(function () {
        const linkPath = $(this).attr('href').toLowerCase();
        if (currentPath === linkPath || currentPath.startsWith(linkPath)) {
            $('.navbar-nav .nav-link').removeClass('active');
            $(this).addClass('active');
        }
    });
});


$.ajaxSetup({
    statusCode: {
        401: handleAuthRedirect,
        403: handleAuthRedirect
    },
    error: handleAjaxError
});
function handleAuthRedirect(xhr) {
    try {
        const response = JSON.parse(xhr.responseText);
        const redirectUrl = response?.result?.redirectUrl || "/User/Login";
        const message = response?.errorMessages?.[0] || response?.message || "Redirecting...";

        notify(false, message, false);
        window.location.href = redirectUrl;
    } catch {
        window.location.href = '/User/Login';
    }
}

function handleAjaxError(xhr, status, error) {
    $(".loader").css("display", "none");

    if (xhr.status === 401 || xhr.status === 403) return;

    let errorMessage = "Something went wrong.";
    try {
        const response = JSON.parse(xhr.responseText);
        if (response?.errorMessages?.length > 0) {
            errorMessage = response.errorMessages[0];
        } else if (response?.message) {
            errorMessage = response.message;
        } else if (xhr.statusText) {
            errorMessage = xhr.statusText;
        }
    } catch {
        errorMessage = error || xhr.statusText || "Unexpected error occurred.";
    }

    notify(false, errorMessage, false);
    console.error(`[AJAX Error] ${xhr.status}: ${errorMessage}`);
}

function notify(IsSuccess, Title, IsValid, position, timeout) {

    position = position == null || position == undefined ? "topCenter" : position;

    if (IsSuccess) {
        iziToast.success({
            title: 'SUCCESS',
            position: position,
            message: Title,
            class: 'large-toast',
        });
    }
    else {
        const options = {
            title: IsValid ? 'Error' : 'Caution',
            position: position,
            message: Title,
            class: 'large-toast',
        };

        //optional timeout property set, else the default timeout of iziToast will work
        if (timeout != null || timeout != undefined) {
            options.timeout = timeout;
        }

        if (IsValid) {
            iziToast.error(options);
        } else {
            iziToast.warning(options);
        }
    }

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

function debounce(func, delay) {
    let timer;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => {
            func.apply(this, args);
        }, delay);
    };
}

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

// Input animations
$('.form-control').on('focus', function () {
    $(this).parent().addClass('focused');
});

$('.form-control').on('blur', function () {
    if ($(this).val() === '') {
        $(this).parent().removeClass('focused');
    }
});