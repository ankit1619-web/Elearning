using GTranslate.Translators;
using Manikalearning.Application.Interface;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Manikalearning.Application.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly GoogleTranslator _googleTranslator;
        private readonly BingTranslator _bingTranslator;

        public TranslationService()
        {
            _googleTranslator = new GoogleTranslator();
            _bingTranslator = new BingTranslator();
        }

        public async Task<string> TranslateAsync(string text, string targetLang = "hi", string sourceLang = "auto")
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            try
            {
                var bingResult = await _bingTranslator.TranslateAsync(text, targetLang, sourceLang);
                return !string.IsNullOrWhiteSpace(bingResult?.Translation) ? bingResult.Translation : text;
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("429"))
            { 
                try
                {
                    var result = await _googleTranslator.TranslateAsync(text, targetLang, sourceLang);
                    return !string.IsNullOrWhiteSpace(result?.Translation) ? result.Translation : text;

                }
                catch
                {
                    return text;  
                }
            }
            catch (Exception)
            { 
                return text;
            }
        }
    }

}
