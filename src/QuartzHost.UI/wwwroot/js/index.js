﻿/**
 * Des：.js
 * Author：dldg
 * CreateTime：2020/02/28
 **/

var _this;

var QuartzHostUI =
{
    init: function (ltype) {
        $(function () {
            _this = QuartzHostUI;
            if (ltype === "base") {
                //LazyLoad.js("/adminlte/plugins/bootstrap/js/bootstrap.bundle.min.js", function () { console.log(ltype); });
                LazyLoad.js("/adminlte/js/adminlte.min.js", function () { console.log(ltype); });
                //LazyLoad.js("/adminlte/plugins/datatables/js/jquery.dataTables.min.js", function () { console.log(ltype); });
                //LazyLoad.js("/adminlte/plugins/datatables/js/dataTables.bootstrap4.min.js", function () { console.log(ltype); });
            }
        });
    },
    dataTableLang: {
        "sProcessing": "处理中...",
        "sLengthMenu": "每页 _MENU_ 项",
        "sZeroRecords": "没有匹配结果",
        "sInfo": "当前显示第 _START_ 至 _END_ 项，共 _TOTAL_ 项。",
        "sInfoEmpty": "当前显示第 0 至 0 项，共 0 项",
        "sInfoFiltered": "(由 _MAX_ 项结果过滤)",
        "sInfoPostFix": "",
        "sSearch": "搜索:",
        "sUrl": "",
        "sEmptyTable": "表中数据为空",
        "sLoadingRecords": "载入中...",
        "sInfoThousands": ",",
        "oPaginate": {
            "sFirst": "首页",
            "sPrevious": "上页",
            "sNext": "下页",
            "sLast": "末页",
            "sJump": "跳转"
        },
        "oAria": {
            "sSortAscending": ": 以升序排列此列",
            "sSortDescending": ": 以降序排列此列"
        }
    },
    initDataTable: function (name) {
        $(function () {
            console.log(name);
            $(name).DataTable({ language: QuartzHostUI.dataTableLang });
        });
    },
    removeDataTable: function (name) {
        $(function () {
            $(name).DataTable().destroy();
        });
    },
    onnav: function () {
        alert(1);
    }
};