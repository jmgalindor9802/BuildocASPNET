// lesionados.js

$(document).ready(function () {
    let lesionadoIndex = 0;

    function updateIndexes() {
        $('#lesionados-container .lesionado-item').each(function (index) {
            $(this).find('input, select').each(function () {
                var name = $(this).attr('name');
                if (name) {
                    var newName = name.replace(/\[\d+\]/, '[' + index + ']');
                    $(this).attr('name', newName);
                }
            });
            // Mostrar u ocultar el botón de eliminar según el índice
            if (index === 0) {
                $(this).find('.remove-lesionado').hide();
            } else {
                $(this).find('.remove-lesionado').show();
            }
        });
    }

    function addLesionado() {
        var container = $('#lesionados-container');
        var template = container.find('.lesionado-item:first').clone();

        // Limpiar valores de los campos en el clon
        template.find('input, select').val('');

        // Agregar el nuevo item al contenedor
        container.append(template);

        lesionadoIndex++;
        updateIndexes();

        // Añadir evento para eliminar lesionado
        template.find('.remove-lesionado').off('click').on('click', function () {
            template.remove();
            updateIndexes();
        });
    }

    // Evento para añadir lesionado
    $('#add-lesionado').off('click').on('click', function () {
        console.log("Añadiendo lesionado");
        addLesionado();
    });

    // Evento para eliminar lesionado existente
    $('#lesionados-container').on('click', '.remove-lesionado', function () {
        // Evitar eliminar el primer lesionado
        if ($(this).closest('.lesionado-item').index() !== 0) {
            $(this).closest('.lesionado-item').remove();
            updateIndexes();
        }
    });

    // Ocultar el botón de eliminar en el primer lesionado al cargar la página
    $(document).ready(function () {
        updateIndexes();
    });

    // Autocompletar campos cuando se ingresa una cédula
    $(document).on('blur', 'input[name$="Cedula"]', function () {
        var inputCedula = $(this);
        var cedula = inputCedula.val();
        if (cedula) {
            fetch(`/Incidentes/GetLesionadoByCedula?cedula=${cedula}`)
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        var lesionado = data.data;
                        // Autocompletar los campos
                        inputCedula.closest('.lesionado-item').find('input[name$="Nombre"]').val(lesionado.nombre);
                        inputCedula.closest('.lesionado-item').find('input[name$="Apellido"]').val(lesionado.apellido);
                        inputCedula.closest('.lesionado-item').find('input[name$="CorreoElectronico"]').val(lesionado.correoElectronico);
                        // Completa otros campos necesarios
                    } else {
                        console.log(data.message);
                        // Opcional: limpiar los campos si no se encuentra un lesionado
                    }
                })
                .catch(error => {
                    console.error("Error al obtener datos del lesionado", error);
                });
        }
    });
});
