var wbc_defaultGate1 = "defaultGate1";  // default gate names
var wbc_defaultGate2 = "defaultGate2";  // default gate names
var wbc_defaultGate3 = "defaultGate3";  // default gate names

var wbcDefaultChannels = [];
var rbcDefaultChannels = [];

var wbc_Gate3Names = ["Neutrophils", "Monocytes", "Lymphocytes"];
var wbc_Gate3Colors = ["rgb(255, 189, 189)", "rgb(173, 233, 255)", "rgb(194, 228, 156)"];

var wbcTotalData;                   // total wbc data loaded from server
var currWbcId;                      // Loaded wbc id
var currWbcName;                    // Loaded wbc name
var wbcNomenclature;                // wbc nomenclature : new_names, old_names, mid..

var rbcTotalData;                   // total rvc data loaded from server
var currRbcId;                      // Loaded rbc id
var currRbcName;                    // Loaded rbc name
var rbcNomenclature;                // rbc nomenclature : new_names, old_names, mid..

var defaultGatePolygons = [];        // the array of default Gate Polygons

var wbc_table;      // wbc data table object
var rbc_table;      // wbc data table object

var wbc_loaded = false;
var rbc_loaded = false;

var rbcPlatelete = [];
var rbcGeneral = [];

var rbcV = [];
var rbcHC = [];
//------------- Initialize Controls when document ready ------------//


$(document).ready(function () {

    wbc_table = InitFCSTable("wbc-table", "/FCS/LoadWBCTable");
    rbc_table = InitFCSTable("rbc-table", "/FCS/LoadRBCTable");

    InitRbcTableEvent();
    InitWbcTableEvent();

    $(".rbc-items").prop("disabled", true);
    $(".wbc-items").prop("disabled", true);
});

function InitFCSTable(table_id, url) {
    return $('#' + table_id).dataTable({
        processing: true, // for show progress bar
        serverSide: true, // for process server side
        filter: true, // this is for disable filter (search box)
        orderMulti: false, // for disable multiple column at once
        scrollX: true,
        scrollY: "58vh",
        scrollCollapse: true,
        ajax: {
            url: url,
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
                    return "<a href='#' class='btn btn-danger' onclick='DeleteData(\"" + table_id + "\", \"" + row.id + "\");' ><i class='fa fa-trash'></i>&nbsp;&nbsp;Delete</a>";
                }
            },
        ],
    });
}

function InitWbcTableEvent() {
    $('#wbc-table tbody').on("click", "tr", function () {
        wbc_loaded = false;
        const tr = $(this);
        const isSelected = tr.hasClass("selected");
        if (!isSelected) {
            $("#wbc-table tbody tr").removeClass("selected");
            tr.addClass("selected");
            const data = wbc_table.fnGetData(tr);
            currWbcId = data.id;
            currWbcName = data.fcs_name;
            LoadWbcData(data.id);
        }
    });

    $('#wbc-table tbody td:last-child').click(function (event) {
        event.preventDefault();
    });
}

function InitRbcTableEvent() {
    $('#rbc-table tbody').on("click", "tr", function () {
        rbc_loaded = false;
        const tr = $(this);
        const isSelected = tr.hasClass("selected");
        if (!isSelected) {
            $("#rbc-table tbody tr").removeClass("selected");
            tr.addClass("selected");
            const data = rbc_table.fnGetData(tr);
            currRbcId = data.id;
            currRbcName = data.fcs_name;
            LoadRbcData(data.id);
        }
    });

    $('#rbc-table tbody td:last-child').click(function (event) {
        event.preventDefault();
    });
}

//------------- Initialize Controls when document ready ------------//




//------------- Control Events Proc ------------//

function ChangeRbcChannel() {
    if ($("#rbc-channel-1").val() == $("#rbc-channel-2").val()) {
        alert("Please choose different channel");
        return;
    }
    ExtractRBCData();
    DrawRBC();
}

function ChangeWbcChannel() {
    if ($("#wbc-channel-1").val() == $("#wbc-channel-2").val()) {
        return;
    }
    ExtractWBCData();
    DrawWBC();
}
//------------- Control Events Proc ------------//





// -------------------- Delete FCS file ----------------------------//

function DeleteData(tableId, fcsID) {
    if (confirm("Are you sure you want to delete this wbc data?")) {
        Delete(tableId, fcsID);
    }
    else {
        return false;
    }
}

function Delete(tableId, fcsID) {
    const url = "/FileManager/Delete";

    $.post(url, { id: fcsID}, function (data) {
        if (data) {
            oTable = $('#' + tableId).DataTable();
            oTable.draw();
            if (tableId == "wbc-table") {
                wbc_loaded = false;
            } else {
                rbc_loaded = false;
            }
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}

//--------------------- Delete FCS file ---------------------- //




// -------------------- Load FCS file, Initialize FCS objects ----------------------------//

function LoadRbcData(rbcId) {
    let url = "/FCS/LoadRbcData";
    rbcV = [];
    rbcHC = [];
    $.post(url, { rbcId: rbcId }, function (data) {
        if (data) {
            rbcTotalData = data;
            InitRbc();
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}

// Load V HC Data for RBC Cell
function LoadVHC() {
    let url = "/FCS/CalculateVHC";
    let points = JSON.stringify( rbcGeneral );
    $.ajax({
        url,
        data: points,
        contentType: 'application/json',
        type: 'POST',
        success: function (data) {
            if (data) {
                rbcV = data.arrV;
                rbcHC = data.arrHC;
                DrawVHC();
                CalculateRBC();
            }
            else {
                console.log("Error in get VHC");
            }
        }
    });
}

function LoadWbcData(wbcId) {
    let url = "/FCS/LoadWbcData";
    $.post(url, { wbcId: wbcId }, function (data) {
        if (data) {
            wbcTotalData = data;
            InitWbc();
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}

function InitWbc() {
    if (!wbcTotalData) {
        return;
    }
    wbc_loaded = true;
    $(".wbc-items").prop("disabled", false);
    wbcNomenclature = wbcTotalData.nomenclature;
    $(".wbc-channels").empty();
    for (let key in wbcTotalData.wbcData) {
        $(".wbc-channels").append("<option>" + key + "</option>");
    }


    defaultChannel1 = GetChannelName("FCS1peak", wbcNomenclature);
    defaultChannel2 = GetChannelName("SSCpeak", wbcNomenclature);
    defaultChannel3 = GetChannelName("FCS1area", wbcNomenclature);

    wbcDefaultChannels = [defaultChannel1, defaultChannel2];

    $("#wbc-channel-1").val(wbcDefaultChannels[0]);
    $("#wbc-channel-2").val(wbcDefaultChannels[1]);

    ExtractWBCData();

    DrawWBC();

    CalculateRBC();
}

function InitRbc() {
    if (!rbcTotalData) {
        return;
    }
    rbc_loaded = true;
    $(".rbc-items").prop("disabled", false);
    rbcNomenclature = rbcTotalData.nomenclature;
    $(".rbc-channels").empty();
    for (let key in rbcTotalData.rbcData) {
        $(".rbc-channels").append("<option>" + key + "</option>");
    }

    rbcDefaultChannels = [GetChannelName("FCS1peak", rbcNomenclature), GetChannelName("FSC2peak", rbcNomenclature)];

    $("#rbc-channel-1").val(rbcDefaultChannels[0]);
    $("#rbc-channel-2").val(rbcDefaultChannels[1]);

    ExtractRBCData();
    LoadVHC();
}

// -------------------- Load WBC file, Initialize wbc objects ----------------------------//




// ------------------------------------------ Draw Charts--------------------------------//

// Draw RBC Chart
function DrawRBC() {
    chartRBCData = [];
    chartRBCData[0] = {
        label: 'RBC - Plateletes',
        backgroundColor: 'rgb(255, 189, 189)',
        borderColor: 'rgb(255, 189, 189)',
        data: rbcPlatelete,
        radius: 1
    };
    chartRBCData[1] = {
        label: 'RBC - General',
        backgroundColor: 'rgb(173, 233, 255)',
        borderColor: 'rgb(173, 233, 255)',
        data: rbcGeneral,
        radius: 1
    };
    chartRBCGraph.data.datasets = chartRBCData;
    chartRBCGraph.update();

    let rbcDetail = "<h4>" + currRbcName + "</h4>";
    rbcDetail += "<br>&nbsp;&nbsp;<strong>Plateletes :</strong>" + rbcPlatelete.length;
    rbcDetail += "<br>&nbsp;&nbsp;<strong>General :</strong>" + rbcGeneral.length;
    $("#rbc-detail").html(rbcDetail);
}

function DrawWBC() {

}

function DrawVHC() {

}
// ------------------------------------------ Draw Charts--------------------------------//



// ------------------------------------------ Extract Chart Data--------------------------------//

// ----------RBC----------
function ExtractRBCData() {
    rbcPlatelete = [];
    rbcGeneral = [];

    if (!rbc_loaded) {
        return;
    }

    let channel1 = $("#rbc-channel-1").val();
    let channel2 = $("#rbc-channel-2").val();

    let orgData = rbcTotalData.rbcData[rbcDefaultChannels[0]].data.map((v, i) => ({
        x: rbcTotalData.rbcData[channel1].data[i],
        y: rbcTotalData.rbcData[channel2].data[i]
    }));
    let orgOrientData = rbcTotalData.rbcData[rbcDefaultChannels[0]].data.map((v, i) => ({
        x: rbcTotalData.rbcData[rbcDefaultChannels[0]].data[i],
        y: rbcTotalData.rbcData[rbcDefaultChannels[1]].data[i]
    }));
    // if there isn't valid gate polygon, treat as general rbc
    if (!rbcTotalData.gatePolygon || rbcTotalData.gatePolygon.length < 2) {
        rbcGeneral = orgData;
    } else {
        rbcPlatelete = ExtractData(rbcTotalData.gatePolygon[0], orgData, orgOrientData);
        rbcGeneral = ExtractData(rbcTotalData.gatePolygon[1], orgData, orgOrientData);
    }
}

// ----------WBC----------
function ExtractWBCData() {

}

// Extract data from original data inside a polygon
function ExtractData(polygon, orgData, orgOrientData) {
    let resData = [];
    orgData.forEach(function (point, idx) {
        if (IsInsidePoly(polygon, orgOrientData[idx].x, orgOrientData[idx].y)) {
            resData.push(point);
        }
    });
    return resData;
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


function GetChannelName(channelHandle, type) {
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

// ------------------------------------------ Extract Chart Data--------------------------------//





// ------------------------------------------ Calculate RBC Based on Extracted RBC & WBC--------------------------------//

function CalculateRBC() {

}

// ------------------------------------------ Calculate RBC Based on Extracted RBC & WBC--------------------------------//