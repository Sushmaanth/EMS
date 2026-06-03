(() => {

    document.addEventListener("DOMContentLoaded", function () {

        initializeCategoryButtons();
        initializeUploadEvents();
        initializeDeleteEvents();
        initializeEditEvents();
        showLoginSuccessMessage();

    });

    function initializeCategoryButtons() {

        const documentCategoryButtons =
            document.querySelectorAll(".category-btn");

        documentCategoryButtons.forEach(button => {

            button.addEventListener("click", async () => {

                try {

                    const categoryId =
                        button.getAttribute("data-categoryid");

                    documentCategoryButtons.forEach(btn =>
                        btn.classList.remove("active"));

                    button.classList.add("active");

                    await reloadCurrentCategory(categoryId);
                }
                catch (error) {

                    console.error(error);

                    Swal.fire({
                        icon: "error",
                        title: "Error",
                        text: "Unable to load documents"
                    });
                }
            });
        });
    }

    //upload btn
    function initializeUploadEvents() {

        document.addEventListener("click", function (e) {

            if (
                e.target.closest(".btn-view") ||
                e.target.closest(".btn-edit") ||
                e.target.closest(".btn-delete")
            ) {
                return;
            }

            const uploadBox =
                e.target.closest(".upload-box");

            if (!uploadBox) {
                return;
            }

            const fileInput =
                uploadBox.closest("form")
                    .querySelector(".file-upload");

            fileInput?.click();
        });

        //on change upload
        document.addEventListener("change", function (e) {

            if (!e.target.classList.contains("file-upload")) {
                return;
            }

            const fileInput = e.target;

            if (fileInput.dataset.mode === "replace") {

                const documentId =
                    fileInput.closest(".document-card")
                        .querySelector(".btn-edit")
                        .dataset.documentid;

                replaceDocument(documentId, fileInput.files[0]);

                delete fileInput.dataset.mode;

                return;
            }

            uploadDocument(fileInput);
        });
    }

        //delete btn
        function initializeDeleteEvents() {

            document.addEventListener("click", async function (e) {

                const deleteButton =
                    e.target.closest(".btn-delete");

                if (!deleteButton) {
                    return;
                }

                const documentId =
                    deleteButton.dataset.documentid;

                await deleteDocument(documentId);

            });
        }

        //edit
        function initializeEditEvents() {

            document.addEventListener("click", function (e) {

                const editButton =
                    e.target.closest(".btn-edit");

                if (!editButton) {
                    return;
                }

                const card =
                    editButton.closest(".document-card");

                const fileInput =
                    card.querySelector(".file-upload");

                if (!fileInput) {
                    return;
                }

                fileInput.dataset.mode = "replace";

                fileInput.click();
            });
        }
        function showLoginSuccessMessage() {

            const successMessage =
                document.getElementById("successfullyUserLoggegIn");

            if (!successMessage) {
                return;
            }

            Swal.fire({
                icon: "success",
                title: "Success",
                text: successMessage.value,
                confirmButtonColor: "#3085d6"
            });
        }

        //upload request
        async function uploadDocument(fileInput) {

            try {

                const form =
                    fileInput.closest("form");

                const documentTypeId =
                    form.querySelector("[name='DocumentTypeId']").value;

                const employeeId =
                    form.querySelector("[name='EmployeeId']").value;

                const file =
                    fileInput.files[0];

                const allowedExtensions =
                    [".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png"];

                const extension =
                    "." + file.name.split(".").pop().toLowerCase();

                if (!allowedExtensions.includes(extension)) {

                    Swal.fire({
                        icon: "error",
                        title: "Invalid File",
                        text: "Only PDF, DOC, DOCX, JPG, JPEG and PNG files are allowed."
                    });

                    return;
                }

                if (!file) {
                    return;
                }

                if (file.size > 50 * 1024 * 1024) {
                    Swal.fire({
                        icon: "error",
                        title: "File Too Large",
                        text: "Maximum file size allowed is 50 MB."
                    });

                    fileInput.value = "";
                    return;
                }

                const progressContainer =
                    form.querySelector(".upload-progress-container");

                const progressBar =
                    form.querySelector(".upload-progress-bar");

                progressContainer.classList.remove("d-none");

                const formData = new FormData();

                formData.append("EmployeeId", employeeId);
                formData.append("DocumentTypeId", documentTypeId);
                formData.append("File", file);

                const xhr = new XMLHttpRequest();

                xhr.open("POST", "/Employee/UploadDocument", true);

                xhr.upload.addEventListener("progress", function (e) {

                    if (e.lengthComputable) {

                        const percent =
                            Math.round((e.loaded / e.total) * 100);

                        progressBar.style.width = percent + "%";
                        progressBar.innerText = percent + "%";
                    }
                });

                xhr.onload = async function () {

                    try {
                        const result = JSON.parse(xhr.responseText);

                        if (xhr.status === 200) {

                            progressBar.classList.add("bg-success");

                            await reloadActiveCategory();

                            Swal.fire({
                                icon: "success",
                                title: result.documentType,
                                text: "Uploaded Successfully"
                            });
                        }
                        else {

                            Swal.fire({
                                icon: "error",
                                title: "Upload Failed"
                            });
                        }
                    }
                    catch (error) {

                        console.error(error);
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
            catch (error) {

                console.error(error);

                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Something went wrong"
                });
            }
        }

        async function reloadActiveCategory() {

            const activeCategory =
                document.querySelector(".category-btn.active");

            if (!activeCategory) {
                return;
            }

            const categoryId = activeCategory.getAttribute("data-categoryid");

            await reloadCurrentCategory(categoryId);
        }

        async function reloadCurrentCategory(categoryId) {
            try {

                const response =
                    await fetch(`/Employee/GetDocumentCards?categoryId=${categoryId}`);

                const html =
                    await response.text();

                document.getElementById("documentCardsContainer")
                    .innerHTML = html;
            }
            catch (error) {
                console.error(error);
            }
        }

        async function deleteDocument(documentId) {

            try {

                const confirmation = await Swal.fire({
                    title: "Delete Document?",
                    text: "This action cannot be undone.",
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonText: "Delete",
                    cancelButtonText: "Cancel"
                });

                if (!confirmation.isConfirmed) {
                    return;
                }

                const response =
                    await fetch(`/Employee/DeleteDocument?documentId=${documentId}`, {
                        method: "DELETE"
                    });

                const result =
                    await response.json();

                console.log(result);

                if (response.ok) {

                    await reloadActiveCategory();

                    Swal.fire({
                        icon: "success",
                        title: result.documentType,
                        text: result.message
                    });
                }
                else {

                    Swal.fire({
                        icon: "error",
                        title: "Delete Failed",
                        text: result.message
                    });
                }
            }
            catch (error) {

                console.error(error);

                Swal.fire({
                    icon: "error",
                    title: "Error",
                    text: "Unable to delete document"
                });
            }
        }

    async function replaceDocument(documentId, file) {

        try {

            if (!file) {
                return;
            }

            const formData = new FormData();
            formData.append("DocumentId", documentId);
            formData.append("File", file);

            const response =
                await fetch("/Employee/ReplaceDocument", {
                    method: "PUT",
                    body: formData
                });

            const result =
                await response.json();

            console.log(result);

            if (response.ok) {

                await reloadActiveCategory();

                Swal.fire({
                    icon: "success",
                    title: result.documentType,
                    text: result.message
                });
            }
            else {

                Swal.fire({
                    icon: "error",
                    title: "Replace Failed",
                    text: result.message
                });
            }
        }
        catch (error) {

            console.error(error);

            Swal.fire({
                icon: "error",
                title: "Error",
                text: "Unable to replace document"
            });
        }
    }

})();