var wbcTotalData;
var wbcChannels;
var wbcNomenclature;

var defaultChannel1;
var defaultChannel2;
var defaultChannel3;

var defaultGate1;
var defaultGate2;
var defaultGate3;

var defaultGatePolygons;
var customGatePolygons;
var currGatePolygon;
var currGateName;

var isDefaultGate = false;
var isDynamicGate = false;

$(document).ready(function () {
    defaultGatePolygons = [];

    let wbc_table = $('#fcs-table').dataTable({
        processing: true, // for show progress bar
        serverSide: true, // for process server side
        filter: true, // this is for disable filter (search box)
        orderMulti: false, // for disable multiple column at once
        scrollX: true,
        scrollY: "30vh",
        scrollCollapse: true,
        ajax: {
            url: "/FCS/LoadWbcs",
            type: "POST",
            datatype: "json"
        },
        columnDefs:
            [{
                targets: [0],
                visible: false,
                searchable: false
            },
            {
                targets: [-1],
                sortable: false
            }],
        columns: [
            { "data": "id", "name": "id" },
            { "data": "fcs_name", "name": "fcs_name" },
            { "data": "createdAt", "name": "createdAt" },
            { "data": "updatedAt", "name": "updatedAt" },
            {
                data: null, render: function (data, type, row) {
                    return "<a href='#' class='btn btn-danger' onclick=DeleteData('" + row.id + "'); >Delete</a>";
                }
            },
        ],
    });

    $('#fcs-table tbody').on("click", "tr", function () {
        const tr = $(this);
        const isSelected = tr.hasClass("selected");
        if (!isSelected) {
            $("#fcs-table tbody tr").removeClass("selected");
            tr.addClass("selected");
            const data = wbc_table.fnGetData(tr);
            LoadWbcData(data.id);
        }
    });
    $('#fcs-table tbody td:last-child').click(function (event) {
        event.preventDefault();
    });
    
    $(".wbc-items").prop("disabled", true);

    $(".btn-gate").on("click", function () {
        let gateDiv = $("#custom-gates");
        if (isDefaultGate) {
            gateDiv = $("#default-gates");
        }
        gateDiv.find(".active").removeClass("active");
        $(this).addClass("active");

        currGateName = $(this).data("gate");
        UpdateChart();
    });
});

function DeleteData(wbcId) {
    if (confirm("Are you sure you want to delete this wbc data?")) {
        Delete(wbcId);
    }
    else {
        return false;
    }
}

function Delete(wbcId) {
    const url = "/FileManager/Delete";

    $.post(url, { id: wbcId }, function (data) {
        if (data) {
            oTable = $('#fcs-table').DataTable();
            oTable.draw();
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}

function LoadWbcData(wbcId) {
    let url = "/FCS/LoadWbcData";
    $.post(url, { wbcId: wbcId }, function (data) {
        if (data) {
            wbcTotalData = data;
            initWbc();
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}

function DrawHeatmap() {

    if ($("#draw-heatmap").prop("checked")) {
        let left = chartGraph.chartArea.left;
        let top = chartGraph.chartArea.top;
        let width = chartGraph.chartArea.right - chartGraph.chartArea.left;
        let height = chartGraph.chartArea.bottom - chartGraph.chartArea.top;

        $("#fcs-chart").css("background", 'url("' + wbcTotalData.heatmapFile + '") ' + left + 'px ' + top + 'px / ' + width + 'px ' + height + 'px no-repeat white');
    } else {
        $("#fcs-chart").css("background", '');
    }
}

function initWbc() {
    if (!wbcTotalData) {
        return;
    }
    $(".wbc-items").prop("disabled", false);
    wbcNomenclature = wbcTotalData.nomenclature;
    wbcChannels = [];
    $(".wbc-channels").empty();
    for (let key in wbcTotalData.wbcData) {
        wbcChannels.push(key);
        $(".wbc-channels").append("<option>" + key + "</option>");
    }

    defaultChannel1 = GetChannelName("FCS1peak", wbcNomenclature);
    defaultChannel2 = GetChannelName("SSCpeak", wbcNomenclature);
    defaultChannel3 = GetChannelName("FCS1area", wbcNomenclature);

    defaultGatePolygons["defaultGate1"] = {
        channel1: defaultChannel1,
        channel2: defaultChannel2,
        polys: wbcTotalData.gate1Polygon
    };
    defaultGatePolygons["defaultGate2"] = {
        channel1: defaultChannel3,
        channel2: defaultChannel1,
        polys: wbcTotalData.gate2Polygon
    };
    defaultGatePolygons["defaultGate3"] = {
        channel1: defaultChannel1,
        channel2: defaultChannel2,
        polys: wbcTotalData.gate3Polygon
    };

    $("#channel-1").val(defaultChannel1);
    $("#channel-2").val(defaultChannel2);

    ChangeChannel();
}

function ChangeChannel() {
    let channel1 = $("#channel-1").val();
    let channel2 = $("#channel-2").val();
    if (channel1 == channel2) {
        $(".channel-items").prop("disabled", true);
        alert("Please choose different channels!.");
        return;
    }
    $(".channel-items").prop("disabled", false);

    if (channel1 == defaultChannel1 && channel2 == defaultChannel2) {
        $("#draw-heatmap").prop("disabled", false);
    } else {
        $("#draw-heatmap").prop("disabled", true);
        $("#draw-heatmap").prop("checked", false);
    }
    UpdateChart();
}

function UpdateChart() {
    currGatePolygon = defaultGatePolygons[currGateName];

    let channel1 = currGatePolygon.channel1;
    let channel2 = currGatePolygon.channel2;

    $("#channel-1").val(channel1);
    $("#channel-2").val(channel2);

    chartData = [];
    if (!$("#draw-heatmap").prop("checked")) {
        chartData[0] = {
            label: 'Inside Gate' + currGateName,
            backgroundColor: 'rgb(132, 99, 255)',
            borderColor: 'rgb(132, 99, 255)',
            data: FilterGateData(true),
            order: 1,
            radius: 1
        };
        chartData[1] = {
            label: 'Outside Gate' + currGateName,
            backgroundColor: 'rgb(255, 99, 132)',
            borderColor: 'rgb(255, 99, 132)',
            data: FilterGateData(false),
            order: 2,
            radius: 1
        };
/*        chartData[1] = {
            label: 'Original Data Points',
            borderColor: 'rgb(132, 99, 255)',
            fill: false,
            data: [
                { x: 4300, y: 4000},
                { x: 2200, y: 4000},
                { x: 1500, y: 5500},
                { x: 2000, y: 14000},
                { x: 8000, y: 14000},
                { x: 8000, y: 9000},
                { x: 4300, y: 4000}
            ],
            pointHitRadius: 10,
            dragable: true,
            type: 'line',
            pointRadius: 5,
            lineTension: 0,
            order: 2
        }
*/    }
    chartData = chartData.concat(GetChartGateLineData());
    chartGraph.data.datasets = chartData;
    chartGraph.update();
    DrawHeatmap();
}

function DefaultGate() {
    isDefaultGate = true;
    $("#tab-custom").removeClass("show");
    $("#tab-default").addClass("show")
    $(".custom-gate-div").hide();
    $(".default-gate-div").show();
    $(".wbc-channels").prop("disabled", true);

    UpdateChart();
}

function CustomeGate() {
    isDefaultGate = false;
    $("#tab-custom").addClass("show");
    $("#tab-default").removeClass("show")
    $(".custom-gate-div").show();
    $(".default-gate-div").hide();
    $(".wbc-channels").prop("disabled", false);
}

function ChangeDynamic(isDynamic) {
    isDynamicGate = isDynamic;
}

function FilterGateData(isInside) {
    if (!currGatePolygon) {
        return [];
    }

    let channel1 = currGatePolygon.channel1;
    let channel2 = currGatePolygon.channel2;

    let xys = wbcTotalData.wbcData[channel1].data.map((v, i) => ({
        x: wbcTotalData.wbcData[channel1].data[i],
        y: wbcTotalData.wbcData[channel2].data[i]
    }));
    return xys.filter(xy => IsInsidePolygons(xy) == isInside);
}

function IsInsidePolygons(xy) {
    let i = 0;
    for (i = 0; i < currGatePolygon.polys.length; i++) {
        if (!IsInsidePoly(currGatePolygon.polys[i], xy.x, xy.y)) {
            return false;
        }
    }
    return true;
}

function IsInsidePoly(poly, x, y)
{
    if (!poly)
        return false;
    let minX = poly[0].x;
    let maxX = poly[0].x;
    let minY = poly[0].y;
    let maxY = poly[0].y;
    let i = 1;

    for (i = 1; i < poly.length; i++)
    {
        minX = Math.min(poly[i].x, minX);
        maxX = Math.max(poly[i].x, maxX);
        minY = Math.min(poly[i].y, minY);
        maxY = Math.max(poly[i].y, maxY);
    }

    if (x < minX || x > maxX || y < minY || y > maxY) {
        return false;
    }

    // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
    let inside = false;
    for (i = 0, j = poly.length - 1; i < poly.length; j = i++)
    {
        if ((poly[i].y > y) != (poly[j].y > y) &&
            x < (poly[j].x - poly[i].x) * (y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x) {
            inside = !inside;
        }
    }
    return inside;
}

function GetChartGateLineData() {
    if (currGateName == "finalGate") {
        return [];
    }

    let order = chartData.length;

    return currGatePolygon.polys.map((v, i) => ({
        label: "Gate - " + currGateName,
        borderColor: 'rgb(100, 200, 100)',
        fill: false,
        data: v,
        pointHitRadius: 10,
        dragable: !isDefaultGate,
        type: 'line',
        pointRadius: 5,
        pointHoverRadius: 10,
        lineTension: 0,
        order: order + i
    }));
}