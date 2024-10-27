using Microsoft.AspNetCore.Mvc;
using P3AddNewFunctionalityDotNetCore.Application.Services;

namespace P3AddNewFunctionalityDotNetCore.Components
{
    public class LanguageSelectorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ILanguageService languageService)
        {
            return View(languageService);
        }
    }
}
