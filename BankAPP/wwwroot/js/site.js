$(document).ready(function () {
    $(".owl-carousel").owlCarousel({
        margin: 10,
        nav: true,
        autoplayHoverPause: true,
        center: true,
        navText: [
            "<i class='fa fa-angle-left'></i>",
            "<i class='fa fa-angle-right'></i>"
        ],
        responsive: {
            0: {
                items: 1,
            },
            600: {
                items: 1
            },
            1000: {
                items: 3
            }
        }
    });
});