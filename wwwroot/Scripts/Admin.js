$(function () {
    let _QueryParameter = window.location.pathname.split("/");
    let _ActionName = _QueryParameter[2].toLocaleLowerCase();

    // #region :: Dashboard
    if (_ActionName === "categorylist") {
        
        $('.categoryTable').DataTable({
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5', 'print'],
            columnDefs: [
                {
                    targets: -1, // Assuming Edit is the last column
                    data: null,
                    defaultContent: '<button class="btn-edit"><i class="fas fa-edit"></i></button>',
                    orderable: false
                }
            ],
            language: {
                searchPlaceholder: "Search category...",
                search: ""
            }
        });
        //getAllcategories();
        $('#addCategoryForm').on('submit', function (e) {
            e.preventDefault();
            const categoryname = $('#categoryName').val().trim();
            if (categoryname === "") {
                notify(false, "Category name cannot be empty.", false);
                return;
            }
            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/Admin/AddCategory',
                type: 'POST',
                data: { categoryName: categoryname },
                dataType: 'json',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    xhr.setRequestHeader("RequestVerificationToken", antiForgeryToken);
                },
                success: function (response) {
                    $(".loader").css("display", "none");                    
                    if (response.isSuccess) {
                        notify(true, response.result, true);
                        //get all category datatable
                    } else {
                        notify(false, response.errorMessages, false);
                    }

                }
            });
        });

        function getAllcategories() {
            $.ajax({
                url: '/Admin/GetAllCategories',
                type: 'GET',
                dataType: 'json',
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    if (response.isSuccess) {
                        // Populate the category table with response.result
                        // Example: $('#categoryTable').DataTable().clear().rows.add(response.result).draw();
                    } else {
                        notify(false, response.errorMessages, false);
                    }
                }
            });
        }
    }
   
    // #endregion :: Dashboard
});