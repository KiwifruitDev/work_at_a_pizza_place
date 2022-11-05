namespace WorkAtAPizzaPlace
{
    public class SoundEvents
    {
        // Use Enumerables.TimeOfDay.TIME_OF_DAY_COUNT to initialize an array of sound events.
        public static string[] BackgroundMusic = new string[(int)Enumerables.TimeOfDay.TIME_OF_DAY_COUNT]
        {
            "pizzaplace.music.midday",
            "pizzaplace.music.midday",
            "pizzaplace.music.midday",
            "pizzaplace.music.midday",
        };
        public static string EntranceTone = "pizzaplace.sfx.entrancetone";
    }
}