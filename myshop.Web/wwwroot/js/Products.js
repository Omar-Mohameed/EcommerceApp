//const { ajax } = require("jquery");

var dtble;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $("#mytable").DataTable({
        "ajax": {
            "url" : "/Admin/Product/GetData"
        },
        "columns": [
            { "data": "name" },
            { "data": "description" },
            { "data": "price" },
            { "data": "category.name" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a class="btn btn-outline-info btn-sm" href="/Admin/Product/Edit/${data}">Edit</a>
                        <a class="btn btn-outline-danger btn-sm" onClick=DeleteItem("/Admin/Product/DeleteProduct/${data}")>Delete</a>

                    `
                }
            }
        ]
    });
}

function DeleteItem(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "POST",
                success: function (data) {
                    if (data.success) {
                        dtble.ajax.reload();
                        Swal.fire(
                            "Deleted!",
                            data.message,
                            "success"
                        );
                    } else {
                        Swal.fire(
                            "Error!",
                            data.message,
                            "error"
                        );
                    }
                },
                error: function () {
                    Swal.fire(
                        "Error!",
                        "Something went wrong!",
                        "error"
                    );
                }
            });
        }
    });
}