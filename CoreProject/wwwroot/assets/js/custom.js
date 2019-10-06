  /** Template Name: OsteriaX
  * Version: 1 
  * Template Scripts
  * Author: MarkUps
  * Author URI: http://www.markups.io/

  Custom JS

  
  1. SCROLL TOP BUTTON
  

  
**/

jQuery(function($){
  
  /* ----------------------------------------------------------- */
  /*  1. SCROLL TOP BUTTON
  /* ----------------------------------------------------------- */

  //Check to see if the window is top if not then display button

    jQuery(window).scroll(function(){
      if (jQuery(this).scrollTop() > 300) {
        jQuery('.scrollToTop').fadeIn();
      } else {
        jQuery('.scrollToTop').fadeOut();
      }
    });
     
    //Click event to scroll to top

    jQuery('.scrollToTop').click(function(){
      jQuery('html, body').animate({scrollTop : 0},800);
      return false;
    });
	
	/* ----------------------------------------------------------- */
  /* THEME STYLE SWITCHER 
  /* ----------------------------------------------------------- */ 

    // style switcher area hide & active

    jQuery('.style-switch-button').click(function(){
      jQuery('.theme_style_switcher').toggleClass("style-switch-active");
    })

    // theme style css changed   

    $('#blue').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/blue-theme.css?v=1.1');
      e.preventDefault();
    });

    $('#default').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/default-theme.css?v=1.1');
      e.preventDefault();
    });

    $('#darkRed').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/dark-red-theme.css?v=1.1');
      e.preventDefault();
    });

    $('#green').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/green-theme.css?v=1.1');
      e.preventDefault();
    });

    $('#violet').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/violet-theme.css?v=1.1');
      e.preventDefault();
    });

    $('#orange').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/orange-theme.css?v=1.1');
      e.preventDefault();
    });

    $('#pink').click(function (e){      
      $('#switcher').attr('href','assets/css/theme-color/pink-theme.css?v=1.1');
      e.preventDefault();
    });

    $('#purple').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/purple-theme.css?v=1.1');
      e.preventDefault();
    });

    $('#red').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/red-theme.css?v=1.1');
      e.preventDefault();
    });  

    $('#tan').click(function (e){
      $('#switcher').attr('href','assets/css/theme-color/tan-theme.css?v=1.1');
      e.preventDefault();
    });
  
});