document.addEventListener("DOMContentLoaded", function () {

    const employeeAddedSuccessMessage =
        document.getElementById("successfullyCreatedEmployee");

    const employeeUpdatedSuccessMessage =
        document.getElementById("employeeUpdateSuccessully");

    const employeeDeletedSuccessullyMessage =
        document.getElementById("employeeDeletedSuccessully");

    const deleteForms =
        document.querySelectorAll(".delete-form");

    const searchBox =
        document.getElementById("searchBox");

    const pageSizeDropdown =
        document.getElementById("pageSize");

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

    

    if (employeeAddedSuccessMessage) {

        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: employeeAddedSuccessMessage.value,
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

});