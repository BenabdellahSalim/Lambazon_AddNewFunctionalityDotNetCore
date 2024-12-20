﻿using Microsoft.AspNetCore.Http;

namespace P3AddNewFunctionalityDotNetCore.Application.Services
{
    public interface ILanguageService
    {
        void ChangeUiLanguage(HttpContext context, string language);
        string SetCulture(string language);
        void UpdateCultureCookie(HttpContext context, string culture);
    }
}
