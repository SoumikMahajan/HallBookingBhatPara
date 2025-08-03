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
                                        <button class="btn btn-sm btn-warning btn-edit" data-id="${item.category_id_pk}" data-name="${item.category_name}">
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
                //dom: 'lBfrtip', // <-- B = Buttons, l = lengthMenu, f = filter, r = processing, t = table, i = info, p = pagination
                //buttons: ['pdfHtml5', 'print'],
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

        $(document).on('click', '.btn-edit', function () {
            const id = $(this).data('id');
            const name = $(this).data('name');

            $('#editCategoryId').val(id);
            $('#editCategoryName').val(name);

            $('#editCategoryModal').modal('show');
        });

        $('#editCategoryForm').on('submit', function (e) {
            e.preventDefault();

            const CatId = $('#editCategoryId').val();
            if (CatId === "" || CatId === "0") {
                notify(false, "Category Id cannot be empty.", false);              
                return;
            }

            const categoryname = $('#editCategoryName').val();            
            if (categoryname === "") {
                notify(false, "Category name cannot be empty.", false);
                $("#editCategoryName").addClass("is-invalid");
                return;
            }
            else {
                $("#editCategoryName").removeClass("is-invalid");
            }


            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/Admin/UpdateCategory',
                type: 'POST',
                data: { categoryId: CatId,categoryName: categoryname },
                dataType: 'json',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    xhr.setRequestHeader("RequestVerificationToken", antiForgeryToken);
                    $('#updateCatText').text("Updateing...").prop('disabled', true);
                },
                success: function (response) {
                    if (response.isSuccess) {
                        notify(true, response.result, true);
                        getAllcategories();
                        $('#editCategoryModal').modal('hide');
                    } else {
                        notify(false, response.errorMessages, false);
                    }

                },
                complete: function () {
                    $(".loader").css("display", "none");
                    $('#updateCatText').text("Updateing...").prop('disabled', true);
                },
            });
        });

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
            const maxSize = 2 * 1024 * 1024; // 2MB

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
                //dom: 'lBfrtip', // <-- B = Buttons, l = lengthMenu, f = filter, r = processing, t = table, i = info, p = pagination
                //buttons: ['pdfHtml5', 'print'],
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

        $(document).on('click', '.btn-edit', function () {
            const id = $(this).data('id');
            $.ajax({
                url: '/Admin/GetSubCategoryById',
                type: 'GET',
                dataType: 'HTML',
                data: { SubCategoryId: id },
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    $('#parialSubCatEditModal').html('');
                    $('#parialSubCatEditModal').html(response);
                    $('#editSubCategoryModal').modal('show');
                }
            });          
            
        });

        

       

        $(document).on('submit', '#editSubCategoryForm', function (e) {
            e.preventDefault();

            const Subcategoryid = $('#editSubCategoryId').val();
            if (Subcategoryid === '0' || Subcategoryid === '') {
                return;
            }

            const categoryid = $('.editcategorylist option:selected').val();
            if (categoryid === "0") {
                notify(false, "Select Category from list.", false);
                $(".editcategorylist").addClass("is-invalid");
                return;
            }
            else {
                $(".editcategorylist").removeClass("is-invalid");
            }
            const Subcategoryname = $('#editHallName').val().trim();
            if (Subcategoryname === "") {
                notify(false, "SubCategory name cannot be empty.", false);
                $("#editHallName").addClass("is-invalid");
                return;
            }
            else {
                $("#editHallName").removeClass("is-invalid");
            }
            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            const formData = new FormData();
            formData.append("Subcategoryid", Subcategoryid);
            formData.append("CategoryId", categoryid);
            formData.append("SubCategoryName", Subcategoryname);

            const fileInput = $('#EditSubCatUpload')[0];

            // Check if no file selected
            if (!fileInput || !fileInput.files || fileInput.files.length === 0) {
                const hasExistingImage = $('#previewImage').attr('src')?.trim() !== '';
                if (!hasExistingImage) {
                    notify(false, "Please upload a hall image.", false);
                    return;
                }
                formData.append("HasNewImage", "false");
            } else {
                const file = fileInput.files[0];

                const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg'];
                const maxSize = 2 * 1024 * 1024;

                if (!allowedTypes.includes(file.type)) {
                    notify(false, "Only JPG or PNG files allowed.", false);
                    return;
                }

                if (file.size > maxSize) {
                    notify(false, "Image must be less than 2 MB.", false);
                    return;
                }
                formData.append("HasNewImage", "true");
                formData.append("fileUpload", file);
            }

            for (var pair of formData.entries()) {
                console.log(`${pair[0]}:`, pair[1]);
            }

            $.ajax({
                url: '/Admin/UpdateSubCategory',
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
                        $('#editSubCategoryModal').modal('hide');
                    } else {
                        notify(false, response.errorMessages, false);
                    }

                }
            });
        });

        const maxFileSizeMB = 2;
        const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg'];
        
       
        $(document).on('change', '#EditSubCatUpload', function () {
            const file = this.files[0];
            const $preview = $('#previewImage');
            const $fileText = $('#editSubfileText');                       

            if (!file) {
                resetImage();
                return;
            }

            // Check file type
            if (!allowedTypes.includes(file.type)) {
                notify(false, "Only JPG, PNG or WEBP images are allowed.", false);
                resetImage();
                return;
            }

            // Check file size
            const sizeInMB = file.size / (1024 * 1024);
            if (sizeInMB > maxFileSizeMB) {
                notify(false, "File size must be less than 2MB.", false);
                resetImage();
                return;
            }

            // Preview image
            const reader = new FileReader();
            reader.onload = function (e) {
                $preview.attr('src', e.target.result).removeClass('d-none');               
            };
            reader.readAsDataURL(file);

            $fileText.text(file.name);

        });

       

        // Utility: reset image UI
        function resetImage() {
            $('#EditSubCatUpload').val('');
            $('#previewImage').attr('src', '#').addClass('d-none');            
            $('#editSubfileText').text('Browse... No file selected.');
        }

        
    }
    // #endregion :: SubCategory


    // #region :: HallAvailability
    if (_ActionName === "addhallavailabilitydetails") {

        const today = new Date().toISOString().split('T')[0];
        $('#availableFrom, #availableTo').attr('min', today);

        $(document).on('change', '#availableFrom', function () {
            const startDate = this.value;
            $('#availableTo').attr('min', startDate);
            if ($('#availableTo').val() && $('#availableTo').val() < startDate) {
                $('#availableTo').val('');
            }
        });
        $(document).on('change', '#availableTo', function () {
            const endDate = this.value;
            $('#availableFrom').attr('max', endDate);
            if ($('#availableFrom').val() && $('#availableFrom').val() > endDate) {
                $('#availableFrom').val('');
            }
        });

        $(document).on('change', '#UpAvailableFrom', function () {
            const startDate = this.value;
            $('#UpAvailableTo').attr('min', startDate);
            if ($('#UpAvailableTo').val() && $('#UpAvailableTo').val() < startDate) {
                $('#UpAvailableTo').val('');
            }
        });
        $(document).on('change', '#UpAvailableTo', function () {
            const endDate = this.value;
            $('#UpAvailableFrom').attr('max', endDate);
            if ($('#UpAvailableFrom').val() && $('#UpAvailableFrom').val() > endDate) {
                $('#UpAvailableFrom').val('');
            }
        });
        
        getHallAvailability();        

        $(document).on('change', 'hallCategorylist', function () {
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

        $(document).on('submit', '#addHallAvailabilityForm', function () {
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
                        getHallAvailability();
                    } else {
                        notify(false, response.errorMessages, false);
                    }

                }
            });
        });        

        function getHallAvailability() {
            $.ajax({
                url: '/Admin/GetAllHallAvailabilityList',
                type: 'GET',
                dataType: 'json',
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    // Destroy existing DataTable BEFORE updating table body
                    if ($.fn.DataTable.isDataTable('.HallAvailabilityTable')) {
                        $('.HallAvailabilityTable').DataTable().destroy();
                    }
                    if (response.isSuccess && response.result && response.result.length > 0) {
                        const hallavail = response.result;
                        let html = '';
                        hallavail.forEach((item, index) => {
                            html += `
                                <tr>
                                    <td>${index + 1}</td>
                                    <td>${item.category_name}</td>
                                    <td>${item.hall_name}</td>
                                    <td>${item.hall_availability_from_date.split('T')[0]}</td>
                                    <td>${item.hall_availability_to_date.split('T')[0]}</td>
                                    <td>${item.rate}</td>
                                    <td>${item.security_money}</td>                                   
                                    <td>
                                        <button class="btn btn-sm btn-warning btn-edit" data-id="${item.hall_availability_id_pk}">
                                            <i class="fas fa-edit"></i>
                                        </button>
                                    </td>
                                </tr>
                            `;
                        });
                        $('#HallAvailabilityTableBody').html(html);

                    }
                    bindDataTable();
                }
            });
        }

        function bindDataTable() {
            $('.HallAvailabilityTable').DataTable({
                //dom: 'lBfrtip', // <-- B = Buttons, l = lengthMenu, f = filter, r = processing, t = table, i = info, p = pagination
                //buttons: ['pdfHtml5', 'print'],
                responsive: true,
                autoWidth: false,
                lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
                language: {
                    searchPlaceholder: "Search hall...",
                    search: ""
                },
                columnDefs: [
                    { targets: -1, orderable: false }
                ]
            });
        }

        $(document).on('click', '.btn-edit', function () {
            const id = $(this).data('id');
            $.ajax({
                url: '/Admin/GetHallAvailById',
                type: 'GET',
                dataType: 'HTML',
                data: { hallId: id },
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    $('#parialHallAvailEditModal').html('');
                    $('#parialHallAvailEditModal').html(response);
                    $('#editHallAvailModal').modal('show');
                }
            });

        });
        $(document).on('change', '#hallAvailCategorylist', function (e) {
            e.preventDefault();
            const categoryId = $(this).val();
            if (!categoryId || categoryId === "0") {
                $('#hallAvailSubCategorylist').html('<option value="0">-- Select Subcategory --</option>');
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
                            $('#hallAvailSubCategorylist').html(html);
                        }
                    } else {
                        $('#hallAvailSubCategorylist').html('<option value="0">-- Select Subcategory --</option>');
                        notify(false, response.errorMessages, false);
                    }

                },
                complete: function () {
                    $(".loader").css("display", "none");
                },
                error: function (xhr, status, error) {
                    $('#hallAvailSubCategorylist').html('<option value="0">-- Select Subcategory --</option>');
                    handleAjaxError(xhr, status, error);
                }
            });

        });

        $(document).on('submit', '#editHallAvailForm', function (e) {
            e.preventDefault();
            const HallId = $('#editHallAvailId').val();


            const categoryid = $('#hallAvailCategorylist option:selected').val();
            if (categoryid === '0' || categoryid === undefined) {
                notify(false, "Please Select Category.", false);
                $("#hallAvailCategorylist").addClass("is-invalid");
                return;
            }
            else {
                $("#hallAvailCategorylist").removeClass("is-invalid");
            }
            const SubcategoryId = $('#hallAvailSubCategorylist option:selected').val();
            if (SubcategoryId === '0' || SubcategoryId === undefined) {
                notify(false, "Please Select Sub Category.", false);
                $("#hallAvailSubCategorylist").addClass("is-invalid");
                return;
            }
            else {
                $("#hallAvailSubCategorylist").removeClass("is-invalid");
            }

            const AvailableFrom = $('#UpAvailableFrom').val();
            if (AvailableFrom === '' || AvailableFrom === undefined) {
                notify(false, "Please Select Date.", false);
                $("#UpAvailableFrom").addClass("is-invalid");
                return;
            }
            else {
                $("#UpAvailableFrom").removeClass("is-invalid");
            }

            const AvailableTo = $('#UpAvailableTo').val();
            if (AvailableTo === '' || AvailableTo === undefined) {
                notify(false, "Please Select Date.", false);
                $("#UpAvailableTo").addClass("is-invalid");
                return;
            }
            else {
                $("#UpAvailableTo").removeClass("is-invalid");
            }

            const ProposedRate = $('#UpProposedRate').val().trim();
            if (ProposedRate === '' || ProposedRate === undefined) {
                notify(false, "Please Fill Proposed Rate.", false);
                $("#UpProposedRate").addClass("is-invalid");
                return;
            }
            else {
                $("#UpProposedRate").removeClass("is-invalid");
            }

            const SecurityMoney = $('#UpSecurityMoney').val().trim();
            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            const formData = new FormData();
            formData.append("HallId", HallId);
            formData.append("CategoryId", categoryid);
            formData.append("SubcategoryId", SubcategoryId);
            formData.append("AvailableFrom", AvailableFrom);
            formData.append("AvailableTo", AvailableTo);
            formData.append("ProposedRate", ProposedRate);
            formData.append("SecurityMoney", SecurityMoney);


            $.ajax({
                url: '/Admin/UpdateHallAvailable',
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
                        $('#editHallAvailModal').modal('hide');
                        getHallAvailability();                        
                    } else {
                        notify(false, response.errorMessages, false);
                    }

                }
            });
        });
       
    }
    // #endregion :: HallAvailability

});