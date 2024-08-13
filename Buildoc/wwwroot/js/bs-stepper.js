$(document).on('shown.bs.modal', '#modal-lg', function () {
    initializeStepper();
});
function initializeStepper() {
    var steppers = document.querySelectorAll('.bs-stepper');
    console.log(steppers); // Verifica cuántos steppers se encuentran
    steppers.forEach(function (stepperElement) {
        console.log(stepperElement); // Confirma la inicialización de cada stepper
        var stepper = new Stepper(stepperElement);

        // Lógica adicional para el stepper, si es necesaria
        $(stepperElement).on('click', '.btn-next', function () {
            stepper.next();
        });

        $(stepperElement).on('click', '.btn-previous', function () {
            stepper.previous();
        });
    });
}