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
            url: '/admin/api/productapi',
            //dataSrc: function (json) {
            //    // 手动清理循环引用
            //    json.data.forEach(item => {
            //        if (item.category?.products) {
            //            item.category.products = null;
            //        }
            //    });
            //    return json.data;
            //}
        },
        "language": { url: '//cdn.datatables.net/plug-ins/2.3.2/i18n/zh.json'},
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'isbn', "width": "15%" },
            { data: 'author', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'productId',
                "render": function (data) {
                    return `
                        <div class="w-50 btn-group" role="group">
                            <a href="/admin/product/updateandinsert?id=${data}" class="btn btn-primary">
                                <i class="bi bi-pencil-square"></i> &ensp;编辑
                            </a>
                            <a Onclick=Delete('/admin/api/productapi/delete/${data}') class="btn btn-danger">
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