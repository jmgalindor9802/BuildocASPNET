window.addEventListener('DOMContentLoaded', (event) => {
    console.log("DOM completamente cargado y parseado");

    var container = d3.select("#map");
    var width = container.node().offsetWidth;
    var height = container.node().offsetHeight;

    var svg = container.append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("viewBox", `0 0 ${width} ${height}`)
        .style("display", "block");

    svg.append("defs").append("filter")
        .attr("id", "shadow")
        .append("feDropShadow")
        .attr("dx", 2)
        .attr("dy", 2)
        .attr("stdDeviation", 2);

    var g = svg.append("g");
    var textGroup = svg.append("g").attr("id", "text-group");

    var zoom = d3.zoom()
        .scaleExtent([1, 8])
        .on("zoom", function (event) {
            g.attr("transform", event.transform);
            textGroup.attr("transform", event.transform);

            var zoomLevel = event.transform.k;
            textGroup.selectAll("text")
                .attr("font-size", Math.max(10 / zoomLevel, 5) + "px");

            g.selectAll("path")
                .attr("stroke-width", Math.max(0.5 / zoomLevel, 0.2));
        });

    svg.call(zoom);

    console.log("SVG creado:", svg);

    var projection = d3.geoMercator()
        .scale(2500)
        .center([-74.0721, 4.7110])
        .translate([width / 2, height / 2]);

    var path = d3.geoPath().projection(projection);

    d3.json("/js/municipios.json").then(function (data) {
        console.log("Datos cargados:", data);
        console.log("Claves de objetos disponibles:", Object.keys(data.objects));

        var departamentos = topojson.feature(data, data.objects.MGN_ANM_DPTOS);
        var municipios = topojson.feature(data, data.objects.MGN_ANM_MPIOS);

        console.log("Departamentos procesados:", departamentos);
        console.log("Municipios procesados:", municipios);

        window.departamentosGeoData = departamentos;
        window.municipiosGeoData = municipios;

        g.selectAll("path.departamento")
            .data(departamentos.features)
            .enter()
            .append("path")
            .attr("class", "departamento")
            .attr("d", path)
            .attr("fill", "#e0e0e0")
            .attr("stroke", "#000000")
            .attr("stroke-width", 1);

        g.selectAll("path.municipio")
            .data(municipios.features)
            .enter()
            .append("path")
            .attr("class", "municipio")
            .attr("d", path)
            .attr("fill", "none")
            .attr("stroke", "#c2c2c2")
            .attr("stroke-width", 0.5);

        // Añadir nombres de los departamentos
        textGroup.selectAll("text.departamento-name")
            .data(departamentos.features)
            .enter()
            .append("text")
            .attr("class", "departamento-name")
            .attr("x", function (d) {
                return projection(d3.geoCentroid(d))[0];
            })
            .attr("y", function (d) {
                return projection(d3.geoCentroid(d))[1];
            })
            .attr("font-size", "8px") // Tamaño más pequeño
            .attr("fill", "#888888") // Color más claro
            .attr("text-anchor", "middle")
            .text(function (d) {
                return d.properties.DPTO_CNMBR;
            });

        // Añadir íconos de inspección y nombres de municipios con inspecciones después de cargar los municipios
        if (window.municipiosGeoData && window.municipiosConInspecciones) {
            addInspectionPoints();
        } else {
            console.log('Los datos de municipios o inspecciones no están disponibles.');
        }
    }).catch(function (error) {
        console.error("Error al cargar el archivo TopoJSON:", error);
    });

    function normalizeName(name) {
        return name.trim().toUpperCase();
    }

    function addInspectionPoints() {
        console.log("Ejecutando addInspectionPoints");

        if (window.municipiosConInspecciones) {
            console.log('Municipios con inspecciones:', window.municipiosConInspecciones);

            window.municipiosConInspecciones.forEach(function (municipio) {
                var found = false;
                var normalizedMunicipio = normalizeName(municipio);

                g.selectAll("path.municipio").each(function (d) {
                    var dMunicipio = normalizeName(d.properties.MPIO_CNMBR);

                    if (dMunicipio === normalizedMunicipio) {
                        found = true;
                        var lat = d.properties.LATITUD;
                        var lng = d.properties.LONGITUD;

                        console.log('Municipio:', municipio);
                        console.log('Coordenadas:', [lat, lng]);

                        g.append("image")
                            .attr("xlink:href", "/location-icon.svg") // Ruta al archivo SVG del ícono
                            .attr("x", projection([lng, lat])[0] - 12) // Ajusta la posición según el tamaño del ícono
                            .attr("y", projection([lng, lat])[1] - 24) // Ajusta la posición según el tamaño del ícono
                            .attr("width", 24) // Ajusta el tamaño del ícono
                            .attr("height", 24) // Ajusta el tamaño del ícono
                            .attr("class", "inspection-point");

                        console.log('Ícono añadido para:', municipio, [lat, lng]);
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
});
