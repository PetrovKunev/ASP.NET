namespace CinemaApp.Common
{
    public static class EntityValidationConstants
    {
        public static class Movie
        {
            public const int TitleMaxLength = 50;
            public const int GenreMinLength = 5;
            public const int GenreMaxLength = 20;
            public const int DirectorMinLength = 10;
            public const int DirectorMaxLength = 80;
            public const int DescriptionMinLength = 50;
            public const int DescriptionMaxLength = 500;
            public const int DurationMinValue = 1;
            public const int DurationMaxValue = 999;
            public const string ReleaseDateFormat = "MM/yyyy";
        }
    }
}
