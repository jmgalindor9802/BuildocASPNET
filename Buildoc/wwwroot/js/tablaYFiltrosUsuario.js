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

	// Función para obtener roles únicos del DataTable
	function getRoles() {
		const roles = [];
		table.rows().every(function () {
			const data = this.data();
			const rol = data[4];
			if (!roles.includes(rol)) {
				roles.push(rol);
			}
		});
		return roles;
	}

	//funcion para obtener las profeciones unicas
	function getProfeciones() {
		const profesiones = [];
		table.rows().every(function () {
			const data = this.data();
			const profesion = data[2];
			if (!profesiones.includes(profesion)) {
				profesiones.push(profesion);
			}
		});
		return profesiones;
	}

	function getLasEps() {
		const lasEps = [];
		table.rows().every(function () {
			const data = this.data();
			const eps = data[3];
			if (!lasEps.includes(eps)) {
				lasEps.push(eps);
			}
		});
		return lasEps;
	}

	// Llenar el select con las opciones de rol
	function fillRolSelect() {
		const rolFilterEl = $('#rol-filter');
		rolFilterEl.empty(); // Limpiar el select antes de llenarlo
		rolFilterEl.append('<option value="">Todos los roles</option>'); // Opción predeterminada
		const roles = getRoles();
		roles.forEach(rol => {
			const option = $('<option></option>').val(rol).text(rol);
			rolFilterEl.append(option);
		});
	}
	// Llenar el select con las opciones de profecion
	function fillProfesionSelect() {
		const profecionFilterEl = $('#profesion-filter');
		profecionFilterEl.empty(); // Limpiar el select antes de llenarlo
		profecionFilterEl.append('<option value="">Todos los profesiones</option>'); // Opción predeterminada
		const profesiones = getProfeciones();
		profesiones.forEach(profesion => {
			const option = $('<option></option>').val(profesion).text(profesion);
			profecionFilterEl.append(option);
		});
	}
	// Llenar el select con las opciones de eps
	function fillEpsSelect() {
		const epsFilterEl = $('#eps-filter');
		epsFilterEl.empty(); // Limpiar el select antes de llenarlo
		epsFilterEl.append('<option value="">Todos las eps</option>'); // Opción predeterminada
		const lasEps = getLasEps();
		lasEps.forEach(eps => {
			const option = $('<option></option>').val(eps).text(eps);
			epsFilterEl.append(option);
		});
	}



	// Inicializa los selects
	fillRolSelect();
	fillEpsSelect();
	fillProfesionSelect();

	// Agregar evento de cambio al select para filtrar la tabla
	$('#rol-filter').on('change', function () {
		const rolFilter = $(this).val();
		table.column(4).search(rolFilter).draw(); // Filtra la columna de proyectos
	});
	$('#profesion-filter').on('change', function () {
		const profecionFilter = $(this).val();
		table.column(2).search(profecionFilter).draw(); // Filtra la columna de proyectos
	});
	$('#eps-filter').on('change', function () {
		const epsFilter = $(this).val();
		table.column(3).search(epsFilter).draw(); // Filtra la columna de proyectos
	});
});