$(document).ready(function () {
	$('#example1').DataTable({
		"dom":
			"<'row'<'col-sm-6'B><'col-sm-3'><'col-sm-3'f>>" +
			"<'row'<'col-sm-12'tr>>" +
			"<'row'<'col-sm-5'i><'col-sm-7 text-left'p>>",

		"language": {
			"url": "/espanol.json"
		},
		"responsive": true,
		"lengthChange": false,
		"autoWidth": false,
		"paging": true,
		"ordering": true,
		"info": true,

		"buttons": ["csv", "excel", "pdf", "colvis"]
	}).buttons().container().appendTo('#example1_wrapper .col-md-6:eq(0)');


});