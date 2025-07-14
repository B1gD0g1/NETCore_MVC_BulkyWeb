var dataTable;

//$(document).ready(function () {
//    loadDataTable();
//});

$(function () {
    var url = window.location.search;

    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else if (url.includes("completed")) 
    { 
        loadDataTable("completed");
    }
    else if (url.includes("approved")) 
    {
        loadDataTable("approved");
    }
    else if (url.includes("pending")) 
    {
        loadDataTable("pending");
    }
    else
    {
        loadDataTable("all");
    }
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/api/orderapi/getall?status=' + status
        },
        "language": { url: '//cdn.datatables.net/plug-ins/2.3.2/i18n/zh.json'},
        "columns": [
            { data: 'id', "width": "8%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "20%" },
            { data: 'applicationUser.email', "width": "22%" },
            { data: 'orderStatus', "width": "10%" },
            { data: 'orderTotal', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-50 btn-group" role="group">
                            <a href="/admin/order/details?orderId=${data}" class="btn btn-primary">
                                <i class="bi bi-pencil-square"></i> &ensp;
                            </a>
                        </div>`;
                },
                "width": "15%",
                "className": "dt-center"
            }
        ]
    });
}