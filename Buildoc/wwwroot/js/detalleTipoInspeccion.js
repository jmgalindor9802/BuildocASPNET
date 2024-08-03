$(document).ready(function () {
    // Este c�digo deber�a funcionar cuando la vista se carga inicialmente
});

$(document).on('shown.bs.modal', '#modal-lg', function () {
    // Este c�digo se ejecutar� cuando el modal se muestre
    $('#TipoInspeccionId').change(function () {
        console.log('El evento change se activ�');
        var tipoInspeccionId = $(this).val();
        console.log('ID de Tipo de Inspecci�n:', tipoInspeccionId);
        if (tipoInspeccionId) {
            $.ajax({
                url: '/TipoInspecciones/GetTipoInspeccionDetails',
                data: { id: tipoInspeccionId },
                success: function (data) {
                    console.log('Datos recibidos:', data);
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
        } else {
            $('#TipoInspeccionNombre').text('');
            $('#TipoInspeccionCategoria').text('');
            $('#TipoInspeccionDescripcion').text('');
        }
    });
});
