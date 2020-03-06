var chartRBCData = [];
var chartWBCData = [];
var chartVData = [];
var chartHCData = [];

var chartRBCGraph;
var chartWBCGraph;
var chartVGraph;
var chartHCGraph;

var bgColor = [
	"rgba(255, 99, 132, 0.2)",
	"rgba(255, 159, 64, 0.2)",
	"rgba(255, 205, 86, 0.2)",
	"rgba(75, 192, 192, 0.2)",
	"rgba(54, 162, 235, 0.2)",
	"rgba(153, 102, 255, 0.2)",
	"rgba(201, 203, 207, 0.2)"];    // graph bar background color
var bdColor = [
	"rgb(255, 99, 132)",
	"rgb(255, 159, 64)",
	"rgb(255, 205, 86)",
	"rgb(75, 192, 192)",
	"rgb(54, 162, 235)",
	"rgb(153, 102, 255)",
	"rgb(201, 203, 207)"];          // graph bar border color

$(document).ready(function () {
	chartRBCGraph = initChart("rbc-chart", chartRBCData, "bubble");
	chartWBCGraph = initChart("wbc-chart", chartWBCData, "bubble");
	chartVGraph = initChart("v-chart", chartVData,   "bar");
	chartHCGraph = initChart("hc-chart", chartHCData,  "bar");
});

// Initialize chart object.
function initChart(canvas_id, data, type) {
	
	const ctx = document.getElementById(canvas_id).getContext('2d');

	return new Chart(ctx, {
		// The type of chart we want to create
		type: type,

		// The data for our dataset
		data: {
			datasets: data
		},

		// Configuration options go here
		options: {
			responsive: true,
			maintainAspectRatio: false,
			animation: false,
			legend: {
				display: false
			},
			responsiveAnimationDuration: 200,
			tooltips: {
				callbacks: {
					label: function (tooltipItem, data) {
						var label = data.datasets[tooltipItem.datasetIndex].label || '';

						if (label) {
							label += ': ';
						}
						label += '(' + tooltipItem.xLabel + ', ' + tooltipItem.yLabel + ')';
						return label;
					}
				}
			}
		}
	});

}