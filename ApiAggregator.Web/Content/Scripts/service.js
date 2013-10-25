/// <reference path="jquery-1.9.1.intellisense.js" />

$(function () {
    Service.init();
});

var Service = {
    init: function () {
        $('.add-row').on('click', this.addRow);
        $(document).on('click', '.remove-row', this.removeRow);
        $('.service-template').on('click', this.populateFromTemplate);
        $('.form-control').popover(
            { trigger: 'hover' }
        );
    },

    addRow: function() {
        var $table = $(this).closest('.row').find('table');
        var type = $table.attr('data-type');
        var context = {
            type: type,
            index: parseInt($table.attr('data-count')),
            prop: type == 'Headers' ? 'Header' : 'Name'
        };
        var row = Service.getCompiledRow(context);
        $table.find('tbody').append(row);
        $table.attr('data-count', context.index + 1);
        $table.find('tr').last().find('input').first().focus();
    },

    removeRow: function () {
        var $row = $(this).closest('tr');
        var $table = $row.closest('table');

        $row.remove();
        $table.attr('data-count', $table.find('tbody tr').length);
        Service.reIndexRows($table);
    },

    reIndexRows: function ($table) {
        var updateId = function (id, newIndex) {
            return id.replace(/(_)(\d+)(__)/g, '$1' + newIndex + '$3');
        };
        var updateArray = function(array, newIndex) {
            return array.replace(/(\w+\[)(\d+)(\]\.\w+)/g, '$1' + newIndex + '$3')
        };

        $table.find('tbody tr').each(function (rowIndex, item) {
            $(item).find('input').each(function (index, item) {
                $(item).attr('id', updateId($(item).attr('id'), rowIndex));
                $(item).attr('name', updateArray($(item).attr('name'), rowIndex));
            });

            $(item).find('span.field-validation-valid').each(function (index, item) {
                var newval = updateArray($(item).attr('data-valmsg-for'), rowIndex);
                $(item).attr('data-valmsg-for', newval);
            });
        });
    },
    
    getCompiledRow: function (context) {
        // context: {type, index, prop}
        var src = $('#row-template').html();
        var tmpl = Handlebars.compile(src);
        return tmpl(context);
    },

    populateFromTemplate: function () {        
        $('#Name').val($(this).text());
        $('#RootUrl').val($(this).attr('data-root-url'));

        // clear any existing headers / querystring pairs before
        // adding template values
        $('.remove-row').click();
        $(this).siblings('span')
            .map(Service.getNameValuePairs)
            .each(Service.populateTemplateRow);

        // Give focus to the first empty textbox in the form
        $('input:text').filter(function () {
            return $.trim($(this).val()).length === 0;
        }).first().focus();
    },

    getNameValuePairs: function () {
        return {
            'name': $(this).attr('data-name'),
            'value': $(this).attr('data-value'),
            'type': /service-(.*)\s*/g.exec($(this).attr('class'))[1]
        };
    },

    populateTemplateRow: function () {
        var $row = $('.add-row.row-' + this.type)
            .click()
            .closest('.row')
            .find('tr:last');

        $row.find('td:nth-child(1) input').val(this.name);
        $row.find('td:nth-child(2) input').val(this.value);
    }
};