var chartAkhlak1Min = 0;
var chartAkhlak1Max = 100;

var dataAkhlak1 = {
    score: 0,
    gradingData: [
        {
            color: "#EC1F24",
            lowScore: 0,
            highScore: 70
        },
        {
            color: "#4DB749",
            lowScore: 71,
            highScore: 100
        }
    ]
};

/**
Grading Lookup
 */
function lookUpGradeAkhlak(lookupScore, grades) {
    // Only change code below this line
    for (var i = 0; i < grades.length; i++) {
        if (
            grades[i].lowScore <= lookupScore &&
            grades[i].highScore >= lookupScore
        ) {
            return grades[i];
        }
    }
    return null;
}

// Themes begin
am4core.useTheme(am4themes_animated);
// Themes end

function createChartAkhlak(chartName, chartValue) {

    // create chart
    var chartAkhlak1 = am4core.create(chartName, am4charts.GaugeChart);
    chartAkhlak1.hiddenState.properties.opacity = 0;
    chartAkhlak1.fontSize = "10pt";
    chartAkhlak1.innerRadius = am4core.percent(80);
    chartAkhlak1.responsive.enabled = true;

    /**
     * Normal axis
     */

    var axisAkhlak1 = chartAkhlak1.xAxes.push(new am4charts.ValueAxis());
    axisAkhlak1.min = chartAkhlak1Min;
    axisAkhlak1.max = chartAkhlak1Max;
    axisAkhlak1.strictMinMax = true;
    axisAkhlak1.renderer.radius = am4core.percent(80);
    axisAkhlak1.renderer.inside = true;
    axisAkhlak1.renderer.line.strokeOpacity = 0.1;
    axisAkhlak1.renderer.ticks.template.disabled = false;
    axisAkhlak1.renderer.ticks.template.strokeOpacity = 1;
    axisAkhlak1.renderer.ticks.template.length = 5;
    axisAkhlak1.renderer.grid.template.disabled = true;
    axisAkhlak1.renderer.labels.template.fontSize = "0.75em";

    /**
     * Axis for ranges
     */

    var axis2Akhlak1 = chartAkhlak1.xAxes.push(new am4charts.ValueAxis());
    axis2Akhlak1.min = chartAkhlak1Min;
    axis2Akhlak1.max = chartAkhlak1Max;
    axis2Akhlak1.strictMinMax = true;
    axis2Akhlak1.renderer.labels.template.disabled = true;
    axis2Akhlak1.renderer.ticks.template.disabled = true;
    axis2Akhlak1.renderer.grid.template.disabled = false;
    axis2Akhlak1.renderer.grid.template.opacity = 0.5;
    axis2Akhlak1.renderer.labels.template.bent = true;

    /**
    Ranges
    */

    for (let grading of dataAkhlak1.gradingData) {
        var range = axis2Akhlak1.axisRanges.create();
        range.axisFill.fill = am4core.color(grading.color);
        range.axisFill.fillOpacity = 0.8;
        range.axisFill.zIndex = -1;
        range.value = grading.lowScore > chartAkhlak1Min ? grading.lowScore : chartAkhlak1Min;
        range.endValue = grading.highScore < chartAkhlak1Max ? grading.highScore : chartAkhlak1Max;
        range.grid.strokeOpacity = 0;
        range.stroke = am4core.color(grading.color).lighten(-0.1);
    }

    var matchingGradeAkhlak = lookUpGradeAkhlak(dataAkhlak1.score, dataAkhlak1.gradingData);

    /**
     * Label 1
     */

    var label1Akhlak1 = chartAkhlak1.radarContainer.createChild(am4core.Label);
    label1Akhlak1.isMeasured = false;
    label1Akhlak1.fontSize = "2em";
    label1Akhlak1.x = am4core.percent(50);
    label1Akhlak1.horizontalCenter = "middle";
    label1Akhlak1.verticalCenter = "bottom";
    //labelAkhlak1.dataItem = dataAkhlak;
    label1Akhlak1.text = dataAkhlak1.score.toFixed(1);
    //labelAkhlak1.text = "{score}";
    label1Akhlak1.fill = am4core.color(matchingGradeAkhlak.color);


    /**
     * Hand
     */

    var handAkhlak1 = chartAkhlak1.hands.push(new am4charts.ClockHand());
    handAkhlak1.axis = axis2Akhlak1;
    handAkhlak1.innerRadius = am4core.percent(45);
    handAkhlak1.startWidth = 8;
    handAkhlak1.pin.disabled = true;
    handAkhlak1.value = dataAkhlak1.score;
    handAkhlak1.fill = am4core.color("#444");
    handAkhlak1.stroke = am4core.color("#000");

    handAkhlak1.events.on("positionchanged", function () {
        label1Akhlak1.text = axis2Akhlak1.positionToValue(handAkhlak1.currentPosition).toFixed(1);
        var value2 = axisAkhlak1.positionToValue(handAkhlak1.currentPosition);
        var matchingGradeAkhlak = lookUpGradeAkhlak(axisAkhlak1.positionToValue(handAkhlak1.currentPosition), dataAkhlak1.gradingData);
        label1Akhlak1.fill = am4core.color(matchingGradeAkhlak.color);
    })

    setTimeout(function () {
        var value = parseFloat($("#" + chartValue).val());
        handAkhlak1.showValue(value, 1000, am4core.ease.cubicOut);
    }, 2000);
}

createChartAkhlak("chart-akhlak1", "gauge-akhlak-value");
//createChartAkhlak("chart-akhlak-LEARNING", "gauge-akhlak-LEARNING");
//createChartAkhlak("chart-akhlak-GROWING", "gauge-akhlak-GROWING");
//createChartAkhlak("chart-akhlak-CONTRIBUTING", "gauge-akhlak-CONTRIBUTING");