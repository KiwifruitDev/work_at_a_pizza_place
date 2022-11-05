namespace WorkAtAPizzaPlace
{
    public static class Enumerables
    {
        public enum Order {
            PepperoniPizza,
            SausagePizza,
            CheesePizza,
            Fizzly,
            ORDER_COUNT,
        }
        public enum TimeOfDay {
            Morning,
            Midday,
            Evening,
            Night,
            TIME_OF_DAY_COUNT, // Not an actual time of day, used to count the amount of items.
        }
    }
}