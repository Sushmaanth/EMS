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
    document.addEventListener("click", function (e) {
        const uploadBox = e.target.closest(".upload-box");

        if (uploadBox == null) {
            return;
        }

        const fileInput = uploadBox.closest("form").parentElement.querySelector(".file-upload");
        if (fileInput) {
            fileInput.click();
        }
    });

    

    //file selected event auto upload
    document.addEventListener("change", function (e) {

        if (e.target.classList.contains("file-upload")) {

            uploadDocument(e.target);
        }
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

//upload document
async function uploadDocument(fileInput) {

    const form = fileInput.closest("form");

    const documentTypeId = form.querySelector("[name='DocumentTypeId']").value;

    const employeeId = form.querySelector("[name='EmployeeId']").value;

    const file = fileInput.files[0];

    if (!file) {
        return;
    }

    const progressContainer = form.querySelector(".upload-progress-container");

    const progressBar = form.querySelector(".upload-progress-bar");

    progressContainer.classList.remove("d-none");

    const formData = new FormData();

    formData.append("EmployeeId", employeeId);

    formData.append("DocumentTypeId", documentTypeId);

    formData.append("File", file);

    const xhr = new XMLHttpRequest();

    xhr.open("POST", "/Employee/UploadDocument", true);

    //progress bar
    xhr.upload.addEventListener("progress", function (e) {

        if (e.lengthComputable) {

            const percent =
                Math.round((e.loaded / e.total) * 100);

            progressBar.style.width = percent + "%";

            progressBar.innerText = percent + "%";
        }
    });

    xhr.onload = function () {

        console.log("Status:", xhr.status);
        console.log("Response:", xhr.responseText);

        if (xhr.status === 200) {

            const result = JSON.parse(xhr.responseText);
            const uploadBox = form.querySelector(".upload-box");
            const extension = file.name.split('.').pop().toLowerCase();

            let previewHtml = "";

            if (["jpg", "jpeg", "png", "svg"].includes(extension)) {

                const imageUrl = URL.createObjectURL(file);

                previewHtml = `<img src="${imageUrl}" class="document-preview-image">`;

            }
            else if (extension === "pdf") {

                previewHtml = `
                <div class="document-preview-pdf">
                    <i class="bi bi-file-earmark-pdf-fill"></i>
                </div>`;
            }
            else {
                previewHtml = `
                <div class="document-preview-pdf">
                    <i class="bi bi-file-earmark-word-fil"></i>
                </div>`;
            }

            uploadBox.innerHTML = `
            <div class="document-preview-wrapper">

                <div class="document-actions">
                    <button class="btn btn-light btn-sm btn-view">
                        <i class="bi bi-eye"></i>
                    </button>

                    <button class="btn btn-light btn-sm btn-edit">
                        <i class="bi bi-pencil"></i>
                    </button>

                    <button class="btn btn-light btn-sm btn-delete">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
                ${previewHtml}
                <div class="document-name">
                    ${file.name}
                </div>
            </div>`;

            Swal.fire({
                icon: "success",
                title: "Uploaded",
                text: "Uploaded Successfully"
            });

            progressBar.classList.add("bg-success");

            console.log(result);
        }
        else {

            Swal.fire({
                icon: "error",
                title: "Upload Failed"
            });
        }
    };

    xhr.onerror = function () {

        Swal.fire({
            icon: "error",
            title: "Upload Failed"
        });
    };

    xhr.send(formData);
}