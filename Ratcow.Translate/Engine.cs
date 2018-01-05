using NickBuhro.Translit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Ratcow.Translation
{
    public class Engine : IEngine
    {
        readonly (string Name, string Code, string Encoding, bool Transliterate) EnglishLanguage = CreateLanguage("english", "en");

        /// <summary>
        /// Simplify creating Tuples in the correct format
        /// </summary>
        public static (string Name, string Code, string Encoding, bool Transliterate) CreateLanguage(string name, string code, string encoding = "utf-8", bool transliterate = false) => (name, code, encoding, transliterate);


        /// <summary>
        /// Always translates from english to language
        /// </summary>
        public async Task<string[]> GetEnglishTranslations(string data, IEnumerable<(string Name, string Code, string Encoding, bool Transliterate)> languages, Action<int, int> progressCallback = null)
        {
            return await GetTranslations(data, EnglishLanguage, languages, progressCallback);
        }

        /// <summary>
        /// Takes the items in the languages enumeration and process each one to get the translated string.
        /// </summary>
        public async Task<string[]> GetTranslations(string data, (string Name, string Code, string Encoding, bool Transliterate) sourceLangauge, IEnumerable<(string Name, string Code, string Encoding, bool Transliterate)> languages, Action<int, int> progressCallback = null)
        {
            return await Task.Run(async () =>
            {
                var results = new List<string>();

                var count = languages.Count();
                var position = 0;

                foreach (var language in languages)
                {
                    var result = await GetTranslation(data, sourceLangauge, language);
                    if (!string.IsNullOrEmpty(result))
                    {
                        results.Add(result);
                    }

                    progressCallback?.Invoke(count, ++position);
                }

                return results.ToArray();
            });
        }

        /// <summary>
        /// Translates a specific english language string
        /// </summary>
        public async Task<string> GetFromEnglishTranslation(string data, (string Name, string Code, string Encoding, bool Transliterate) toLanguage)
        {
            return await GetTranslation(data, EnglishLanguage, toLanguage);
        }

        /// <summary>
        /// Translates a specific english language string
        /// </summary>
        public async Task<string> GetToEnglishTranslation(string data, (string Name, string Code, string Encoding, bool Transliterate) fromLanguage)
        {
            return await GetTranslation(data, fromLanguage, EnglishLanguage);
        }

        /// <summary>
        /// https://translate.google.co.uk/#auto/is/test
        /// 
        /// does not currently work.. needs some extra love.
        /// </summary>
        public async Task<string> GetAutoTranslation(string data, (string Name, string Code, string Encoding, bool Transliterate) toLanguage)
        {
            return await Task.Run(async () =>
            {
                var url = $"https://translate.google.co.uk/#auto/{toLanguage.Code}/{data}";

                var encoding = Encoding.GetEncoding(toLanguage.Encoding);

                var rawdata = string.Empty;

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(url))
                    {
                        using (var content = response.Content)
                        {
                            var buffer = await content.ReadAsByteArrayAsync();
                            rawdata = encoding.GetString(buffer, 0, buffer.Length);
                        }
                    }
                }

                var result = string.Empty;

                if (!string.IsNullOrEmpty(rawdata))
                {
                    result = rawdata.Substring(rawdata.IndexOf("<span title=\"") + "<span title=\"".Length);
                    result = result.Substring(result.IndexOf(">") + 1);
                    result = result.Substring(0, result.IndexOf("</span>"));

                    result = System.Net.WebUtility.HtmlDecode(result);

                    if (toLanguage.Transliterate)
                    {
                        result = $"{result} ({await CyrilicTransliterate(result, toLanguage)})";
                    }
                }
                return $"{toLanguage.Name} : {result.Trim()}";
            });
        }

        /// <summary>
        /// Translates a specific language string
        /// </summary>
        public async Task<string> GetTranslation(string data, (string Name, string Code, string Encoding, bool Transliterate) fromLanguage, (string Name, string Code, string Encoding, bool Transliterate) toLanguage)
        {
            return await Task.Run(async () =>
            {
                var url = $"http://www.google.com/translate_t?hl=en&ie=UTF8&text={data}&langpair={fromLanguage.Code}|{toLanguage.Code}";

                var encoding = Encoding.GetEncoding(toLanguage.Encoding);

                var rawdata = string.Empty;

                using (var client = new HttpClient())
                {
                    using (var response = await client.GetAsync(url))
                    {
                        using (var content = response.Content)
                        {
                            var buffer = await content.ReadAsByteArrayAsync();
                            rawdata = encoding.GetString(buffer, 0, buffer.Length);
                        }
                    }
                }

                var result = string.Empty;

                if (!string.IsNullOrEmpty(rawdata))
                {
                    result = rawdata.Substring(rawdata.IndexOf("<span title=\"") + "<span title=\"".Length);
                    result = result.Substring(result.IndexOf(">") + 1);
                    result = result.Substring(0, result.IndexOf("</span>"));

                    result = System.Net.WebUtility.HtmlDecode(result);

                    if (toLanguage.Transliterate)
                    {
                        result = $"{result} ({await CyrilicTransliterate(result, toLanguage)})";
                    }
                }
                return $"{toLanguage.Name} : {result.Trim()}";
            });
        }

        /// <summary>
        /// Uses a third party lib to transliterate the Cyrilic to Latin text.
        /// </summary>
        public async Task<string> CyrilicTransliterate(string data, (string Name, string Code, string Encoding, bool Transliterate) language)
        {
            return await Task.Run(() =>
            {
                var latin = Transliteration.CyrillicToLatin(data, GetLanguage(language));
                return latin;
            });

        }

        /// <summary>
        /// This is a hardcoded lookup table for the cyrilic languages supported.
        /// </summary>
        Language GetLanguage((string Name, string Code, string Encoding, bool Transliterate) language)
        {
            switch (language.Code)
            {
                case "be": return Language.Belorussian;
                case "bg": return Language.Bulgarian;
                case "mk": return Language.Macedonian;
                case "ru": return Language.Russian;
                case "uk": return Language.Ukrainian;
                default: return Language.Unknown;
            }
        }
    }
}
