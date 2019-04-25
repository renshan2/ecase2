
function renderPieChart(numPieItems, pieChartContainer, pieContainerHeight, pieTitle, pieData) {	
	if (numPieItems > 0) {
		$('#' + pieChartContainer).height(pieContainerHeight);
		drawPieChart(pieChartContainer, pieTitle, pieData);				
	}  
	else {
		$('#' + pieChartContainer).html('<p style="text-align: center;"><br/>No data found.</p>');
	}
}

function drawPieChart(chartContainer, chartTitle, pieData) {
	// Build the chart
	var pieChart = new Highcharts.Chart({
			chart: {
				renderTo: chartContainer,
				plotBackgroundColor: null,
				plotBorderWidth: null,
				plotShadow: false,
				marginTop: 0,
				marginLeft: 0,
				marginRight: 0,
				spacingTop: 5,
				spacingLeft: 0,
				spacingRight: 0
			},
			legend: {
				layout: 'vertical',
				maxHeight: '100px',
				margin: 5,
				labelFormatter: function () {
					return this.name + ': ' + this.y;
				}
			},
			title: {
				text: chartTitle,
				style: {
					color: '#0959a2',
					fontWeight: 'normal',
					fontFamily: 'Georgia',
					fontSize: '13px'
				}
			},
			tooltip: {
				formatter: function () {
					return '<b>' + this.point.name + ': ' + this.point.y + '</b>';
				}
			},
			plotOptions: {
				pie: {
					allowPointSelect: true,
					cursor: 'pointer',
					dataLabels: {
						enabled: false
					},
					showInLegend: true
				}
			},
			credits: {
				enabled: false
			},
			series: [{
				type: 'pie',
				name: chartTitle,
				data: pieData
			}]
		});
}