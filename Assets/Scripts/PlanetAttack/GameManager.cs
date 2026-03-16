using PlanetAttack;

public static class GameManager
{
    public static PlanetsController PlanetsController = PlanetsController.Instance;

    public static GameController GameController = GameController.Instance;

    public static ActionsController ActionsController = ActionsController.Instance;

    public static void StartGame()
    {
        GameBoardUtils.GeneratePlanets();
        GameBoardUtils.RandomizePlanetsInSpace();
        GameBoardUtils.InitializePlanetsState();
        GameBoardUtils.RandomizeStartingPlanets();
    }

    public static void EndGame()
    {
        GameBoardUtils.CleanupBoard();
        ActionsController.KillAllActions();
    }
}