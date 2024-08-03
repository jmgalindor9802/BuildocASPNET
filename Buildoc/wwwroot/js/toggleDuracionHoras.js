$(document).ready(function () {
    function toggleDuracionHoras() {
        // Verificar si el elemento existe antes de manipularlo
        if ($('#EsTodoElDia').length && $('#DuracionHoras').length) {
            if ($('#EsTodoElDia').is(':checked')) {
                $('#DuracionHoras').closest('.form-group').hide();
            } else {
                $('#DuracionHoras').closest('.form-group').show();
            }
        } else {
            console.error('Uno o ambos elementos no se encuentran en el DOM.');
        }
    }

    // Ejecutar la funci�n al cargar el documento
    toggleDuracionHoras();

    // Adjuntar la funci�n al evento change del checkbox
    $(document).on('change', '#EsTodoElDia', function () {
        toggleDuracionHoras();
    });

    // Ejecutar la funci�n al abrir el modal
    $(document).on('shown.bs.modal', '#modal-lg', function () {
        toggleDuracionHoras();
    });
});
