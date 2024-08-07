$(document).on('shown.bs.modal', '#modal-lg', function () {
    console.log("Modal abierto");

    // Configuraci�n del cambio de categor�a de tipo de incidente
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

    // Configuraci�n del cambio de tipo de incidente
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


    // Configuraci�n del cambio de SwitchAfectados
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

    toggleAfectadosFields(); // Llama a la funci�n al cargar la p�gina para establecer el estado inicial


    // Configuraci�n del bot�n para a�adir afectados
    $('#addAfectadoButton').off('click').on('click', function () {
        console.log("A�adiendo afectado");
        addAfectado();
    });

    function addAfectado() {
        var afectadosContainer = $('#afectadosContainer');
        var template = $('#afectadoTemplate').clone().removeAttr('id');
        template.show().removeClass('afectado-template').addClass('afectado-group');

        // Encuentra el n�mero de afectados actuales y usa ese �ndice
        var afectadoCount = afectadosContainer.find('.afectado-group').length;

        // Actualiza los nombres de los campos para reflejar el �ndice correcto
        template.find('input, textarea, select').each(function () {
            var name = $(this).attr('name');
            if (name) {
                $(this).attr('name', name.replace(/\[\d+\]/, '[' + afectadoCount + ']'));
            }
        });

        afectadosContainer.append(template);

        // A�adir evento para eliminar afectado
        template.find('.remove-afectado').off('click').on('click', function () {
            template.remove();
        });
    }


    // Configuraci�n del checkbox desconoceHora
    $('#desconoceHora').on('change', function () {
        console.log("Cambio en desconoceHora detectado");
        if ($(this).is(':checked')) {
            $('#horaIncidente').prop('disabled', true).val('');
        } else {
            $('#horaIncidente').prop('disabled', false);
        }
    });
});
