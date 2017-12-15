using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace RankPeoplesChoice
{
    class Program
    {
        private static Dictionary<int, string> _top10;

        static void Main()
        {
            _top10 = ReadTop10FromFile();
            var articles = GetArticles();
            CalculateMatches(articles);

            var maxCorrect = articles.Max(p => p.CorrectGuesses.Count);
            foreach (var article in articles.Where(p=>p.CorrectGuesses.Count == maxCorrect))
            {
                Console.WriteLine(article.Username);
                Console.WriteLine(article.Link);
                foreach (var correctGuessesValue in article.CorrectGuesses.Values)
                {
                    Console.WriteLine(correctGuessesValue);
                }

                Console.WriteLine();
            }
        }

        private static Dictionary<int, string> ReadTop10FromFile()
        {
            var result = new Dictionary<int, string>();
            var lines = File.ReadAllLines(@".\Top10.txt");
            for (int i = 1; i <= 10; i++)
            {
                var line = lines[i - 1];
                result.Add(i, line);
            }

            return result;
        }

        private static void CalculateMatches(List<Article> articles)
        {
            foreach (var article in articles)
            {
                ParseGuesses(article);
            }
        }

        private static void ParseGuesses(Article article)
        {
            var body = article.Body;
            var lines = body.Split("<br/>");
            if (lines.Length < 10) return;

            var correctGuesses = new Dictionary<int, string>();
            foreach (var line in lines)
            {
                foreach (var top10Value in _top10)
                {
                    if (line.ToLower().Contains(top10Value.Value) && line.Contains(top10Value.Key.ToString()))
                    {
                        correctGuesses.Add(top10Value.Key, line);
                    }
                }
            }

            article.CorrectGuesses = correctGuesses;
        }

        private static List<Article> GetArticles()
        {
            var xml = new WebClient().DownloadString("https://www.boardgamegeek.com/xmlapi2/thread?id=1901961");
            XDocument doc = XDocument.Parse(xml);
            var articles = doc.Descendants("article");

            var result = new List<Article>();
            foreach (var article in articles)
            {
                var art = new Article();
                art.Id = int.Parse(article.Attribute("id").Value);
                art.Username = article.Attribute("username").Value;
                art.Link = article.Attribute("link").Value;
                art.Body = article.Element("body").Value;

                result.Add(art);
            }

            return result;
        }
    }
}
