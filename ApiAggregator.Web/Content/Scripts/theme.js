/// <reference path="jquery-1.9.1.intellisense.js" />

$(function () {
    Theme.init();
});

var Theme = {
    init: function () {
        var css = null;
        var nav = null;

        $('a.theme').on('click', this.swapTheme);        
        if ((css = localStorage.getItem('theme')) === null) {
            css = '';
        }
        $('a.theme[data-css="' + css + '"]').click();

        $('label.nav-theme').on('click', this.setNav);
        if ((nav = localStorage.getItem('nav')) === null) {
            nav = 'nav_light';
        }        
        $('#' + nav).parent().click();

        $('.input-validation-error').closest('.form-group, tr').addClass('has-error');
    },

    swapTheme: function () {
        var css = $(this).attr('data-css');
        $('link.theme').attr('href', css);
        localStorage.setItem('theme', css);

        $('a.theme').parent().removeClass('active');
        $(this).parent().addClass('active');
        Theme.setBackground($(this));
    },

    setBackground: function ($a) {
        var img = 'url(/Content/img/squairy_light.png)';

        if ($a.hasClass('theme-dark')) {
            img = 'url(/Content/img/debut_dark.png)';
        }

        $('body').css('background-image', img);
    },

    setNav: function () {
        var nav = $(this).find('input').attr('id');
        localStorage.setItem('nav', nav);
        if ($('nav').hasClass('navbar-inverse') && nav === 'nav_light' ||
           !$('nav').hasClass('navbar-inverse') && nav === 'nav_dark') {
            $('nav').toggleClass('navbar-inverse');
        }
    }
};