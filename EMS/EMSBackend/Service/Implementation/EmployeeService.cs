using AutoMapper;
using ClosedXML.Excel;
using Dtos;
using Dtos.Constants;
using Dtos.Repository.Abstraction;
using Dtos.Repository.Implementation;
using Dtos.Validation;
using Dtos.Validation.Abstraction;
using EMSBackend.Service.Abstraction;
using Entities;
using FluentValidation;


namespace EMSBackend.Service.Implementation
{
    public class EmployeeService : IEmployeeService
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBlobService _blobService;
        private readonly IDocumentRepository _documentRepository;
        private readonly IEmployeeValidation _employeeValidation;
        private readonly IMapper _mapper;
        private readonly IEmployeeDuplicateUploadValidator _duplicateUploadValidator;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuthRepository _authRepository;

        public EmployeeService(IEmployeeRepository repository, IBlobService blobService, IDocumentRepository documentRepository, IEmployeeValidation employeeValidation, IMapper mapper,
            IEmployeeDuplicateUploadValidator duplicateUploadValidator, IRoleRepository roleRepository,
            IAuthRepository authRepository)
        {
            _employeeRepository = repository;
            _blobService = blobService;
            _documentRepository = documentRepository;
            _employeeValidation = employeeValidation;
            _mapper = mapper;
            _duplicateUploadValidator = duplicateUploadValidator;
            _roleRepository = roleRepository;
            _authRepository = authRepository;
        }

        public async Task<ServiceResponseDto<CreateEmployeeDto>> Create(CreateEmployeeDto dto)
        {
            var errors = await _employeeValidation.Validate(dto);

            if (errors.Any())
            {
                return ServiceResponseDto<CreateEmployeeDto>.Fail(
                    "Create Employee Validation",
                    errors);
            }

            var employee = _mapper.Map<Employee>(dto);

            var createdEmployee = _employeeRepository.Create(employee);

            return ServiceResponseDto<CreateEmployeeDto>.Ok(
                _mapper.Map<CreateEmployeeDto>(createdEmployee),
                "Employee created successfully");
        }

        public ServiceResponseDto<ICollection<EmployeeDto>> View()
        {

            var employees = _employeeRepository.View();

            var employeeDtos = _mapper.Map<ICollection<EmployeeDto>>(employees);

            return ServiceResponseDto<ICollection<EmployeeDto>>.Ok(
                            employeeDtos, "Employees fetched successfully");
        }


        public async Task<ServiceResponseDto<CreateEmployeeDto>> Update(int id,
                     CreateEmployeeDto dto)
        {
            var errors = await _employeeValidation.Validate(dto);

            if (errors.Any())
            {
                return ServiceResponseDto<CreateEmployeeDto>.Fail(
                    "Update Employee Validation",
                    errors);
            }

            var foundEmployee = _employeeRepository.GetById(id);

            if (foundEmployee == null)
            {
                return ServiceResponseDto<CreateEmployeeDto>.Fail(
                    "Employee not found");
            }

            var oldRoleId = foundEmployee.RoleId;

            var role = await _roleRepository.GetByIdAsync(dto.RoleId);

            if (role == null)
            {
                return ServiceResponseDto<CreateEmployeeDto>.Fail(
                    "Selected role does not exist");
            }

            if (dto.ManagerId == id)
            {
                return ServiceResponseDto<CreateEmployeeDto>.Fail(
                    "Employee cannot be their own manager");
            }

            if (dto.RoleId == RoleConstants.Employee &&
                !dto.ManagerId.HasValue)
            {
                return ServiceResponseDto<CreateEmployeeDto>.Fail(
                    "Manager is required for employees");
            }

            if (dto.ManagerId.HasValue)
            {
                var manager = _employeeRepository.GetById(dto.ManagerId.Value);

                if (manager == null)
                {
                    return ServiceResponseDto<CreateEmployeeDto>.Fail(
                        "Selected manager does not exist");
                }

                if (manager.RoleId != RoleConstants.Manager &&
                    manager.RoleId != RoleConstants.Admin)
                {
                    return ServiceResponseDto<CreateEmployeeDto>.Fail(
                        "Selected employee is not a manager");
                }
            }

            _mapper.Map(dto, foundEmployee);

            var updatedEmployee = _employeeRepository.Update(foundEmployee);

            if (oldRoleId != dto.RoleId)
            {
                var user = _authRepository.GetUserByEmployeeId(id);

                if (user != null)
                {
                    user.RoleId = dto.RoleId;

                    _authRepository.Save();
                }
            }

            return ServiceResponseDto<CreateEmployeeDto>.Ok(
                _mapper.Map<CreateEmployeeDto>(updatedEmployee),
                "Employee updated successfully");
        }


        public ServiceResponseDto<EmployeeDto> Delete(int id)
        {

            var foundEmployee = _employeeRepository.GetById(id);

            if (foundEmployee == null)
            {
                return ServiceResponseDto<EmployeeDto>.Fail("Employee not found");
            }

            var deletedEmployee = _employeeRepository.Delete(foundEmployee);


            return ServiceResponseDto<EmployeeDto>.Ok(
                 _mapper.Map<EmployeeDto>(deletedEmployee),
                 "Employee deleted successfully");
        }


        public ServiceResponseDto<EmployeeDto> GetById(int id)
        {
            var employee = _employeeRepository.GetById(id);

            if (employee == null)
            {
                return ServiceResponseDto<EmployeeDto>.Fail("Employee not found");
            }

            return ServiceResponseDto<EmployeeDto>.Ok(
                _mapper.Map<EmployeeDto>(employee),
                "Employee fetched successfully");
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

            return ServiceResponseDto<PagenationDto<EmployeeDto>>.Ok(
                new PagenationDto<EmployeeDto>
                {
                    Data = employeeDtos,
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchText = searchText
                },
                "Employees fetched successfully");
        }


        public ServiceResponseDto<ICollection<DepartmentDto>> GetDepartments()
        {

            var departments = _employeeRepository.GetDepartments();

            var departmentDtos = departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                DepartmentName = d.DepartmentName
            }).ToList();

            return ServiceResponseDto<ICollection<DepartmentDto>>.Ok(
               departmentDtos, "Departments fetched successfully");
        }

        public async Task<ServiceResponseDto<EmployeeDocumentResponseDto>> UploadDocumentAsync(EmployeeDocumentUploadDto dto)
        {
            var employee = _employeeRepository.GetById(dto.EmployeeId);

            if (employee == null)
            {
                return ServiceResponseDto<EmployeeDocumentResponseDto>.Fail("Employee not found");
            }

            if (dto.File == null || dto.File.Length == 0)
            {
                return ServiceResponseDto<EmployeeDocumentResponseDto>.Fail("Please upload a file");
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

            var existingDocument = _documentRepository.GetEmployeeDocument(dto.EmployeeId, dto.DocumentTypeId);

            if (existingDocument != null)
            {
                return ServiceResponseDto<EmployeeDocumentResponseDto>.Fail(
                    "Document already uploaded. Please use Replace.");
            }

            var validationResult = ValidateFile(dto.File);

            if (validationResult != null)
            {
                return validationResult;
            }

            var uploadResult = await _blobService.UploadFileAsync(dto.File,
                employee.Name, employee.Id, documentType.Name);

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

            return ServiceResponseDto<EmployeeDocumentResponseDto>.Ok(
                new EmployeeDocumentResponseDto
                {
                    DocumentId = employeeDocument.Id,
                    DocumentCategory = documentType.DocumentCategory.Name,
                    DocumentType = documentType.Name,
                    OriginalFileName = employeeDocument.OriginalFileName,
                    StoredFileName = employeeDocument.StoredFileName,
                    BlobUrl = employeeDocument.BlobUrl,
                    UploadedDate = employeeDocument.UploadedDate
                }, "Document uploaded successfully");
        }

        public ServiceResponseDto<IEnumerable<DocumentTypeResponseDto>> GetByCategory(int categoryId)
        {
            var documentTypes = _documentRepository.GetByCategory(categoryId);

            var dtos = documentTypes.Select(dt =>
                new DocumentTypeResponseDto
                {
                    Id = dt.Id,

                    Name = dt.Name,

                    IsMandatory = dt.IsMandatory,

                    DocumentCategoryId = dt.DocumentCategoryId,

                    DocumentCategoryName = dt.DocumentCategory.Name
                }).ToList();

            return ServiceResponseDto<IEnumerable<DocumentTypeResponseDto>>.Ok(
                dtos, "Document types fetched successfully");
        }

        public ServiceResponseDto<IEnumerable<DocumentCategoryResponseDto>> GetAll()
        {
            var categories = _documentRepository.GetAll();

            var dto = categories.Select(dc =>
            new DocumentCategoryResponseDto
            {
                Id = dc.Id,
                Name = dc.Name
            }).ToList();

            return ServiceResponseDto<IEnumerable<DocumentCategoryResponseDto>>.Ok(
                dto, "Document categories fetched successfully");
        }

        public async Task<ServiceResponseDto<DeleteDocumentResponseDto>> DeleteDocumentAsync(int documentId)
        {
            var document = _documentRepository.GetDocumentById(documentId);

            if (document == null)
            {
                return ServiceResponseDto<DeleteDocumentResponseDto>.Fail("Document not found");
            }

            await _blobService.DeleteFileAsync(document.StoredFileName);

            _documentRepository.DeleteDocument(document);

            await _documentRepository.SaveChangesAsync();

            return ServiceResponseDto<DeleteDocumentResponseDto>.Ok(
                new DeleteDocumentResponseDto
                {
                    DocumentId = document.Id,
                    DocumentType = document.DocumentType.Name,
                    Message = "Deleted successfully"
                }, "Document deleted successfully");
        }

        public async Task<ServiceResponseDto<EmployeeDocumentResponseDto>> ReplaceDocumentAsync(ReplaceDocumentDto dto)
        {
            var document = _documentRepository.GetDocumentById(dto.DocumentId);

            if (document == null)
            {
                return ServiceResponseDto<EmployeeDocumentResponseDto>.Fail("Document not found");
            }

            await _blobService.DeleteFileAsync(document.StoredFileName);

            var uploadResult = await _blobService.UploadFileAsync(dto.File,
                document.Employee.Name, document.EmployeeId, document.DocumentType.Name);

            document.OriginalFileName = dto.File.FileName;

            document.StoredFileName = uploadResult.storedFileName;

            document.BlobUrl = uploadResult.bloburl;

            document.UploadedDate = DateTime.UtcNow;

            await _documentRepository.SaveChangesAsync();

            return ServiceResponseDto<EmployeeDocumentResponseDto>.Ok(
                new EmployeeDocumentResponseDto
                {
                    DocumentId = document.Id,
                    DocumentCategory = document.DocumentType.DocumentCategory.Name,
                    DocumentType = document.DocumentType.Name,
                    OriginalFileName = document.OriginalFileName,
                    StoredFileName = document.StoredFileName,
                    BlobUrl = document.BlobUrl,
                    UploadedDate = document.UploadedDate
                }, "Document replaced successfully");
        }

        public async Task<ServiceResponseDto<DocumentViewResponseDto>> GetDocumentUrlAsync(int documentId)
        {
            var document = _documentRepository.GetDocumentById(documentId);

            if (document == null)
            {
                return ServiceResponseDto<DocumentViewResponseDto>.Fail("Document not found");
            }

            var sasUrl = _blobService.GenerateReadSasUrl(document.StoredFileName);

            return ServiceResponseDto<DocumentViewResponseDto>.Ok(
                new DocumentViewResponseDto { SasUrl = sasUrl });
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

            return ServiceResponseDto<ICollection<DocumentTypeDto>>.Ok(
                 result, "Document types fetched successfully");
        }

        public ServiceResponseDto<DashboardDto> GetEmployeeDashboard(int employeeId)
        {
            var data = _employeeRepository.GetDashboardData(employeeId);

            if (data.Employee == null)
            {
                return ServiceResponseDto<DashboardDto>.Fail("Employee not found");
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

            return ServiceResponseDto<DashboardDto>.Ok(
                 new DashboardDto
                 {
                     EmployeeName = data.Employee.Name,
                     DepartmentName = data.Employee.Department?.DepartmentName ?? "Department not assigned",
                     DateOfJoining = data.Employee.DateOfJoining,
                     TotalMandatoryDocuments = totalMandatoryDocuments,
                     UploadedDocuments = uploadedDocuments,
                     MissingDocuments = missingDocuments,
                     CompletionPercentage = completionPercentage,
                     RequiredDocuments = requiredDocuments
                 },
                 "Dashboard fetched successfully");
        }

        public ServiceResponseDto<EmployeeDocumentResponseDto>? ValidateFile(IFormFile file)
        {
            const long maxFileSize = 50 * 1024 * 1024; // 50 MB

            if (file == null || file.Length == 0)
            {
                return ServiceResponseDto<EmployeeDocumentResponseDto>.Fail("Please select a file");
            }

            if (file.Length > maxFileSize)
            {
                return ServiceResponseDto<EmployeeDocumentResponseDto>.Fail("File size cannot exceed 50 MB");
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
                return ServiceResponseDto<EmployeeDocumentResponseDto>.Fail(
                    "Only PDF, DOC, DOCX, JPG, JPEG and PNG files are allowed");
            }

            return null;
        }



        //validate the excel file
        private ServiceResponseDto<EmployeeUploadExcelResponseDto>? ValidateExcelFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail("Please select a valid file.");
            }

            var allowedExtensions = new[] { ".xlsx" };

            var extension = Path.GetExtension(file.FileName);

            if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail("Only .xlsx files are allowed.");
            }

            return null;
        }

        //validate excel template
        private ServiceResponseDto<EmployeeUploadExcelResponseDto>? ValidateTemplate(IXLWorksheet worksheet)
        {
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
                var actualHeader = worksheet.Cell(1, i + 1)
                    .GetString()
                    .Trim();

                if (!actualHeader.Equals(
                    expectedHeaders[i],
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail(
                        $"Invalid template. Expected column: '{expectedHeaders[i]}'. Please download the latest template.");
                }
            }

            return null;
        }

        //parse helper
        private DateOnly ParseDateOnly(IXLCell cell)
        {
            if (cell.TryGetValue<DateTime>(out var dt))
                return DateOnly.FromDateTime(dt);

            return default;
        }

        private long ParseLong(IXLCell cell)
        {
            var value = cell.GetString().Trim();

            if (long.TryParse(value, out var result))
                return result;

            return 0;
        }
        private decimal ParseDecimal(IXLCell cell)
        {
            if (decimal.TryParse(cell.GetString(), out var value))
                return value;

            return 0;
        }

        //upload employee parse excel
        private List<EmployeeExcelUploadDto> ParseExcel(IXLWorksheet worksheet)
        {
            var employees = new List<EmployeeExcelUploadDto>();

            var rows = worksheet.RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                var dto = new EmployeeExcelUploadDto
                {
                    EmployeeCode = row.Cell(1).GetString().Trim(),
                    Name = row.Cell(2).GetString().Trim(),
                    Gender = row.Cell(3).GetString().Trim(),

                    DateOfBirth = ParseDateOnly(row.Cell(4)),
                    EmailId = row.Cell(5).GetString().Trim(),

                    Mobile = ParseLong(row.Cell(6)),
                    Salary = ParseDecimal(row.Cell(7)),

                    DateOfJoining = ParseDateOnly(row.Cell(8)),
                    DepartmentName = row.Cell(9).GetString().Trim()
                };

                employees.Add(dto);
            }

            return employees;
        }

        //to get uploaded and exsiting in the contextr
        private async Task<EmployeeUploadValidationContext> BuildContextAsync(List<EmployeeExcelUploadDto> rows)
        {
            var uploadedCodes = rows
                                .Select(x => x.EmployeeCode)
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var uploadedEmails = rows
                                .Select(x => x.EmailId)
                                .Where(x => !string.IsNullOrWhiteSpace(x))
                                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var uploadedMobiles = rows
                                  .Select(x => x.Mobile)
                                  .ToHashSet();


            var existingEmployeeCodes = (await _employeeRepository.GetExistingEmployeeCodesAsync(uploadedCodes)).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingEmails = (await _employeeRepository.GetExistingEmailsAsync(uploadedEmails)).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var existingMobiles = (await _employeeRepository.GetExistingMobilesAsync(uploadedMobiles)).ToHashSet();

            var departments = _employeeRepository.GetDepartments().ToDictionary(
                               x => x.DepartmentName.Trim(),
                               x => x.Id,
                               StringComparer.OrdinalIgnoreCase);

            return new EmployeeUploadValidationContext
            {
                ExistingEmployeeCodes = existingEmployeeCodes,
                ExistingEmails = existingEmails,
                ExistingMobiles = existingMobiles,

                UploadedEmployeeCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                UploadedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                UploadedMobiles = new HashSet<long>(),

                Departments = departments
            };
        }

        //processRowAsync
        private async Task<UploadResult> ProcessRowsAsync(List<EmployeeExcelUploadDto> rows, EmployeeUploadValidationContext context, EmployeeExcelUploadValidator validator)
        {
            var result = new UploadResult
            {
                ValidEmployees = new List<Employee>(),
                Errors = new List<UploadEmployeeExcelErrorDto>()
            };

            int rowNumber = 2;

            foreach (var dto in rows)
            {
                var errors = new List<string>();

                // 1. FluentValidation
                var validation = await validator.ValidateAsync(dto);
                errors.AddRange(validation.Errors.Select(x => x.ErrorMessage));

                // 2. Duplicate validation
                errors.AddRange(_duplicateUploadValidator.Validate(dto, context));

                if (errors.Any())
                {
                    result.Errors.Add(new UploadEmployeeExcelErrorDto
                    {
                        RowNumber = rowNumber,
                        EmployeeData = dto,
                        ErrorMessage = string.Join(", ", errors)
                    });

                    rowNumber++;
                    continue;
                }

                // mark duplicates 
                context.UploadedEmployeeCodes.Add(dto.EmployeeCode);
                context.UploadedEmails.Add(dto.EmailId);
                context.UploadedMobiles.Add(dto.Mobile);

                // mapping
                result.ValidEmployees.Add(new Employee
                {
                    EmployeeCode = dto.EmployeeCode,
                    Name = dto.Name,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth,
                    EmailId = dto.EmailId,
                    Mobile = dto.Mobile,
                    Salary = dto.Salary,
                    DateOfJoining = dto.DateOfJoining,
                    DepartmentId = context.Departments[dto.DepartmentName]
                });

                rowNumber++;
            }

            return result;
        }

        //upload excel response
        private ServiceResponseDto<EmployeeUploadExcelResponseDto> BuildResponse(UploadResult result, int totalRecords)
        {
            var uploadResponse = new EmployeeUploadExcelResponseDto
            {
                TotalRecords = totalRecords,
                SuccessRecords = result.ValidEmployees.Count,
                FailedRecords = result.Errors.Count,
                Errors = result.Errors
            };

            return new ServiceResponseDto<EmployeeUploadExcelResponseDto>
            {
                Success = result.Errors.Count == 0,
                Message = result.ValidEmployees.Count == 0
                    ? "No employee records were uploaded due to validation errors."
                    : result.Errors.Any()
                        ? $"{result.ValidEmployees.Count} records uploaded successfully and {result.Errors.Count} records failed validation."
                        : "All employee records uploaded successfully.",
                Data = uploadResponse
            };
        }

        public async Task<ServiceResponseDto<EmployeeUploadExcelResponseDto>> UploadEmployeesAsync(IFormFile file)
        {

            var fileValidation = ValidateExcelFile(file);

            if (fileValidation != null)
            {
                return fileValidation;
            }

            using var stream = new MemoryStream();

            await file.CopyToAsync(stream);

            stream.Position = 0;

            using var workbook = new XLWorkbook(stream);

            if (workbook.Worksheets.Count != 1)
            {
                return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail(
                    "Excel file should contain only one worksheet.");
            }

            var worksheet = workbook.Worksheet(1);

            var templateValidation = ValidateTemplate(worksheet);

            if (templateValidation != null)
            {
                return templateValidation;
            }

            var rows = ParseExcel(worksheet);

            if (!rows.Any())
            {
                return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail(
                    "Excel file does not contain any records.");
            }

            if (rows.Count > 100)
            {
                return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail(
                    "Maximum 100 employee records can be uploaded at a time.");
            }

            var context = await BuildContextAsync(rows);


            var validator = new EmployeeExcelUploadValidator(context);

            var result = await ProcessRowsAsync(rows, context, validator);

            if (result.ValidEmployees.Any())
            {
                await _employeeRepository
                    .BulkInsertAsync(result.ValidEmployees);
            }

            return BuildResponse(result, rows.Count);
        }


        //**Upload Employee Excel Data
        /*  public async Task<ServiceResponseDto<EmployeeUploadExcelResponseDto>> UploadEmployeesAsync(IFormFile file)
          {
              var response = new ServiceResponseDto<EmployeeUploadExcelResponseDto>();

              //empty file upload
              if (file == null || file.Length == 0)
              {
                  return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail("Please select a valid file.");
              }

              //File Extension Validation
              var allowedExtensions = new[] { ".xlsx" };

              var extension = Path.GetExtension(file.FileName);

              if (!allowedExtensions.Contains(extension,StringComparer.OrdinalIgnoreCase))
              {
                  return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail("Only .xlsx files are allowed.");
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
                  return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail(
                      "Excel file should contain only one worksheet.");
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
                      return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail(
                          $"Invalid template. Expected column: '{expectedHeaders[i]}'. Please download the latest template.");
                  }
              }

              var rows = worksheet.RowsUsed().Skip(1).ToList();

              //no data in the excel
              if (!rows.Any())
              {
                  return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail(
                      "Excel file does not contain any records.");
              }

              // Maximum 100 Records
              if (rows.Count > 100)
              {
                  return ServiceResponseDto<EmployeeUploadExcelResponseDto>.Fail(
                      "Maximum 100 employee records can be uploaded at a time.");
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

                          ErrorMessage = string.Join(",", rowErrors)
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
          }*/

        public ServiceResponseDto<byte[]> DownloadTemplate()
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

            var departmentRange = worksheet.Range("I2:I101");

            departmentRange.CreateDataValidation()
               .List($"\"{departmentList}\"");


            worksheet.Column(4).Style.DateFormat.Format = "dd/MM/yyyy";

            worksheet.Column(8).Style.DateFormat.Format = "dd/MM/yyyy";

            //auto column width
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return ServiceResponseDto<byte[]>.Ok(stream.ToArray(), "Template generated successfully.");
        }

        public ServiceResponseDto<byte[]> DownloadFailedRecordsAsync(List<UploadEmployeeExcelErrorDto> errors)
        {
            var response = new ServiceResponseDto<byte[]>();

            if (errors == null || !errors.Any())
            {
                return ServiceResponseDto<byte[]>.Fail("No failed records available.");
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

            return ServiceResponseDto<byte[]>.Ok(
                stream.ToArray(), "Failed records file generated successfully.");
        }


        public ServiceResponseDto<IEnumerable<RoleDto>> GetRoles()
        {
            var roles = _roleRepository.GetRoles();

            var roleDtos = roles.Select(r => new RoleDto
            {
                Id = r.Id,
                RoleName = r.RoleName
            }).ToList();

            return ServiceResponseDto<IEnumerable<RoleDto>>.Ok(
               roleDtos, "Roles fetched successfully");
        }

        public ServiceResponseDto<IEnumerable<EmployeeDropdownDto>> GetManagers()
        {
            var managers = _employeeRepository.GetManagers().Select(e => new EmployeeDropdownDto
            {
                Id = e.Id,
                ManagerName = e.Name
            });

            return ServiceResponseDto<IEnumerable<EmployeeDropdownDto>>.Ok(managers);
        }
    }
}


