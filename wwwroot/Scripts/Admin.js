$(function () {
    let _QueryParameter = window.location.pathname.split("/");
    let _ActionName = _QueryParameter[2].toLocaleLowerCase();

    // #region :: Category
    if (_ActionName === "categorylist") {        

        getAllcategories();
      
        $('#addCategoryForm').on('submit', function (e) {
            e.preventDefault();
            const categoryname = $('#categoryName').val().trim();
            if (categoryname === "") {
                notify(false, "Category name cannot be empty.", false);
                $("#categoryName").addClass("is-invalid");
                return;
            }
            else {
                $("#categoryName").removeClass("is-invalid");
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

        getAllSubcategories();


        const fileInput = $('#fileUpload');
        // Handle manual file selection
        fileInput.on('change', function () {
            const file = this.files[0];
            if (file) {
                handleFile(file);
            }
        });

        // Preview and display file
        function handleFile(file) {
            const validTypes = ['image/jpeg', 'image/png', 'image/jpg'];
            const maxSize = 5 * 1024 * 1024; // 5MB

            if (!validTypes.includes(file.type)) {
                notify(false, "Only JPG, JPEG, or PNG files are allowed.", false);
                fileInput.val('');
                $('#fileUpload').addClass("is-invalid");
                return;
            }
            else {
                $('#fileUpload').removeClass("is-invalid");
            }

            if (file.size > maxSize) {
                notify(false, "File size must be under 2MB.", false);
                fileInput.val('');
                previewImage.hide();
                $('#fileUpload').addClass("is-invalid");
                return;
            }
            else {
                $('#fileUpload').removeClass("is-invalid");
            }            
        }

        $('#addSubCategoryForm').on('submit', function (e) {
            e.preventDefault();
            const categoryid = $('#categorylist option:selected').val();
            if (categoryid === "0") {
                notify(false, "Select Category from list.", false);
                $("#categorylist").addClass("is-invalid");
                return;
            }
            else {
                $("#categorylist").removeClass("is-invalid");
            }
            const categoryname = $('#hallName').val().trim();
            if (categoryname === "") {
                notify(false, "SubCategory name cannot be empty.", false);
                $("#hallName").addClass("is-invalid");
                return;
            }
            else {
                $("#hallName").removeClass("is-invalid");
            }
            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            const formData = new FormData();
            formData.append("CategoryId", categoryid);
            formData.append("SubCategoryName", categoryname);
            const fileInput = $('#fileUpload')[0];

            if (fileInput?.files?.length == 0) {
                notify(false, "please choose a hall type image.", false);
                $('#fileUpload').addClass("is-invalid");
                return;
            }

            if (fileInput?.files?.length > 0) {
                formData.append("fileUpload", fileInput.files[0]);
            }


            $.ajax({
                url: '/Admin/AddSubCategory',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                dataType: 'json',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    xhr.setRequestHeader("RequestVerificationToken", antiForgeryToken);
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    if (response.isSuccess) {
                        notify(true, response.result, true);
                        getAllSubcategories();
                    } else {
                        notify(false, response.errorMessages, false);
                    }

                }
            });
        });

        function getAllSubcategories() {
            $.ajax({
                url: '/Admin/GetAllSubCategoryList',
                type: 'GET',
                dataType: 'json',
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                   
                    if ($.fn.DataTable.isDataTable('.SubcategoryTable')) {
                        $('.SubcategoryTable').DataTable().destroy();
                    }
                    if (response.isSuccess && response.result && response.result.length > 0) {
                        const subCategories = response.result;
                        let html = '';
                        subCategories.forEach((item, index) => {
                            html += `
                                <tr>
                                    <td>${item.rowNumber}</td>
                                    <td>${item.category_name}</td>
                                    <td>${item.hall_name}</td>
                                    <td>
                                        ${item.hall_image_base64
                                            ? `<img src="${item.hall_image_base64}" width="70" height="50" style="object-fit: cover;" />`
                                            : 'No Image'}
                                    </td>
                                    <td>
                                        <button class="btn btn-sm btn-warning btn-edit" data-id="${item.hall_id_pk}">
                                            <i class="fas fa-edit"></i>
                                        </button>
                                    </td>
                                </tr>
                                    `;
                        });
                        $('#SubcategoryTableBody').html(html);

                    }
                    bindDataTable();
                }
            });
        }

        function bindDataTable() {
            $('.SubcategoryTable').DataTable({
                dom: 'lBfrtip', // <-- B = Buttons, l = lengthMenu, f = filter, r = processing, t = table, i = info, p = pagination
                buttons: ['pdfHtml5', 'print'],
                responsive: true,
                autoWidth: false,
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                language: {
                    searchPlaceholder: "Search SubCategory...",
                    search: ""
                },
                columnDefs: [
                    { targets: -1, orderable: false }
                ]
            });
        }

        
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

        $('#hallCategorylist').on('change', function (e) {
            e.preventDefault();
            const categoryId = $(this).val();
            if (!categoryId || categoryId === "0") {
                $('#hallSubCategorylist').html('<option value="0">-- Select Subcategory --</option>');
                return;
            }                      
            $.ajax({
                url: '/Admin/GetSubCategoriesByCatId',
                type: 'GET',
                data: { categoryid: categoryId },
                dataType: 'json',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");                   
                },
                success: function (response) {
                    if (response.isSuccess) {
                        if (response.isSuccess && response.result && response.result.length > 0) {
                            let html = '<option value="0">-- Select Subcategory --</option>';

                            response.result.forEach(item => {
                                html += `<option value="${item.id}">${item.name}</option>`;
                            });
                            $('#hallSubCategorylist').html(html);
                        }
                    } else {
                        $('#hallSubCategorylist').html('<option value="0">-- Select Subcategory --</option>');
                        notify(false, response.errorMessages, false);
                    }

                },
                complete: function () {
                    $(".loader").css("display", "none");                    
                },
                error: function (xhr, status, error) {
                    $('#hallSubCategorylist').html('<option value="0">-- Select Subcategory --</option>');
                    handleAjaxError(xhr, status, error);
                }
            });
        });

        $('#addHallAvailabilityForm').on('submit', function (e) {            
            e.preventDefault();
            const categoryid = $('#hallCategorylist option:selected').val();
            if (categoryid === '0' || categoryid === undefined) {
                notify(false, "Please Select Category.", false);
                $("#hallCategorylist").addClass("is-invalid");
                return;
            }
            else {
                $("#hallCategorylist").removeClass("is-invalid");
            }
            const SubcategoryId = $('#hallSubCategorylist option:selected').val();
            if (SubcategoryId === '0' || SubcategoryId === undefined) {
                notify(false, "Please Select Sub Category.", false);
                $("#hallSubCategorylist").addClass("is-invalid");
                return;
            }
            else {
                $("#hallSubCategorylist").removeClass("is-invalid");
            }

            const AvailableFrom = $('#availableFrom').val();
            if (AvailableFrom === '' || AvailableFrom === undefined) {
                notify(false, "Please Select Date.", false);
                $("#availableFrom").addClass("is-invalid");
                return;
            }
            else {
                $("#availableFrom").removeClass("is-invalid");
            }

            const AvailableTo = $('#availableTo').val();
            if (AvailableTo === '' || AvailableTo === undefined) {
                notify(false, "Please Select Date.", false);
                $("#availableTo").addClass("is-invalid");
                return;
            }
            else {
                $("#availableTo").removeClass("is-invalid");
            }

            const ProposedRate = $('#proposedRate').val().trim();
            if (ProposedRate === '' || ProposedRate === undefined) {
                notify(false, "Please Fill Proposed Rate.", false);
                $("#proposedRate").addClass("is-invalid");
                return;
            }
            else {
                $("#proposedRate").removeClass("is-invalid");
            }

            const SecurityMoney = $('#securityMoney').val().trim();
            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            const formData = new FormData();
            formData.append("CategoryId", categoryid);
            formData.append("SubcategoryId", SubcategoryId);
            formData.append("AvailableFrom", AvailableFrom);
            formData.append("AvailableTo", AvailableTo);
            formData.append("ProposedRate", ProposedRate);
            formData.append("SecurityMoney", SecurityMoney);


            $.ajax({
                url: '/Admin/AddHallAvailable',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                dataType: 'json',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    xhr.setRequestHeader("RequestVerificationToken", antiForgeryToken);
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    if (response.isSuccess) {
                        notify(true, response.result, true);
                        //getAllSubcategories();
                    } else {
                        notify(false, response.errorMessages, false);
                    }

                }
            });
        });
        
    }
    // #endregion :: HallAvailability

});