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