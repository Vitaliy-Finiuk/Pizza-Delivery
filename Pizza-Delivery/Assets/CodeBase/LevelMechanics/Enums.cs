namespace CodeBase.LevelMechanics
{
    //===================================================================================

    public enum ChunkType
    {
        Null,
        TurnPoint,
        Collectible,
        Obstacle,
        Gate,
        Stair,
        Random
    }

    //===================================================================================

    public enum SplineType
    {
        Straight,
        Left,
        Right,
        UpAndDown,
        Random
    }

    //===================================================================================

    public enum StageProgress
    {
        Null,
        RunnerStage,
        IdleStage
    }

    //===================================================================================

    public enum ProductionAlgorithm
    {
        Standard,
        Multiplicative,
        Additive,
        Custom
    }

    //===================================================================================

    public enum GateType
    {
        Positive,
        Negative
    }

    //===================================================================================

    public enum ObstacleType
    {
        Backpacker,
        Trolley,
        Tent
    }

    //===================================================================================

    public interface ICollidable
    {
        void OnCollision();
    }

    //===================================================================================

    public interface IPurchaseable
    {
        double purchaseableCost { get; set; }

        void Purchase();
    }

    //===================================================================================

}