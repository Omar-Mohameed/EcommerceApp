var dtble;
$(document).ready(function () {
    loaddata();
});

function loaddata() {
    dtble = $("#mytable").DataTable({
        "ajax": {
            "url" : "/Admin/Order/GetData"
        },
        "columns": [
            { "data": "id" },
            { "data": "name" },
            { "data": "phoneNumber" },
            { "data": "applicationUser.email" },
            { "data": "orderStatus" },
            { "data": "totalPrice" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <a class="btn btn-outline-info btn-sm" href="/Admin/Order/Details?orderid=${data}">Details</a>

                    `
                }
            }
        ]
    });
}