$(document).on('shown.bs.modal', '#modal-lg', function () {
    console.log("Modal abierto");

    // Configuración del cambio de categoría de tipo de incidente
    $('#CategoriaTipoIncidente').on('change', function () {
        console.log("Cambio en CategoriaTipoIncidente detectado");
        var categoriaId = $(this).val();
        if (categoriaId) {
            fetch(`/Incidentes/GetTiposDeIncidentePorCategoria?categoriaId=${categoriaId}`)
                .then(response => response.json())
                .then(data => {
                    var tipoIncidenteSelect = $('#tipoIncidenteSelect');
                    tipoIncidenteSelect.empty();
                    tipoIncidenteSelect.append('<option value="">Seleccione un tipo de incidente</option>');
                    data.forEach(tipo => {
                        tipoIncidenteSelect.append(new Option(tipo.nombre, tipo.id));
                    });
                })
                .catch(error => {
                    console.error("Error al obtener tipos de incidente", error);
                });
        }
    });

    // Configuración del cambio de tipo de incidente
    $('#tipoIncidenteSelect').on('change', function () {
        console.log("Cambio en tipoIncidenteSelect detectado");
        var tipoIncidenteId = $(this).val();

        if (tipoIncidenteId) {
            fetch(`/Incidentes/GetTipoIncidenteDetalles?tipoId=${tipoIncidenteId}`)
                .then(response => response.json())
                .then(data => {
                    $('#TipoIncidenteCategoria').text(data.categoria || 'N/A');
                    $('#TipoIncidenteTitulo').text(data.titulo || 'N/A');
                    $('#TipoIncidenteGravedad').text(data.gravedad || 'N/A');
                    $('#TipoInspeccionDescripcion').text(data.descripcion || 'N/A');
                })
                .catch(error => {
                    console.error("Error al obtener detalles del tipo de incidente", error);
                });
        } else {
            $('#TipoIncidenteCategoria').text('');
            $('#TipoIncidenteTitulo').text('');
            $('#TipoIncidenteGravedad').text('');
            $('#TipoInspeccionDescripcion').text('');
        }
    });


    // Configuración del cambio de SwitchAfectados
    $('#SwitchAfectados').on('change', function () {
        console.log("Cambio en SwitchAfectados detectado");
        toggleAfectadosFields();
    });

    function toggleAfectadosFields() {
        var switchAfectados = $('#SwitchAfectados').is(':checked');
        var afectadosContainer = $('#afectadosContainer');
        var firstAfectado = $('#afectadoTemplate');

        if (switchAfectados) {
            afectadosContainer.show();
            if (firstAfectado.length) {
                firstAfectado.show();
            }
        } else {
            afectadosContainer.hide();
            afectadosContainer.find('input, textarea, select').val('');
            afectadosContainer.find('input[type=checkbox], input[type=radio]').prop('checked', false);
        }
    }

    toggleAfectadosFields(); // Llama a la función al cargar la página para establecer el estado inicial


    // Configuración del botón para añadir afectados
    $('#addAfectadoButton').off('click').on('click', function () {
        console.log("Añadiendo afectado");
        addAfectado();
    });

    function addAfectado() {
        var afectadosContainer = $('#afectadosContainer');
        var template = $('#afectadoTemplate').clone().removeAttr('id');
        template.show().removeClass('afectado-template').addClass('afectado-group');

        // Encuentra el número de afectados actuales y usa ese índice
        var afectadoCount = afectadosContainer.find('.afectado-group').length;

        // Actualiza los nombres de los campos para reflejar el índice correcto
        template.find('input, textarea, select').each(function () {
            var name = $(this).attr('name');
            if (name) {
                $(this).attr('name', name.replace(/\[\d+\]/, '[' + afectadoCount + ']'));
            }
        });

        afectadosContainer.append(template);

        // Añadir evento para eliminar afectado
        template.find('.remove-afectado').off('click').on('click', function () {
            template.remove();
        });
    }


    // Configuración del checkbox desconoceHora
    $('#desconoceHora').on('change', function () {
        console.log("Cambio en desconoceHora detectado");
        if ($(this).is(':checked')) {
            $('#horaIncidente').prop('disabled', true).val('');
        } else {
            $('#horaIncidente').prop('disabled', false);
        }
    });
});
