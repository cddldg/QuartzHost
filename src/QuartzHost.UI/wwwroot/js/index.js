/**
 * Des：.js
 * Author：dldg
 * CreateTime：2020/02/28
 **/

var _this;
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}
var QHUI =
{
    init: function () {
        QHUI.menu();
    },
    initDataTable: function (name) {
    },
    removeDataTable: function (name) {
    },
    menu: function () {
    },
    toastrs: function (type, title, msg) {
        toastr[type](msg, title);
    }
};