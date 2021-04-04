am4core.useTheme(am4themes_animated);

var chart = am4core.create("chart-engagement2", am4charts.RadarChart);


chart.data = JSON.parse($("#radar-engagement-value").val());

console.log(chart.data);

chart.padding(10, 10, 10, 10);

var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
categoryAxis.renderer.grid.template.location = 0;
categoryAxis.dataFields.category = "x";
categoryAxis.renderer.minGridDistance = 60;

var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

var series = chart.series.push(new am4charts.RadarSeries());
series.dataFields.categoryX = "x";
series.dataFields.valueY = "y";
series.tooltipText = "{valueY.value}"
series.fillOpacity = 0.4;

chart.cursor = new am4charts.RadarCursor();