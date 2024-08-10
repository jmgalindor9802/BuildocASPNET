$(document).ready(function () {
    console.log('El script de detalles de la inspeccion en la respuesta de la inspeccion está ejecutándose');

    var inspeccionIdElement = $('#InspeccionId');
    console.log('Elemento de InspeccionId:', inspeccionIdElement);

    var inspeccionId = $('#InspeccionId').val(); // Asegúrate de tener el ID de la inspección disponible
    console.log('ID de inspección:', inspeccionId);
    if (inspeccionId) {
        $.ajax({
            url: '/RespuestaInspeccion/InspeccionDetalles',
            data: { id: inspeccionId },
            success: function (response) {
                if (response.success) {
                    console.log('Respuesta completa:', response);
                    // Rellenar los campos en el formulario de RespuestaInspeccion
                    $('#FechaInspeccion').text(response.data.FechaInspeccion);
                    $('#TipoInspeccionNombre').text(response.data.TipoInspeccion.Nombre);
                    $('#ProyectoNombre').text(response.data.Proyecto.Nombre);
                    $('#InspectorNombre').text(response.data.Inspector.Nombre);
                    $('#Descripcion').text(response.data.Descripcion);
                    $('#DuracionHoras').val(response.data.DuracionHoras);
                    $('#EsTodoElDia').prop('checked', response.data.EsTodoElDia);
                    $('#Estado').val(response.data.Estado);
                } else {
                    console.error('Error: ' + response.message);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error al cargar los detalles de la inspección:', textStatus, errorThrown);
            }
        });
    }
});
