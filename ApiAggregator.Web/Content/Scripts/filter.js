/// <reference path="jquery-1.9.1.intellisense.js" />

$(function () {
    Filter.init();
});

var Filter = {
    init: function () {
        $('.service-filter a').on('click', this.filter);
    },

    filter: function () {
        var value = $(this).text();
        var $rows = $('table tbody tr');
        $('.current-filter').text(value);

        if (value === 'All') {
            $rows.show();
        } else {
            $rows.each(function () {
                var name = $(this).find('td:nth-child(2)').text();
                $(this).toggle(name === value);
            });
        }
    }
};