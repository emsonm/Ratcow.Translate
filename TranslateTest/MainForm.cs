using NickBuhro.Translit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace TranslateTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            updateProgress = new UpdateProgressDelegate(UpdateProgress);
        }

        (string Name, string Code, string Encoding, bool Transliterate)[] languages =
        {
            //west 
            ("polish", "pl", "iso-8859-2", false), ("czech", "cs", "iso-8859-2", false), ("slovak", "sk", "iso-8859-2", false),
            //south west
            ("croatian", "hr", "iso-8859-2", false), ("serbian", "sr", "iso-8859-2", true), ("bosnian", "bs", "iso-8859-2", false), ("slovinian", "sl", "iso-8859-2", false),
            //South east
            ("bulgarian", "bg", "windows-1251", true), ("macedonian", "mk", "windows-1251", true),
            //eastern
            ("russian", "ru", "windows-1251", true), ("belarusian", "be", "windows-1251", true), ("ukrainian", "uk", "windows-1251",true)
        };

        (string Name, string Code, string Encoding, bool Transliterate)[] skandinavian_languages =
        {
             ("swedish", "sv", "iso-8859-1", false),
             ("norwegian", "no", "iso-8859-1", false),
             ("danish", "da", "iso-8859-1", false),
             ("icelandic", "is", "iso-8859-1", false)
        };

        async void button1_Click(object sender, EventArgs e)
        {
            var data = textBox1.Text;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = languages.Length;

            var translator = new Ratcow.Translation.Engine();

            if (!string.IsNullOrEmpty(data))
            {
                var results = await translator.GetEnglishTranslations(
                    data,
                    languages,
                    (int count, int position) =>
                    {
                        UpdateProgress(count, position);
                    });

                if (Properties.Settings.Default.OutputResults)
                {
                    await SaveTranslations(data, results);
                }

                listBox1.DataSource = results;
            }
        }

        string MakeValidFileName(string name)
        {
            var invalidChars = new string(Path.GetInvalidFileNameChars());
            var escapedInvalidChars = Regex.Escape(invalidChars);
            var invalidRegex = string.Format(@"([{0}]*\.+$)|([{0}]+)", escapedInvalidChars);

            return Regex.Replace(name, invalidRegex, "_");
        }

        async Task SaveTranslations(string data, string[] results)
        {
            await Task.Run(() =>
            {
                using (var file = new StreamWriter(File.Create(Path.Combine(Properties.Settings.Default.OutputDirectory, $"{MakeValidFileName(data.Substring(0, data.Length > 10 ? 10 : data.Length).Trim().Replace(" ", "_"))}.md"))))
                {
                    file.WriteLine($"# {data}");
                    foreach (var item in results)
                    {
                        file.WriteLine($"* {item}");
                    }

                    file.Close();
                }
            });
        }

        delegate void UpdateProgressDelegate(int count, int position);

        UpdateProgressDelegate updateProgress;

        private void UpdateProgress(int count, int position)
        {
            if (progressBar1.InvokeRequired)
            {
                this.Invoke(updateProgress, new object[] { count, position });
            }
            else
            {
                progressBar1.Value = position;
            }
        }

        async void button2_Click(object sender, EventArgs e)
        {
            var data = textBox1.Text;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = skandinavian_languages.Length;

            var translator = new Ratcow.Translation.Engine();

            if (!string.IsNullOrEmpty(data))
            {
                var results = await translator.GetEnglishTranslations(
                    data,
                    skandinavian_languages,
                    (int count, int position) =>
                    {
                        UpdateProgress(count, position);
                    });

                if (Properties.Settings.Default.OutputResults)
                {
                    await SaveTranslations(data, results);
                }

                listBox1.DataSource = results;
            }

        }
    }
}
