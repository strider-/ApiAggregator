/// <reference path="jquery-1.9.1.intellisense.js" />

$(function () {
    Prompt.init();
});

var Prompt = {
    init: function () {
        $('.delete-prompt').on('click', this.showModal);
        $(document).on('click', '.delete-confirm', this.deleteItem);
    },

    showModal: function () {
        var context = Prompt.getContext($(this));
        var modal = Prompt.getCompiledModal(context);
        var $row = $(this).closest('tr');

        var state = $row.addClass('danger').hasClass('warning');
        $row.removeClass('warning');

        $('body').append($.parseHTML(modal));
        $('#deleteModal')
            .on('hide.bs.modal', function() { $row.removeClass('danger').toggleClass('warning', state); })
            .on('hidden.bs.modal', Prompt.removeModal)
            .modal('show');
    },

    removeModal: function () {
        $(this).remove();
    },

    deleteItem: function () {
        var context = Prompt.getContext($(this));
        var url ='/' + context.type + '/Delete/' + context.id; 
        $.post(url, function (data) {
            if (data.Success) {
                $('tr[data-id=' + context.id + ']').remove();
                $('#deleteModal').modal('hide');
            } else {
                $('#deleteModal .alert').text(data.Message).removeClass('hidden');
            }
        });        
    },

    getContext: function ($a) {
        return {
            name: $a.attr('data-name'),
            id: parseInt($a.attr('data-id')),
            type: $a.attr('data-type')
        }
    },

    getCompiledModal: function (context) {
        // context: { name, id, type }
        var src = $('#delete-template').html();
        var tmpl = Handlebars.compile(src);
        return tmpl(context);
    }
};