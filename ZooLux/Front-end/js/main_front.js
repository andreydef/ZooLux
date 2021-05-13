$(window).load(function() {
	$('#slider').nivoSlider({
		effect: 'random', 
		autoplay: true,              
		slices: 15,                     
		boxCols: 8,                     
		boxRows: 4,                     
		animSpeed: 500,                
		pauseTime: 5000,            
		startSlide: 0,               
		directionNav: true,             
		controlNav: false,               
		controlNavThumbs: false,        
		pauseOnHover: false,            
		manualAdvance: false,           
		prevText: '<',          
		nextText: '>',              
		randomStart: false,             
		beforeChange: function(){},    
		afterChange: function(){},      
		slideshowEnd: function(){},    
		lastSlide: function(){},        
		afterLoad: function(){}         
	});

	$(".active-hot-deals").owlCarousel({
		autoPlay: false, 
		slideSpeed:2000,
		pagination:true,
		navigation:false,	  
		items : 1,
		navigationText:["<i class='fa fa-angle-left'></i>","<i class='fa fa-angle-right'></i>"],
		itemsDesktop : [1169,1],
		itemsTablet: [991,1],
		itemsTabletSmall: [767,2],
		itemsMobile : [479,1],
	});

	$(".active-bestseller").owlCarousel({
		autoPlay: false, 
		slideSpeed:2000,
		pagination:false,
		navigation:true,	  
		items : 1,
		navigationText:["<i class='fa fa-angle-left'></i>","<i class='fa fa-angle-right'></i>"],
		itemsDesktop : [1169,1],
		itemsTablet: [991,1],
		itemsTabletSmall: [767,1],
		itemsMobile : [479,1],
	});

	$(".active-recent-posts").owlCarousel({
		autoPlay: false, 
		slideSpeed:2000,
		pagination:true,
		navigation:false,	  
		items : 1,
		itemsDesktop : [1169,1],
		itemsTablet: [991,1],
		itemsTabletSmall: [767,1],
		itemsMobile : [479,1],
	});

	$('.category-heading').on('click', function(){
	 $('.category-menu-list').slideToggle(300);
	});

	$.scrollUp({
        easingType: 'linear',
        scrollSpeed: 900,
        animation: 'fade'
    }); 	
});

$(document).ready(function() {

	$('[data-toggle="tooltip"]').tooltip(); // Data-Toggle Tooltip

	new WOW().init(); // WOW JS Activate

	jQuery('nav#mobile-menu').meanmenu(); // jQuery MeanMenu 

	$('.dropdown-toggle').dropdown(); // dropdown toggle
});