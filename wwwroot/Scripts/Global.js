const _BaseURL = window.location.origin;
const currentlocation = window.location.href;
const _QueryParameter = window.location.pathname.split('/');
const actionName = _QueryParameter[2] || '';


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
    
});


$.ajaxSetup({
    statusCode: {
        401: function (xhr) {
            handleAuthRedirect(xhr);
        },
        403: function (xhr) {
            handleAuthRedirect(xhr);
        }
    },
    error: function (xhr, status, error) {
        $(".loader").css("display", "none");

        if (xhr.status === 401 || xhr.status === 403) return;

        let errorMessage = "Something went wrong.";

        if (xhr.status === 0) {
            errorMessage = "Cannot connect to the server";
        } else {
            try {
                const response = JSON.parse(xhr.responseText);
                if (response?.message) {
                    errorMessage = response.message;
                } else if (xhr.statusText) {
                    errorMessage = xhr.statusText;
                }
            } catch {
                errorMessage = error || xhr.statusText || "Unexpected error occurred.";
            }
        }

        notify(false, errorMessage, false);
        console.error(`[AJAX Error] ${xhr.status}: ${errorMessage}`);
    }
});
function handleAuthRedirect(xhr) {
    try {
        const response = JSON.parse(xhr.responseText);
        if (response.redirectUrl) {
            notify(false, response.message || "Soemthing went wrong. Redirecting...", false);
            window.location.href = response.redirectUrl;
        } else {
            window.location.href = '/Home/Index';
        }
    } catch {
        window.location.href = '/Home/Index';
    }
}

function notify(IsSuccess, Title, IsValid, position, timeout) {

    position = position == null || position == undefined ? "topRight" : position;

    if (IsSuccess) {
        iziToast.success({
            title: 'SUCCESS',
            position: position,
            message: Title,
        });
    }
    else {
        const options = {
            title: IsValid ? 'Error' : 'Caution',
            position: position,
            message: Title,

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

function debounce(func, delay) {
    let timer;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => {
            func.apply(this, args);
        }, delay);
    };
}