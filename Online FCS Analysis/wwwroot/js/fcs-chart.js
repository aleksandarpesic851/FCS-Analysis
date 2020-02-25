var data = [];
var orgData = [
	{
		"category": "Immunooncology",
		"company": "Kite Pharma",
		"employees": 447,
		"revenue": 32.1,
		"rev_employee": 0.072
	},
	{
		"category": "Immunooncology",
		"company": "Inovio",
		"employees": 239,
		"revenue": 6.3,
		"rev_employee": 0.026
	},
	{
		"category": "Immunooncology",
		"company": "Immune Design",
		"employees": 50,
		"revenue": 2.2,
		"rev_employee": 0.044
	},
	{
		"category": "Immunooncology",
		"company": "Blue Bird Bio",
		"employees": 764,
		"revenue": 53.9,
		"rev_employee": 0.071
	}
];

$(document).ready(function () {
    initChart();
});

function initChart() {
	Chart.defaults.global.tooltips.custom = function (tooltipModel) {
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
		tooltipEl.innerHTML = getTooltipContent(tooltipModel.dataPoints[0].index);

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

	updateDisplayData();

	new Chart(ctx, {
		// The type of chart we want to create
		type: 'bubble',

		// The data for our dataset
		data: {
			labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
			datasets: [{
				label: 'My First dataset',
				backgroundColor: 'rgb(255, 99, 132)',
				borderColor: 'rgb(255, 99, 132)',
				data: data
			}]
		},

		// Configuration options go here
		options: {
			responsive: true,
			legend: {
				display: false
			},
			responsiveAnimationDuration: 200,
			tooltips: {
				enabled: false,
			}
		}
	});
}

function updateDisplayData() {
	data = [];
	orgData.forEach(function (item, index) {
		data.push(item);
		data[index].x = item.employees;
		data[index].y = item.revenue;
		data[index].r = item.rev_employee * 100;
	});
}

function getTooltipContent(idx) {
	var innerHtml = "";
	innerHtml += "<b> Company: </b>" + orgData[idx].company + " <br> ";
	innerHtml += "<b> Employees: </b>" + orgData[idx].employees + " <br> ";
	innerHtml += "<b> Revenue: </b>" + orgData[idx].revenue + " <br> ";
	innerHtml += "<b> Rev/Employee: </b>" + orgData[idx].rev_employee;
	return innerHtml;
}