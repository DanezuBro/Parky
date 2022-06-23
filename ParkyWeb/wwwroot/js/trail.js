var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    //var d = $.ajax({
    //    "url": "/Trails/GetAllTrails",
    //    "type": "GET",
    //    "datatype": "json"
    //});
    //console.log(d);

    dataTable = $('#tblTest').DataTable({
        "ajax": {
            "url": "/Trails/GetAllTrails",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "nationalPark.name", "width": "20%" },
            { "data": "name", "width": "20%" },
            { "data": "distance", "width": "15%" },
            { "data": "elevation", "width": "15%" },
            {
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                                <a href="/Trails/Upsert/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                                    <i class="fa-solid fa-pen-to-square"></i>
                                </a>
                                    &nbsp;
                                <a onclick=Delete("/Trails/Delete/${data}")  class="btn btn-primary text-danger" style="cursor:pointer">
                                    <i class="fa-solid fa-trash-can"></i>
                                </a>
                                </div>
                            `;
                }, "width": "30%"
            }
        ]
    });
}

function Delete(url) {
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}