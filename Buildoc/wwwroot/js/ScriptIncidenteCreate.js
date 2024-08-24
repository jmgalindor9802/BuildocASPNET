$(document).on('shown.bs.modal', '#modal-lg', function () {
    console.log("Modal abierto");
    // Inicializar bs-stepper
    console.log("Se esta utilizando unicamente el script de incidente")
    var stepper = new Stepper($('.bs-stepper')[0]);

    // Función para actualizar la visibilidad del botón "Siguiente"
    function updateNextButtonVisibility() {
        var isChecked = $('#SwitchAfectados').is(':checked');
        if (isChecked) {
            $('#btnNext').show();
        } else {
            $('#btnNext').hide();
        }
    }

    // Inicializa la visibilidad del botón en función del estado inicial del switch
    updateNextButtonVisibility();

    // Maneja el cambio en el switch para mostrar/ocultar el botón "Siguiente"
    $('#SwitchAfectados').on('change', function () {
        updateNextButtonVisibility();
    });

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

    // Configuración del checkbox desconoceHora
    $('#desconoceHora').on('change', function () {
        console.log("Cambio en desconoceHora detectado");
        if ($(this).is(':checked')) {
            $('#horaIncidente').prop('disabled', true).val('');
        } else {
            $('#horaIncidente').prop('disabled', false);
        }
    });

    // Lógica para el botón "Siguiente"
    $(document).on('click', '.btn-next', function () {
        console.log("Botón Siguiente presionado");
        stepper.next();
    });

    // Lógica para el botón "Anterior"
    $(document).on('click', '.btn-previous', function () {
        console.log("Botón Anterior presionado");
        stepper.previous();
    });
});
