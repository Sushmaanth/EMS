(() => {

    document.addEventListener("DOMContentLoaded", function () {
        initializeLeaveReview();

        new DataTable('#team-leaves', {
            pageLength: 10,
            lengthChange: false,
            ordering: false,
            searching: false
        });
    });

    //review
    function initializeLeaveReview() {

        const modalElement = document.getElementById("reviewLeaveModal");

        if (!modalElement) {
            return;
        }

        const modal = new bootstrap.Modal(modalElement);

        document.addEventListener("click", function (e) {

            const button = e.target.closest(".review-btn");

            if (!button) {
                return;
            }

            console.log("Review button clicked");

            document.getElementById("LeaveRequestId").value = button.dataset.id;

            document.getElementById("IsApproved").value = button.dataset.approved;

            const comments = document.getElementById("ManagerComments");

            comments.value = "";
            comments.classList.remove("is-invalid");

            modal.show();
        });

        const form = modalElement.querySelector("form");

        form.addEventListener("submit", function (e) {

            const isApproved = document.getElementById("IsApproved").value === "true";

            const comments = document.getElementById("ManagerComments");

            if (!isApproved &&
                comments.value.trim() === "") {

                e.preventDefault();

                comments.classList.add("is-invalid");

                return;
            }

            comments.classList.remove("is-invalid");
        });
    }

})();