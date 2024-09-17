$(document).ready(function () {
    console.log('jQuery está listo');

    // Manejar clics en los elementos que abren el modal
    $(document).on('click', '.create-new, .edit-item, .details-view, .delete-item, .restore-item,.respond-item, .deactivate-item', function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        var title = $(this).data('title');
        var action = $(this).data('action'); // 'create', 'edit', 'details', 'delete', 'restore', respond, deactivate

        console.log('Abrir modal con acción:', action);
        $('#modal-lg .modal-title').text(title);

        // Cargar contenido del modal
        $.get(url).done(function (data) {
            $('#modal-lg .modal-body').html(data);
            $('#modal-lg').modal('show');
            console.log('Contenido cargado en el modal.');

            // Iniciar las validaciones unobtrusive
            $.validator.unobtrusive.parse('#modal-lg form');
            console.log('Validaciones unobtrusive inicializadas.');

            // Configurar los botones del modal según la acción
            configureModalButtons(action);

        }).fail(function () {
            console.log('Error al cargar el contenido del modal.');
        });
    });

    // Función para configurar los botones del modal
    function configureModalButtons(action) {
        // Ocultar todos los botones por defecto
        console.log('Configurando botones para la acción:', action);
        $('.btn-save, .btn-delete, .btn-edit, .btn-restore, btn-deactivate').hide();

        if (action === 'create' || action === 'edit' || action === 'respond') {
            console.log('Mostrando botón de guardar');
            $('.btn-save').show(); // Mostrar el botón de guardar
        } else if (action === 'delete') {
            console.log('Mostrando botón de eliminar');
            $('.btn-delete').show(); // Mostrar el botón de eliminar
            $('.btn-deactivate').hide();
        } else if (action === 'restore') {
            console.log('Mostrando botón de restaurar');
            $('.btn-restore').show(); // Mostrar el botón de restaurar
        } else if (action === 'deactivate') {
            console.log('Mostrando botón de desactivar');
            $('.btn-deactivate').show(); // Mostrar el boton de desactivar
        } else {
            console.log('No se necesita mostrar ningún botón.');
        }
    }

    // Acción para guardar el formulario
    $('#modal-lg').on('click', '.btn-save', function (e) {
        e.preventDefault();
        console.log('Botón de guardar clicado');

        var form = $('#modal-lg').find('form');
        if (form.length === 0) {
            console.log('No se encontró el formulario dentro del modal.');
            addAlert('No se encontró el formulario dentro del modal.', 'danger');
            return;
        }
        // Verificar si el plugin de validación está disponible
        if (typeof form.valid === "function") {
            if (!form.valid()) {
                console.log('El formulario no es válido');
                return;
            }
        } else {
            console.error('El método form.valid() no está disponible.');
        }

        var formData = new FormData(form[0]);
        console.log('Datos del formulario (FormData) listos para enviar.');

        $('#spin').addClass('show');

        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                $('#spin').removeClass('show');
                console.log('Respuesta recibida:', response);

                if (response.success) {
                    $('#modal-lg').modal('hide');
                    location.reload();
                } else {
                    addAlert(response.message || 'Error no especificado.', 'danger');
                }
            },
            error: function () {
                $('#spin').removeClass('show');
                addAlert('Se produjo un error al procesar la solicitud.', 'danger');
            }
        });
    });

    // Acción para eliminar
    $('#modal-lg').on('click', '.btn-delete', function (e) {
        e.preventDefault();
        console.log('Botón de eliminar clicado');

        var form = $('#modal-lg').find('form');
        if (form.length === 0) {
            addAlert('No se encontró el formulario dentro del modal.', 'danger');
            return;
        }

        var formData = form.serialize();
        $('#spin').addClass('show');

        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: formData,
            success: function (response) {
                $('#spin').removeClass('show');
                console.log('Respuesta recibida:', response);

                if (response.success) {
                    $('#modal-lg').modal('hide');
                    location.reload();
                } else {
                    addAlert(response.message || 'Error no especificado.', 'danger');
                }
            },
            error: function () {
                $('#spin').removeClass('show');
                addAlert('Se produjo un error al procesar la solicitud.', 'danger');
            }
        });
    });

    // Acción de restaurar
    $('#modal-lg').on('click', '.btn-restore', function (e) {
        e.preventDefault();
        console.log('Botón de restaurar clicado');

        var form = $('#modal-lg').find('form');
        if (form.length === 0) {
            addAlert('No se encontró el formulario dentro del modal.', 'danger');
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
                    addAlert(response.message || 'Error no especificado.', 'danger');
                }
            },
            error: function () {
                addAlert('Se produjo un error al procesar la solicitud.', 'danger');
            }
        });
    });

    // Acción para Desactivar
    $('#modal-lg').on('click', '.btn-deactivate', function (e) {
        e.preventDefault();
        console.log('Botón de desactivar clicado');

        var form = $('#modal-lg').find('form');
        if (form.length === 0) {
            addAlert('No se encontró el formulario dentro del modal.', 'danger');
            return;
        }

        var formData = form.serialize();
        $('#spin').addClass('show');

        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: formData,
            success: function (response) {
                $('#spin').removeClass('show');
                console.log('Respuesta recibida:', response);

                if (response.success) {
                    $('#modal-lg').modal('hide');
                    location.reload();
                } else {
                    addAlert(response.message || 'Error no especificado.', 'danger');
                }
            },
            error: function () {
                $('#spin').removeClass('show');
                addAlert('Se produjo un error al procesar la solicitud.', 'danger');
            }
        });
    });

    // Acción de guardar con Enter
    $('#modal-lg').on('keypress', 'form', function (e) {
        if (e.which === 13) { // Código de tecla Enter
            e.preventDefault(); // Evitar el comportamiento predeterminado
            $('#modal-lg .btn-save').click(); // Simular clic en el botón de guardar
        }
    });

    // Cargar validación unobtrusive al abrir el modal
    $(document).on('shown.bs.modal', '#modal-lg', function () {
        $.validator.unobtrusive.parse('#modal-lg form'); // Cargar validación manualmente al abrir el modal
    });


    function addAlert(message, type) {
        $('#modal-lg .modal-body .alert').remove(); // Elimina cualquier alerta existente
        var alertHtml = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">'
            + message
            + '<button type="button" class="close" data-dismiss="alert" aria-label="Close">'
            + '<span aria-hidden="true">&times;</span>'
            + '</button></div>';
        $('#modal-lg .modal-body').prepend(alertHtml); // Inserta el mensaje de alerta al inicio del modal
    }



});
