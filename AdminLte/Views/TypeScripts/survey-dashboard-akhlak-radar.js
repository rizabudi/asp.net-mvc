am4core.useTheme(am4themes_animated);

var chartAkhlak2 = am4core.create("chart-akhlak2", am4charts.RadarChart);

chartAkhlak2.data = JSON.parse($("#radar-akhlak-value").val());

console.log(chartAkhlak2.data);

chartAkhlak2.padding(10, 10, 10, 10);

var categoryAxisAkhlak = chartAkhlak2.xAxes.push(new am4charts.CategoryAxis());
categoryAxisAkhlak.renderer.grid.template.location = 0;
categoryAxisAkhlak.dataFields.category = "x";
categoryAxisAkhlak.renderer.minGridDistance = 60;

var valueAxisAkhlak = chartAkhlak2.yAxes.push(new am4charts.ValueAxis());

var seriesAkhlak = chartAkhlak2.series.push(new am4charts.RadarSeries());
seriesAkhlak.dataFields.categoryX = "x";
seriesAkhlak.dataFields.valueY = "y";
seriesAkhlak.tooltipText = "{valueY.value}"
seriesAkhlak.fillOpacity = 0.4;

chartAkhlak2.cursor = new am4charts.RadarCursor();