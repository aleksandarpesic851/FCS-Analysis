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

var Gate3Names = ["Neutrophils", "Monocytes", "Lymphocytes"];
var Gate3Colors = ["rgb(255, 189, 189)", "rgb(173, 233, 255)", "rgb(194, 228, 156)"];

let wbc_table;

$(document).ready(function () {
    defaultGatePolygons = [];

    wbc_table = $('#fcs-table').dataTable({
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

// Draw or remove heatmap
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
    // Draw or remove heatmap
    DrawHeatmap();

    if ($("#draw-heatmap").prop("checked")) {
        $("#channel-1").val(defaultChannel1);
        $("#channel-2").val(defaultChannel2);
        chartData = [
            {
                backgroundColor: 'rgba(0, 0, 0, 0)',
                borderColor: 'rgba(0, 0, 0, 0)',
                data: [{ x: 0, y: 0 }, { x: wbcTotalData.wbcData[defaultChannel1].range, y: wbcTotalData.wbcData[defaultChannel2].range }],
                order: 1,
                radius: 0
            }];

    } else {
        //---------Initialize Components-----//
        let gateDiv = $("#custom-gates");
        if (isDefaultGate) {
            gateDiv = $("#default-gates");
        }
        currGateName = gateDiv.find(".active").data("gate");

        if (currGateName == "finalGate") {
            $("#channel-1").val(defaultChannel1);
            $("#channel-2").val(defaultChannel2);
        } else {
            currGatePolygon = defaultGatePolygons[currGateName];
            $("#channel-1").val(currGatePolygon.channel1);
            $("#channel-2").val(currGatePolygon.channel2);
        }
        
        chartData = [];

        //---------Update Chart-----//

        // Add Points for gates
        if (currGateName == "defaultGate3") {
            chartData = GetDefaultGate3();
        } else if (currGateName == "finalGate") {
            chartData = GetFinalGateData();
        } else {
            chartData = GetPolygonGateData();
        }

        // Add Gate Lines
        if ((currGateName != "defaultGate3" || !isDynamicGate) && currGateName != "finalGate") {
            chartData = chartData.concat(GetChartGateLineData());
        }
    }

    // Update Graph And Redraw
    chartGraph.data.datasets = chartData;
    chartGraph.update();
}


function GetFinalGateData() {
    let selectedGates = [];
    let gateDiv = "#custom-gates";
    if (isDefaultGate) {
        gateDiv = "#default-gates";
    }
    $(gateDiv + ' .final input:checked').each(function () {
        selectedGates.push($(this).data('gate'));
    });

    let gatePolygons = isDefaultGate ? defaultGatePolygons : currGatePolygon;
    let gateRes = [];

    let finalRes = wbcTotalData.wbcData[defaultChannel1].data.map((v, i) => ({
        x: wbcTotalData.wbcData[defaultChannel1].data[i],
        y: wbcTotalData.wbcData[defaultChannel2].data[i],
        isInside: true
    }));

    selectedGates.forEach(function (gateName, idx) {
        let gatePolygon = gatePolygons[gateName];
        let channel1 = gatePolygon.channel1;
        let channel2 = gatePolygon.channel2;

        let xys = wbcTotalData.wbcData[channel1].data.map((v, i) => ({
            x: wbcTotalData.wbcData[channel1].data[i],
            y: wbcTotalData.wbcData[channel2].data[i]
        }));

        xys.forEach(function (xy, idx) {
            xys[idx].isInside = IsInsidePolygons(gatePolygon, xy);
            finalRes[idx].isInside &= xys[idx].isInside;
        });
        gateRes.push({ gateName: gateName, data: xys, polygon: gatePolygon });
        selectedGates[idx] = { gateName: gateName, data: xys };
    });

    let data = [];
    let insideData = finalRes.filter(xy => xy.isInside);
    let outsideData = finalRes.filter(xy => !xy.isInside);

    data[0] = {
        label: 'Inside Gate' + currGateName,
        backgroundColor: 'rgb(132, 99, 255)',
        borderColor: 'rgb(132, 99, 255)',
        data: insideData,
        order: 1,
        radius: 1
    };
    data[1] = {
        label: 'Outside Gate' + currGateName,
        backgroundColor: 'rgb(255, 99, 132)',
        borderColor: 'rgb(255, 99, 132)',
        data: outsideData,
        order: 2,
        radius: 1
    };

    let fcsName = wbc_table.fnGetData($("#fcs-table tbody tr.selected"));
    let detailHtml = "<h4>" + fcsName.fcs_name + "</h4>";
    selectedGates.forEach(function (v) {
        detailHtml += "<br><strong>Gate : </strong>" + v.gateName;
        detailHtml += "<br><strong> &nbsp; &nbsp;Inside : </strong>" + v.data.filter(xy => xy.isInside).length;
        detailHtml += "<br><strong> &nbsp; &nbsp;Outside : </strong>" + v.data.filter(xy => !xy.isInside).length;
    });

    detailHtml += "<br><hr class='mt-2 mb-2 mr-5'><strong>Gate : </strong>" + currGateName;
    detailHtml += "<br><strong> &nbsp; &nbsp;Inside : </strong>" + insideData.length;
    detailHtml += "<br><strong> &nbsp; &nbsp;Outside : </strong>" + outsideData.length;
    $("#fcs-chart-details").html(detailHtml);

    return data;
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
    if (currGateName == "defaultGate3") {
        UpdateChart();
    }
}

// Get Ploygon gate data
function GetPolygonGateData() {
    let insideData = FilterGateData(currGatePolygon, true);
    let outsideData = FilterGateData(currGatePolygon, false);
    let data = [];
    data[0] = {
        label: 'Inside Gate' + currGateName,
        backgroundColor: 'rgb(132, 99, 255)',
        borderColor: 'rgb(132, 99, 255)',
        data: insideData,
        order: 1,
        radius: 1
    };
    data[1] = {
        label: 'Outside Gate' + currGateName,
        backgroundColor: 'rgb(255, 99, 132)',
        borderColor: 'rgb(255, 99, 132)',
        data: outsideData,
        order: 2,
        radius: 1
    };

    let fcsName = wbc_table.fnGetData($("#fcs-table tbody tr.selected"));
    let detailHtml = "<h4>" + fcsName.fcs_name + "</h4>";
    detailHtml += "<br> <strong>Gate : </strong>" + currGateName;
    detailHtml += "<br> <strong> &nbsp; &nbsp;Inside : </strong>" + insideData.length;
    detailHtml += "<br> <strong> &nbsp; &nbsp;Outside : </strong>" + outsideData.length;
    $("#fcs-chart-details").html(detailHtml);
    return data;
}

// Get Gate3 data
function GetDefaultGate3() {
    let data = [];
    let nmlData = [];
    let outsideData = [];

    if (!isDefaultGate) {
        return data;
    }
    if (isDynamicGate) {
        nmlData = [GetDynamicGateData(wbcTotalData.wbc3Cell.wbc_n), GetDynamicGateData(wbcTotalData.wbc3Cell.wbc_m), GetDynamicGateData(wbcTotalData.wbc3Cell.wbc_l)];
        outsideData = GetDynamicGateOutData();
        for (let i = 0; i < 3; i++) {
            data[i] = {
                label: Gate3Names[i],
                backgroundColor: Gate3Colors[i],
                borderColor: Gate3Colors[i],
                data: nmlData[i],
                order: i + 1,
                radius: 1
            };
        }

        data[3] = {
            label: 'Outside Default Gate3',
            backgroundColor: 'rgb(255, 99, 132)',
            borderColor: 'rgb(255, 99, 132)',
            data: outsideData,
            order: 4,
            radius: 1
        };
    } else {
        for (let i = 0; i < 3; i++) {
            nmlData[i] = FilterGate3Data(i);
            data[i] = {
                label: Gate3Names[i],
                backgroundColor: Gate3Colors[i],
                borderColor: Gate3Colors[i],
                data: nmlData[i],
                order: i + 1,
                radius: 1
            };
        }
        outsideData = FilterGateData(currGatePolygon, false);
        data[3] = {
            label: 'Outside Default Gate3',
            backgroundColor: 'rgb(255, 99, 132)',
            borderColor: 'rgb(255, 99, 132)',
            data: outsideData,
            order: 4,
            radius: 1
        };
    }

    let fcsName = wbc_table.fnGetData($("#fcs-table tbody tr.selected"));
    let detailHtml = "<h4>" + fcsName.fcs_name + "</h4>";
    detailHtml += "<br><strong>Gate : </strong>" + currGateName;
    for (let i = 0; i < 3; i++) {
        detailHtml += "<br><strong> &nbsp; &nbsp;" + Gate3Names[i] + ": </strong>" + nmlData[i].length;
    }
    detailHtml += "<br><strong> &nbsp; &nbsp;Outside : </strong>" + outsideData.length;
    $("#fcs-chart-details").html(detailHtml);

    return data;
}

function GetDynamicGateData(Idxs) {
    return Idxs.map((v, i) => ({
        x: wbcTotalData.wbcData[defaultChannel1].data[v],
        y: wbcTotalData.wbcData[defaultChannel2].data[v]
    }));
}

function GetDynamicGateOutData() {
    let i = 0, nCnt = wbcTotalData.wbcData.Count.data.length;
    let data = [];
    for (i = 0; i < nCnt; i++) {
        if (wbcTotalData.wbc3Cell.wbc_n.includes(i)) {
            continue;
        }
        if (wbcTotalData.wbc3Cell.wbc_m.includes(i)) {
            continue;
        }
        if (wbcTotalData.wbc3Cell.wbc_l.includes(i)) {
            continue;
        }
        data.push({
            x: wbcTotalData.wbcData[defaultChannel1].data[i],
            y: wbcTotalData.wbcData[defaultChannel2].data[i]
        });
    }
    return data;
}

// Get Gate3 Fixed Polygon Data
function FilterGate3Data(nIdx) {
    if (!currGatePolygon) {
        return [];
    }

    let channel1 = currGatePolygon.channel1;
    let channel2 = currGatePolygon.channel2;

    let xys = wbcTotalData.wbcData[channel1].data.map((v, i) => ({
        x: wbcTotalData.wbcData[channel1].data[i],
        y: wbcTotalData.wbcData[channel2].data[i]
    }));
    return xys.filter(xy => IsInsidePoly(currGatePolygon.polys[nIdx], xy.x, xy.y));
}

// Get Polygon gate data
function FilterGateData(gatePolygon, isInside) {
    if (!gatePolygon) {
        return [];
    }

    let channel1 = gatePolygon.channel1;
    let channel2 = gatePolygon.channel2;

    let xys = wbcTotalData.wbcData[channel1].data.map((v, i) => ({
        x: wbcTotalData.wbcData[channel1].data[i],
        y: wbcTotalData.wbcData[channel2].data[i]
    }));
    return xys.filter(xy => IsInsidePolygons(gatePolygon, xy) == isInside);
}

// Check whether current point xy is inside polygons
function IsInsidePolygons(gatePolygon, xy) {
    let i = 0;
    for (i = 0; i < gatePolygon.polys.length; i++) {
        if (IsInsidePoly(gatePolygon.polys[i], xy.x, xy.y)) {
            return true;
        }
    }
    return false;
}
// check whether current point (x,y) is inside polygon
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