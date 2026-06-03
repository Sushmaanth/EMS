using Azure.Core;
using EMSFrontend.Api.Abstraction;
using EMSFrontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace EMSFrontend.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IRequest _request;

        public EmployeeController(IRequest request)
        {
            _request = request;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            try
            {
                string token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "Auth");
                }
                return View();
            }
            catch (UnauthorizedAccessException)
            {
                TempData["SessionExpired"] = "Your session has expired. Please login again.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Documents()
        {
            EmployeeDocumentPageViewModel model = new();

            //categories
            var categories = await _request.SendGetDocumentCategoriesAsync();

            model.Categories = categories.ToList();

            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;

            //get category id
            var firstCategory = model.Categories.FirstOrDefault()?.Id ?? 0;

            var documentType = await _request.SendGetDocumentTypesByCategoryAsync(firstCategory,employeeId);

            model.DocumentTypes = documentType.ToList();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentCards(int categoryId)
        {
            var employeeId = HttpContext.Session.GetInt32("EmployeeId") ?? 0;
            var documents = await _request.SendGetDocumentTypesByCategoryAsync(categoryId, employeeId);

            return PartialView("_DocumentCardsPartial", documents);
        }

        [HttpPost]
        public async Task<IActionResult>UploadDocument(EmployeeDocumentUploadViewModel model)
        {
            model.EmployeeId = HttpContext.Session.GetInt32("EmployeeId")?? 0;

            var result = await _request.SendUploadDocumentAsync(model);
            return Json(result);
        }

        [HttpDelete]
        public async Task<IActionResult>DeleteDocument(int documentId)
        {
            var result = await _request.SendDeleteDocumentAsync(documentId);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult>ViewDocument(int documentId)
        {
            var result = await _request.SendViewDocumentAsync(documentId);
            
            Console.WriteLine(result.SasUrl);

            return Redirect(result.SasUrl);
        }

        [HttpPut]
        public async Task<IActionResult> ReplaceDocument([FromForm] ReplaceDocumentViewModel model)
        {
            var result =
                await _request.SendReplaceDocumentAsync(model);

            return Json(result);
        }
    }
}
