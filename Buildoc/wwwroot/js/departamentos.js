// Definición global de la función initDepartamentos
function initDepartamentos(container) {
    $.getJSON('colombia.json', function (data) {
        var departamentoSelect = container.find('#departamentos');
        var municipiosData = {};
        // Rellenar el select de departamentos y almacenar los municipios por departamento
        data.forEach(function (departamento) {
            departamentoSelect.append(new Option(departamento.departamento, departamento.id));
            municipiosData[departamento.id] = departamento.ciudades;
        });

      

        // Manejar el cambio de selección del departamento
        container.find('#departamentos').change(function () {
            var departamentoId = $(this).val();
            var municipioSelect = container.find('#municipios');

            if (departamentoId) {
                var municipios = municipiosData[departamentoId];
                municipioSelect.empty().append(new Option('Seleccione un municipio', ''));
                municipios.forEach(function (municipio) {
                    municipioSelect.append(new Option(municipio, municipio));
                });
                municipioSelect.prop('disabled', false);
            } else {
                municipioSelect.empty().append(new Option('Seleccione un municipio', '')).prop('disabled', true);
            }
        });
    });
}

$(document).ready(function () {
    console.log("departamentos.js se está ejecutando");

    // Inicializar en el DOM principal
    initDepartamentos($(document));

    // Inicializar en el contenido del modal cuando se cargue
    $('#modal-lg').on('shown.bs.modal', function () {
        console.log("Modal se ha mostrado");
        initDepartamentos($('#modal-lg'));
    });
});
