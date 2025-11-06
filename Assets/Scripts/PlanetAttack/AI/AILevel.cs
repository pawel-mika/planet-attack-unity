namespace PlanetAttack.AI
{
    public record AILevel
    {

        public static readonly AILevel EASY = new(0.15f, 8, 8, 0.4f, 0.6f, 0.5f, 0.5f, 10f);
        public static readonly AILevel NORMAL = new(0.25f, 7, 7, 0.4f, 0.6f, 0.5f, 0.5f, 25f);
        public static readonly AILevel HARD = new(0.5f, 6, 6, 0.45f, 0.55f, 0.55f, 0.45f, 50f);
        public static readonly AILevel MEGAMIND = new(1f, 5, 5, 0.5f, 0.5f, 0.6f, 0.4f, 75f);

        public float DecisionsPerSecond { get; }
        public int AttackSpeed { get; }
        public int TransferSpeed { get; }
        public float AttackThreshold { get; }
        public float TransferThreshold { get; }
        public float AttackWeight { get; }
        public float TransferWeight { get; }
        public float MaxFreePlanetShips { get; }

        private AILevel(float decisionsPerSecond, int attackSpeed, int transferSpeed, float attackThreshold,
                float transferThreshold, float attackWeight, float transferWeight, float maxFreePlanetShips)
        {
            this.DecisionsPerSecond = decisionsPerSecond;
            this.AttackSpeed = attackSpeed;
            this.TransferSpeed = transferSpeed;
            this.AttackThreshold = attackThreshold;
            this.TransferThreshold = transferThreshold;
            this.AttackWeight = attackWeight;
            this.TransferWeight = transferWeight;
            this.MaxFreePlanetShips = maxFreePlanetShips;
        }

    }
}