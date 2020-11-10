namespace GoogleVilnius
{
    public class CollectedData
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public CollectedData(string link, string title, string text)
        {
            this.Link = link;
            this.Title = title;
            this.Text = text;
        }
    }
}
