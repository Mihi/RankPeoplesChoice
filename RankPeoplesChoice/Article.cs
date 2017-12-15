using System.Collections.Generic;

namespace RankPeoplesChoice
{
    public class Article
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Link { get; set; }
        public string Body { get; set; }
        public Dictionary<int, string> CorrectGuesses { get; set; } = new Dictionary<int, string>();
    }
}