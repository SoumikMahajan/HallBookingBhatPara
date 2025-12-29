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
                        $('#addCategoryForm').trigger("reset");
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

        $('#categorylist').select2({
            theme: 'bootstrap-5',
            placeholder: '--Select Category--',
            allowClear: true,
            width: '100%'
        });

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
            if (categoryid === '' || categoryid === undefined) {
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
                        $('#addSubCategoryForm').trigger("reset");
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

        $('#hallCategorylist').select2({
            theme: 'bootstrap-5',
            placeholder: '--Select Category--',
            allowClear: true,
            width: '100%'
        });

        const today = flatpickr.formatDate(new Date(), "Y-m-d");
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

        $(document).on('change', '#hallCategorylist', function (e) {
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
                            let html = '<option></option>';

                            response.result.forEach(item => {
                                html += `<option value="${item.id}">${item.name}</option>`;
                            });
                            $('#hallSubCategorylist').html(html);

                            
                        }
                    } else {
                        $('#hallSubCategorylist').html('<option></option>');
                        notify(false, response.errorMessages, false);
                    }

                    $('#hallSubCategorylist').select2({
                        theme: 'bootstrap-5',
                        placeholder: '--Select SubCategory--',
                        allowClear: true,
                        width: '100%'
                    });

                },
                complete: function () {
                    $(".loader").css("display", "none");
                },
                error: function (xhr, status, error) {
                    $('#hallSubCategorylist').html('<option></option>');
                    handleAjaxError(xhr, status, error);
                }
            });
        }); 

        $(document).on('change', '#hallSubCategorylist', function (e) {
            e.preventDefault();
            const SubcategoryId = $(this).val();
            if (!SubcategoryId || SubcategoryId === "0") {
                $("#IsHasFloor").addClass('d-none');
                $('#hallFloorlist').html('<option value="0">-- Select Floor --</option>');
                return;
            }
            $.ajax({
                url: '/Admin/GetFloorListBySubCatId',
                type: 'GET',
                data: { SubCategoryid: SubcategoryId },
                dataType: 'json',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    if (response.isSuccess) {
                        if (response.isSuccess && response.result && response.result.length > 0) {
                            $("#IsHasFloor").removeClass('d-none');
                            let html = '<option value="0">-- Select Floor --</option>';

                            response.result.forEach(item => {
                                html += `<option value="${item.id}">${item.name}</option>`;
                            });
                            $('#hallFloorlist').html(html);
                        }
                        else {
                            $("#IsHasFloor").addClass('d-none');
                        }
                    }
                    else {
                        $("#IsHasFloor").addClass('d-none');
                        $('#hallFloorlist').html('<option value="0">-- Select Floor --</option>');
                        notify(false, response.errorMessages, false);
                    }

                },
                complete: function () {
                    $(".loader").css("display", "none");
                },
                error: function (xhr, status, error) {
                    $("#IsHasFloor").addClass('d-none');
                    $('#hallFloorlist').html('<option value="0">-- Select Floor --</option>');
                    handleAjaxError(xhr, status, error);
                }
            });
        });

        $(document).on('submit', '#addHallAvailabilityForm', function (e) {
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

            var IsHasFloor = $("#IsHasFloor").hasClass('d-none');
            var FloorId = 0;
            if (!IsHasFloor) {
                FloorId = $("#hallFloorlist option:selected").val();
                if (FloorId == 0) {
                    notify(false, "Please Select Floor.", false);
                    $("#hallFloorlist").addClass("is-invalid");
                    return;
                }
                else {
                    $("#hallFloorlist").removeClass("is-invalid");
                }
            }

            const paymentTypeId = $('#paymentType option:selected').val();
            if (paymentTypeId === '0' || paymentTypeId === undefined) {
                notify(false, "Please Select Payment Type.", false);
                $("#paymentType").addClass("is-invalid");
                return;
            }
            else {
                $("#paymentType").removeClass("is-invalid");
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
            var IsHasSecurityMoney = $("#IsHasSecurityMoney").hasClass('d-none');
            var SecurityMoney = '';
            if (!IsHasSecurityMoney) {
                SecurityMoney = $('#securityMoney').val().trim();
                if (SecurityMoney == '')
                {
                    notify(false, "Please Enter Security money.", false);
                    $("#securityMoney").addClass("is-invalid");
                    return;
                }
                else {
                    $("#securityMoney").removeClass("is-invalid");
                }
            }
            

            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            const formData = new FormData();
            formData.append("CategoryId", categoryid);
            formData.append("SubcategoryId", SubcategoryId);
            formData.append("PaymentTypeId", paymentTypeId);
            formData.append("AvailableFrom", AvailableFrom);
            formData.append("AvailableTo", AvailableTo);
            formData.append("ProposedRate", ProposedRate);
            formData.append("SecurityMoney", SecurityMoney);
            formData.append("FloorId", FloorId);


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
                        $('#addHallAvailabilityForm').trigger("reset");
                        getHallAvailability();
                    } else {
                        notify(false, response.errorMessages, true);
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

                            const paymnetType = (item.payment_type_id_fk === 1) ? '(F)' : (item.payment_type_id_fk === 2) ? '(H)' : '';
                            const floorDisplay = (item.floor_id_fk !== 0) ? `(${item.floor_name})` : '';
                            

                            html += `
                                <tr>
                                    <td>${index + 1}</td>
                                    <td>${item.category_name}</td>
                                    <td>${item.hall_name}${floorDisplay} <span style="color: red;">${paymnetType}</span></td>
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

            const UppaymentTypeId = $('#UpPaymentType option:selected').val();
            if (UppaymentTypeId === '0' || UppaymentTypeId === undefined) {
                notify(false, "Please Select Payment Type.", false);
                $("#UpPaymentType").addClass("is-invalid");
                return;
            }
            else {
                $("#UpPaymentType").removeClass("is-invalid");
            } 


            var IsHasSecurityMoney = $("#UpIsHasSecurityMoney").hasClass('d-none');
            var SecurityMoney = '';
            if (!IsHasSecurityMoney) {
                SecurityMoney = $('#UpSecurityMoney').val().trim();
                if (SecurityMoney == '') {
                    notify(false, "Please Enter Security money.", false);
                    $("#UpSecurityMoney").addClass("is-invalid");
                    return;
                }
                else {
                    $("#UpSecurityMoney").removeClass("is-invalid");
                }
            }           

            var IsHasFloor = $("#IsHasFloorForUpdate").hasClass('d-none');
            var FloorId = 0;
            if (!IsHasFloor) {
                FloorId = $("#UphallFloorlist option:selected").val();
                if (FloorId == 0) {
                    notify(false, "Please Select Floor.", false);
                    $("#UphallFloorlist").addClass("is-invalid");
                    return;
                }
                else {
                    $("#UphallFloorlist").removeClass("is-invalid");
                }
            }            


            let antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

            const formData = new FormData();
            formData.append("HallAvailId", HallId);
            formData.append("CategoryId", categoryid);
            formData.append("SubcategoryId", SubcategoryId);
            formData.append("PaymentTypeId", UppaymentTypeId);
            formData.append("AvailableFrom", AvailableFrom);
            formData.append("AvailableTo", AvailableTo);
            formData.append("ProposedRate", ProposedRate);
            formData.append("SecurityMoney", SecurityMoney);
            formData.append("FloorId", FloorId);


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

        $(document).on('change', '#paymentType', function (e) {
            let paymentType = $(this).val();
            if (paymentType === "2") { // Half Payment
                $("#IsHasSecurityMoney").removeClass("d-none");
            } else { // Full Payment or default
                $("#IsHasSecurityMoney").addClass("d-none");
                $("#securityMoney").val("");
            }
        });

        $(document).on('change', '#UpPaymentType', function (e) {
            let UppaymentType = $(this).val();
            if (UppaymentType === "2") { // Half Payment
                $("#UpIsHasSecurityMoney").removeClass("d-none");
            } else { // Full Payment or default
                $("#UpIsHasSecurityMoney").addClass("d-none");
                $("#UpSecurityMoney").val("");
            }
        });
       
    }
    // #endregion :: HallAvailability

    // #region :: users manage list
    if (_ActionName === "userslist") {

        const today = flatpickr.formatDate(new Date(), "Y-m-d");

        let startPicker = flatpickr("#dob", {
            dateFormat: "Y-m-d",
            defaultDate: today
        });


        getAllUserList();

        function getAllUserList() {
            $.ajax({
                url: '/Admin/GetAllUsersList',
                type: 'GET',
                dataType: 'HTML',                
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    if (response != '') {
                        if ($.fn.DataTable.isDataTable('#userListTable')) {
                            $('#userListTable').DataTable().destroy();
                        }
                        $('#partialUserList').html(response);
 
                        $('#userListTable').DataTable({
                            responsive: true,
                            autoWidth: false,
                            lengthMenu: [[50, 100, -1], [50, 100, "All"]],
                            language: {
                                searchPlaceholder: "Search Users...",
                                search: ""
                            },
                            order: [[0, 'asc']]
                        });
                       
                    }
                },
                complete: function () {
                    $(".loader").css("display", "none");
                }
            });
        }

        $(document).on('click', '#openAddUser', function (e) {
            $('#addUserModal').modal('show');

        });

        $(".togglePassword").on("click", function (e) {
            const passwordInput = $('#password');
            const toggleIcon = $(this).find('i');

            if (passwordInput.attr('type') === 'password') {
                passwordInput.attr('type', 'text');
                toggleIcon.removeClass('fa-eye').addClass('fa-eye-slash');
            } else {
                passwordInput.attr('type', 'password');
                toggleIcon.removeClass('fa-eye-slash').addClass('fa-eye');
            }
        });

        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*[\W_]).{6,}$/;

        $(document).on('click', '#adminAdduser', function (e) {
            e.preventDefault();

            if (validateRegistrationForm()) {
                adduser();
            }
        });


        function validateRegistrationForm() {
            let hasError = false;

            const firstName = $('#firstName').val()?.trim() || "";
            const lastName = $('#lastName').val()?.trim() || "";
            const email = $('#email').val()?.trim() || "";
            const phone = $('#phone').val()?.trim() || "";
            const gender = $('#gender').val();
            const role = $('#role').val();
            const dob = $('#dob').val()?.trim() || "";
            const address = $('#address').val()?.trim() || "";
            const city = $('#city').val()?.trim() || "";
            const pincode = $('#pincode').val()?.trim() || "";
            const password = $('#password').val().trim() || "";
            const confirmPassword = $('#confirmPassword').val().trim() || "";

            const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            const phonePattern = /^\d{10}$/;
            const pinPattern = /^\d{6}$/;

            if (firstName === '') {
                showError("#firstName", "Enter a valid First Name.");
                hasError = true;
            } else {
                showValid('#firstName');
            }

            if (lastName === '') {
                showError("#lastName", "Enter a valid Last Name.");
                hasError = true;
            } else {
                showValid('#lastName');
            }

            if (!emailPattern.test(email)) {
                showError("#email", "Enter a valid email address.");
                hasError = true;
            } else {
                showValid('#email');
            }

            if (!phonePattern.test(phone)) {
                showError("#phone", "Enter a 10-digit mobile number.");
                hasError = true;
            } else {
                showValid('#phone');
            }

            if (gender === '') {
                showError("#gender", "Please select gender.");
                hasError = true;
            } else {
                showValid('#gender');
            }

            if (role === '') {
                showError("#role", "Please select role.");
                hasError = true;
            } else {
                showValid('#role');
            }

            if (!dob) {
                showError("#dob", "Please enter birthdate.");
                hasError = true;
            } else {
                showValid('#dob');
            }

            if (!address) {
                showError("#address", "Enter full address.");
                hasError = true;
            } else {
                showValid('#address');
            }

            if (!city) {
                showError("#city", "Enter city.");
                hasError = true;
            } else {
                showValid('#city');
            }

            if (!pinPattern.test(pincode)) {
                showError("#pincode", "Enter a 6-digit PIN code.");
                hasError = true;
            } else {
                showValid('#pincode');
            }
            if (password === '') {
                showError('#password', 'Enter a password.');
                hasError = true;
            }

            else if (!passwordRegex.test(password)) {
                showError(
                    '#password',
                    'Password must be 6+ chars with uppercase, lowercase & special character.'
                );
                hasError = true;
            } else {
                showValid('#password');
            }

            if (confirmPassword === '') {
                showError('#confirmPassword', 'Confirm your password.');
                hasError = true;
            }
            // 4️⃣ Match validation
            else if (password !== confirmPassword) {
                showError('#confirmPassword', 'Passwords do not match.');
                hasError = true;
            } else {
                showValid('#confirmPassword');
            }

            return !hasError;
        }

        function adduser() {
            const formData = new FormData();

            formData.append("FirstName", $('#firstName').val()?.trim() || "");
            formData.append("LastName", $('#lastName').val()?.trim() || "");
            formData.append("Email", $('#email').val()?.trim() || "");
            formData.append("Phone", $('#phone').val()?.trim() || "");
            formData.append("Gender", $('#gender').val());
            formData.append("Role", $('#role').val());
            formData.append("DOB", $('#dob').val()?.trim() || "");
            formData.append("Address", $('#address').val()?.trim() || "");
            formData.append("City", $('#city').val()?.trim() || "");
            formData.append("Pincode", $('#pincode').val()?.trim() || "");
            formData.append("Password", $('#password').val());
            formData.append("BasePassword", $('#password').val());

            $.ajax({
                url: '/Admin/AddUser',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    $('#adminAdduser').prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Registering...');
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    if (response.isSuccess) {
                        notify(true, response.result, true);
                        getAllUserList();
                        $('#addUserModal').modal('hide');
                    }
                    else {
                        notify(false, response.errorMessages, false);
                    }
                },
                complete: function () {
                    $('#adminAdduser').prop('disabled', false).html('<i class="fas fa-user-plus me-2"></i>Create Account');
                }
            });
        }

        function showError(selector, message) {
            const $input = $(selector);
            const $parent = $input.closest(".form-floating, .input-group, .form-check");

            $input.addClass("is-invalid");
            $parent.find(".invalid-feedback").text(message).show();

            // Auto-clear after 2 seconds
            setTimeout(() => {
                $input.removeClass("is-invalid");
                $parent.find(".invalid-feedback").hide();
            }, 2000);
        }

        function showValid(input) {
            const $input = $(input);
            const $container = $input.closest('.form-floating, .input-group, .form-check');

            $input.removeClass('is-invalid');
            $container.find('.invalid-feedback').hide();
        } 



        $(document).on('click', '.editUser', function (e) {

            var userId = $(this).data('userid');
            var roleId = $(this).data('roleid');

            $.ajax({
                url: '/Admin/GetUserById',
                type: 'GET',
                data: { UserId: userId, roleId: roleId },
                dataType: 'HTML',
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    if (response != '') {
                        $('#updateUserForm').html(response);

                        flatpickr("#UpDob", {
                            dateFormat: "Y-m-d",
                            defaultDate: today
                        });

                        $('#editUserModal').modal('show');

                    }
                    else {
                        notify(false, 'Unable to fetch user details.', false);
                    }
                },
                complete: function () {
                    $(".loader").css("display", "none");
                }
            });

        });

        $(document).on('click', '#adminUpdateUser', function (e) {
            e.preventDefault();

            if (validateUpdateUserForm()) {
                upDateUser();
            }
        });

        function validateUpdateUserForm() {
            let hasError = false;

            const firstName = $('#UpFirstName').val()?.trim() || "";
            const lastName = $('#UpLastName').val()?.trim() || "";           
            const gender = $('#UpGender').val();
            //const role = $('#UpRole').val();
            const dob = $('#UpDob').val()?.trim() || "";
            const address = $('#UpAddress').val()?.trim() || "";
            const city = $('#UpCity').val()?.trim() || "";
            const pincode = $('#UpPincode').val()?.trim() || "";           
            const pinPattern = /^\d{6}$/;


            if (firstName === '') {
                showError("#UpFirstName", "Enter a valid First Name.");
                hasError = true;
            } else {
                showValid('#UpFirstName');
            }

            if (lastName === '') {
                showError("#UpLastName", "Enter a valid Last Name.");
                hasError = true;
            } else {
                showValid('#UpLastName');
            }            

            if (gender === '') {
                showError("#UpGender", "Please select gender.");
                hasError = true;
            } else {
                showValid('#UpGender');
            }

            

            if (!dob) {
                showError("#UpDob", "Please enter birthdate.");
                hasError = true;
            } else {
                showValid('#UpDob');
            }

            if (!address) {
                showError("#UpAddress", "Enter full address.");
                hasError = true;
            } else {
                showValid('#UpAddress');
            }

            if (!city) {
                showError("#UpCity", "Enter city.");
                hasError = true;
            } else {
                showValid('#UpCity');
            }

            if (!pinPattern.test(pincode)) {
                showError("#UpPincode", "Enter a 6-digit PIN code.");
                hasError = true;
            } else {
                showValid('#UpPincode');
            }

            return !hasError;
        }

        function upDateUser() {
            const formData = new FormData();

            formData.append("UserId", $('#UpUserId').val()?.trim() || "");
            formData.append("RoleId", $('#UproleId').val()?.trim() || "");
            formData.append("FirstName", $('#UpFirstName').val()?.trim() || "");
            formData.append("LastName", $('#UpLastName').val()?.trim() || "");            
            formData.append("Gender", $('#UpGender').val());
            formData.append("DOB", $('#UpDob').val()?.trim() || "");
            formData.append("Address", $('#UpAddress').val()?.trim() || "");
            formData.append("City", $('#UpCity').val()?.trim() || "");
            formData.append("Pincode", $('#UpPincode').val()?.trim() || "");


            $.ajax({
                url: '/Admin/UpdateUser',
                type: 'POST',
                data: formData,
                contentType: false,
                processData: false,
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    $('#adminUpdateUser').prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Updateing...');
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    if (response.isSuccess) {
                        notify(true, response.result, true);

                        $('#editUserModal').modal('hide');
                        getAllUserList();
                    }
                    else {
                        notify(false, response.errorMessages, false);
                    }
                },
                complete: function () {
                    $('#adminUpdateUser').prop('disabled', false).html('<i class="fas fa-user-plus me-2"></i>Update Account');
                }
            });
        }

        $(document).on('click', '.ChangePassword', function (e) {

            var userId = $(this).data('userid');
            var emailId = $(this).data('emailid');

            $("#passChangeUserid").val(userId);
            $("#passChangeEmailId").val(emailId);

            $('#UserPasswordModal').modal('show');

        });

        $(".togglePasswordForChange").on("click", function (e) {
            const passwordInput = $('#UpPassword');
            const toggleIcon = $(this).find('i');

            if (passwordInput.attr('type') === 'password') {
                passwordInput.attr('type', 'text');
                toggleIcon.removeClass('fa-eye').addClass('fa-eye-slash');
            } else {
                passwordInput.attr('type', 'password');
                toggleIcon.removeClass('fa-eye-slash').addClass('fa-eye');
            }
        });

        

        $(document).on('click', '#adminUpdateUserPass', function (e) {
            e.preventDefault();

            let hasError = false;

            const password = $('#UpPassword').val().trim();
            const confirmPassword = $('#UpConfirmPassword').val().trim();

            if (password === '') {
                showError('#UpPassword', 'Enter a password.');
                hasError = true;
            }

            else if (!passwordRegex.test(password)) {
                showError(
                    '#UpPassword',
                    'Password must be 6+ chars with uppercase, lowercase & special character.'
                );
                hasError = true;
            } else {
                showValid('#UpPassword');
            }

            if (confirmPassword === '') {
                showError('#UpConfirmPassword', 'Confirm your password.');
                hasError = true;
            }
            // 4️⃣ Match validation
            else if (password !== confirmPassword) {
                showError('#UpConfirmPassword', 'Passwords do not match.');
                hasError = true;
            } else {
                showValid('#UpConfirmPassword');
            }

            if (hasError) return;

            var UserId = $("#passChangeUserid").val();
            var EmailId = $("#passChangeEmailId").val();

            $.ajax({
                url: '/Admin/UpdateUserPassWord',
                type: 'POST',
                data: { Password: password, UserId: UserId, EmailId: EmailId },
                dataType: 'JSON',
                beforeSend: function (xhr) {
                    $(".loader").css("display", "flex");
                    $('#adminUpdateUserPass').prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-2"></i>Updateing...');
                },
                success: function (response) {
                    $(".loader").css("display", "none");
                    if (response.isSuccess) {
                        notify(true, response.result, true);
                        getAllUserList();
                        $('#UserPasswordModal').modal('hide');
                    }
                    else {
                        notify(false, response.errorMessages, false);
                    }
                },
                complete: function () {
                    $('#adminUpdateUserPass').prop('disabled', false).html('<i class="fas fa-user-plus me-2"></i>Change Password');
                }
            });
        });
    }
    // #endregion :: users manage list

    // #region :: user Booking manage list
    if (_ActionName === "usersbookings") {
       
       

        getAllUserBookingList();

        function getAllUserBookingList() {
            $.ajax({
                url: '/Admin/GetAllHallBookingList',
                type: 'GET',
                dataType: 'HTML',
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    if (response != '') {                       
                        // Destroy existing DataTable if it exists
                        if ($.fn.DataTable.isDataTable('#adminbookingsTable')) {
                            $('#adminbookingsTable').DataTable().destroy();
                        }

                        $('#partialGetAllHallBooking').html(response);

                        // Initialize DataTable
                        $('#adminbookingsTable').DataTable({
                            responsive: true,
                            autoWidth: false,
                            lengthMenu: [[50, 100, -1], [50, 100, "All"]],
                            language: {
                                searchPlaceholder: "Search Users...",
                                search: ""
                            },
                            order: [[0, 'asc']]
                        });
                    }
                },
                complete: function () {
                    $(".loader").css("display", "none");
                }
            });
        }

        $(document).on('click', '#openAdvancedSearch', function (e) {
            $('#modalOverlay').fadeIn(300);
            $('#advancedSearchModal').fadeIn(300);
        });

        function closeModal() {
            $('#modalOverlay').fadeOut(300);
            $('#advancedSearchModal').fadeOut(300);
        }

        $(document).on('click', '#closeModal, #modalOverlay', function (e) {
            closeModal();
        });

        $(document).on('click', '#advancedSearchModal', function (e) {
            e.stopPropagation();
        });

        $(document).on('change', '#statusAll', function (e) {
            if ($(this).is(':checked')) {
                $('.status-checkbox').not('#statusAll').prop('checked', false);
            }
        });

        $(document).on('change', '.status-checkbox:not(#statusAll)', function () {
            if ($(this).is(':checked')) {
                $('#statusAll').prop('checked', false);
            }

            // If no checkbox is selected, check "All"
            if ($('.status-checkbox:checked').length === 0) {
                $('#statusAll').prop('checked', true);
            }
        });

        $(document).on('click', '#resetFilters', function (e) {
            $('#fromDate, #toDate, #searchHall').val('');
            $('.status-checkbox').prop('checked', false);
            $('#statusAll').prop('checked', true);
        });

        $(document).on('click', '#applyFilters', function (e) {
            const filters = {
                statuses: [],
                fromDate: $('#fromDate').val(),
                toDate: $('#toDate').val(),
                hallName: $('#searchHall').val()
            };

            // Get selected statuses
            if ($('#statusAll').is(':checked')) {
                filters.statuses.push('all');
            } else {
                $('.status-checkbox:checked').each(function () {
                    const statusId = $(this).attr('id').replace('status', '').toLowerCase();
                    filters.statuses.push(statusId);
                });
            }

            console.log('Applied Filters:', filters);

            // Add your filtering logic here

            closeModal();
        });


        // Close modal with Escape key
        $(document).on('keydown', function (e) {
            if (e.key === 'Escape') {
                closeModal();
            }
        });

        $(document).on('click', '.table-btn-approve', function (e) {
            var BookingReferenceId = $(this).data('booking-ref-id');
            var BookingId = $(this).data('booking-id');

            Swal.fire({
                title: 'Confirm Approval',
                html: `Are you sure you want to approve this booking?<br><br><strong>Booking ID:</strong> ${BookingReferenceId}`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#28a745',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Yes, Approve',
                cancelButtonText: 'Cancel',
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    // Call the approval function
                    approveBooking(BookingId, BookingReferenceId);
                }
            });


        });

        function approveBooking(BookingId, BookingReferenceId) {
            $.ajax({
                url: '/Admin/ApproveBookedHall',
                type: 'POST',
                dataType: 'JSON',
                data: { BookingId: BookingId, BookingReferenceId: BookingReferenceId },
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    if (response.isSuccess) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Approved Submitted Successfully!',
                            html: response.result || 'Booking has been approved.',
                            confirmButtonText: 'OK',
                            confirmButtonColor: '#28a745',
                            allowOutsideClick: false,
                            timer: 10000,
                            timerProgressBar: true
                        }).then((result) => {
                            // Redirect if user clicked OK or timer expired
                            if (result.isConfirmed || result.dismiss === Swal.DismissReason.timer) {
                                getAllUserBookingList();
                            }
                        });

                    }
                    else {
                        notify(false, response.errorMessages, false);
                    }
                },
                complete: function () {
                    $(".loader").css("display", "none");
                }
            });
        }

        $(document).on('click', '.table-btn-reject', function (e) {
            var BookingReferenceId = $(this).data('booking-ref-id');
            var BookingId = $(this).data('booking-id');

            Swal.fire({
                title: 'Confirm Reject',
                html: `Are you sure you want to reject this booking?<br><br><strong>Booking ID:</strong> ${BookingReferenceId}`,
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Yes, Reject',
                cancelButtonText: 'Cancel',
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    // Call the approval function
                    rejectBooking(BookingId, BookingReferenceId);
                }
            });


        });

        function rejectBooking(BookingId, BookingReferenceId) {
            $.ajax({
                url: '/Admin/RejectBookedHall',
                type: 'POST',
                dataType: 'JSON',
                data: { BookingId: BookingId, BookingReferenceId: BookingReferenceId },
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    if (response.isSuccess) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Rejected Successfully!',
                            html: response.result || 'Your booking has been rejected.',
                            confirmButtonText: 'OK',
                            confirmButtonColor: '#28a745',
                            allowOutsideClick: false,
                            timer: 10000,
                            timerProgressBar: true
                        }).then((result) => {
                            // Redirect if user clicked OK or timer expired
                            if (result.isConfirmed || result.dismiss === Swal.DismissReason.timer) {
                                getAllUserBookingList();
                            }
                        });

                    }
                    else {
                        notify(false, response.errorMessages, false);
                    }
                },
                complete: function () {
                    $(".loader").css("display", "none");
                }
            });
        }

        $(document).on('click', '.btnHallBookedDetails', function (e) {
            e.preventDefault();

            // Get booking data from the clicked element or its parent
            const hallId = $(this).data('hallid');
            const hallName = $(this).data('hallname');

            // Populate modal with data (optional)
            $('#bookingDetailsModal .modal-title').text('Booking Details - ' + hallName);

            $.ajax({
                url: '/UserBooking/UserBookingDetailsById',
                type: 'GET',
                data: { HallId: hallId },
                dataType: 'HTML',
                beforeSend: function () {
                    $(".loader").css("display", "flex");
                },
                success: function (response) {
                    $('#partialBookedHallDetails').html('');
                    $('#partialBookedHallDetails').html(response);
                    $('#bookingDetailsModal').modal('show');
                },
                complete: function () {
                    $(".loader").css("display", "none");
                }
            });


        });
        
    }
    // #endregion :: users Booking list

});