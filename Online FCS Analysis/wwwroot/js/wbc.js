$(document).ready(function () {
    let wbc_table = $('#fcs-table').dataTable({
        processing: true, // for show progress bar
        serverSide: true, // for process server side
        filter: true, // this is for disable filter (search box)
        orderMulti: false, // for disable multiple column at once
        scrollX: true,
        scrollY: true,
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
            LoadWbcData(data.fcs_file_name);
        }
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

function LoadWbcData(wbcFilename) {
    let url = "/FCS/LoadWbcData";
    $.post(url, { wbcFilename: wbcFilename }, function (data) {
        if (data) {
            console.log(data);
        }
        else {
            alert("Something Went Wrong!");
        }
    });
}