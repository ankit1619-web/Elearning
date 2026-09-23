using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Interface
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string text, string targetLang = "hi", string sourceLang = "en"); 
    }
}
