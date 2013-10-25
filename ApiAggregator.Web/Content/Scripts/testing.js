/// <reference path="jquery-1.9.1.intellisense.js" />

$(function () {
    Testing.init();
});

var Testing = {
    init: function () {
        $('button:not(.reset)').on('click', this.getData);
        $('button.reset').on('click', function () {
            $('.variable').first().focus();
            $('.error').addClass('hidden');
        });
        PR.prettyPrint();
    },

    getData: function () {
        var form = $('form').serializeArray();
        
        if (Testing.validateForm(form)) {
            $('.btn').attr('disabled', true);
            $('.loading').removeClass('hidden');

            var $pre = $('pre').text('').removeClass('prettyprinted');
            var debug = $(this).hasClass('debug');
            var payload = Testing.generateObject(form, debug);

            $.post('/test/mapping', payload)
                .done(function (result) {
                    $pre.text(JSON.stringify(result, null, 4));
                })
                .fail(function (result) {
                    // parsing & re-stringing for formatting
                    var response = JSON.parse(result.responseText);
                    $pre.text(JSON.stringify(response, null, 4));
                })
                .always(function () {
                    $('.loading').addClass('hidden');
                    PR.prettyPrint();
                    $('.btn').removeAttr('disabled');
                });
        }
    },

    validateForm: function (form) {
        var isValid = true;
        $(form).each(function (index, item) {
            if ($.trim(item.value).length === 0) {
                $('.error').removeClass('hidden');
                $('input[name=' + item.name + ']').focus();
                return isValid = false;
            }
        });
        if (isValid) {
            $('.error').addClass('hidden');
            return true;
        }
    },

    generateObject: function (form, debug) {
        var data = {};

        form.map(function (item) { data[item.name] = item.value; });
        var id = data.id;
        delete data.id;

        return {
            'id': id,
            'debug': debug,
            'pairs': data
        };
    }
};