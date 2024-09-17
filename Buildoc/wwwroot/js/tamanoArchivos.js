
    document.getElementById("UploadedFiles").addEventListener("change", function (event) {
        const maxFileSize = 25 * 1024 * 1024; // 25MB en bytes
    const files = event.target.files;
    let hasLargeFile = false;

    // Limpiar mensajes de error previos
    document.getElementById("fileSizeError").textContent = "";

    // Verificar el tamaño de cada archivo
    for (let i = 0; i < files.length; i++) {
            if (files[i].size > maxFileSize) {
        hasLargeFile = true;
    break;
            }
        }

    if (hasLargeFile) {
        // Mostrar mensaje de error al usuario
        alert("Uno o más archivos superan el límite permitido de 25MB.");
    // Limpiar el input de archivos
    event.target.value = "";
        }
    });
