$(document).ready(function () {
    console.log('Script JavaScript detalles cargado correctamente');

    function cargarCategorias() {
        $.ajax({
            url: '/Inspecciones/GetCategoriasConTipoInspeccion',
            success: function (data) {
                console.log('Categor�as recibidas:', data); // Depuraci�n
                var categoriaSelect = $('#CategoriaInspeccion');
                categoriaSelect.empty();
                categoriaSelect.append('<option value="" disabled selected>Seleccionar categoria</option>');

                $.each(data, function (i, categoria) {
                    console.log('A�adiendo categor�a:', categoria); // Depuraci�n
                    categoriaSelect.append(
                        $('<option></option>')
                            .val(categoria.value)
                            .text(categoria.text)
                    );
                });
                console.log('Categor�as a�adidas al select'); // Depuraci�n
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error al cargar las categor�as:', textStatus, errorThrown);
            }
        });
    }

    function cargarTiposInspeccionPorCategoria(categoria) {
        console.log('Cargando tipos de inspecci�n para la categor�a:', categoria);
        $.ajax({
            url: '/Inspecciones/GetTipoInspeccionesPorCategoria',
            data: { categoria: categoria },
            success: function (data) {
                var tipoInspeccionSelect = $('#TipoInspeccionId');
                tipoInspeccionSelect.empty();
                tipoInspeccionSelect.append('<option value="" disabled selected>Seleccionar</option>');

                $.each(data, function (i, tipoInspeccion) {
                    tipoInspeccionSelect.append('<option value="' + tipoInspeccion.id + '">' + tipoInspeccion.nombre + '</option>');
                });

                // Limpiar los detalles del tipo de inspecci�n al cambiar de categor�a
                $('#TipoInspeccionNombre').text('');
                $('#TipoInspeccionCategoria').text('');
                $('#TipoInspeccionDescripcion').text('');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error en la solicitud AJAX:', textStatus, errorThrown);
            }
        });
    }

    // Cargar detalles del tipo de inspecci�n cuando la p�gina se carga inicialmente
    var tipoInspeccionId = $('#TipoInspeccionId').val();
    if (tipoInspeccionId) {
        cargarDetallesTipoInspeccion(tipoInspeccionId);
    }

    // Cargar categor�as cuando el modal se muestra
    $(document).on('shown.bs.modal', '#modal-lg', function () {
        cargarCategorias(); // Carga las categor�as al abrir el modal

        var tipoInspeccionId = $('#TipoInspeccionId').val();
        if (tipoInspeccionId) {
            cargarDetallesTipoInspeccion(tipoInspeccionId);
        }

        // Delegado de eventos para el dropdown de categor�as
        $(document).on('change', '#CategoriaInspeccion', function () {
            var categoria = $(this).val();
            console.log('Categor�a seleccionada:', categoria); // Verifica el valor
            if (categoria) {
                cargarTiposInspeccionPorCategoria(categoria);
            } else {
                $('#TipoInspeccionId').empty().append('<option value="" disabled selected>Seleccionar</option>');

                // Limpiar los detalles del tipo de inspecci�n si no se selecciona una categor�a
                $('#TipoInspeccionNombre').text('');
                $('#TipoInspeccionCategoria').text('');
                $('#TipoInspeccionDescripcion').text('');
            }
        });

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
});
