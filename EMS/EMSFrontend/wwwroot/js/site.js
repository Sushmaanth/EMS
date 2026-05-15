document.addEventListener("DOMContentLoaded", function () {
    const employeeAddedSuccessMessage = document.getElementById("successfullyCreatedEmployee");

    const employeeUpdatedSuccessMessage = document.getElementById("employeeUpdateSuccessully");

    const deleteForms = document.querySelectorAll(".delete-form");

    deleteForms.forEach(form => form.addEventListener("submit", function (e) {
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
    }));

    const employeeDeletedSuccessullyMessage = document.getElementById("employeeDeletedSuccessully");

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
