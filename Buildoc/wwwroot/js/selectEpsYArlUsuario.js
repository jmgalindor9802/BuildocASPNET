$(document).ready(function () {
    // Inicializar EPS cuando el modal se abre
    $('#modal-lg').on('shown.bs.modal', function () {
        cargarEPS($('#modal-lg')); // Cargar EPS en el modal
        cargarARL($('#modal-lg'));
    });
});

// Función para cargar EPS en el select
function cargarEPS(container) {
    fetch('/js/eps.json')
        .then(response => response.json())
        .then(data => {
            let select = container.find('#eps-select');
            select.empty().append(new Option('Seleccione una EPS', '')); // Reiniciar el select
            data.eps_colombia.forEach(eps => {
                let option = document.createElement('option');
                option.value = eps;
                option.textContent = eps;
                select.append(option);
            });
        })
        .catch(error => console.error('Error al cargar las EPS:', error));
}
// Función para cargar ARL en el select
function cargarARL(container) {
    fetch('/js/arl.json')
        .then(response => response.json())
        .then(data => {
            let select = container.find('#arl-select');
            select.empty().append(new Option('Seleccione una ARL', '')); // Reiniciar el select
            data.arl_colombia.forEach(arl => {
                let option = document.createElement('option');
                option.value = arl;
                option.textContent = arl;
                select.append(option);
            });
        })
        .catch(error => console.error('Error al cargar las ARL:', error));
}