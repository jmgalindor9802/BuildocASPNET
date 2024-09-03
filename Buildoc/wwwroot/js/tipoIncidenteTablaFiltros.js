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

        // Función para obtener categoria únicos del DataTable
        function getCategorias() {
            const categorias = [];
            table.rows().every(function () {
                const data = this.data();
                const categoria = data[0]; // Asume que la columna de proyecto es la 3ra (índice 2)
                if (!categorias.includes(categoria)) {
                    categorias.push(categoria);
                }
            });
            return categorias;
        }
        // Función para obtener gravedad únicos del DataTable
        function getGravedades() {
            const gravedades = [];
            table.rows().every(function () {
                const data = this.data();
                const gravedad = data[2]; // Asume que la columna de proyecto es la 3ra (índice 2)
                if (!gravedades.includes(gravedad)) {
                    gravedades.push(gravedad);
                }
            });
            return gravedades;
        }
        // Llenar el select con las opciones de categoria
        function fillCategoriaSelect() {
            const categoriaFilterEl = $('#categoria-filter');
            categoriaFilterEl.empty(); // Limpiar el select antes de llenarlo
            categoriaFilterEl.append('<option value="">Todas las categorias</option>'); // Opción predeterminada
            const categorias = getCategorias();
            categorias.forEach(categoria => {
                const option = $('<option></option>').val(categoria).text(categoria);
                categoriaFilterEl.append(option);
            });
        }
        // Llenar el select con las opciones de gravedad
        function fillGravedadSelect() {
            const gravedadFilterEl = $('#gravedad-filter');
            gravedadFilterEl.empty(); // Limpiar el select antes de llenarlo
            gravedadFilterEl.append('<option value="">Todas las gravedades</option>'); // Opción predeterminada
            const gravedades = getGravedades();
            gravedades.forEach(gravedad => {
                const option = $('<option></option>').val(gravedad).text(gravedad);
                gravedadFilterEl.append(option);
            });
        }

        fillCategoriaSelect();
        fillGravedadSelect();

        // Agregar evento de cambio al select para filtrar la tabla
        $('#categoria-filter').on('change', function () {
            const categoriaFilter = $(this).val();
            table.column(0).search(categoriaFilter).draw(); // Filtra la columna de proyectos
        });

        $('#gravedad-filter').on('change', function () {
            const gravedadFilter = $(this).val();
            table.column(2).search(gravedadFilter).draw(); // Filtra la columna de proyectos
        });
    });