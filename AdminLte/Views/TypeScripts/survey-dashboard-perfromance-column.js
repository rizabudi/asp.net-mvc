am4core.useTheme(am4themes_animated);

var chart = am4core.create("chart-performance", am4charts.XYChart);

chart.data = JSON.parse($("#column-performance-value").val());;

chart.padding(40, 40, 40, 40);

chart.colors.list = [
	am4core.color("#4DB749"),
	am4core.color("#175EAC"),
	am4core.color("#EC1F24")
];

var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
categoryAxis.renderer.grid.template.location = 0;
categoryAxis.dataFields.category = "x";
categoryAxis.renderer.minGridDistance = 60;

var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

var series = chart.series.push(new am4charts.ColumnSeries());
series.dataFields.categoryX = "x";
series.dataFields.valueY = "y";
series.tooltipText = "{valueY.value}"
series.columns.template.strokeOpacity = 0;

chart.cursor = new am4charts.XYCursor();

// as by default columns of the same series are of the same color, we add adapter which takes colors from chart.colors color set
series.columns.template.adapter.add("fill", function (fill, target) {
	return chart.colors.getIndex(target.dataItem.index);
});