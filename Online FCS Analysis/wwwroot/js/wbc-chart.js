var chartData = [];
var chartGraph;
var lineNormalColor = 'rgb(100, 200, 100)';
var lineHighlightColor = 'rgb(250, 20, 20)';

$(document).ready(function () {
	initChart();
});

function initChart() {
	Chart.defaults.global.tooltips.custom = function (tooltipModel) {
		if (isGateEditing) {
			return;
		}
		// Tooltip Element
		var tooltipEl = document.getElementById('chartjs-tooltip');

		// Create element on first render
		if (!tooltipEl) {
			tooltipEl = document.createElement('div');
			tooltipEl.id = 'chartjs-tooltip';
			tooltipEl.innerHTML = '<table></table>';
			document.body.appendChild(tooltipEl);
		}

		// Hide if no tooltip
		if (tooltipModel.opacity === 0) {
			tooltipEl.style.opacity = 0;
			return;
		}

		// Set caret Position
		tooltipEl.classList.remove('above', 'below', 'no-transform');
		if (tooltipModel.yAlign) {
			tooltipEl.classList.add(tooltipModel.yAlign);
		} else {
			tooltipEl.classList.add('no-transform');
		}

		// `this` will be the overall tooltip
		var position = this._chart.canvas.getBoundingClientRect();
		tooltipEl.innerHTML = getTooltipContent(tooltipModel.dataPoints[0].datasetIndex, tooltipModel.dataPoints[0].index);

		// Display, position, and set styles for font
		tooltipEl.style.opacity = 1;
		tooltipEl.style.left = position.left + window.pageXOffset + tooltipModel.caretX + 'px';
		tooltipEl.style.top = position.top + window.pageYOffset + tooltipModel.caretY + 'px';
		tooltipEl.style.padding = tooltipModel.yPadding + 'px ' + tooltipModel.xPadding + 'px';
		tooltipEl.style.fontFamily = tooltipModel._bodyFontFamily;
		tooltipEl.style.fontSize = tooltipModel.bodyFontSize + 'px';
		tooltipEl.style.fontStyle = tooltipModel._bodyFontStyle;
	};

	const ctx = document.getElementById('fcs-chart').getContext('2d');

	chartGraph = new Chart(ctx, {
		// The type of chart we want to create
		type: 'bubble',

		// The data for our dataset
		data: {
			datasets: chartData
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
				enabled: false,
			},
			onClick: function (event, array) {
				if (event.cancelable) {
					event.preventDefault();
				}
				if (isGateEditing && chartData.length > 1) {
					let newXY = getCoordinate(event);
					AddOrMoveCustomPoints(newXY);
				}
			},
			dragData: true,
			dragX: true,
			onDragStart: function (e, target) {
				if (e.cancelable) {
					e.preventDefault();
				}
				if (!chartData[target._datasetIndex].dragable || !isGateEditing)
					return false;
				choosenPolygon = target._datasetIndex;
				choosenPoint = target._index;
				HighlightLine(choosenPolygon);
			},
			/*onDrag: function (e, datasetIndex, index, value) {
				// do something
			},
			onDragEnd: function (e, datasetIndex, index, value) {
				// do something
			}*/
		}
	});
}

function getCoordinate(event) {
	var yTop = chartGraph.chartArea.top;
	var yBottom = chartGraph.chartArea.bottom;

	var yMin = chartGraph.scales['y-axis-0'].min;
	var yMax = chartGraph.scales['y-axis-0'].max;
	var newY = 0;

	if (event.offsetY <= yBottom && event.offsetY >= yTop) {
		newY = Math.abs((event.offsetY - yTop) / (yBottom - yTop));
		newY = (newY - 1) * -1;
		newY = newY * (Math.abs(yMax - yMin)) + yMin;
	};

	var xTop = chartGraph.chartArea.left;
	var xBottom = chartGraph.chartArea.right;
	var xMin = chartGraph.scales['x-axis-0'].min;
	var xMax = chartGraph.scales['x-axis-0'].max;
	var newX = 0;

	if (event.offsetX <= xBottom && event.offsetX >= xTop) {
		newX = Math.abs((event.offsetX - xTop) / (xBottom - xTop));
		newX = newX * (Math.abs(xMax - xMin)) + xMin;
	};

	return { x: newX, y: newY };
}

function getTooltipContent(datasetIdx, idx) {
	var innerHtml = "";
	innerHtml += "<strong> Category: </strong>" + chartData[datasetIdx].label + " <br> ";
	innerHtml += "<strong> " + $("#channel-1").val() + ": </strong>" + chartData[datasetIdx].data[idx].x + " <br> ";
	innerHtml += "<strong> " + $("#channel-2").val() + ": </strong>" + chartData[datasetIdx].data[idx].y + " <br> ";
	return innerHtml;
}

function GetChannelName(channelHandle, type)
{
	let channelName = "";
	if (type == "old_names") {
		if (channelHandle == "FCS1peak")
			channelName = "FSC1LG,Peak"; //string FSC1peak = "FSC1LG,Peak"; 
		else if (channelHandle == "SSCpeak")
			channelName = "SSCLG,Peak"; //string SSCpeak = "SSCLG,Peak"; 
		else if (channelHandle == "FCS1area")
			channelName = "FSC1LG,Area"; //string FCS1area = "FSC1LG,Area"; 
		else if (channelHandle == "SSCarea")
			channelName = "SSCLG,Area"; //string SSCarea = "SSCLG,Area";
		else if (channelHandle == "FSC2peak")
			channelName = "FSC2HG,Peak";
		else if (channelHandle == "FLpeak")
			channelName = "FLLG,Peak"; //"FLLG,Peak"
	}
	else if (type == "middleaged_names") {
		if (channelHandle == "FCS1peak")
			channelName = "BS1CH1; fsc1lg-H";
		else if (channelHandle == "SSCpeak")
			channelName = "BS1CH2; ssclg-H";
		else if (channelHandle == "FCS1area")
			channelName = "BS1CH1; fsc1lg-A";
		else if (channelHandle == "SSCarea")
			channelName = "BS1CH4; ssclg-A";
		else if (channelHandle == "FSC2peak")
			channelName = "BS1CH2; fsc2lg-H";
		else if (channelHandle == "FLpeak")
			channelName = "BS1CH3;fllg-H";//BS1CH2; 
	}
	else if (type == "new_names")// NEW names
	{
		if (channelHandle == "FCS1peak")
			channelName = "BS1CH1; fsc1lg-H"; //string FCS1peak = "BS1CH1; fsc1lg-H";
		else if (channelHandle == "SSCpeak")
			channelName = "BS1CH4; ssclg-H"; //string SSCpeak = "BS1CH4; ssclg-H";
		else if (channelHandle == "FCS1area")
			channelName = "BS1CH1; fsc1lg-A"; //string FCS1area = "BS1CH1; fsc1lg-A";
		else if (channelHandle == "SSCarea")
			channelName = "BS1CH4; ssclg-A"; //string SSCarea = "BS1CH4; ssclg-A";
		else if (channelHandle == "FSC2peak")
			channelName = "BS1CH2; fsc2lg-H";
		else if (channelHandle == "FLpeak")
			channelName = "BS1CH3;fllg-H";
	}

	return channelName;
}

// clean highlighted polygon
function CleanHighlight() {
	chartData.forEach(function (v, idx) {
		if (v.type == "line") {
			v.borderColor = lineNormalColor;
		}
	})
}

function HighlightLine(idx) {
	CleanHighlight();
	chartData[idx].borderColor = lineHighlightColor;
	chartGraph.update();
}