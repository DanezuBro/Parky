﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
                        "url": "/NationalParks/GetAllNationalParks",
                        "type": "GET",
                        "datatype":"json"
        },
        "columns": [
            { "data": "name", "width": "50%" },
            { "data": "state", "width": "25%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/NationalParks/Upsert/${data}" class="btn btn-primary text-white" style="cursor:pointer">
                                    <i class="fa-solid fa-pen-to-square"></i> 
                                </a>
                                &nbsp;
                                <a onclick = Delete("/NationalParks/Delete/${data}") class="btn btn-primary text-danger" style="cursor:pointer">
                                    <i class="fa-solid fa-trash-can"></i>
                                </a>
                            </div>
                           `;
                }, "width": "25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success == true) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })

        }
    })
}