namespace GameLogic
{
    public enum GameState
    {
        Inactive,
        Countdown,
        Play,
        Explosion,
        ReadyToStart,
        Result
    }

    public enum WordCondition
    {
        Begin,
        Anywhere,
        End,
    }
}