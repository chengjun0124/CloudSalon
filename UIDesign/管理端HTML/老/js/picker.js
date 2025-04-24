$(function(){
	$( document ).on ( "click" ,"#rest_time",function () {
		$( ".fileMask" ).css ( "display" , "block" );
	} );
	$("#rest_time").click ( function () {
		$(".picker-modal" ).css( "display" , "block" );
		$(".picker-modal > div" ).click( function () {
			$ ( ".picker-modal > div" ).removeClass ( "picker-selected" );
			$ ( this ).addClass ( "picker-selected" );
			$ ( "#rest_time").val ( $ ( this ).attr ( "picker-value" ) );
			$ ( "#rest_time").attr ( "value" , $ ( this ).attr ( "picker-value" ) );
			$ ( ".picker-modal" ).css ( "display" , "none" );
			$ ( ".fileMask" ).css ( "display" , "none" );
		} );
	} );
} );
