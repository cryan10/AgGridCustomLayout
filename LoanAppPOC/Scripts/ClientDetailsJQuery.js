$(document).ready(function () {
    $('#gridSavedConfirmation').hide();

    $('#saveGridLayout').on('click', function () {
        //Saved! message appears
        $('#gridSavedConfirmation').fadeIn(800);
        $('#gridSavedConfirmation').delay(1500);
        $('#gridSavedConfirmation').fadeOut(1000);
    });

});