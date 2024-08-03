// Función para añadir una alerta si no existe una con el mismo mensaje, o actualizarla si ya existe
function addAlert(message, type) {
    $('#modal-lg .modal-body .alert').remove();
    $('#modal-lg .modal-body').prepend('<div class="alert alert-' + type + '">' + message + '</div>');
}

$(document).ready(function () {
    // Manejar el clic en los enlaces de operación (crear, editar, detalles, eliminar, restaurar)
    $(document).on('click', '.create-new, .edit-item, .details-view, .delete-item, .restore-item', function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        var title = $(this).data('title');
        var action = $(this).data('action'); // 'create', 'edit', 'details', 'delete', 'restore'

        $('#modal-lg .modal-title').text(title);
        $.get(url).done(function (data) {
            $('#modal-lg .modal-body').html(data);
            $('#modal-lg').modal('show');
            // Cargar el script después de que el contenido del modal se haya cargado
            $.getScript('/js/detalleTipoInspeccion.js')
                .done(function () {
                    console.log('Script detalleTipoInspeccion.js cargado correctamente.');
                })
                .fail(function (jqxhr, settings, exception) {
                    console.error('Error al cargar el script detalleTipoInspeccion.js:', exception);
                }); 
            // Configurar los botones del modal según la acción
            if (action === 'create' || action === 'edit') {
                $('.btn-save').show();
                $('.btn-delete').hide();
                $('.btn-edit').hide();
                $('.btn-restore').hide();
            } else if (action === 'delete') {
                $('.btn-save').hide();
                $('.btn-delete').show();
                $('.btn-edit').hide();
                $('.btn-restore').hide();
            } else if (action === 'details') {
                $('.btn-save').hide();
                $('.btn-delete').hide();
                $('.btn-edit').hide();
                $('.btn-restore').hide();
            } else if (action === 'restore') {
                $('.btn-save').hide();
                $('.btn-delete').hide();
                $('.btn-edit').hide();
                $('.btn-restore').show();
            }
        }).fail(function () {
            console.log('Error al cargar el contenido del modal.');
        });
    });

    // Manejar la acción de guardar
    $('#modal-lg').on('click', '.btn-save', function (e) {
        e.preventDefault();

        var form = $('#modal-lg').find('form');
        if (form.length === 0) {
            console.log('No se encontró el formulario dentro del modal.');
            addAlert('No se encontró el formulario dentro del modal.', 'danger');
            return;
        }

        var formData = form.serialize();

        // Mostrar el spinner
        $("#spinner").show();

        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: formData,
            success: function (response) {
                // Ocultar el spinner
                $("#spinner").hide();

                if (response.success) {
                    $('#modal-lg').modal('hide');
                    location.reload();
                } else {
                    addAlert(response.message || 'Error no especificado.', 'danger');
                }
            },
            error: function () {
                // Ocultar el spinner
                $("#spinner").hide();

                addAlert('Se produjo un error al procesar la solicitud.', 'danger');
            }
        });
    });

    // Manejar la acción de eliminar
    $('#modal-lg').on('click', '.btn-delete', function (e) {
        e.preventDefault();

        var form = $('#modal-lg').find('form');
        if (form.length === 0) {
            console.log('No se encontró el formulario dentro del modal.');
            addAlert('No se encontró el formulario dentro del modal.', 'danger');
            return;
        }

        var formData = form.serialize();

        // Mostrar el spinner
        $("#spinner").show();

        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: formData,
            success: function (response) {
                // Ocultar el spinner
                $("#spinner").hide();

                if (response.success) {
                    $('#modal-lg').modal('hide');
                    location.reload();
                } else {
                    addAlert(response.message || 'Error no especificado.', 'danger');
                }
            },
            error: function () {
                // Ocultar el spinner
                $("#spinner").hide();

                addAlert('Se produjo un error al procesar la solicitud.', 'danger');
            }
        });
    });

    // Manejar la acción de editar desde los detalles
    $('#modal-lg').on('click', '.btn-edit', function (e) {
        e.preventDefault();
        var editUrl = $(this).data('url');

        $.get(editUrl).done(function (data) {
            $('#modal-lg .modal-body').html(data);
            $('#modal-lg').modal('show');
            $('#modal-lg .btn-save').show();
            $('#modal-lg .btn-edit').hide();
        }).fail(function () {
            console.log('Error al cargar el contenido del modal de edición.');
        });
    });

    // Manejar la acción de restaurar
    $('#modal-lg').on('click', '.btn-restore', function (e) {
        e.preventDefault();

        var form = $('#modal-lg').find('form');
        if (form.length === 0) {
            console.log('No se encontró el formulario dentro del modal.');
            $('#modal-lg .modal-body').prepend('<div class="alert alert-danger">No se encontró el formulario dentro del modal.</div>');
            return;
        }

        var formData = form.serialize();
        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: formData,
            success: function (response) {
                if (response.success) {
                    $('#modal-lg').modal('hide');
                    location.reload();
                } else {
                    if ($('#modal-lg .alert.alert-danger').length === 0 ||
                        $('#modal-lg .alert.alert-danger').text().indexOf(response.message) === -1) {
                        $('#modal-lg .modal-body').prepend('<div class="alert alert-danger">' + (response.message || 'Error no especificado.') + '</div>');
                    }
                }
            },
            error: function () {
                if ($('#modal-lg .alert.alert-danger').length === 0 ||
                    $('#modal-lg .alert.alert-danger').text().indexOf('Se produjo un error al procesar la solicitud.') === -1) {
                    $('#modal-lg .modal-body').prepend('<div class="alert alert-danger">Se produjo un error al procesar la solicitud.</div>');
                }
            }
        });
    });

    // Función para manejar la visibilidad del campo de nueva categoría
    function handleCategoriaChange() {
        const categoriaSelect = $('#categoriaSelect');
        const nuevaCategoriaInput = $('#nuevaCategoria');

        function updateCategoryVisibility() {
            if (categoriaSelect.val() === 'Otra') {
                nuevaCategoriaInput.show();
            } else {
                nuevaCategoriaInput.hide();
            }
        }

        // Inicializar la visibilidad en función del valor actual
        updateCategoryVisibility();

        // Agregar evento para manejar cambios en el select
        categoriaSelect.on('change', updateCategoryVisibility);
    }

    handleCategoriaChange();
});
