$(function() {

	/* Initializing the scrollbar */
	if($('.scrollbar-inner').size()>0){
		$('.scrollbar-inner').scrollbar({
			"disableBodyScroll":true
		});
	}

	if($(window).width() < 1199 ){
		if($('.scrollbar-variant').size()>0){
			$('.scrollbar-variant').scrollbar();
		}
	}

	if($('form.fn_fast_button').size()>0){
		$(document).on('change', '.fn_action_block:not(.fn_fast_action_block) select', function(e, trigger) {
			if (!trigger) {
				var name = $(this).attr('name'),
				selected = $(this).children(':selected').val();
				$('.fn_fast_save select[name="' + name + '"]').val(selected).trigger('change', {trigger: true});
			}
		});
		
		$(document).on('change', '.fn_fast_save select', function(e, trigger) {
			if (!trigger) {
				var name = $(this).attr('name'),
				selected = $(this).children(':selected').val();
				$('form.fn_fast_button select[name="' + name + '"]').val(selected).trigger('change', {trigger: true});
			}
		});
		
		
		if ($('.fn_action_block').size()>0) {
			var action_block = $('.okay_list_option').clone(true);
			$('.fn_fast_action_block .action').html(action_block);
			if ($('.fn_additional_params').size()) {
				var additional_params = $('.fn_additional_params').clone(true);
				$('.fn_fast_action_block .additional_params').html(additional_params);
			}
		}
		
		$('input,textarea,select, .dropdown-toggle, .fn_sort_item, .fn_category_item').bind('keyup change dragover click',function(){
			$('.fn_fast_save').show();
		});
		$('.fn_fast_save .fast_save_button').on('click', function () {
			$('body').find("form.fn_fast_button").trigger('submit');
		});
	}

	/* Check */
	if($('.fn_check_all').size() > 0) {
		$(document).on('change', '.fn_check_all', function() {
			if($(this).is(":checked")) {
				console.log($(this).closest("form").find('.hidden_check'))
				$(this).closest("form").find('.hidden_check').each(function() {
					if(!$(this).is(":checked")){
						$(this).trigger("click");
					}
				});
			} else {
				$(this).closest("form").find('.hidden_check').each(function() {
					if($(this).is(":checked")) {
						$(this).trigger("click");
					}
				})
			}
		});
	}
	/* Check end*/

	// Small script for tooltip
	$(function(){
		$(".fn_tooltips").tooltip();
	});

	/* Catalog items toggle */
	if($('.fn_item_switch').size()>0){
		$('.fn_item_switch').on('click',function(e){
			var parent = $(this).closest("ul"),
			li = $(this).closest(".fn_item_sub_switch"),
			sub = li.find(".fn_submenu_toggle");

			if(li.hasClass("open active")){

				sub.slideUp(200, function () {
					li.removeClass("open active")
				})

			} else {
				parent.find("li.open").children(".fn_submenu_toggle").slideUp(200),
				parent.find("li.open").removeClass("open active"),
				li.children(".arrow").addClass("open active"),

				sub.slideDown(200, function () {
					li.addClass("open active")
				})
			}
		});
	}

	/* Left menu toggle */
	if($('.fn_switch_menu').size()>0){
		$(document).on("click", ".fn_switch_menu", function () {
			$("body").toggleClass("menu-pin");
		});
		$(document).on("click", ".fn_mobile_menu", function () {
			$("body").toggleClass("menu-pin");
			$(".fn_quickview").removeClass("open");
		});
	}

	/* Right menu toggle */
	if($('.fn_switch_quickview').size()>0){
		$(document).on("click", ".fn_mobile_menu_right", function () {
			$(this).next().toggleClass("open");
			$("body").removeClass("menu-pin");
		});
		$(document).on("click", ".fn_switch_quickview", function () {
			$(this).closest(".fn_quickview").toggleClass("open");
		});
	}

	/* Script for products */
	$(document).on('click', '.fn_variants_toggle', function() {
		$(this).find('.fn_icon_arrow').toggleClass('rotate_180');
		$(this).parent('.fn_row').find('.products_variants_block').slideToggle();
	});

	$(document).on('change', '.fn_action_block select.products_action', function() {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if($('.fn_' + elem).size() > 0){
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	$(document).on('click', '.fn_show_icon_menu', function() {
		$(this).toggleClass('show');
	});

	/* Script for fn_action_block button */
	$(document).on('change', '.fn_action_block select.brands_action', function() {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	$(document).on('change', '.fn_action_block select.categories_action', function() {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	/* Script for brands */
	$(document).on('change', '.fn_action_block select.brands_action', function () {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if ($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	/* Script for categories */
	$(document).on('change', '.fn_action_block select.categories_action', function () {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if ($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});

	/* Infinity in the warehouse(склад) */
	$('input[name*="stock"]').focus(function() {
		if($(this).val() == '∞')
			$(this).val('');
		return false;
	});
	$('input[name*="stock"]').blur(function() {
		if($(this).val() == '∞')
			$(this).val('∞');
	});	

	/* Initializing sorting */
	if($("sortable").size() > 0){
		$(".sortable").each(function() {
			Sortable.create(this, {
				handle: ".move_zone",  // Drag handle selector within list items
                    sort: true,  // sorting inside list
                    animation: 150,  // ms, animation speed moving items when sorting, `0` — without animation
                    ghostClass: "sortable-ghost",  // Class name for the drop placeholder
                    chosenClass: "sortable-chosen",  // Class name for the chosen item
                    dragClass: "sortable-drag",  // Class name for the dragging item
                    scrollSensitivity: 100, // px, how near the mouse must be to an edge to start scrolling.
                    scrollSpeed: 10, // px
                    
                    // Changed sorting within list
                    onUpdate: function (evt) {
                    	if ($(".product_images_list").size() > 0) {
                            var itemEl = evt.item;  // dragged HTMLElement
                            if ($(itemEl).closest(".fn_droplist_wrap").data("image") == "product") {
                            	$(".product_images_list").find("li.first_image").removeClass("first_image");
                            	$(".product_images_list").find("li:nth-child(2)").addClass("first_image");
                            }
                         }
                      }
                   });
		});
	}

	if($(".sort_extended").size() > 0){

		// Explicitly indicate the height of the list, otherwise when the script deletes the element and puts a stub in its place, the page jumps
		$(".fn_sort_list").css('min-height', $(".fn_sort_list").outerHeight());

		$(".sort_extended").sortable({
			items: ".fn_sort_item",
			tolerance: "pointer",
			handle: ".move_zone",
			scrollSensitivity: 50,
			scrollSpeed: 100,
			scroll: true,
			opacity: 0.5,
			containment: "document",
			helper: function(event, ui) {
				if($('input[type="checkbox"][name*="check"]:checked').size() < 1) return ui;
				var helper = $('<div/>');
				$('input[type="checkbox"][name*="check"]:checked').each(function() {
					var item = $(this).closest(".fn_row");
					helper.height(helper.height() + item.innerHeight());
					if (item[0] != ui[0]){
						helper.append(item.clone());
						$(this).closest('.fn_row').remove();
					} else{
						helper.append(ui.clone());
						item.find('input[type="checkbox"][name*="check"]').attr('checked', false);
					}
				});
				return helper;
			},
			start: function(event, ui) {
				if (ui.helper.children('.fn_row').size() > 0)
					$('.ui-sortable-placeholder').height(ui.helper.height());
			},
			beforeStop: function(event, ui) {
				if(ui.helper.children('.fn_row').size() > 0){
					ui.helper.children('.fn_row').each(function() {
						$(this).insertBefore(ui.item);
					});
					ui.item.remove();
				}
			},
			update: function(event, ui) {
				$("#list_form input[name*='check']").attr('checked', false);
			}
		});
	}

	$(".fn_pagination a.droppable").droppable({
		activeClass: "drop_active",
		hoverClass: "drop_hover",
		tolerance: "pointer",
		drop: function(event, ui) {
			$(ui.helper).find('input[type="checkbox"][name*="check"]'.attr('checked', true));
			$(ui.draggable).closest("form").find('select[name="action"] option[value=move_to_page]').attr('selected', 'selected');
			$(ui.draggable).closest("form").find('select[name=target_page] option[value'+$(this).html()+']').attr('selected', 'selected');
			$(ui.draggable).closest("form").submit();
			return false;
		}
	});

	$(document).on('change', '.fn_action_block select.brands_action', function () {
		var elem = $(this).find('option:selected').val();
		$('.fn_hide_block').addClass('hidden');
		if ($('.fn_' + elem).size() > 0) {
			$('.fn_' + elem).removeClass('hidden');
		}
	});
});

$(document).on('click', '.fn_light_remove', function() {
	$(this).closest('.fn_row').remove();
});

if($('.fn_remove').size() > 0) {
	function success_action ($this){
		$(document).on('click','.fn_submit_delete',function(){
			$('.fn_form_list input[type="checkbox"][name*="check"]').attr('checked', false);
			$this.closest(".fn_row").find('input[type="checkbox"][name*="check"]').prop('checked', true);
			$this.closest(".fn_form_list").find('select[name="action"] option[value=delete]').prop('selected', true);
			$this.closest(".fn_form_list").submit();
		});
		$(document).on('click','.fn_dismiss_delete',function(){
			$('.fn_form_list input[type="checkbox"][name*="check"]').prop('checked', false);
			$this.closest(".fn_form_list").find('select[name="action"] option[value=delete]').removeAttr('selected');
			return false;
		});
	}
}

$(window).on('load', function() {
	/* Script to collapse information blocks */
	$(document).on('click', ".fn_toggle_card", function() {
		$(this).closest(".fn_toggle_wrap").find('.fn_icon_arrow').toggleClass('rotate_180');
		$(this).closest(".fn_toggle_wrap").find('.fn_card').slideToggle(500);
	});
});