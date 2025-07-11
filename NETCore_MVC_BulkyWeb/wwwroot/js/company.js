var dataTable;

//$(document).ready(function () {
//    loadDataTable();
//});

$(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            url: '/admin/api/companyapi/getall',
        },
        "language": { url: '//cdn.datatables.net/plug-ins/2.3.2/i18n/zh.json'},
        "columns": [
            { data: 'name', "width": "25%" },
            { data: 'state', "width": "15%" },
            { data: 'city', "width": "15%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="w-50 btn-group" role="group">
                            <a href="/admin/company/updateandinsert?id=${data}" class="btn btn-primary">
                                <i class="bi bi-pencil-square"></i> &ensp;编辑
                            </a>
                            <a Onclick=Delete('/admin/api/companyapi/delete/${data}') class="btn btn-danger">
                                <i class="bi bi-trash-fill"></i> &ensp;删除
                            </a>
                        </div>`;
                },
                "width": "20%",
                "className": "dt-center"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "你确定吗？",
        text: "您将无法恢复此状态！",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "确认删除！",
        cancelButtonText: "取消"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                },
            });
        }
    });
}