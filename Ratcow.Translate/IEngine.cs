using System;
using System.Threading.Tasks;

namespace Ratcow.Translation
{
    public interface IEngine
    {
        Task<string> CyrilicTransliterate(string data, (string Name, string Code, string Encoding, bool Transliterate) language);
        Task<string> GetAutoTranslation(string data, (string Name, string Code, string Encoding, bool Transliterate) toLanguage);
        Task<string> GetFromEnglishTranslation(string data, (string Name, string Code, string Encoding, bool Transliterate) toLanguage);
        Task<string[]> GetEnglishTranslations(string data, System.Collections.Generic.IEnumerable<(string Name, string Code, string Encoding, bool Transliterate)> languages, Action<int, int> progressCallback = null);
        Task<string> GetTranslation(string data, (string Name, string Code, string Encoding, bool Transliterate) fromLanguage, (string Name, string Code, string Encoding, bool Transliterate) toLanguage);
        Task<string[]> GetTranslations(string data, (string Name, string Code, string Encoding, bool Transliterate) sourceLangauge, System.Collections.Generic.IEnumerable<(string Name, string Code, string Encoding, bool Transliterate)> languages, Action<int, int> progressCallback = null);
    }
}