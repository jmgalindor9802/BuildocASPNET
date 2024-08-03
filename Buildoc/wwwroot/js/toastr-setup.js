console.log('toastr-setup.js cargado');

$(document).ready(function () {
    // Configuración global de Toastr
    toastr.options = {
        closeButton: true,
        debug: false,
        newestOnTop: false,
        progressBar: true,
        positionClass: 'toast-top-right',
        preventDuplicates: false,
        onclick: null,
        showDuration: "300",
        hideDuration: "1000",
        timeOut: "5000",
        extendedTimeOut: "1000",
        showEasing: "swing",
        hideEasing: "linear",
        showMethod: "fadeIn",
        hideMethod: "fadeOut"
    };

   

    // Mostrar Toastr si las variables tienen mensajes
    if (successMessage) {
        toastr.success(successMessage);
    }
    if (errorMessage) {
        toastr.error(errorMessage);
    }
    if (warningMessage) {
        toastr.warning(warningMessage);
    }
    if (infoMessage) {
        toastr.info(infoMessage);
    }
});
