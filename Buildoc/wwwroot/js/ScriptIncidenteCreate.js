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
        var descripcionDiv = $('#descripcionTipoIncidente');
        var descripcionTextArea = $('#descripcionTextArea');

        if (tipoIncidenteId) {
            fetch(`/Incidentes/GetTipoIncidenteDescripcion?tipoIncidenteId=${tipoIncidenteId}`)
                .then(response => response.json())
                .then(data => {
                    descripcionDiv.show();
                    descripcionTextArea.val(data.descripcion);
                })
                .catch(error => {
                    console.error("Error al obtener descripción de tipo de incidente", error);
                });
        } else {
            descripcionDiv.hide();
            descripcionTextArea.val('');
        }
    });

    // Configuración del cambio de SwitchAfectados
    $('#SwitchAfectados').on('change', function () {
        console.log("Cambio en SwitchAfectados detectado");
        toggleAfectadosFields();
    });

    toggleAfectadosFields();

    function toggleAfectadosFields() {
        var switchAfectados = $('#SwitchAfectados').is(':checked');
        var afectadosContainer = $('#afectadosContainer');
        if (switchAfectados) {
            afectadosContainer.show();
        } else {
            afectadosContainer.hide();
            afectadosContainer.find('input, textarea').val('');
        }
    }

    // Configuración del botón para añadir afectados
    $('#addAfectadoButton').on('click', function () {
        console.log("Añadiendo afectado");
        addAfectado();
    });

    function addAfectado() {
        var afectadosContainer = $('#afectadosContainer');
        var template = $('#afectadoTemplate').clone().removeAttr('id');
        template.show().removeClass('afectado-template').addClass('afectado-group');

        var afectadoCount = $('.afectado-group').length;
        template.find('input, textarea, select').each(function () {
            var name = $(this).attr('name');
            if (name) {
                $(this).attr('name', name.replace('Afectados[0]', 'Afectados[' + afectadoCount + ']'));
            }
        });

        afectadosContainer.append(template);

        // Añadir evento para eliminar afectado
        template.find('.remove-afectado').on('click', function () {
            template.remove();
        });
    }

    // Configuración del checkbox desconoceHora
    $('#desconoceHora').on('change', function () {
        console.log("Cambio en desconoceHora detectado");
        $('#horaIncidente').prop('disabled', $(this).is(':checked'));
    });
});
