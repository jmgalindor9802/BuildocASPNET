    document.getElementById('CategoriaTipoIncidente').addEventListener('change', function () {
            var categoriaId = this.value;
    fetch(`/Incidentes/GetTiposDeIncidentePorCategoria?categoriaId=${categoriaId}`)
                .then(response => response.json())
                .then(data => {
                    var tipoIncidenteSelect = document.getElementById('tipoIncidenteSelect');
    tipoIncidenteSelect.innerHTML = '<option value="">Seleccione un tipo de incidente</option>';
                    data.forEach(tipo => {
                        var option = document.createElement('option');
    option.value = tipo.id;
    option.text = tipo.nombre;
    tipoIncidenteSelect.appendChild(option);
                    });
                });
        });
    // Obtener el elemento select del tipo de incidente
    document.getElementById('tipoIncidenteSelect').addEventListener('change', function () {
            // Obtener el valor seleccionado
            var tipoIncidenteId = this.value;
    // Obtener los elementos del div y textarea de descripci�n
    var descripcionDiv = document.getElementById('descripcionTipoIncidente');
    var descripcionTextArea = document.getElementById('descripcionTextArea');

    // Si se ha seleccionado un tipo de incidente (valor no vac�o)
    if (tipoIncidenteId) {
        // Realizar una petici�n fetch para obtener la descripci�n del tipo de incidente
        fetch(`/Incidentes/GetTipoIncidenteDescripcion?tipoIncidenteId=${tipoIncidenteId}`)
            .then(response => response.json())
            .then(data => {
                // Mostrar el div de descripci�n y establecer el valor del textarea
                descripcionDiv.style.display = 'block';
                descripcionTextArea.value = data.descripcion;
            });
            } else {
        // Si no se ha seleccionado un tipo de incidente, ocultar el div de descripci�n y limpiar el textarea
        descripcionDiv.style.display = 'none';
    descripcionTextArea.value = '';
            }
        });

    function toggleAfectadosFields() {
            var switchAfectados = document.getElementById('SwitchAfectados');
    var afectadosContainer = document.getElementById('afectadosContainer');
    if (switchAfectados.checked) {
        afectadosContainer.style.display = 'block';
            } else {
        afectadosContainer.style.display = 'none';

    // Limpiar los datos de los afectados cuando el switch est� desmarcado
    var inputs = afectadosContainer.querySelectorAll('input, textarea');
    inputs.forEach(function (input) {
        input.value = '';
                });
            }
        }

    // Ejecutar la funci�n cuando se cambie el estado del switch
    document.getElementById('SwitchAfectados').addEventListener('change', toggleAfectadosFields);

    // Ejecutar la funci�n al cargar la p�gina para establecer el estado inicial
    toggleAfectadosFields();

    // Funci�n para a�adir un nuevo afectado
    function addAfectado() {
            var afectadosContainer = document.getElementById('afectadosContainer');
    var template = document.getElementById('afectadoTemplate');
    var clone = template.cloneNode(true);
    clone.style.display = 'block';
    clone.classList.remove('afectado-template');
    clone.classList.add('afectado-group');

    var afectadoCount = document.getElementsByClassName('afectado-group').length;
    clone.querySelectorAll('input, textarea, select').forEach(function (input) {
                var name = input.getAttribute('name');
    if (name) {
        input.setAttribute('name', name.replace('Afectados[0]', 'Afectados[' + afectadoCount + ']'));
                }
            });

    afectadosContainer.insertBefore(clone, document.getElementById('addAfectadoButton'));

    // A�adir evento para eliminar afectado
    clone.querySelector('.remove-afectado').addEventListener('click', function () {
        clone.remove();
            });
        }

    document.getElementById('addAfectadoButton').addEventListener('click', addAfectado);

    // Obtener el elemento checkbox "desconoceHora"
    document.getElementById('desconoceHora').addEventListener('change', function () {
            // Obtener el elemento input de hora del incidente
            var horaIncidente = document.getElementById('horaIncidente');
    // Habilitar o deshabilitar el input de hora seg�n el estado del checkbox
    horaIncidente.disabled = this.checked;
        });