$(document).on('ready', function () {

    $().UItoTop({ easingType: 'easeOutQuart' });

    $(".footer-ad").mustang({
        item: '.banner',
        time: 5000,
        //buttonActive: true,
        next: "#next",
        //prev: "#prev"
    });

    $('#banner-sliders').skdslider({
        'delay': 5000, 
        'animationSpeed': 1000,
        'showNextPrev': false, 
        'showPlayButton': false,
        'autoSlide': true, 
        'animationType': 'sliding'
    });

    $('#responsive').change(function () {
        $('#responsive_wrapper').width($(this).val());
    });
    
    $(".university-regular").slick(getSliderSettings());
    $('.student-regular').slick(getSliderSettings());

    $("#student-highline").on("click", function () {
        $.ajax({
            url: '/cmsbase/student',
            type: 'GET',
            dataType: 'html',
            success: function (result) {
                $(".tab-pane").empty();
                $("#tab-1").html(result);
                $('.student-regular').slick(getSliderSettings());
                $(".slick-next").trigger("click");
            },
            error: function () { alert('Error!'); }

        });
    });

    //get teacher highlight
    $("#teacher-highline").on("click", function () {
        $.ajax({
            url: '/cmsbase/teacher',
            type: 'GET',
            dataType: 'html',
            success: function (result) {
                $(".tab-pane").empty();
                $("#tab-2").html(result);
                $('.teacher-regular').slick(getSliderSettings());
               // $(".slick-next").trigger("click");
            },
            error: function () { alert('Error!'); }

        });
    });

   

    //ModalPopup();
});

function ModalPopup() {
    $.ajaxSetup({ cache: false });
    $("a[data-modal]").on("click", function (e) {
        $('#myModalContent').empty();
        $('#myModalContent').load(this.href, function () {
            $('#myModal').modal({
                keyboard: true
            }, 'show');
            
        });
        return false;
    });
}

function getSliderSettings() {
    return {
        autoplay: true,
        autoplaySpeed: 5000,
        centerMode: false,
        centerPadding: '0px',
        slidesToShow: 4,
        responsive: [
          {
              breakpoint: 768,
              settings: {
                  arrows: false,
                  centerMode: true,
                  centerPadding: '0px',
                  slidesToShow: 1
              }
          },
          {
              breakpoint: 480,
              settings: {
                  arrows: false,
                  centerMode: true,
                  centerPadding: '0px',
                  slidesToShow: 1
              }
          }
        ]
    }
}
function doLayout() {
    var height = $(window).height();
    $("#consultant").height(height);
    $(window).resize(function () {
        height = $(window).height();
        $("#consultant").height(height);
    });
}
