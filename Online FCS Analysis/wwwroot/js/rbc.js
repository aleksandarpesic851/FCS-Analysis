var wbc_defaultGate1 = "defaultGate1";  // default gate names
var wbc_defaultGate2 = "defaultGate2";  // default gate names
var wbc_defaultGate3 = "defaultGate3";  // default gate names

var wbc_Gate3Names = ["Neutrophils", "Monocytes", "Lymphocytes"];
var wbc_Gate3Colors = ["rgb(255, 189, 189)", "rgb(173, 233, 255)", "rgb(194, 228, 156)"];



//------------- Initialize Controls when document ready ------------//


$(document).ready(function () {

    InitFCSTable("wbc-table", "/FCS/LoadWBCTable");
    InitFCSTable("rbc-table", "/FCS/LoadRBCTable");

});

function InitFCSTable(table_id, url) {
    wbc_table = $('#' + table_id).dataTable({
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

//------------- Initialize Controls when document ready ------------//





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
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}

//--------------------- Delete FCS file ---------------------- //

