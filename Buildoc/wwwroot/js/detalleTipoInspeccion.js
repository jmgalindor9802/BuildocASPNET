$(document).ready(function () {
    // Cargar detalles del tipo de inspección cuando la página se carga inicialmente
    // Si el valor está preseleccionado, cargar los detalles.
    var tipoInspeccionId = $('#TipoInspeccionId').val();
    if (tipoInspeccionId) {
        cargarDetallesTipoInspeccion(tipoInspeccionId);
    }
});

$(document).on('shown.bs.modal', '#modal-lg', function () {
    // Código para ejecutar cuando el modal se muestra
    var tipoInspeccionId = $('#TipoInspeccionId').val();
    if (tipoInspeccionId) {
        cargarDetallesTipoInspeccion(tipoInspeccionId);
    }

    $('#TipoInspeccionId').change(function () {
        var tipoInspeccionId = $(this).val();
        if (tipoInspeccionId) {
            cargarDetallesTipoInspeccion(tipoInspeccionId);
        } else {
            $('#TipoInspeccionNombre').text('');
            $('#TipoInspeccionCategoria').text('');
            $('#TipoInspeccionDescripcion').text('');
        }
    });
});

function cargarDetallesTipoInspeccion(tipoInspeccionId) {
    $.ajax({
        url: '/TipoInspecciones/GetTipoInspeccionDetails',
        data: { id: tipoInspeccionId },
        success: function (data) {
            $('#TipoInspeccionNombre').text(data.nombre);
            $('#TipoInspeccionCategoria').text(data.categoria);
            $('#TipoInspeccionDescripcion').text(data.descripcion);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('Error en la solicitud AJAX:', textStatus, errorThrown);
            $('#TipoInspeccionNombre').text('Error al obtener los detalles');
            $('#TipoInspeccionCategoria').text('');
            $('#TipoInspeccionDescripcion').text('');
        }
    });
}
