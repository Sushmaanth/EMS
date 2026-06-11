document.addEventListener("DOMContentLoaded", function () {

    const employeeAddedSuccessMessage =
        document.getElementById("successfullyCreatedEmployee");

    const employeeUpdatedSuccessMessage =
        document.getElementById("employeeUpdateSuccessully");

    const employeeDeletedSuccessullyMessage =
        document.getElementById("employeeDeletedSuccessully");

    const successfullyAccountActivated =
        document.getElementById("successfullyAccountActivated");

    const successfullyUserLoggegIn =
        document.getElementById("successfullyUserLoggegIn");

    const deleteForms =
        document.querySelectorAll(".delete-form");

    const searchBox =
        document.getElementById("searchBox");

    const pageSizeDropdown =
        document.getElementById("pageSize");

    const sessionExpired =
        document.getElementById("sessionExpired");

    const oTPSentSuccessMessage =
        document.getElementById("oTPSentSuccessMessage");

    const passwordSuccessResetMessage =
        document.getElementById("passwordSuccessResetMessage");

    const microsoftLoginErrorMessage =
        document.getElementById("microsoftLoginErrorMessage");

    const loggedOutSuccessfully =
        document.getElementById("loggedOutSuccessfully");

    const tenantError =
        document.getElementById("TenantError");

    let debounceTimer;

    function loadEmployees(searchValue) {

        const pageSize =
            pageSizeDropdown.value;

        fetch(`/Home/SearchEmployees?searchText=${encodeURIComponent(searchValue)}&pageSize=${pageSize}`)
            .then(response => response.text())
            .then(data => {
                document.getElementById("employee-table").innerHTML = data;
            });
    }


    if (searchBox) {

        searchBox.addEventListener("input", function () {

            clearTimeout(debounceTimer);

            let searchValue = this.value;

            debounceTimer = setTimeout(() => {

                loadEmployees(searchValue);

            }, 500);

        });
    }

    if (pageSizeDropdown) {

        pageSizeDropdown.addEventListener("change", function () {

            let searchValue = "";

            if (searchBox) {
                searchValue = searchBox.value;
            }

            loadEmployees(searchValue);

        });
    }


    deleteForms.forEach(form =>
        form.addEventListener("submit", function (e) {

            e.preventDefault();

            Swal.fire({
                title: 'Are you sure?',
                text: "This employee will be deleted.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Yes, delete it!'
            })
                .then((result) => {

                    if (result.isConfirmed) {

                        form.submit();

                    }
                });
        })
    );


    if (employeeDeletedSuccessullyMessage) {

        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: employeeDeletedSuccessullyMessage.value,
            confirmButtonColor: '#3085d6'
        });
    }

    if (oTPSentSuccessMessage) {

        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: oTPSentSuccessMessage.value,
            confirmButtonColor: '#3085d6',
            heightAuto: false
        });
    }

    if (loggedOutSuccessfully) {

        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: loggedOutSuccessfully.value,
            confirmButtonColor: '#3085d6',
            heightAuto: false
        });
    }

    if (microsoftLoginErrorMessage) {

        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: microsoftLoginErrorMessage.value,
            confirmButtonColor: '#3085d6',
            heightAuto: false
        });
    }

    if (tenantError) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: tenantError.value,
            confirmButtonColor: '#3085d6',
            heightAuto: false
        });
    }

    if (passwordSuccessResetMessage) {

        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: passwordSuccessResetMessage.value,
            confirmButtonColor: '#3085d6',
            heightAuto: false
        });
    }

    if (sessionExpired) {
        Swal.fire({
            icon: 'warning',
            title: '⏳',
            text: sessionExpired.value,
            confirmButtonColor: '#3085d6',
            heightAuto: false
        });
    }

    if (employeeAddedSuccessMessage) {

        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: employeeAddedSuccessMessage.value,
            confirmButtonColor: '#3085d6'
        });
    }

    if (successfullyAccountActivated) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: successfullyAccountActivated.value,
            confirmButtonColor: '#3085d6',
            heightAuto: false
        });
    }

    if (successfullyUserLoggegIn) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: successfullyUserLoggegIn.value,
            confirmButtonColor: '#3085d6'
        });
    }   

    if (employeeUpdatedSuccessMessage) {

        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: employeeUpdatedSuccessMessage.value,
            confirmButtonColor: '#3085d6'
        });
    }

    //cdn pagenation
    new DataTable('#errorTable', {
        pageLength: 5,
        lengthChange: false,
        ordering: false,
        searching: true
    });
});