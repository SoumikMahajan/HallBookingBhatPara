$(function () {
    let _QueryParameter = window.location.pathname.split("/");
    let _ActionName = _QueryParameter[2].toLocaleLowerCase();

    // #region :: Category
    if (_ActionName === "categorylist") {

        $('.categoryTable').DataTable({
            destroy: true, // Auto-destroy existing instance
            dom: 'Bfrtip',
            buttons: ['copyHtml5', 'excelHtml5', 'csvHtml5', 'pdfHtml5', 'print'],
            responsive: true,
            autoWidth: false,            
            language: {
                searchPlaceholder: "Search category...",
                search: ""
            },
            columnDefs: [
                { targets: -1, orderable: false } // Last column (Edit) not sortable
            ]
        });

        getAllcategories();
      
        $('#addCategoryForm').on('submit', function (e) {
            e.preventDefault();
            const categoryname = $('#categoryName').val().trim();
            if (categoryname === "") {
                notify(false, "Category name cannot be empty.", false);
                $('#categoryName').focus();
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
                    $('#submitText').text("Saving...").prop('disabled', true);
                },
                success: function (response) {                                        
                    if (response.isSuccess) {
                        notify(true, response.result, true);
                        getAllcategories();
                    } else {
                        notify(false, response.errorMessages, false);
                    }

                },
                complete: function () {
                    $(".loader").css("display", "none");
                    $('#submitText').text("Submit").prop('disabled', false);
                },
            });
        });

        function getAllcategories() {
            $.ajax({
                url: '/Admin/GetAllCategoryList',
                type: 'GET',
                dataType: 'json',
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {  
                    $(".loader").css("display", "none"); 
                    // Destroy existing DataTable BEFORE updating table body
                    if ($.fn.DataTable.isDataTable('.categoryTable')) {
                        $('.categoryTable').DataTable().destroy();
                    }
                    if (response.isSuccess && response.result && response.result.length > 0) {
                        const categories = response.result;
                        let html = '';
                        categories.forEach((item, index) => {
                            html += `
                                <tr>
                                    <td>${index + 1}</td>
                                    <td>${item.category_name}</td>
                                    <td>
                                        <button class="btn btn-sm btn-warning btn-edit" data-id="${item.category_id_pk}">
                                            <i class="fas fa-edit"></i>
                                        </button>
                                    </td>
                                </tr>
                            `;
                        });
                        $('#categoryTableBody').html(html);
                        
                    }                                    
                    bindDataTable();
                }
            });
        }

        function bindDataTable() {
            $('.categoryTable').DataTable({
                dom: 'lBfrtip', // <-- B = Buttons, l = lengthMenu, f = filter, r = processing, t = table, i = info, p = pagination
                buttons: ['pdfHtml5', 'print'],
                responsive: true,
                autoWidth: false,
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                language: {
                    searchPlaceholder: "Search category...",
                    search: ""
                },
                columnDefs: [
                    { targets: -1, orderable: false }
                ]
            });
        }
    }
   
    // #endregion :: Category


    // #region :: SubCategory
    if (_ActionName === "subcategorylist") {

        $('.SubcategoryTable').DataTable({
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
        ////getAllcategories();
        //$('#addCategoryForm').on('submit', function (e) {
        //    e.preventDefault();
        //    const categoryname = $('#categoryName').val().trim();
        //    if (categoryname === "") {
        //        notify(false, "Category name cannot be empty.", false);
        //        return;
        //    }
        //    let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
        //    $.ajax({
        //        url: '/Admin/AddCategory',
        //        type: 'POST',
        //        data: { categoryName: categoryname },
        //        dataType: 'json',
        //        beforeSend: function (xhr) {
        //            $(".loader").css("display", "flex");
        //            xhr.setRequestHeader("RequestVerificationToken", antiForgeryToken);
        //        },
        //        success: function (response) {
        //            $(".loader").css("display", "none");
        //            if (response.isSuccess) {
        //                notify(true, response.result, true);
        //                //get all category datatable
        //            } else {
        //                notify(false, response.errorMessages, false);
        //            }

        //        }
        //    });
        //});

        //function getAllcategories() {
        //    $.ajax({
        //        url: '/Admin/GetAllCategories',
        //        type: 'GET',
        //        dataType: 'json',
        //        beforeSend: function () {
        //            $(".loader").css("display", "flex");
        //        },
        //        success: function (response) {
        //            $(".loader").css("display", "none");
        //            if (response.isSuccess) {
        //                // Populate the category table with response.result
        //                // Example: $('#categoryTable').DataTable().clear().rows.add(response.result).draw();
        //            } else {
        //                notify(false, response.errorMessages, false);
        //            }
        //        }
        //    });
        //}
    }
    // #endregion :: SubCategory


    // #region :: HallAvailability
    if (_ActionName === "addhallavailabilitydetails") {

        $('.HallAvailabilityTable').DataTable({
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
        
    }
    // #endregion :: HallAvailability

});