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

    var projection = d3.geoMercator()
        .scale(2500)
        .center([-73.0721, 4.0110])
        .translate([width / 2, height / 2]);

    var path = d3.geoPath().projection(projection);

    d3.json("/js/municipios.json").then(function (data) {
        var departamentos = topojson.feature(data, data.objects.MGN_ANM_DPTOS);
        var municipios = topojson.feature(data, data.objects.MGN_ANM_MPIOS);

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
            .attr("font-size", "8px")
            .attr("fill", "#888888")
            .attr("text-anchor", "middle")
            .text(function (d) {
                return d.properties.DPTO_CNMBR;
            });

        if (window.municipiosConInspecciones && window.inspecciones) {
            addInspectionPoints();
        }
        else if (window.municipiosConIncidentes && window.incidentes) {
            addIncidentPoints();
        }
        else {
            console.log('Los datos de municipios o inspecciones no están disponibles.');
        }

        var filterEstado = document.getElementById('filterEstado');
        filterEstado.addEventListener('change', function () {
            var selectedEstado = this.value;
            updateInspectionPoints(selectedEstado);
        });
    }).catch(function (error) {
        console.error("Error al cargar el archivo TopoJSON:", error);
    });

    function normalizeName(name) {
        return name.trim().toUpperCase();
    }

    function addInspectionPoints() {
        console.log("Ejecutando addInspectionPoints");

        if (window.inspecciones) {
            console.log('Inspecciones:', window.inspecciones);

            window.inspecciones.forEach(function (inspeccion) {
                var found = false;
                var normalizedMunicipio = normalizeName(inspeccion.municipio);

                g.selectAll("path.municipio").each(function (d) {
                    var dMunicipio = normalizeName(d.properties.MPIO_CNMBR);

                    if (dMunicipio === normalizedMunicipio) {
                        found = true;
                        var centroid = d3.geoCentroid(d);
                        var lat = centroid[1];
                        var lng = centroid[0];

                        console.log('Municipio:', inspeccion.municipio);
                        console.log('Coordenadas:', [lat, lng]);

                        var iconColor;
                        switch (inspeccion.estado) {
                            case 0:
                                iconColor = '#ffc107'; // Pendiente de aprobacion
                                break;
                            case 1:
                                iconColor = '#fd7e14'; // En proceso
                                break;
                            case 2:
                                iconColor = '#17a2b8'; // Finalizada
                                break;
                            case 3:
                                iconColor = '#17a2b8'; // Programada
                                break;
                            case 4:
                                iconColor = '#dc3545'; // Sin responder
                                break;
                            case 5:
                                iconColor = '#28a745'; // Aprobada
                                break;
                            case 6:
                                iconColor = '#6610f2'; // Desaprobada
                                break;
                            default:
                                iconColor = '#6c757d'; // Gris personalizado
                                break;
                        }

                        var iconSvg = `
                            <svg class="icon" viewBox="0 0 64 64" width="24" height="24">
                                <path d="M42.138,23.162c0-5.566-4.548-10.094-10.138-10.094s-10.138,4.528-10.138,10.094S26.41,33.256,32,33.256   S42.138,28.728,42.138,23.162z" fill="${iconColor}"/>
                                <path d="M31.995,63.996l4.109-5.375c4.289-5.678,18.282-25.024,18.282-35.601C54.387,9.253,45.391,0.004,32,0.004   S9.613,9.253,9.613,23.021c0,11.39,16.432,33.166,18.301,35.605L31.995,63.996z M17.862,23.162c0-7.771,6.342-14.094,14.138-14.094   s14.138,6.323,14.138,14.094S39.796,37.256,32,37.256S17.862,30.934,17.862,23.162z" fill="${iconColor}"/>
                            </svg>`;

                        g.append("g")
                            .attr("transform", `translate(${projection([lng, lat])[0] - 12}, ${projection([lng, lat])[1] - 12})`)
                            .html(iconSvg);
                    }
                });

                if (!found) {
                    console.log('Municipio no encontrado:', inspeccion.municipio);
                }
            });
        } else {
            console.log('No se encontraron inspecciones.');
        }
    }

    function addIncidentPoints() {
        console.log("Ejecutando addIncidentPoints");

        if (window.incidentes) {
            console.log('Incidentes:', window.incidentes);

            window.incidentes.forEach(function (incidente) {
                var found = false;
                var normalizedMunicipio = normalizeName(incidente.municipio);

                g.selectAll("path.municipio").each(function (d) {
                    var dMunicipio = normalizeName(d.properties.MPIO_CNMBR);

                    if (dMunicipio === normalizedMunicipio) {
                        found = true;
                        var centroid = d3.geoCentroid(d);
                        var lat = centroid[1];
                        var lng = centroid[0];

                        console.log('Municipio:', incidente.municipio);
                        console.log('Coordenadas:', [lat, lng]);

                        var iconColor;
                        switch (incidente.estado) {
                            case 0:
                                iconColor = '#dc3545'; // Pendiente de aprobacion
                                break;
                            case 1:
                                iconColor = '#ffc107'; // En proceso
                                break;
                            case 2:
                                iconColor = '#28a745'; // Finalizada
                                break;
                            default:
                                iconColor = '#6c757d'; // Gris personalizado
                                break;
                        }

                        var iconSvg = `
                            <svg class="icon" viewBox="0 0 64 64" width="24" height="24">
                                <path d="M42.138,23.162c0-5.566-4.548-10.094-10.138-10.094s-10.138,4.528-10.138,10.094S26.41,33.256,32,33.256   S42.138,28.728,42.138,23.162z" fill="${iconColor}"/>
                                <path d="M31.995,63.996l4.109-5.375c4.289-5.678,18.282-25.024,18.282-35.601C54.387,9.253,45.391,0.004,32,0.004   S9.613,9.253,9.613,23.021c0,11.39,16.432,33.166,18.301,35.605L31.995,63.996z M17.862,23.162c0-7.771,6.342-14.094,14.138-14.094   s14.138,6.323,14.138,14.094S39.796,37.256,32,37.256S17.862,30.934,17.862,23.162z" fill="${iconColor}"/>
                            </svg>`;

                        g.append("g")
                            .attr("transform", `translate(${projection([lng, lat])[0] - 12}, ${projection([lng, lat])[1] - 12})`)
                            .html(iconSvg);
                    }
                });

                if (!found) {
                    console.log('Municipio no encontrado:', incidente.municipio);
                }
            });
        } else {
            console.log('No se encontraron incidentes.');
        }
    }
    function updateInspectionPoints(selectedEstado) {
        console.log("Actualizando puntos de inspección para el estado:", selectedEstado);

        if (window.inspecciones) {
            g.selectAll("g").remove(); // Elimina los puntos anteriores

            window.inspecciones.forEach(function (inspeccion) {
                if (inspeccion.estado == selectedEstado) {
                    var found = false;
                    var normalizedMunicipio = normalizeName(inspeccion.municipio);

                    g.selectAll("path.municipio").each(function (d) {
                        var dMunicipio = normalizeName(d.properties.MPIO_CNMBR);

                        if (dMunicipio === normalizedMunicipio) {
                            found = true;
                            var centroid = d3.geoCentroid(d);
                            var lat = centroid[1];
                            var lng = centroid[0];

                            var iconColor;
                            switch (inspeccion.estado) {
                                case 0:
                                    iconColor = '#ffc107'; // Pendiente de aprobacion
                                    break;
                                case 1:
                                    iconColor = '#fd7e14'; // En proceso
                                    break;
                                case 2:
                                    iconColor = '#17a2b8'; // Finalizada
                                    break;
                                case 3:
                                    iconColor = '#17a2b8'; // Programada
                                    break;
                                case 4:
                                    iconColor = '#dc3545'; // Sin responder
                                    break;
                                case 5:
                                    iconColor = '#28a745'; // Aprobada
                                    break;
                                case 6:
                                    iconColor = '#dc3545'; // Desaprobada
                                    break;
                                default:
                                    iconColor = '#6c757d'; // Gris personalizado
                                    break;
                            }

                            var iconSvg = `
                                <svg class="icon" viewBox="0 0 64 64" width="24" height="24">
                                    <path d="M42.138,23.162c0-5.566-4.548-10.094-10.138-10.094s-10.138,4.528-10.138,10.094S26.41,33.256,32,33.256   S42.138,28.728,42.138,23.162z" fill="${iconColor}"/>
                                    <path d="M31.995,63.996l4.109-5.375c4.289-5.678,18.282-25.024,18.282-35.601C54.387,9.253,45.391,0.004,32,0.004   S9.613,9.253,9.613,23.021c0,11.39,16.432,33.166,18.301,35.605L31.995,63.996z M17.862,23.162c0-7.771,6.342-14.094,14.138-14.094   s14.138,6.323,14.138,14.094S39.796,37.256,32,37.256S17.862,30.934,17.862,23.162z" fill="${iconColor}"/>
                                </svg>`;

                            g.append("g")
                                .attr("transform", `translate(${projection([lng, lat])[0] - 12}, ${projection([lng, lat])[1] - 12})`)
                                .html(iconSvg);
                        }
                    });

                    if (!found) {
                        console.log('Municipio no encontrado:', inspeccion.municipio);
                    }
                }
            });
        } else {
            console.log('No se encontraron inspecciones.');
        }
    }

    // Manejadores de eventos para los botones de zoom
    document.getElementById('zoom-in').addEventListener('click', function () {
        zoom.scaleBy(svg.transition().duration(500), 1.2);
    });

    document.getElementById('zoom-out').addEventListener('click', function () {
        zoom.scaleBy(svg.transition().duration(500), 0.8);
    });

    document.getElementById('zoom-reset').addEventListener('click', function () {
        svg.transition().duration(500).call(zoom.transform, d3.zoomIdentity);
    });
});
