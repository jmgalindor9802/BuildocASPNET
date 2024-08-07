$(document).ready(function () {
    console.log('Script JavaScript detalles cargado correctamente');

    function cargarCategorias() {
        $.ajax({
            url: '/Inspecciones/GetCategoriasConTipoInspeccion',
            success: function (data) {
                console.log('Categorías recibidas:', data); // Depuración
                var categoriaSelect = $('#CategoriaInspeccion');
                categoriaSelect.empty();
                categoriaSelect.append('<option value="" disabled selected>Seleccionar categoria</option>');

                $.each(data, function (i, categoria) {
                    console.log('Añadiendo categoría:', categoria); // Depuración
                    categoriaSelect.append(
                        $('<option></option>')
                            .val(categoria.value)
                            .text(categoria.text)
                    );
                });
                console.log('Categorías añadidas al select'); // Depuración
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error al cargar las categorías:', textStatus, errorThrown);
            }
        });
    }

    function cargarTiposInspeccionPorCategoria(categoria) {
        console.log('Cargando tipos de inspección para la categoría:', categoria);
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

                // Limpiar los detalles del tipo de inspección al cambiar de categoría
                $('#TipoInspeccionNombre').text('');
                $('#TipoInspeccionCategoria').text('');
                $('#TipoInspeccionDescripcion').text('');
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.error('Error en la solicitud AJAX:', textStatus, errorThrown);
            }
        });
    }

    // Cargar detalles del tipo de inspección cuando la página se carga inicialmente
    var tipoInspeccionId = $('#TipoInspeccionId').val();
    if (tipoInspeccionId) {
        cargarDetallesTipoInspeccion(tipoInspeccionId);
    }

    // Cargar categorías cuando el modal se muestra
    $(document).on('shown.bs.modal', '#modal-lg', function () {
        cargarCategorias(); // Carga las categorías al abrir el modal

        var tipoInspeccionId = $('#TipoInspeccionId').val();
        if (tipoInspeccionId) {
            cargarDetallesTipoInspeccion(tipoInspeccionId);
        }

        // Delegado de eventos para el dropdown de categorías
        $(document).on('change', '#CategoriaInspeccion', function () {
            var categoria = $(this).val();
            console.log('Categoría seleccionada:', categoria); // Verifica el valor
            if (categoria) {
                cargarTiposInspeccionPorCategoria(categoria);
            } else {
                $('#TipoInspeccionId').empty().append('<option value="" disabled selected>Seleccionar</option>');

                // Limpiar los detalles del tipo de inspección si no se selecciona una categoría
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
