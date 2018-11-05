// JavaScript Document
$(document).ready(function() {
    var logoHeight = $("#logo").height();
	$('#menu').css('margin-top',logoHeight-35);
	
	//equal name
	var heighestGoName = 0;
	$(".go-name").each(function() {
       if($(this).height() > heighestGoName){
		   heighestGoName = $(this).height();
	   }
    });
	$(".go-name").height(heighestGoName);
	
	//equal logo school
	var HeighestGoLogo = 0;
	$(".go-logo").each(function() {
       if($(this).height() > HeighestGoLogo){
		   HeighestGoLogo = $(this).height();
	   }
    });
	$(".go-logo").height(HeighestGoLogo);
	
	var hometestHeight = $("#home-test").height();
	var goodoneHeight = $("#good-one").height();
	if(hometestHeight < goodoneHeight){
		$("#home-test").css('position','relative');
		$("#home-test").height(goodoneHeight+11);
		$(".home-test-child").eq(1).css({'position':'absolute', 'bottom':0, 'padding-right':'5px'});
	}
	else{
		$("#good-one").height(hometestHeight-11);
	}
	
	//equal  teacher-info
	var TeacherInfoHeight = 0;
	$(".teacher-info").each(function() {
        if($(this).height() > TeacherInfoHeight){
			TeacherInfoHeight = $(this).height();
		}
    });
	$(".teacher-info").height(TeacherInfoHeight);
	
	var TeacherNameHeight = 0;
	$(".teacher-name").each(function() {
        if($(this).height() > TeacherNameHeight){
			TeacherNameHeight = $(this).height();
		}
    });
	$(".teacher-name").height(TeacherNameHeight);
	
	var RelVidImgWidth = $('#related-video img').width();
	$('#related-video img').height(RelVidImgWidth/1.5);
	
	//equal banner and img
	if($('.item img').height() < $('#home-test').height()){
		$('.item img').height($('#home-test').height()-5)
	}
	
	if($('#main-left').height() < $('#main-right').height()){
		$('#main-left').height($('#main-right').height())
	}
	
	/*if ($('#regis-button').length) {
		var scrollTrigger = $('header').height() + $('#home-test').height()+20,
			ShowIt = function(){
				var CurrentHeight = $(window).scrollTop();
				if(CurrenHeight > scrollTrigger){
					$('#regis-button').addClass('show');
				}else{
					$('#regis-button').removeClass('show');
				}
			};
			ShowIt();
			$(window).on('scroll', function () {
			ShowIt();
			});
	}*/
	
	$(window).scroll(function() {
		if ($(this).scrollTop() > $('header').height() + $('#home-test').height()) {
			$('#regis-button').fadeIn();
		} else {
			$('#regis-button').fadeOut();
		}
	});
	
	$('.tabs-docs .top10-title').click(function() {
        $('.tabs-docs .top10-title').removeClass('active');
		$(this).addClass('active');
    });
	
	popupTop10();
	popupPointResults();
	
	$(".thongtin-khaigiang").slick({
        //dots: true,
        arrows:false,
        infinite: true,
  		focusOnSelect: true,
        slidesToShow: 4,
        slidesToScroll:4,
		autoplay: true,
		autoplaySpeed: 4500,
		responsive: [
			{
			  breakpoint: 1024,
			  settings: {
				slidesToShow: 3,
				slidesToScroll: 3
			  }
			},
			{
			  breakpoint: 600,
			  settings: {
				slidesToShow: 2,
				slidesToScroll: 2
			  }
			},
			{
			  breakpoint: 480,
			  settings: {
				slidesToShow: 1,
				slidesToScroll: 1
			  }
			}
		  ]
    });	
    $(".doi-ngu-giao-vien").slick({
        //dots: true,
        arrows:false,
        infinite: true,
  		focusOnSelect: true,
        slidesToShow: 5,
        slidesToScroll:1,
		autoplay: true,
		autoplaySpeed: 4500,
		responsive: [
			{
			  breakpoint: 1400,
			  settings: {
				slidesToShow: 5,
				slidesToScroll: 1
			  }
			},
			{
			  breakpoint: 1200,
			  settings: {
				slidesToShow: 4,
				slidesToScroll: 1
			  }
			},
			{
			  breakpoint: 950,
			  settings: {
				slidesToShow: 3,
				slidesToScroll: 1
			  }
			},
			{
			  breakpoint: 800,
			  settings: {
				slidesToShow: 2,
				slidesToScroll: 1
			  }
			},
			{
			  breakpoint: 480,
			  settings: {
				slidesToShow: 1,
				slidesToScroll: 1
			  }
			}
		  ]
    });
});

function popupTop10(){
	var moveLeft = -110;
    var moveDown = 30;        
    $('.top10-child li').hover(function(e) {
      $(this).find('.pop-up').show();
      //.css('top', e.pageY + moveDown)
      //.css('left', e.pageX + moveLeft)
      //.appendTo('body');
    }, function() {
      $('div.pop-up').hide();
    });
    
    $('.top10-child li').mousemove(function(e) {
      //$("div.pop-up").css('top', e.pageY + moveDown).css('left', e.pageX + moveLeft);
    }); 
}

function popupPointResults(){
	var moveLeft = -360;
    var moveDown = -120;        
    $('.reportResult .tab-pane li b').hover(function(e) {
		
		$('div#pop-up-point-result').show();
		//.css('top', e.pageY + moveDown)
		//.css('left', e.pageX + moveLeft)
		//.appendTo('body');
    }, function() {
      $('div#pop-up-point-result').hide();
    });
    
    $('.reportResult .tab-pane li b').mousemove(function(e) {
      $("div#pop-up-point-result").css('top', e.pageY + moveDown).css('left', e.pageX + moveLeft);
    }); 
}

$("#homeicon").hover(function(){
    $(this).children('img').attr('src','content/img/home-icon-orange.png');
}, function(){
    $(this).children('img').attr('src','content/img/home-icon-white.png');
});
