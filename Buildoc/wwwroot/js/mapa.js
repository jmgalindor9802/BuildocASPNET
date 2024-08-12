window.addEventListener('DOMContentLoaded', (event) => {
    console.log("DOM completamente cargado y parseado");
    // Obtener el tamaño del contenedor #map después de que el DOM haya sido cargado
    var container = d3.select("#map");
    var width = container.node().offsetWidth;
    var height = container.node().offsetHeight;

    // Configurar el tamaño del SVG dinámicamente
    var svg = container.append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("viewBox", `0 0 ${width} ${height}`)  // Ajustar la vista previa del SVG
        .style("display", "block");

    // Añadir un filtro de sombra al SVG
    svg.append("defs").append("filter")
        .attr("id", "shadow")
        .append("feDropShadow")
        .attr("dx", 2)
        .attr("dy", 2)
        .attr("stdDeviation", 2);

    // Definir el grupo que contendrá los paths del mapa
    var g = svg.append("g");

    // Añadir un grupo para el texto
    var textGroup = svg.append("g")
        .attr("id", "text-group");

    // Configurar la funcionalidad de zoom
    var zoom = d3.zoom()
        .scaleExtent([1, 8])  // Definir el nivel de zoom mínimo y máximo
        .on("zoom", function (event) {
            g.attr("transform", event.transform);  // Aplicar el zoom/pan al grupo de paths
            textGroup.attr("transform", event.transform);  // Aplicar el zoom/pan al grupo de texto

            // Actualizar el tamaño del texto basado en el nivel de zoom
            var zoomLevel = event.transform.k;
            textGroup.selectAll("text")
                .attr("font-size", Math.max(10 / zoomLevel, 5) + "px"); // Ajusta el tamaño mínimo según sea necesario

            // Actualizar el grosor de los bordes de los municipios basado en el nivel de zoom
            g.selectAll("path")
                .attr("stroke-width", Math.max(0.5 / zoomLevel, 0.2)); // Ajusta el grosor mínimo según sea necesario
        });

    // Aplicar el comportamiento de zoom al SVG
    svg.call(zoom);

    console.log("SVG creado:", svg);  // Agregar este log   

    // Configurar la proyección para Colombia
    var projection = d3.geoMercator()
        .scale(2500)  // Ajustar la escala según sea necesario
        .center([-74, 4.5])  // Centrar en Colombia (longitud, latitud)
        .translate([width / 2, height / 2]);  // Transladar al centro del SVG

    // Crear un generador de paths utilizando la proyección
    var path = d3.geoPath().projection(projection);

    // Cargar y procesar el archivo TopoJSON
    d3.json("/js/municipios.json").then(function (data) {

        // Convertir la colección de objetos TopoJSON en GeoJSON
        var municipios = topojson.feature(data, data.objects.MGN_ANM_MPIOS);
        console.log("Municipios procesados:", municipios);  // Verificar la conversión a GeoJSON

        // Guardar los datos de municipios en window para asegurarte de que estén disponibles globalmente
        window.municipiosGeoData = municipios;


        // Dibujar los municipios en el mapa
        g.selectAll("path")
            .data(municipios.features)
            .enter()
            .append("path")
            .attr("d", path)
            .attr("fill", "#cccccc")  // Color de relleno para los municipios
            .attr("stroke", "#001f3f")  // Color del borde de los municipios
            .attr("stroke-width", 0.5)  // Grosor inicial del borde de los municipios
            .on("click", function (event, d) {  // Agregar el manejador de eventos de clic
                // Limpiar cualquier selección previa
                g.selectAll("path").classed("municipio-selected", false);

                // Aplicar la clase de selección al municipio clicado
                d3.select(this).classed("municipio-selected", true);

                var coords = projection(d3.geoCentroid(d));
                var name = d.properties.MPIO_CNMBR;  // Ajusta esto según el nombre del campo en tu TopoJSON

                // Limpiar cualquier texto previo
                textGroup.selectAll("*").remove();

                // Añadir un texto en la posición del municipio clicado
                textGroup.append("text")
                    .attr("x", coords[0])
                    .attr("y", coords[1])
                    .attr("font-size", "12px")
                    .attr("fill", "black")
                    .attr("text-anchor", "middle")
                    .text(name);

                // Actualizar el tamaño del texto basado en el nivel de zoom inicial
                var zoomLevel = d3.zoomTransform(svg.node()).k;
                textGroup.selectAll("text")
                    .attr("font-size", Math.max(12 / zoomLevel, 5) + "px"); // Ajusta el tamaño mínimo según sea necesario
            });

        // Asegurarse de que los datos necesarios estén disponibles antes de llamar a addInspectionPoints
        if (window.municipiosGeoData && window.municipiosConInspecciones) {
            addInspectionPoints();
        } else {
            console.log('Los datos de municipios no están disponibles.');
        }


        // AQUI AGREGAS LA FUNCION QUE BUSCA LAS COORDENADAS
        function normalizeName(name) {
            return name.trim().toUpperCase(); // Normaliza el nombre eliminando espacios y convirtiendo a mayúsculas
        }

        function addInspectionPoints() {
            console.log("Ejecutando addInspectionPoints");

            if (window.municipiosConInspecciones) {
                console.log('Municipios con inspecciones:', window.municipiosConInspecciones);

                // Iterar sobre los municipios con inspecciones
                window.municipiosConInspecciones.forEach(function (municipio) {
                    var found = false;
                    var normalizedMunicipio = normalizeName(municipio);

                    // Buscar el municipio en los datos geográficos
                    g.selectAll("path").each(function (d) {
                        var dMunicipio = normalizeName(d.properties.MPIO_CNMBR);

                        if (dMunicipio === normalizedMunicipio) {
                            found = true;
                            var lat = d.properties.LATITUD;
                            var lng = d.properties.LONGITUD;

                            console.log('Municipio:', municipio); // Imprime el nombre del municipio
                            console.log('Coordenadas:', [lat, lng]); // Imprime las coordenadas

                            g.append("circle")
                                .attr("cx", projection([lng, lat])[0])
                                .attr("cy", projection([lng, lat])[1])
                                .attr("r", 5) // Tamaño del círculo
                                .attr("fill", "red")
                                .attr("stroke", "black")
                                .attr("stroke-width", 1);                         

                            console.log('Punto añadido para:', municipio, [lat, lng]);
                        }
                    });

                    if (!found) {
                        console.log('Municipio no encontrado en los datos geográficos:', municipio);
                    }
                });
            } else {
                console.log("Los datos de municipios no están disponibles");
            }
        }



    }).catch(function (error) {
        console.error("Error al cargar el archivo TopoJSON:", error);  // Manejo de errores
    });
});
