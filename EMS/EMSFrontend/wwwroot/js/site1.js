document.addEventListener("DOMContentLoaded", function () {

    //ajax document category
    const documentCategoryButtons =
        document.querySelectorAll(".category-btn");

    const successfullyUserLoggegIn =
        document.getElementById("successfullyUserLoggegIn");

    documentCategoryButtons.forEach(documentCategoryButton => {

        documentCategoryButton.addEventListener("click", async () => {

            const categoryId =
                documentCategoryButton.getAttribute("data-categoryid");

            console.log(categoryId);

            documentCategoryButtons.forEach(button =>
                button.classList.remove("active"));

            documentCategoryButton.classList.add("active");

            const response =
                await fetch(`/Employee/GetDocumentCards?categoryId=${categoryId}`);

            const html =
                await response.text();

            document.getElementById("documentCardsContainer")
                .innerHTML = html;
        });

    });

    //upload document event for container and browse button
    document.querySelectorAll(".upload-box").forEach(uploadBox => {

        uploadBox.addEventListener("click", () => {

            const fileInput =
                uploadBox.parentElement.querySelector("#file-upload");

            fileInput.click();

        });

    });

    //after login success message

    if (successfullyUserLoggegIn) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            text: successfullyUserLoggegIn.value,
            confirmButtonColor: '#3085d6'
        });
    }

});