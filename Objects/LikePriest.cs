namespace GridExplorerBot
{
    public class LikePriest : StaticObject
    {
        public override string TalkTo(Objects.ID subject, Game game)
        {
            if (subject == Objects.ID.ThumbsUp)
            {
                return "Likes are our bread and butter. They're worth 1 prayer.";
            }
            else if (subject == Objects.ID.Retweet)
            {
                return "I don't know anything about that.";
            }
            else if (subject == Objects.ID.Worshipper)
            {
                return "They're our worshippers. They offer up their thoughts and prayers.";
            }
            else
            {
                return "We offer a mobile phone for 10 prayers. Like = 1 prayer. Retweet = 5 prayers.";
            }
        }
    }
}