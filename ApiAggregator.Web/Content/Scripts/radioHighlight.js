/// <reference path="jquery-1.9.1.intellisense.js" />

$(function () {
    RadioHighlight.init();
});

var RadioHighlight = {
    init: function () {
        $('.btn-option').on('click', this.highlightOption);
        $('input[type=radio]:checked').click();
    },

    highlightOption: function () {
        var $btns = $(this).siblings('.btn-option');
        var $input = $(this).find('input');
        var highlight = $input.attr('data-highlight');
        var helptext = $input.attr('data-info');
        var $help = $(this).parent().next('.help-block');

        $btns.each(function (index, item) {
            $(item).removeClass($(item).find('input').attr('data-highlight'));
        });
        $(this).addClass(highlight);

        if ($help.length > 0) {
            $help.find('.help-desc').text(helptext);
        }
    }
};