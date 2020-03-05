// -------------------- WBC Global Variables ------------------//

var wbcTotalData;                   // total wbc data loaded from server
var currWBCId;                      // Loaded wbc id
var wbcChannels;                    // wbc channel names
var wbcNomenclature;                // wbc nomenclature : new_names, old_names, mid..

var defaultChannel1;                // default gate channel names
var defaultChannel2;                // default gate channel names
var defaultChannel3;                // default gate channel names

var defaultGate1 = "defaultGate1";  // default gate names
var defaultGate2 = "defaultGate2";  // default gate names
var defaultGate3 = "defaultGate3";  // default gate names

var defaultGatePolygons = [];        // the array of default Gate Polygons
var customGatePolygons = [];         // the array of custom Gate Polygons

var currGatePolygon;            // current polygon gate. For final gate, it's empty.
var currGateName;               // current gate name

var isDefaultGate = false;      // Flag for dynamic or custom
var isDynamicGate = false;      // Flag for dynamic gate of default gate3

var Gate3Names = ["Neutrophils", "Monocytes", "Lymphocytes"];
var Gate3Colors = ["rgb(255, 189, 189)", "rgb(173, 233, 255)", "rgb(194, 228, 156)"];

var wbc_table;      // wbc data table object

var isGateEditing = false;          // Flag for drawing custom gate
var choosenPolygon = -1;                 // Choosen Polygon while editing custom polygon
var choosenPoint = -1;                   // Choosen point while editing custom polygon
var editingGateName;                // Editing Gate Name
// -------------------- WBC Global Variables ------------------//




//------------- Initialize Controls when document ready ------------//

$(document).ready(function () {

    InitWbcTable();

    $(".wbc-items").prop("disabled", true);
    
    AddGateChangeEvent();

});

function InitWbcTable() {
    wbc_table = $('#fcs-table').dataTable({
        processing: true, // for show progress bar
        serverSide: true, // for process server side
        filter: true, // this is for disable filter (search box)
        orderMulti: false, // for disable multiple column at once
        scrollX: true,
        scrollY: "58vh",
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
                    return "<a href='#' class='btn btn-danger' onclick=DeleteData('" + row.id + "'); ><i class='fa fa-trash'></i>&nbsp;&nbsp;Delete</a>";
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
}

function AddGateChangeEvent() {
    $(".btn-gate").on("click", function () {
        let gateDiv = $("#custom-gates");
        if (isDefaultGate) {
            gateDiv = $("#default-gates");
        }
        gateDiv.find(".active").removeClass("active");
        $(this).addClass("active");

        UpdateChart();
    });
}

//------------- Initialize Controls when document ready ------------//





// -------------------- Delete WBC file ----------------------------//

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

// ------------------------- Delete WBC file ---------------------- //






// -------------------- Load WBC file, Initialize wbc objects ----------------------------//

function LoadWbcData(wbcId) {
    if (isGateEditing) {
        CompleteEditPoygon();
    }

    let url = "/FCS/LoadWbcData";
    $.post(url, { wbcId: wbcId }, function (data) {
        if (data) {
            wbcTotalData = data;
            currWBCId = wbcId;
            initWbc();
        }
        else {
            alert("Something Went Wrong!");
        }
    });
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

    defaultGatePolygons[defaultGate1] = {
        channel1: defaultChannel1,
        channel2: defaultChannel2,
        polys: wbcTotalData.gate1Polygon
    };
    defaultGatePolygons[defaultGate2] = {
        channel1: defaultChannel3,
        channel2: defaultChannel1,
        polys: wbcTotalData.gate2Polygon
    };
    defaultGatePolygons[defaultGate3] = {
        channel1: defaultChannel1,
        channel2: defaultChannel2,
        polys: wbcTotalData.gate3Polygon
    };

    let customGates = wbcTotalData.customGate;
    $("#custom-btn-gate-div").empty();
    $("#custom-gate-checks").empty();

    if (!customGates) {
        customGatePolygons = [];
        $("#custom-final-gate").removeClass("active").addClass("active");
    } else {
        let newHtml, i = 0, active;
        customGatePolygons = [];
        customGates.forEach(function (gate, idx) {
            let gateName = gate.gateName;

            customGatePolygons[gateName] = {
                channel1: gate.channel1,
                channel2: gate.channel2,
                polys: gate.polygons
            };

            if (i == 0) {
                actove = "active";
            } else {
                active = "";
            }
            newHtml = '<div class="alert alert-dismissible btn-gate pr-5 ' + active + '" data-gate="' + gateName + '" id="custom-' + gateName + '">';
            newHtml += '<strong>' + gateName + '</strong>';
            newHtml += '<button type="button" class="close" data-dismiss="alert" onclick="RemoveGate(\'' + gateName + '\')">×</button>';
            newHtml += '</div>';

            $("#custom-btn-gate-div").append(newHtml);

            newHtml = '<input type="checkbox" id="final-custom-' + gateName + '" data-gate="' + gateName + '" class="wbc-items" checked/>';
            newHtml += '<label for= "final-custom-' + gateName + '" > ' + gateName + '</label >';
            $("#custom-gate-checks").append(newHtml);

            i++;
        });
    }

    $("#channel-1").val(defaultChannel1);
    $("#channel-2").val(defaultChannel2);

    $("#custom-gates").find(".active").removeClass("active");
    $("#custom-gates .btn-gate").first().addClass("active");
    
    AddGateChangeEvent();
    UpdateChart();

    $("#btn-tab-graph").trigger("click");
}

// -------------------- Load WBC file, Initialize wbc objects ----------------------------//





// -------------------- Save Custom Gate on Server ----------------------------//
function SaveCustomGate() {
    let url = "/FCS/SaveCustomGate";
    let arrGates = [];
    for (let key in customGatePolygons) {
        arrGates.push({
            gateName: key,
            channel1: customGatePolygons[key].channel1,
            channel2: customGatePolygons[key].channel2,
            polygons: customGatePolygons[key].polys
        });
    }
    $.post(url, { wbcId: currWBCId, customGate: arrGates }, function (data) {
        console.log("save custom gate data", data);
    });
}
// -------------------- Save Custom Gate on Server----------------------------//





// -------------------- Events Control ----------------------------//

// change channel name
function ChangeChannel() {
    if (currGateName != "finalGate") {
        return;
    }

    let channel1 = $("#channel-1").val();
    let channel2 = $("#channel-2").val();
    if (channel1 == channel2) {
        $(".channel-items").prop("disabled", true);
        alert("Please choose different channels!.");
        return;
    }
    $(".channel-items").prop("disabled", false);
    UpdateChart();
}

// change to default gate state
function DefaultGate() {
    $("#tab-custom").removeClass("active");
    $("#tab-default").addClass("active")
    $(".custom-gate-div").hide();
    $(".default-gate-div").show();

    if (isGateEditing) {
        CompleteEditPoygon();
    }

    isDefaultGate = true;
    UpdateChart();
}

// change to custom gate state
function CustomeGate() {
    isDefaultGate = false;
    $("#tab-custom").addClass("active");
    $("#tab-default").removeClass("active")
    $(".custom-gate-div").show();
    $(".default-gate-div").hide();

    UpdateChart();
}

// change dynamic states. this is affected only when default gate 3 and final gate for default gate.
function ChangeDynamic(isDynamic) {
    isDynamicGate = isDynamic;
    if (currGateName == defaultGate3 || (isDefaultGate && currGateName == "finalGate" && $("#defaultGate3").is(":checked"))) {
        UpdateChart();
    }
}

// Remove Custom Gate
function RemoveGate(gateName) {
    if (!confirm("Are you sure to delete this gate - " + gateName + "?")) {
        return;
    }

    isGateEditing = false;

    if (gateName == currGateName) {
        $("#custom-gates").find(".active").removeClass("active");
        $("#custom-gates .btn-gate").first().addClass("active");
    }

    delete customGatePolygons[gateName];
    SaveCustomGate();

    $("#final-custom-" + gateName).remove();
    $('label[for=final-custom-' + gateName + ']').remove();
    UpdateChart();
}

// Add new custom gate
function AddNewCustomGate() {
    let channel1 = $("#channel-1").val();
    let channel2 = $("#channel-2").val();
    if (channel1 == channel2) {
        alert("Please choose different channels!.");
        return;
    }

    editingGateName = $("#gate-name").val();
    if (!editingGateName) {
        alert("Please insert gate name");
        $("#gate-name").focus();
        return;
    }

    if (editingGateName in customGatePolygons) {
        alert("The gate name was already used. Please insert other gate name.");
        $("#gate-name").focus();
        return;
    }

    // check channel is used alread
    let isChannelUsed = false;
    for (let key in customGatePolygons) {
        if (key != "finalGate" && customGatePolygons[key].channel1 == channel1 && customGatePolygons[key].channel2 == channel2) {
            isChannelUsed = true;
        }
    };

    if (isChannelUsed) {
        alert("Current Channels are used already. Please update exsiting gate or choose other channels");
        return;
    }

    if (!confirm("Are you sure to create new gate on these channels?")) {
        return;
    }

    customGatePolygons[editingGateName] = {
        channel1: channel1,
        channel2: channel2,
        polys: [[]]
    };

    $("#custom-gates").find(".active").removeClass("active");

    let newHtml = '<div class="alert alert-dismissible btn-gate pr-5 active" data-gate="' + editingGateName + '" id="custom-' + editingGateName + '">';
    newHtml += '<strong>' + editingGateName + '</strong>';
    newHtml += '<button type="button" class="close" data-dismiss="alert" onclick="RemoveGate(\'' + editingGateName + '\')">×</button>';
    newHtml += '</div>';

    $("#custom-btn-gate-div").append(newHtml);
    AddGateChangeEvent();

    newHtml = '<input type="checkbox" id="final-custom-' + editingGateName + '" data-gate="' + editingGateName + '" class="wbc-items" checked/>';
    newHtml += '<label for= "final-custom-' + editingGateName + '" > ' + editingGateName + '</label >';
    $("#custom-gate-checks").append(newHtml);

    UpdateChart();
    StartEditPolygon();
    AddNewPolygon();

    $("#fcs-chart-details").html("<h4> Editing Custom Gate - " + editingGateName + "</h4>");
    //UpdateChart();
}

function StartEditPolygon() {
    if (isDefaultGate || currGateName == "finalGate") {
        return;
    }

    $(".wbc-channels").prop("disabled", true);

    isGateEditing = true;
    $(".custom-gate-div").hide();
    $("#edit-custom-gate").show();
    chartData.forEach(function (v, idx) {
        if (v.type == "line") {
            v.dragable = true;
        }
    });
    $("#fcs-chart-details").html("<h4> Editing Custom Gate - " + editingGateName + "</h4>");
}

function AddNewPolygon() {
    CleanHighlight();
    choosenPolygon = 0;
    choosenPoint = -1;
    chartData.unshift({
        label: "Gate - " + editingGateName,
        borderColor: lineHighlightColor,
        fill: false,
        data: [],
        pointHitRadius: 10,
        dragable: true,
        type: 'line',
        pointRadius: 5,
        pointHoverRadius: 10,
        lineTension: 0,
    });
    chartGraph.update();
}

function CompleteEditPoygon() {
    isGateEditing = false;
    $(".custom-gate-div").show();
    $("#edit-custom-gate").hide();

    $(".wbc-channels").prop("disabled", false);

    if (editingGateName) {
        customGatePolygons[editingGateName].polys = [];
        chartData = chartData.filter(data => data.type != "line" || data.data.length > 3);
        chartData.forEach(function (v, idx) {
            if (v.type == "line") {
                customGatePolygons[editingGateName].polys.push(v.data);
            }
        });
        SaveCustomGate();
        UpdateChart();
    }
}

function DeleteChoosenPolygon() {
    if (choosenPolygon < 0) {
        return;
    }
    chartData.splice(choosenPolygon, 1);
    chartGraph.update();
    choosenPolygon = -1;
}

function DeleteChoosenPoint() {
    if (choosenPolygon < 0 || choosenPoint < 0) {
        return;
    }
    // For Triangle polygon
    // If the number of polygon points are less than 5 and closed polygon, remove the entire polygon
    let tmpData = chartData[choosenPolygon].data;
    if (tmpData.length < 5 && tmpData[0].x == tmpData[tmpData.length - 1].x && tmpData[0].y == tmpData[tmpData.length - 1].y) {
        DeleteChoosenPolygon();
        return;
    }
    chartData[choosenPolygon].data.splice(choosenPoint, 1);
    chartGraph.update();
    choosenPoint = -1;
}

// Add or Move Clicked points. When a point is closed to current selected point, move. Else, add
function AddOrMoveCustomPoints(newXY) {

    if (choosenPolygon > -1) {
        // if current polygon is closed, don't add any point more
        let tmpData = chartData[choosenPolygon].data;
        if (tmpData.length > 1 && (tmpData[0].x == tmpData[tmpData.length - 1].x) && (tmpData[0].y == tmpData[tmpData.length - 1].y)) {
            let isClosed = false;
            tmpData.forEach(function (v, idx) {
                if (Math.abs(v.x - newXY.x) < chartGraph.scales['x-axis-0'].max / 60 &&
                    Math.abs(v.y - newXY.y) < chartGraph.scales['y-axis-0'].max / 40) {
                    isClosed = true;
                }
            });
            if (isClosed) {
                HighlightLine(choosenPolygon);
                return;
            } else {
                AddNewPolygon();
            }
            
        }
        // When choosen point and new point isn't close, or not choosen any point, add new point
        if (choosenPoint < 0 ) {
            choosenPoint = chartData[choosenPolygon].data.length;
            chartData[choosenPolygon].data.push(newXY);
        }
        // When final point is close to first point, generate polygon by adding first point again
        else if (Math.abs(chartData[choosenPolygon].data[0].x - newXY.x) < chartGraph.scales['x-axis-0'].max / 60 &&
                Math.abs(chartData[choosenPolygon].data[0].y - newXY.y) < chartGraph.scales['y-axis-0'].max / 40) {
                chartData[choosenPolygon].data.push(chartData[choosenPolygon].data[0]);
                AddNewPolygon();
                return;
        } else if (Math.abs(chartData[choosenPolygon].data[choosenPoint].x - newXY.x) > chartGraph.scales['x-axis-0'].max / 60 ||
            Math.abs(chartData[choosenPolygon].data[choosenPoint].y - newXY.y) > chartGraph.scales['y-axis-0'].max / 40) {
            choosenPoint = chartData[choosenPolygon].data.length;
            chartData[choosenPolygon].data.push(newXY);
        } else {
            chartData[choosenPolygon].data[choosenPoint] = newXY;
        }
        chartGraph.update();
    } else {
        AddNewPolygon();
    }
}

// -------------------- Events Control ----------------------------//






// --------------------  Draw WBC on Chart Graph -----------------//

// Draw or remove heatmap
function DrawHeatmap() {
    
    if ($("#draw-heatmap").prop("checked")) {
        let left = chartGraph.chartArea.left;
        let top = chartGraph.chartArea.top;
        let width = chartGraph.chartArea.right - chartGraph.chartArea.left;
        let height = chartGraph.chartArea.bottom - chartGraph.chartArea.top;

        $("#fcs-chart").css("background", 'url("' + wbcTotalData.heatmapFile + '") ' + left + 'px ' + top + 'px / ' + width + 'px ' + height + 'px no-repeat white');
    } else {
        $("#fcs-chart").css("background", 'white');
    }
}

// Draw Chart
function UpdateChart() {
    if (!wbcTotalData) {
        ClearChart();
        return;
    }
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
                radius: 0
            }];
    } else {
        //---------Initialize Components-----//
        let gateDiv = $("#custom-gates");
        if (isDefaultGate) {
            gateDiv = $("#default-gates");
        }
        currGateName = gateDiv.find(".active").data("gate");

        if (isDefaultGate && currGateName != "finalGate") {
            $(".wbc-channels").prop("disabled", true);
        } else {
            $(".wbc-channels").prop("disabled", false);
        }

        if (currGateName != "finalGate") {
            if (isDefaultGate) {
                currGatePolygon = defaultGatePolygons[currGateName];
            } else {
                currGatePolygon = customGatePolygons[currGateName];
            }
            
            $("#channel-1").val(currGatePolygon.channel1);
            $("#channel-2").val(currGatePolygon.channel2);
        }
        
        chartData = [];

        //---------Update Chart-----//
        
        // Add Points for gates
        if (currGateName == defaultGate3) {
            chartData = GetDefaultGate3();
        } else if (currGateName == "finalGate") {
            chartData = GetFinalGateData();
        } else {
            chartData = GetPolygonGateData();
        }

        // Append Gate Lines
        if ((currGateName != defaultGate3 || !isDynamicGate) && currGateName != "finalGate") {
            Array.prototype.unshift.apply(chartData, GetChartGateLineData());
        }
    }

    // Update Graph And Redraw
    chartGraph.data.datasets = chartData;
    chartGraph.update();
}

function ClearChart() {
    chartGraph.data.datasets = [];
    chartGraph.update();
}

// --------------------  Draw WBC on Chart Graph -----------------//






// -------------------- Extract Chart Data from WBC Data --------//

// Get Final Gate Data
function GetFinalGateData() {
    let selectedGates = [];
    let gateDiv = "#custom-gates";
    if (isDefaultGate) {
        gateDiv = "#default-gates";
    }
    $(gateDiv + ' .final input:checked').each(function () {
        selectedGates.push($(this).data('gate'));
    });

    let gatePolygons = isDefaultGate ? defaultGatePolygons : customGatePolygons;
    let isContainsGate3 = false;

    let finalChannel1 = $("#channel-1").val();
    let finalChannel2 = $("#channel-2").val();
    let finalRes = wbcTotalData.wbcData[defaultChannel1].data.map((v, i) => ({
        x: wbcTotalData.wbcData[finalChannel1].data[i],
        y: wbcTotalData.wbcData[finalChannel2].data[i],
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
        
        if (gateName == defaultGate3) {
            isContainsGate3 = true;

            let isN, isM, isL;
            if (isDynamicGate) {
                xys.forEach(function (xy, idx) {
                    isN = wbcTotalData.wbc3Cell.wbc_n.includes(idx);
                    isM = wbcTotalData.wbc3Cell.wbc_m.includes(idx);
                    isL = wbcTotalData.wbc3Cell.wbc_l.includes(idx);
                    finalRes[idx].isInside &= (isN || isM || isL);
                    finalRes[idx].isNML = [isN, isM, isL];
                });
                selectedGates[idx] = {
                    gateName: gateName,
                    c3: [wbcTotalData.wbc3Cell.wbc_n.length, wbcTotalData.wbc3Cell.wbc_m.length, wbcTotalData.wbc3Cell.wbc_l.length]
                };
            } else {
                let nN = 0, nM = 0, nL = 0;
                xys.forEach(function (xy, idx) {
                    isN = IsInsidePoly(gatePolygon.polys[0], xy.x, xy.y);
                    isM = IsInsidePoly(gatePolygon.polys[1], xy.x, xy.y);
                    isL = IsInsidePoly(gatePolygon.polys[2], xy.x, xy.y);
                    if (isN) {
                        nN++;
                    }
                    if (isM) {
                        nM++;
                    }
                    if (isL) {
                        nL++;
                    }
                    finalRes[idx].isInside &= (isN || isM || isL);
                    finalRes[idx].isNML = [isN, isM, isL];
                });
                selectedGates[idx] = {
                    gateName: gateName,
                    c3: [nN, nM, nL]
                };
            }
        } else {
            xys.forEach(function (xy, idx) {
                xys[idx].isInside = IsInsidePolygons(gatePolygon, xy);
                finalRes[idx].isInside &= xys[idx].isInside;
            });
            selectedGates[idx] = {
                gateName: gateName,
                inside: xys.filter(xy => xy.isInside).length,
                outside: xys.filter(xy => !xy.isInside).length
            };
        }
    });

    let insideData = finalRes.filter(xy => xy.isInside);
    let outsideData = finalRes.filter(xy => !xy.isInside);
    let data = [];

    if (isContainsGate3) {
        for (let i = 0; i < 3; i++) {
            data[i] = {
                label: Gate3Names[i],
                backgroundColor: Gate3Colors[i],
                borderColor: Gate3Colors[i],
                data: finalRes.filter(xy => xy.isInside && xy.isNML[i]),
                radius: 1
            };
        }
        data[3] = {
            label: 'Outside Gate' + currGateName,
            backgroundColor: 'rgb(255, 99, 132)',
            borderColor: 'rgb(255, 99, 132)',
            data: outsideData,
            radius: 1
        };
    } else {
        data[0] = {
            label: 'Inside Gate' + currGateName,
            backgroundColor: 'rgb(132, 99, 255)',
            borderColor: 'rgb(132, 99, 255)',
            data: insideData,
            radius: 1
        };
        data[1] = {
            label: 'Outside Gate' + currGateName,
            backgroundColor: 'rgb(255, 99, 132)',
            borderColor: 'rgb(255, 99, 132)',
            data: outsideData,
            radius: 1
        };
    }

    let fcsName = wbc_table.fnGetData($("#fcs-table tbody tr.selected"));
    let detailHtml = "<h4>" + fcsName.fcs_name + "</h4>";
    selectedGates.forEach(function (v) {
        detailHtml += "<br><strong>Gate : </strong>" + v.gateName;
        if (v.gateName == defaultGate3) {
            for (let i = 0; i < 3; i++) {
                detailHtml += "<br><strong> &nbsp; &nbsp;" + Gate3Names[i] + " : </strong>" + v.c3[i];
            }
        } else {
            detailHtml += "<br><strong> &nbsp; &nbsp;Inside : </strong>" + v.inside;
            detailHtml += "<br><strong> &nbsp; &nbsp;Outside : </strong>" + v.outside;
        }
    });

    detailHtml += "<br><hr class='mt-2 mb-2 mr-5'><strong>Gate : </strong>" + currGateName;
    detailHtml += "<br><strong> &nbsp; &nbsp;Inside : </strong>" + insideData.length;
    detailHtml += "<br><strong> &nbsp; &nbsp;Outside : </strong>" + outsideData.length;
    $("#fcs-chart-details").html(detailHtml);

    return data;
}

// Get Ploygon Gate Data : It's used in custom gate & default gate1,2
function GetPolygonGateData() {
    let insideData = FilterGateData(currGatePolygon, true);
    let outsideData = FilterGateData(currGatePolygon, false);
    let data = [];
    data[0] = {
        label: 'Inside Gate' + currGateName,
        backgroundColor: 'rgb(132, 99, 255)',
        borderColor: 'rgb(132, 99, 255)',
        data: insideData,
        radius: 1
    };
    data[1] = {
        label: 'Outside Gate' + currGateName,
        backgroundColor: 'rgb(255, 99, 132)',
        borderColor: 'rgb(255, 99, 132)',
        data: outsideData,
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
function IsInsidePoly(poly, x, y) {
    if (!poly || poly.length < 1)
        return true;
    
    let minX = poly[0].x;
    let maxX = poly[0].x;
    let minY = poly[0].y;
    let maxY = poly[0].y;
    let i = 1;

    for (i = 1; i < poly.length; i++) {
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
    for (i = 0, j = poly.length - 1; i < poly.length; j = i++) {
        if ((poly[i].y > y) != (poly[j].y > y) &&
            x < (poly[j].x - poly[i].x) * (y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x) {
            inside = !inside;
        }
    }
    return inside;
}

// Get Default Gate3 data
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
                radius: 1
            };
        }

        data[3] = {
            label: 'Outside Default Gate3',
            backgroundColor: 'rgb(255, 99, 132)',
            borderColor: 'rgb(255, 99, 132)',
            data: outsideData,
            radius: 1
        };
    } else {
        for (let i = 0; i < 3; i++) {
            nmlData[i] = FilterGate3Data(currGatePolygon, i);
            data[i] = {
                label: Gate3Names[i],
                backgroundColor: Gate3Colors[i],
                borderColor: Gate3Colors[i],
                data: nmlData[i],
                radius: 1
            };
        }
        outsideData = FilterGateData(currGatePolygon, false);
        data[3] = {
            label: 'Outside Default Gate3',
            backgroundColor: 'rgb(255, 99, 132)',
            borderColor: 'rgb(255, 99, 132)',
            data: outsideData,
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

// Get Dynamic Gate Data for Gate 3
function GetDynamicGateData(Idxs) {
    return Idxs.map((v, i) => ({
        x: wbcTotalData.wbcData[defaultChannel1].data[v],
        y: wbcTotalData.wbcData[defaultChannel2].data[v]
    }));
}

// Get Data outside of Dynamic Gate
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
function FilterGate3Data(poygon, nIdx) {
    if (!poygon) {
        return [];
    }

    let channel1 = poygon.channel1;
    let channel2 = poygon.channel2;

    let xys = wbcTotalData.wbcData[channel1].data.map((v, i) => ({
        x: wbcTotalData.wbcData[channel1].data[i],
        y: wbcTotalData.wbcData[channel2].data[i]
    }));
    return xys.filter(xy => IsInsidePoly(poygon.polys[nIdx], xy.x, xy.y));
}

// Gate Polygon Gate Line Data
function GetChartGateLineData() {
    if (currGateName == "finalGate") {
        return [{
            label: "Gate - " + currGateName,
            borderColor: lineNormalColor,
            fill: false,
            data: v,
            pointHitRadius: 10,
            dragable: false,
            type: 'line',
            pointRadius: 5,
            pointHoverRadius: 10,
            lineTension: 0,
        }];
    }

    let polygon = currGatePolygon.polys.map((v, i) => ({
        label: "Gate - " + currGateName,
        borderColor: 'rgb(100, 200, 100)',
        fill: false,
        data: v,
        pointHitRadius: 10,
        dragable: false,
        type: 'line',
        pointRadius: 5,
        pointHoverRadius: 10,
        lineTension: 0,
    }));

    if (!polygon) {
        polygon = [{
            label: "Gate - " + currGateName,
            borderColor: 'rgb(100, 200, 100)',
            fill: false,
            data: [],
            pointHitRadius: 10,
            dragable: false,
            type: 'line',
            pointRadius: 5,
            pointHoverRadius: 10,
            lineTension: 0,
        }];
    }

    return polygon;
}

// -------------------- Extract Chart Data from WBC Data --------//






// -------------------- Export Gate Results --------//
function SaveChartAsImage() {
    let wbc = wbc_table.fnGetData($("#fcs-table tbody tr.selected"));
    if (!wbc || !wbc.fcs_name || !currGateName) {
        return;
    }

    html2canvas(document.getElementById('fcs-chart'), {
        onrendered: function (canvas) {
            var link = document.createElement('a');
            link.href = canvas.toDataURL('image/jpeg');
            link.download = wbc.fcs_name + " - " + currGateName + '.png';
            link.click();
        }
    });
}
// -------------------- Export Gate Results --------//
