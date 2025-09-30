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
                        Swal.fire(
                            "Deleted!",
                            data.message,
                            "success"
                        ).then(() => {
                            location.reload(); // إعادة تحميل الصفحة بعد الحذف
                        });
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