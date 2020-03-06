var chartRBCData = [];
var chartWBCData = [];
var chartVData = [];
var chartHCData = [];

var chartRBCGraph;
var chartWBCGraph;
var chartVGraph;
var chartHCGraph;

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