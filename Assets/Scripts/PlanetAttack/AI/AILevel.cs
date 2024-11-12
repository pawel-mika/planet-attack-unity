namespace PlanetAttack.AI
{
    public record AILevel
    {

        public static readonly AILevel EASY = new(0.15f, 8, 8, 0.4f, 0.6f, 0.5f, 0.5f);
        public static readonly AILevel NORMAL = new(0.25f, 7, 7, 0.4f, 0.6f, 0.5f, 0.5f);
        public static readonly AILevel HARD = new(0.5f, 6, 6, 0.45f, 0.55f, 0.55f, 0.45f);
        public static readonly AILevel MEGAMIND = new(1f, 5, 5, 0.5f, 0.5f, 0.6f, 0.4f);

        public float DecisionsPerSecond { get; }
        private int AttackSpeed { get; }
        private int TransferSpeed { get; }
        private float AttackThreshold { get; }
        private float TransferThreshold { get; }
        private float AttackWeight { get; }
        private float TransferWeight { get; }

        private AILevel(float decisionsPerSecond, int attackSpeed, int transferSpeed, float attackThreshold,
                float transferThreshold, float attackWeight, float transferWeight)
        {
            this.DecisionsPerSecond = decisionsPerSecond;
            this.AttackSpeed = attackSpeed;
            this.TransferSpeed = transferSpeed;
            this.AttackThreshold = attackThreshold;
            this.TransferThreshold = transferThreshold;
            this.AttackWeight = attackWeight;
            this.TransferWeight = transferWeight;
        }

    }
}