var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable(url) {
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
                                <a href="/NationalParks/Delete/${data}" class="btn btn-primary text-danger" style="cursor:pointer">
                                    <i class="fa-solid fa-trash-can"></i>
                                </a>
                            </div>
                           `;
                }, "width": "25%"
            }
        ]
    });
}