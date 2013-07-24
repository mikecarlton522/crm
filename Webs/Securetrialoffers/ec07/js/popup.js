var popup = popup || {};
popup.popupShow = function (el) {
  var options = {
    speed: 400
  };
  options = arguments[1] || options;
  $(popup.getOverlay()).fadeIn(options.speed);
  var marginL = 0 - $(el).width() / 2 + 'px';
  var marginT = 0 - $(el).height() / 2 + 'px';
  $(el).fadeIn(options.speed).css('marginLeft', marginL).css('marginTop', marginT);
  self.popupEl = el;
}
popup.popupHide = function () {
  var options = {
    speed: 400
  };
  $(popup.getOverlay()).fadeOut(options.speed);
  $(self.popupEl).fadeOut(options.speed);
  self.popupEl = null;
}
popup.getOverlay = function () {
  var overlay = $('.popup-overlay')[0] || $('<div class="popup-overlay"></div>').css("filter", "alpha(opacity=80)").click(function () {
    popup.popupHide()
  }).appendTo($('body'))[0];
  return overlay;
}

$(document).ready(function() {
	
	});








/*Exception*/ 