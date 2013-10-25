/// <reference path="jquery-1.9.1.intellisense.js" />

$(function () {
    Mapping.init();
});

var Mapping = {
    init: function () {
        $('.form-control').popover(
            { trigger: 'hover' }
        );
    }
};