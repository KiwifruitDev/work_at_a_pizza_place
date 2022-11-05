namespace WorkAtAPizzaPlace
{
    public class GlobalVariables
    {
        public Enumerables.TimeOfDay CurrentTimeOfDay = Enumerables.TimeOfDay.Midday;
        public string CurrentMusic = SoundEvents.BackgroundMusic[(int)Enumerables.TimeOfDay.Midday];
    }
}