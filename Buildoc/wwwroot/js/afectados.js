$(document).on('shown.bs.modal', '#modal-lg', function () {
    console.log("Modal abierto");
    // Ocultar el contenedor de lesiones al cargar la página
    $(document).ready(function () {
        $('#contenedorLesionesAfectado').hide();

        // Mostrar u ocultar el contenedor de lesiones basado en el estado del checkbox
        $('#lesionAfectado').change(function () {
            if ($(this).is(':checked')) {
                $('#contenedorLesionesAfectado').slideDown(); // Mostrar con efecto deslizante
            } else {
                $('#contenedorLesionesAfectado').slideUp(); // Ocultar con efecto deslizante
            }
        });
    });

    // Mantener el estado del contenedor al abrir el modal
    $(document).on('shown.bs.modal', '#modal-lg', function () {
        if ($('#lesionAfectado').is(':checked')) {
            $('#contenedorLesionesAfectado').show(); // Mostrar si el checkbox está marcado
        } else {
            $('#contenedorLesionesAfectado').hide(); // Ocultar si el checkbox no está marcado
        }
    });
}