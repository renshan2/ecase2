var stepLabels = {
    0: '',
    1: 'Intake',
    2: 'Investigation',
    3: 'Referral',
    4: 'Coordination',
    5: 'Action',
    6: 'Closed',
    7: 'Agreement',
    8: 'Court'
};

var investigatorData = [];
var caseData = [];
var caseDataFill = [];

function buildCaseArrays() {
    for (var i = 0; i < allCaseData.length; i++) {
        var caseStatus = allCaseData[i].status;
        var caseStatusRemaining = 8 - caseStatus;
        var caseItem = { name: allCaseData[i].name, y: caseStatus, status: allCaseData[i].statustext, step: allCaseData[i].steptext, color: allCaseData[i].barcolor, url: allCaseData[i].url };
        var caseItemFill = { name: '', y: caseStatusRemaining, status: '', step: '', color: allCaseData[i].fillcolor, url: '#' };
        var investigatorItem = allCaseData[i].investigator;
        caseData.push(caseItem);
        caseDataFill.push(caseItemFill);
        investigatorData.push(investigatorItem);
    }

    drawCaseCharts();
}

function drawCaseCharts() {
    var ecaseStatusChart = new Highcharts.Chart({
        chart: {
            renderTo: "ecase-status-chart",
            type: 'bar',
            spacingTop: 30,
            spacingRight: 30,
            spacingLeft: 15,
            spacingBottom: 20
        },

        title: {
            text: '.',
            style: {
                color: '#ffffff'
            }
        },

        legend: {
            enabled: false
        },

        credits: {
            enabled: false
        },

        xAxis: {            
            categories: investigatorData
        },

        yAxis: {
            min: 0,
            max: 8,
            offset: 15,
            showFirstLabel: false,
            minTickInterval: 1,
            opposite: true,
            title: '',
            labels: {
                align: 'right',
                formatter: function () {
                    var value = stepLabels[this.value];
                    if (value !== 'undefined') {                        
                        return '<div class="step"><div class="step-number">' + this.value + '</div><div title="' + value + '" class="step-text">' + value + '</div></div>';
                    }
                    else {
                        return '<div class="step"><div class="step-number">' + this.value + '</div><div title="' + this.value + '" class="step-text"> </div></div>';
                    }
                },
                useHTML: true
            }
        },

        tooltip: {
            formatter: function () {
                return '<b>' + this.point.name + '</b><br/><b>Status: ' + this.point.status + '</b><br/><b>Step: ' + this.point.step + '</b>';
            }
        },

        plotOptions: {
            series: {
                dataLabels: {
                    enabled: true,
                    align: 'left',
                    color: '#FFFFFF',
                    x: 5,
                    formatter: function () {
                        if (this.series.name == 'Status') {
                            return this.point.name + ' (Status: ' + this.point.status + ')';
                        }
                        else {
                            return '';
                        }
                    }
                },
                groupPadding: 0,
                shadow: false,
                stacking: 'normal',
                cursor: 'pointer',
                point: {
                    events: {
                        click: function () {
                            location.href = this.options.url;
                        }
                    }
                }
            }
        },

        series: [{
            data: caseDataFill,
            enableMouseTracking: false
        }, {
            name: 'Status',
            data: caseData
        }]
    });
}

function drawPieChart() {
    var pieChart = new Highcharts.Chart({
        chart: {
            renderTo: 'ecase-pie-chart',
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
            text: 'Number of Cases by Bureau',
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
                showInLegend: true,
                dataLabels: {
                    enabled: false
                },
                events: {
                    click: function (event) {
                        location.href = 'managerdashboard2.aspx';
                    }
                }                
            }
        },
        credits: {
            enabled: false
        },
        series: [{
            type: 'pie',
            name: 'Number of Cases by Bureau',
            data: pieData
        }]
    });

}