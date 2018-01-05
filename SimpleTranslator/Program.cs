using Ratcow.Translation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTranslator
{
    class Program
    {
        static ValueTuple<string, string, string, bool> defaultTuple = default(ValueTuple<string, string, string, bool>);

        /// <summary>
        /// this is not a full list, this is a list for the languages I wanted to be able to translate.
        /// 
        /// I will make the code autodetect the encoding in a later iteration (it's in the rreturned document.)
        /// </summary>
        static (string Name, string Code, string Encoding, bool Transliterate)[] languages =
        {
            ("english", "en", "iso-8859-1", false),
            //west 
            ("polish", "pl", "iso-8859-2", false), ("czech", "cs", "iso-8859-2", false), ("slovak", "sk", "iso-8859-2", false),
            //south west
            ("croatian", "hr", "iso-8859-2", false), ("serbian", "sr", "iso-8859-2", true), ("bosnian", "bs", "iso-8859-2", false),
            ("slovinian", "sl", "iso-8859-2", false),
            //South east
            ("bulgarian", "bg", "windows-1251", true), ("macedonian", "mk", "windows-1251", true),
            //eastern
            ("russian", "ru", "windows-1251", true), ("belarusian", "be", "windows-1251", true), ("ukrainian", "uk", "windows-1251",true),
            //skandinavian
            ("swedish", "sv", "iso-8859-1", false),
            ("norwegian", "no", "iso-8859-1", false),
            ("danish", "da", "iso-8859-1", false),
            ("icelandic", "is", "iso-8859-1", false)
        };

        static void Main(string[] args)
        {

            if (args.Length >= 2)
            {
                //language
                var langaugeCode = args[0];
                var language = languages.FirstOrDefault(l => l.Code == langaugeCode);
                if (!language.Equals(defaultTuple))
                {
                    if (args.Length > 2)
                    {
                        langaugeCode = args[1];
                        var language2 = languages.FirstOrDefault(l => l.Code == langaugeCode);
                        if (!language.Equals(defaultTuple))
                        {
                            var task = Translate(language, language2, args[2]);
                            task.Wait(TimeSpan.FromSeconds(30));
                            return;
                        }
                    }
                    else
                    {
                        var task = Translate(language, args[1]);
                        task.Wait(TimeSpan.FromSeconds(30));
                        return;
                    }
                }
            }

            Console.WriteLine("Type : \r\n\tSimpleTranslator.exe {code} {phrase}");
            Console.WriteLine("\tSimpleTranslator.exe {from_code} {to_code} {phrase}");
            Console.WriteLine("Valid codes are:");
            foreach (var tlanguage in languages)
            {
                Console.WriteLine($"\t{tlanguage.Code} - {tlanguage.Name}");
            }
            Console.WriteLine();
        }

        static Task Translate((string Name, string Code, string Encoding, bool Transliterate) language, string data)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine($"English to {language.Name}");
                var engine = new Engine();
                var result = await engine.GetFromEnglishTranslation(data, language);
                Console.WriteLine($"{data} : {result}");
            });
        }

        static Task Translate((string Name, string Code, string Encoding, bool Transliterate) language, (string Name, string Code, string Encoding, bool Transliterate) language2, string data)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine($"{language.Name} to {language2.Name}");
                var engine = new Engine();
                var result = await engine.GetTranslation(data, language, language2);
                Console.WriteLine($"{data} : {result}");
            });
        }
    }
}
