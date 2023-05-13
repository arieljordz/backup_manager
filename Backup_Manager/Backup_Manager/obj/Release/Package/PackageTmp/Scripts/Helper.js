function ProgressBarModal(showHide, successMessage) {
    $("#mod-preparing").modal("hide");
    if (showHide === 'show') {
        $('#mod-progress').modal('show');
        if (arguments.length >= 2) {
            $('#progressBarParagraph').text(arguments[1]);
        } else {
            $('#progressBarParagraph').text('U tijeku...');
        }
        window.progressBarActive = true;
    } else {
        $('#mod-progress').modal('hide');
        window.progressBarActive = false;
        Swal.fire('Success!', successMessage, 'success');
    }
}