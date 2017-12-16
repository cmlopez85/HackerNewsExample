namespace HackerNewsExample.Models
{
    /// <summary>  
    ///  This class exists to store the relevant information about news stories from Hacker News,
    ///  which is currently only the title and the user that submitted the story.
    /// </summary>  
    public class Story
    {
        public string title { get; set; }
        public string by { get; set; }
    }
}