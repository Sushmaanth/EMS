using AutoMapper;
using ClosedXML.Excel;
using Dtos;
using Dtos.Repository.Abstraction;
using Dtos.Validation.Abstraction;
using EMSBackend.Common.Exceptions;
using EMSBackend.Service.Abstraction;
using Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace EMSBackend.Service.Implementation
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBlobService _blobService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IEmployeeValidation _employeeValidation;
        private readonly IMapper _mapper;
        public EmployeeService(IEmployeeRepository repository,IBlobService blobService,IDocumentRepository documentRepository, IEmployeeValidation employeeValidation,IMapper mapper)
        {
            _employeeRepository = repository;
            _blobService = blobService;
            _documentRepository = documentRepository;
            _employeeValidation = employeeValidation;
            _mapper = mapper;
        }

        public async Task<ServiceResponseDto<CreateEmployeeDto>>Create(CreateEmployeeDto dto)
        {
            var errors = await _employeeValidation.Validate(dto);

            if (errors.Any())
            {
                return new ServiceResponseDto<CreateEmployeeDto>
                {
                    Success = false,
                    Message = "Create Employee Validation",
                    Errors = errors
                };
            }

            var employee = _mapper.Map<Employee>(dto);

            var createdEmployee = _employeeRepository.Create(employee);

            var responseDto = _mapper.Map<CreateEmployeeDto>(createdEmployee);

            return new ServiceResponseDto<CreateEmployeeDto>
            {
                Success = true,
                Message = "Employee created successfully",
                Data = responseDto
            };
        }

        public ServiceResponseDto<ICollection<EmployeeDto>> View()
        {

            var employees = _employeeRepository.View();

            var employeeDtos = _mapper.Map<ICollection<EmployeeDto>>(employees);

            return new ServiceResponseDto<ICollection<EmployeeDto>>
            {
                Success = true,
                Message = "Employees fetched successfully",
                Data = employeeDtos
            };
        }


        public async Task<ServiceResponseDto<CreateEmployeeDto>>Update(int id, CreateEmployeeDto dto)
        {
            var errors = await _employeeValidation.Validate(dto);

            if (errors.Any())
            {
                return new ServiceResponseDto<CreateEmployeeDto>
                {
                    Success = false,
                    Message = "Update Employee Validation",
                    Errors = errors
                };
            }

            var foundEmployee = _employeeRepository.GetById(id);

            if (foundEmployee == null)
            {
                return new ServiceResponseDto<CreateEmployeeDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            _mapper.Map(dto, foundEmployee);

            var updatedEmployee = _employeeRepository.Update(foundEmployee);


            return new ServiceResponseDto<CreateEmployeeDto>
            {
                Success = true,
                Message = "Employee updated successfully",
                Data = _mapper.Map<CreateEmployeeDto>(updatedEmployee)
            };
        }


        public ServiceResponseDto<EmployeeDto> Delete(int id)
        {

            var foundEmployee = _employeeRepository.GetById(id);

            if (foundEmployee == null)
            {
                return new ServiceResponseDto<EmployeeDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            var deletedEmployee = _employeeRepository.Delete(foundEmployee);


            return new ServiceResponseDto<EmployeeDto>
            {
                Success = true,
                Message = "Employee deleted successfully",
                Data = _mapper.Map<EmployeeDto>(deletedEmployee)
            };
        }


        public ServiceResponseDto<EmployeeDto> GetById(int id)
        {
            var employee = _employeeRepository.GetById(id);

            if (employee == null)
            {
                return new ServiceResponseDto<EmployeeDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            return new ServiceResponseDto<EmployeeDto>
            {
                Success = true,
                Message = "Employee fetched successfully",
                Data = _mapper.Map<EmployeeDto>(employee)
            };
        }


        public ServiceResponseDto<PagenationDto<EmployeeDto>> GetEmployees(string? searchText, int pageNumber, int pageSize)
        {

            var query = _employeeRepository.GetEmployees(searchText);

            var totalRecords = query.Count();

            var employees = query
                .OrderByDescending(e => e.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);

            return new ServiceResponseDto<PagenationDto<EmployeeDto>>
            {
                Success = true,
                Message = "Employees fetched successfully",

                Data = new PagenationDto<EmployeeDto>
                {
                    Data = employeeDtos,
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchText = searchText
                }
            };
        }


        public ServiceResponseDto<ICollection<DepartmentDto>> GetDepartments()
        {

            var departments = _employeeRepository.GetDepartments();

            var departmentDtos = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                DepartmentName = d.DepartmentName
            }).ToList();

            return new ServiceResponseDto<ICollection<DepartmentDto>>
            {
                Success = true,
                Message = "Departments fetched successfully",
                Data = departmentDtos
            };
        }

        public async Task<ServiceResponseDto<EmployeeDocumentResponseDto>> UploadDocumentAsync(EmployeeDocumentUploadDto dto)
        {
            var employee = _employeeRepository.GetById(dto.EmployeeId);

            if (employee == null)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            if (dto.File == null || dto.File.Length == 0)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Please upload a file"
                };
            }

            var documentType = _documentRepository.GetById(dto.DocumentTypeId);

            if (documentType == null)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Invalid document type"
                };
            }

            var existingDocument = _documentRepository.GetEmployeeDocument(dto.EmployeeId,dto.DocumentTypeId);

            if (existingDocument != null)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Document already uploaded. Please use Replace."
                };
            }

            var validationResult = ValidateFile(dto.File);

            if (validationResult != null)
            {
                return validationResult;
            }

            var uploadResult = await _blobService.UploadFileAsync(dto.File,
                employee.Name, employee.Id,documentType.Name);

            EmployeeDocument employeeDocument = new()
            {
                EmployeeId = dto.EmployeeId,
                DocumentTypeId = dto.DocumentTypeId,
                OriginalFileName = dto.File.FileName,
                StoredFileName = uploadResult.storedFileName,
                BlobUrl = uploadResult.bloburl,
                UploadedDate = DateTime.UtcNow
            };

            await _employeeRepository.AddDocumentAsync(employeeDocument);

            EmployeeDocumentResponseDto responseDto = new()
            {
                DocumentId = employeeDocument.Id,
                DocumentCategory =documentType.DocumentCategory.Name,

                DocumentType = documentType.Name,

                OriginalFileName = employeeDocument.OriginalFileName,

                StoredFileName = employeeDocument.StoredFileName,

                BlobUrl = employeeDocument.BlobUrl,

                UploadedDate = employeeDocument.UploadedDate
            };

            return new ServiceResponseDto<EmployeeDocumentResponseDto>
            {
                Success = true,
                Message = "Document Uploaded Successfully",
                Data = responseDto
            };
        }

        public ServiceResponseDto<IEnumerable<DocumentTypeResponseDto>> GetByCategory(int categoryId)
        {
            var documentTypes =_documentRepository.GetByCategory(categoryId);

            var dtos = documentTypes.Select(dt =>
                new DocumentTypeResponseDto
                {
                    Id = dt.Id,

                    Name = dt.Name,

                    IsMandatory = dt.IsMandatory,

                    DocumentCategoryId =dt.DocumentCategoryId,

                    DocumentCategoryName =dt.DocumentCategory.Name
                }).ToList();

            return new ServiceResponseDto<IEnumerable<DocumentTypeResponseDto>>
            {
                Success = true,

                Message ="Document types fetched successfully",

                Data = dtos
            };
        }

        public ServiceResponseDto<IEnumerable<DocumentCategoryResponseDto>> GetAll()
        {
            var categories = _documentRepository.GetAll();

            var dto = categories.Select(dc =>
            new DocumentCategoryResponseDto
            {
                Id = dc.Id,
                Name =dc.Name
            }).ToList();

            return new ServiceResponseDto<IEnumerable<DocumentCategoryResponseDto>>
            {
                Success = true,
                Message = "Document categories fetched successfully",
                Data = dto
            };
        }

        public async Task<ServiceResponseDto<DeleteDocumentResponseDto>> DeleteDocumentAsync(int documentId)
        {
            var document =_documentRepository.GetDocumentById(documentId);

            if (document == null)
            {
                return new ServiceResponseDto<DeleteDocumentResponseDto>
                {
                    Success = false,
                    Message = "Document not found"
                };
            }

            await _blobService.DeleteFileAsync(document.StoredFileName);

            _documentRepository.DeleteDocument(document);

            await _documentRepository.SaveChangesAsync();

            return new ServiceResponseDto<DeleteDocumentResponseDto>
            {
                Success = true,
                Message = "Document deleted successfully",
                Data = new DeleteDocumentResponseDto
                {
                    DocumentId = document.Id,
                    DocumentType = document.DocumentType.Name,
                    Message = "Deleted Successfully"
                }
            };
        }

        public async Task<ServiceResponseDto<EmployeeDocumentResponseDto>> ReplaceDocumentAsync(ReplaceDocumentDto dto)
        {
            var document = _documentRepository.GetDocumentById(dto.DocumentId);

            if (document == null)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Document not found"
                };
            }

            await _blobService.DeleteFileAsync(document.StoredFileName);

            var uploadResult = await _blobService.UploadFileAsync(dto.File,
                document.Employee.Name,document.EmployeeId,document.DocumentType.Name);

            document.OriginalFileName = dto.File.FileName;

            document.StoredFileName = uploadResult.storedFileName;

            document.BlobUrl = uploadResult.bloburl;

            document.UploadedDate = DateTime.UtcNow;

            await _documentRepository.SaveChangesAsync();

            return new ServiceResponseDto<EmployeeDocumentResponseDto>
            {
                Success = true,
                Message = "Document replaced successfully",
                Data = new EmployeeDocumentResponseDto
                {
                    DocumentId = document.Id,

                    DocumentCategory = document.DocumentType.DocumentCategory.Name,

                    DocumentType = document.DocumentType.Name,

                    OriginalFileName = document.OriginalFileName,

                    StoredFileName = document.StoredFileName,

                    BlobUrl = document.BlobUrl,

                    UploadedDate = document.UploadedDate
                }
            };
        }

        public async Task<ServiceResponseDto<DocumentViewResponseDto>> GetDocumentUrlAsync(int documentId)
        {
            var document = _documentRepository.GetDocumentById(documentId);

            if (document == null)
            {
                return new ServiceResponseDto<DocumentViewResponseDto>
                {
                    Success = false,
                    Message = "Document not found"
                };
            }

            var sasUrl =_blobService.GenerateReadSasUrl(document.StoredFileName);

            return new ServiceResponseDto<DocumentViewResponseDto>
            {
                Success = true,
                Data = new DocumentViewResponseDto
                {
                    SasUrl = sasUrl
                }
            };
        }

        public async Task<ServiceResponseDto<ICollection<DocumentTypeDto>>> GetDocumentTypesByCategoryAsync(int categoryId, int employeeId)
        {
            var documentTypes = await _documentRepository.GetDocumentTypesByCategoryAsync(categoryId, employeeId);

            var result = documentTypes.Select(dt =>
            {
                var employeeDocuments = dt.EmployeeDocuments.FirstOrDefault();

                return new DocumentTypeDto
                {
                    Id = dt.Id,
                    Name = dt.Name,
                    IsMandatory = dt.IsMandatory,
                    DocumentId = employeeDocuments?.Id,
                    FileName = employeeDocuments?.OriginalFileName,
                    BlobUrl = employeeDocuments?.BlobUrl,
                    IsUploaded = employeeDocuments != null
                };
            }).ToList();

            return new ServiceResponseDto<ICollection<DocumentTypeDto>>
            {
                Success = true,
                Message = "Document types fetched successfull",
                Data = result
            };
        }

        public ServiceResponseDto<DashboardDto> GetEmployeeDashboard(int employeeId)
        {
            var data = _employeeRepository.GetDashboardData(employeeId);

            if (data.Employee == null)
            {
                return new ServiceResponseDto<DashboardDto>
                {
                    Success = false,
                    Message = "Employee not found"
                };
            }

            var uploadedDocumentTypeIds = data.UploadedDocuments
                                          .Select(d => d.DocumentTypeId)
                                          .ToHashSet();

            var requiredDocuments = data.MandatoryDocumentTypes
                                        .Select(d => new DocumentStatusDto
                                        {
                                            DocumentName = d.Name,
                                            IsUploaded = uploadedDocumentTypeIds.Contains(d.Id)
                                        }).ToList();

            int totalMandatoryDocuments = data.MandatoryDocumentTypes.Count;

            int uploadedDocuments = requiredDocuments.Count(d => d.IsUploaded);

            int missingDocuments = totalMandatoryDocuments - uploadedDocuments;

            decimal completionPercentage = totalMandatoryDocuments == 0
                        ? 100 : (uploadedDocuments * 100m) / totalMandatoryDocuments;

            DashboardDto dashboardDto = new()
            {
                EmployeeName = data.Employee.Name,

                DepartmentName = data.Employee.Department?.DepartmentName ?? "Department not assigned",

                DateOfJoining = data.Employee.DateOfJoining,

                TotalMandatoryDocuments = totalMandatoryDocuments,

                UploadedDocuments = uploadedDocuments,

                MissingDocuments = missingDocuments,

                CompletionPercentage = completionPercentage,

                RequiredDocuments = requiredDocuments
            };

            return new ServiceResponseDto<DashboardDto>
            {
                Success = true,
                Message = "Dashboard fetched successfully",
                Data = dashboardDto
            };
        }

        public ServiceResponseDto<EmployeeDocumentResponseDto>? ValidateFile(IFormFile file)
        {
            const long maxFileSize = 50 * 1024 * 1024; // 50 MB

            if (file == null || file.Length == 0)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Please select a file"
                };
            }

            if (file.Length > maxFileSize)
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "File size cannot exceed 50 MB"
                };
            }

            string[] allowedExtensions =
            {
            ".pdf",
            ".doc",
            ".docx",
            ".jpg",
            ".jpeg",
            ".png"
            };

            string extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return new ServiceResponseDto<EmployeeDocumentResponseDto>
                {
                    Success = false,
                    Message = "Only PDF, DOC, DOCX, JPG, JPEG and PNG files are allowed"
                };
            }

            return null;
        }




        //**Upload Employee Excel Data
        public async Task<ServiceResponseDto<EmployeeUploadExcelResponseDto>> UploadEmployeesAsync(IFormFile file)
        {
            var response = new ServiceResponseDto<EmployeeUploadExcelResponseDto>();

            //empty file upload
            if (file == null || file.Length == 0)
            {
                response.Success = false;
                response.Message = "Please select a valid file.";

                return response;
            }

            //File Extension Validation
            var allowedExtensions = new[] { ".xlsx" };

            var extension = Path.GetExtension(file.FileName);

            if (!allowedExtensions.Contains(
                    extension,
                    StringComparer.OrdinalIgnoreCase))
            {
                response.Success = false;
                response.Message = "Only .xlsx files are allowed.";

                return response;
            }

            var employeesToInsert = new List<Employee>();
            //var validationErrors = new Dictionary<string, List<string>>();

            var uploadResponse = new EmployeeUploadExcelResponseDto();

            using var stream = new MemoryStream();

            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);

            //one worksheet validation
            if (workbook.Worksheets.Count != 1)
            {
                response.Success = false;
                response.Message =
                    "Excel file should contain only one worksheet.";

                return response;
            }

            var worksheet = workbook.Worksheet(1);

            var expectedHeaders = new[]
            {
                "EmployeeCode",
                "Name",
                "Gender",
                "DateOfBirth",
                "EmailId",
                "Mobile",
                "Salary",
                "DateOfJoining",
                "DepartmentName"
            };

            for (int i = 0; i < expectedHeaders.Length; i++)
            {
                var actualHeader = worksheet.Cell(1, i + 1).GetString().Trim();
                if (!actualHeader.Equals(expectedHeaders[i], StringComparison.OrdinalIgnoreCase))
                {
                    response.Success = false;
                    response.Message = $"Invalid template. Expected column: '{expectedHeaders[i]}'. Please download the latest template and upload the file again.";
                    response.Data = uploadResponse;

                    return response;
                }
            }

            var rows = worksheet.RowsUsed().Skip(1).ToList();

            //no data in the excel
            if (!rows.Any())
            {
                response.Success = false;
                response.Message = "Excel file does not contain any records.";

                return response;
            }

            // Maximum 100 Records
            if (rows.Count > 100)
            {
                response.Success = false;
                response.Message =
                    "Maximum 100 employee records can be uploaded at a time.";

                return response;
            }

            //load department
            var departments = _employeeRepository.GetDepartments();

            var departmentDictionary = departments.ToDictionary(
                d => d.DepartmentName.Trim(),
                d => d.Id,
                StringComparer.OrdinalIgnoreCase);

            // Collect values from Excel with uploaded code removes the duplicate

            var uploadedCodes = rows
                .Select(r => r.Cell(1).GetString().Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var uploadedEmails = rows
               .Select(r => r.Cell(5).GetString().Trim())
               .Where(x => !string.IsNullOrWhiteSpace(x))
               .Distinct(StringComparer.OrdinalIgnoreCase)
               .ToList();

            var uploadedMobiles = rows
                .Select(r => r.Cell(6).GetString().Trim())
                .Where(x => long.TryParse(x, out _))
                .Select(long.Parse)
                .Distinct()
                .ToList();

            //compare the excel uploaded data and dataase for duplicate

            var existingEmployeeCodes =
               (await _employeeRepository
                   .GetExistingEmployeeCodesAsync(uploadedCodes))
               .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingEmails =
                (await _employeeRepository
                    .GetExistingEmailsAsync(uploadedEmails))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingMobiles =
                (await _employeeRepository
                    .GetExistingMobilesAsync(uploadedMobiles))
                .ToHashSet();

            // Duplicate tracking inside uploaded file

            var uploadedEmployeeCodes =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var uploadedEmailSet =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var uploadedMobileSet =
                new HashSet<long>();

            var allowedGenders = new[]
               {
                    "Male",
                    "Female",
                    "Other"
                };

            var today = DateOnly.FromDateTime(DateTime.Today);

            int rowNumber = 2;

            foreach (var row in rows)
            {
                var rowErrors = new List<string>();

                string employeeCode = row.Cell(1).GetString().Trim();
                string name = row.Cell(2).GetString().Trim();
                string gender = row.Cell(3).GetString().Trim();
                string emailId = row.Cell(5).GetString().Trim();
                string mobileText = row.Cell(6).GetString().Trim();
                string departmentName = row.Cell(9).GetString().Trim();

                // Mandatory validations

                if (string.IsNullOrWhiteSpace(employeeCode))
                    rowErrors.Add("Employee Code is required.");

                if (string.IsNullOrWhiteSpace(name))
                    rowErrors.Add("Employee Name is required.");

                if (string.IsNullOrWhiteSpace(gender))
                    rowErrors.Add("Gender is required.");

                if (string.IsNullOrWhiteSpace(emailId))
                    rowErrors.Add("Email Id is required.");

                if (string.IsNullOrWhiteSpace(mobileText))
                    rowErrors.Add("Mobile Number is required.");

                if (string.IsNullOrWhiteSpace(departmentName))
                    rowErrors.Add("Department is required.");

                //dupliate values in the excel data validation employee code

                if (!string.IsNullOrWhiteSpace(employeeCode))
                {
                    if (!uploadedEmployeeCodes.Add(employeeCode))
                    {
                        rowErrors.Add(
                            "Duplicate Employee Code found in uploaded file.");
                    }
                }


                //compare the sheet and DB duplicate employee code
                if (!string.IsNullOrWhiteSpace(employeeCode) &&
                    existingEmployeeCodes.Contains(employeeCode))
                {
                    rowErrors.Add(
                        "Employee Code already exists.");
                }

                //invalid email
                if (!string.IsNullOrWhiteSpace(emailId))
                {
                    if (!new EmailAddressAttribute().IsValid(emailId))
                    {
                        rowErrors.Add("Invalid Email Id.");
                    }
                }

                // Email Duplicate in File

                if (!string.IsNullOrWhiteSpace(emailId))
                {
                    if (!uploadedEmailSet.Add(emailId))
                    {
                        rowErrors.Add(
                            "Duplicate Email Id found in uploaded file.");
                    }
                }

                // Email Duplicate in DB

                if (!string.IsNullOrWhiteSpace(emailId) &&
                    existingEmails.Contains(emailId))
                {
                    rowErrors.Add(
                        "Email Id already exists.");
                }

                // Department Exists

                if (!string.IsNullOrWhiteSpace(departmentName) &&
                    !departmentDictionary.ContainsKey(departmentName))
                {
                    rowErrors.Add("Department does not exist.");
                }

                // Gender Validation

                if (!string.IsNullOrWhiteSpace(gender) &&
                    !allowedGenders.Contains(
                        gender,
                        StringComparer.OrdinalIgnoreCase))
                {
                    rowErrors.Add("Invalid Gender.");
                }

                // Mobile Validation

                long mobile = 0;

                if (!Regex.IsMatch(
                        mobileText,
                        @"^[6-9]\d{9}$"))
                {
                    rowErrors.Add("Invalid Mobile Number.");
                }
                else
                {
                    mobile = long.Parse(mobileText);

                    if (!uploadedMobileSet.Add(mobile))
                    {
                        rowErrors.Add(
                            "Duplicate Mobile Number found in uploaded file.");
                    }

                    if (existingMobiles.Contains(mobile))
                    {
                        rowErrors.Add(
                            "Mobile Number already exists.");
                    }
                }

                // Salary Validation

                decimal salary = 0;

                if (!decimal.TryParse(
                        row.Cell(7).GetString(),
                        out salary))
                {
                    rowErrors.Add("Invalid Salary.");
                }
                else if (salary <= 0)
                {
                    rowErrors.Add("Salary must be greater than zero.");
                }

                // Date Of Birth Validation

                DateOnly dateOfBirth = default;

                if (!row.Cell(4).TryGetValue<DateTime>(out var dob))
                {
                    rowErrors.Add("Invalid Date Of Birth.");
                }
                else
                {
                    dateOfBirth = DateOnly.FromDateTime(dob);

                    if (dateOfBirth > today)
                    {
                        rowErrors.Add(
                            "Date Of Birth cannot be a future date.");
                    }

                    int age = today.Year - dateOfBirth.Year;

                    if (dateOfBirth > today.AddYears(-age))
                    {
                        age--;
                    }

                    if (age < 18)
                    {
                        rowErrors.Add(
                            "Employee must be at least 18 years old.");
                    }
                }

                // Date Of Joining Validation

                DateOnly dateOfJoining = default;

                if (!row.Cell(8).TryGetValue<DateTime>(out var doj))
                {
                    rowErrors.Add("Invalid Date Of Joining.");
                }
                else
                {
                    dateOfJoining = DateOnly.FromDateTime(doj);

                    if (dateOfJoining > today)
                    {
                        rowErrors.Add(
                            "Date Of Joining cannot be a future date.");
                    }

                    if (dateOfBirth != default &&
                        dateOfJoining <= dateOfBirth)
                    {
                        rowErrors.Add(
                            "Date Of Joining must be after Date Of Birth.");
                    }
                }

                if (rowErrors.Any())
                {
                    uploadResponse.Errors.Add(new UploadEmployeeExcelErrorDto
                    {
                        RowNumber = rowNumber,

                        EmployeeData = new EmployeeExcelUploadDto
                        {
                            EmployeeCode = employeeCode,
                            Name = name,
                            Gender = gender,
                            DateOfBirth = dateOfBirth,
                            EmailId = emailId,
                            Mobile = mobile,
                            Salary = salary,
                            DateOfJoining = dateOfJoining,
                            DepartmentName = departmentName
                        },

                        ErrorMessage = string.Join(",",rowErrors)
                    });

                    rowNumber++;
                    continue;
                }

                employeesToInsert.Add(
                    new Employee
                    {
                        EmployeeCode = employeeCode,
                        Name = name,
                        Gender = gender,
                        DateOfBirth = dateOfBirth,
                        EmailId = emailId,
                        Mobile = mobile,
                        Salary = salary,
                        DateOfJoining = dateOfJoining,
                        DepartmentId = departmentDictionary[departmentName]
                    });

                rowNumber++;
            }

            if (employeesToInsert.Any())
            {
                await _employeeRepository
                    .BulkInsertAsync(employeesToInsert);
            }

            response.Success = uploadResponse.Errors.Count == 0;

            if (employeesToInsert.Count == 0)
            {
                response.Message =
                    "No employee records were uploaded due to validation errors.";
            }
            else if (uploadResponse.Errors.Any())
            {
                response.Message =
                    $"{employeesToInsert.Count} records uploaded successfully and {uploadResponse.Errors.Count} records failed validation.";
            }
            else
            {
                response.Message =
                    "All employee records uploaded successfully.";
            }

            uploadResponse.TotalRecords = rows.Count;

            uploadResponse.SuccessRecords = employeesToInsert.Count;

            uploadResponse.FailedRecords = uploadResponse.Errors.Count;

            response.Data = uploadResponse;

            return response;
        }

        public ServiceResponseDto<byte[]>DownloadTemplate()
        {
            var response = new ServiceResponseDto<byte[]>();

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Employee Upload");

            // Headers
            worksheet.Cell(1, 1).Value = "EmployeeCode";
            worksheet.Cell(1, 2).Value = "Name";
            worksheet.Cell(1, 3).Value = "Gender";
            worksheet.Cell(1, 4).Value = "DateOfBirth";
            worksheet.Cell(1, 5).Value = "EmailId";
            worksheet.Cell(1, 6).Value = "Mobile";
            worksheet.Cell(1, 7).Value = "Salary";
            worksheet.Cell(1, 8).Value = "DateOfJoining";
            worksheet.Cell(1, 9).Value = "DepartmentName";

            worksheet.Row(1).Style.Font.Bold = true;

            var genderRange = worksheet.Range("C2:C101");

            genderRange.CreateDataValidation()
                       .List("\"Male,Female,Other\"");

            var departments = _employeeRepository.GetDepartments();

            var departmentList = string.Join(",", departments.Select(x => x.DepartmentName));

            //create a hidden sheet
            //var lookupSheet = workbook.Worksheets.Add("Lookups");

            //int row = 1;

            //foreach (var dept in departments)
            //{
            //    lookupSheet.Cell(row, 1).Value =
            //        dept.DepartmentName;

            //    row++;
            //}

            //lookupSheet.Hide();

            var departmentRange =worksheet.Range("I2:I101");

            departmentRange.CreateDataValidation()
               .List($"\"{departmentList}\"");


            worksheet.Column(4).Style.DateFormat.Format ="dd/MM/yyyy";

            worksheet.Column(8).Style.DateFormat.Format ="dd/MM/yyyy";

            //auto column width
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            response.Success = true;
            response.Message = "Template generated successfully.";

            response.Data = stream.ToArray();

            return response;
        }

        public ServiceResponseDto<byte[]> DownloadFailedRecordsAsync(List<UploadEmployeeExcelErrorDto> errors)
        {
            var response = new ServiceResponseDto<byte[]>();

            if (errors == null || !errors.Any())
            {
                response.Success = false;
                response.Message = "No failed records available.";

                return response;
            }

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Failed Records");

            // Headers
            worksheet.Cell(1, 1).Value = "Row";
            worksheet.Cell(1, 2).Value = "EmployeeCode";
            worksheet.Cell(1, 3).Value = "Name";
            worksheet.Cell(1, 4).Value = "Gender";
            worksheet.Cell(1, 5).Value = "DateOfBirth";
            worksheet.Cell(1, 6).Value = "EmailId";
            worksheet.Cell(1, 7).Value = "Mobile";
            worksheet.Cell(1, 8).Value = "Salary";
            worksheet.Cell(1, 9).Value = "DateOfJoining";
            worksheet.Cell(1, 10).Value = "DepartmentName";
            worksheet.Cell(1, 11).Value = "Error Message";

            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;

            foreach (var error in errors)
            {
                worksheet.Cell(row, 1).Value = error.RowNumber;
                worksheet.Cell(row, 2).Value = error.EmployeeData.EmployeeCode;
                worksheet.Cell(row, 3).Value = error.EmployeeData.Name;
                worksheet.Cell(row, 4).Value = error.EmployeeData.Gender;

                if (error.EmployeeData.DateOfBirth != default)
                {
                    worksheet.Cell(row, 5).Value =
                        error.EmployeeData.DateOfBirth.ToString("dd/MM/yyyy");
                }

                worksheet.Cell(row, 6).Value = error.EmployeeData.EmailId;
                worksheet.Cell(row, 7).Value = error.EmployeeData.Mobile;
                worksheet.Cell(row, 8).Value = error.EmployeeData.Salary;

                if (error.EmployeeData.DateOfJoining != default)
                {
                    worksheet.Cell(row, 9).Value =
                        error.EmployeeData.DateOfJoining.ToString("dd/MM/yyyy");
                }

                worksheet.Cell(row, 10).Value =
                    error.EmployeeData.DepartmentName;

                worksheet.Cell(row, 11).Value =
                    error.ErrorMessage;

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            response.Success = true;
            response.Message =
                "Failed records file generated successfully.";

            response.Data = stream.ToArray();

            return response;
        }
    }
}


