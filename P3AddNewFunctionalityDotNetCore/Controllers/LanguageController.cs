using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Application.Services;
using P3AddNewFunctionalityDotNetCore.Data.Models.ViewModels;

namespace P3AddNewFunctionalityDotNetCore.Controllers
{
    public class LanguageController : Controller
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeUiLanguage(LanguageViewModel model, string returnUrl)
        {
            if (model.Language != null)
            {
                _languageService.ChangeUiLanguage(HttpContext, model.Language);
            }

            return Redirect(returnUrl);
        }
    }
}
