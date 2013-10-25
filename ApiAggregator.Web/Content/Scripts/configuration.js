/// <reference path="jquery-1.9.1.intellisense.js" />

$(function () {
    Security.init();
});

var Security = {
    init: function () {
        $('.tt').tooltip();
        $('.btn-keygen').on('click', this.generateKey);
    },

    generateKey: function () {
        var $btn = $(this).attr('disabled', true);
        var $alert = $('.alert-keygen');

        $.post('/configuration/generateapikey')
            .done(function (result) {
                if (result.Success) {
                    $('.input-apikey').val(result.Message);
                    $alert.addClass('hidden');
                } else {
                    $alert.text(result.Message).removeClass('hidden');
                }
            })
            .fail(function () {
                $alert.text('There was an error updating the API key!').removeClass('hidden');
            })
            .always(function () {
                $btn.removeAttr('disabled');
            });
    }
};