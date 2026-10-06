namespace WordDeduction
{
    internal static class StarterWords
    {
        internal sealed class Pair
        {
            public string Id;
            public string[] German;
            public string[] English;
            public Pair(string id, string deA, string deB, string enA, string enB) { Id = id; German = new[] { deA, deB }; English = new[] { enA, enB }; }
        }
        internal static readonly Pair[] Pairs = {
            new Pair("arts-media-001", "Gitarre", "Geige", "Guitar", "Violin"),
            new Pair("bakery-sweets-001", "Baguette", "Brötchen", "Baguette", "Bread roll"),
            new Pair("birds-water-creatures-001", "Ente", "Gans", "Duck", "Goose"),
            new Pair("care-health-001", "Zahnbürste", "Zahnseide", "Toothbrush", "Dental floss"),
            new Pair("clothing-001", "T-Shirt", "Hemd", "T-shirt", "Shirt"),
            new Pair("drinks-pantry-001", "Kaffee", "Tee", "Coffee", "Tea"),
            new Pair("fruit-vegetables-001", "Apfel", "Birne", "Apple", "Pear"),
            new Pair("games-leisure-001", "Schach", "Dame", "Chess", "Draughts"),
            new Pair("garden-nature-001", "Rose", "Tulpe", "Rose", "Tulip"),
            new Pair("home-001", "Stuhl", "Hocker", "Chair", "Stool"),
            new Pair("kitchen-001", "Teller", "Schüssel", "Plate", "Bowl"),
            new Pair("landscape-weather-001", "Berg", "Hügel", "Mountain", "Hill"),
            new Pair("mammals-001", "Hund", "Katze", "Dog", "Cat"),
            new Pair("meals-001", "Pizza", "Quiche", "Pizza", "Quiche"),
            new Pair("places-001", "Bahnhof", "Flughafen", "Railway station", "Airport"),
            new Pair("school-office-001", "Bleistift", "Kugelschreiber", "Pencil", "Ballpoint pen"),
            new Pair("sports-001", "Fußball", "Basketball", "Football", "Basketball"),
            new Pair("technology-001", "Smartphone", "Tablet", "Smartphone", "Tablet"),
            new Pair("tools-materials-001", "Hammer", "Schraubendreher", "Hammer", "Screwdriver"),
            new Pair("transport-travel-001", "Auto", "Motorrad", "Car", "Motorcycle"),
        };
    }
}
