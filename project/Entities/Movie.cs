namespace movie_recommendation.Entities
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; }
        public string Genres { get; set; }
        public int ImdbId { get; set; }
        public int TmdbId { get; set; }
    }
}
