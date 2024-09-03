$(document).ready(function () {
    // Inicializa la DataTable con la configuración deseada
    let table = $('#example1').DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print', 'colvis'
        ],
        language: {
            url: "/espanol.json" // Ruta al archivo JSON para la traducción en español
        },
        responsive: true,
        lengthChange: false,
        autoWidth: false,
        paging: true,
        ordering: true,
        info: true
    });
    // Función para obtener proyectos únicos del DataTable
    function getProyectos() {
        const proyectos = [];
        table.rows().every(function () {
            const data = this.data();
            const proyecto = data[2]; // Asume que la columna de proyecto es la 3ra (índice 2)
            if (!proyectos.includes(proyecto)) {
                proyectos.push(proyecto);
            }
        });
        return proyectos;
    }

    // Función para obtener tipos de incidentes únicos del DataTable
    function getTiposIncidentes() {
        const tiposIncidentes = [];
        table.rows().every(function () {
            const data = this.data();
            const tipoIncidente = data[3]; // Asume que la columna de tipo de incidente es la 4ta (índice 3)
            if (!tiposIncidentes.includes(tipoIncidente)) {
                tiposIncidentes.push(tipoIncidente);
            }
        });
        return tiposIncidentes;
    }

    // Llenar el select con las opciones de proyecto
    function fillProyectoSelect() {
        const proyectoFilterEl = $('#proyecto-filter');
        proyectoFilterEl.empty(); // Limpiar el select antes de llenarlo
        proyectoFilterEl.append('<option value="">Todos los proyectos</option>'); // Opción predeterminada
        const proyectos = getProyectos();
        proyectos.forEach(proyecto => {
            const option = $('<option></option>').val(proyecto).text(proyecto);
            proyectoFilterEl.append(option);
        });
    }

    // Llenar el select con las opciones de tipo de incidente
    function fillTipoIncidenteSelect() {
        const tipoIncidenteFilterEl = $('#tipo-incidente-filter');
        tipoIncidenteFilterEl.empty(); // Limpiar el select antes de llenarlo
        tipoIncidenteFilterEl.append('<option value="">Todos los tipos de incidentes</option>'); // Opción predeterminada
        const tiposIncidentes = getTiposIncidentes();
        tiposIncidentes.forEach(tipoIncidente => {
            const option = $('<option></option>').val(tipoIncidente).text(tipoIncidente);
            tipoIncidenteFilterEl.append(option);
        });
    }

    // Inicializa los selects
    fillProyectoSelect();
    fillTipoIncidenteSelect();

    // Agregar evento de cambio al select para filtrar la tabla
    $('#proyecto-filter').on('change', function () {
        const proyectoFilter = $(this).val();
        table.column(2).search(proyectoFilter).draw(); // Filtra la columna de proyectos
    });

    $('#tipo-incidente-filter').on('change', function () {
        const tipoIncidenteFilter = $(this).val();
        table.column(3).search(tipoIncidenteFilter).draw(); // Filtra la columna de tipo de incidente
    });
});