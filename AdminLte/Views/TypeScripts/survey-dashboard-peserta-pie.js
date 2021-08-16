am4core.useTheme(am4themes_animated);

var chart = am4core.create("chart-peserta", am4charts.PieChart);

chart.data = JSON.parse($("#pie-peserta-value").val());

var colorSet = new am4core.ColorSet();
colorSet.list = ["#4DB749", "#175EAC", "#EC1F24"].map(function (color) {
	return new am4core.color(color);
});

var pieSeries = chart.series.push(new am4charts.PieSeries());
pieSeries.dataFields.value = "value";
pieSeries.dataFields.category = "key";
pieSeries.colors = colorSet;