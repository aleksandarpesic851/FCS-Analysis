$(document).ready(function () {
    InitInteract();
//    InitWbcTable();
});

function InitInteract() {
    // target elements with the "resize-drag" class
    interact('.interact-element')
        .resizable({
            // resize from all edges and corners
            edges: { left: true, right: true, bottom: true, top: true },

            listeners: {
                move(event) {
                    var target = event.target
                    var x = (parseFloat(target.getAttribute('data-x')) || 0)
                    var y = (parseFloat(target.getAttribute('data-y')) || 0)

                    // update the element's style
                    target.style.width = event.rect.width + 'px'
                    target.style.height = event.rect.height + 'px'

                    // translate when resizing from top or left edges
                    x += event.deltaRect.left
                    y += event.deltaRect.top

                    target.style.webkitTransform = target.style.transform =
                        'translate(' + x + 'px,' + y + 'px)'

                    target.setAttribute('data-x', x)
                    target.setAttribute('data-y', y)
                    target.textContent = Math.round(event.rect.width) + '\u00D7' + Math.round(event.rect.height)
                }
            },
            modifiers: [
                // keep the edges inside the parent
                /*interact.modifiers.restrictEdges({
                    outer: 'parent'
                }),*/

                // minimum size
                interact.modifiers.restrictSize({
                    min: { width: 100, height: 100 }
                })
            ],

            inertia: true
        })
        .draggable({
            listeners: { move: dragMoveListener },
            // enable inertial throwing
            inertia: true,
            // enable autoScroll
            autoScroll: true,
            // keep the element within the area of it's parent
            /*modifiers: [
                interact.modifiers.restrictRect({
                    restriction: 'parent',
                    endOnly: true
                })
            ]*/
        });
}

function dragMoveListener(event) {
    var target = event.target
    // keep the dragged position in the data-x/data-y attributes
    var x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx
    var y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy

    // translate the element
    target.style.webkitTransform =
        target.style.transform =
        'translate(' + x + 'px, ' + y + 'px)'

    // update the posiion attributes
    target.setAttribute('data-x', x)
    target.setAttribute('data-y', y)
}

function InitWbcTable() {
    wbc_table = $('#fcs-table').dataTable({
        processing: true, // for show progress bar
        serverSide: true, // for process server side
        filter: true, // this is for disable filter (search box)
        orderMulti: false, // for disable multiple column at once
        scrollX: true,
        scrollY: "85vh",
        scrollCollapse: true,
        ajax: {
            url: "/FCS/LoadWBCTable",
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
            { "data": "updatedAt", "name": "updatedAt" }
        ],
    });
/*
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
    });*/
}
