namespace SeaBattle.Server.StateMachine.UpdateStrategy
{
    public enum UpdateStrategyState
    {
        Started,
        
        WaitingForStrategy,
        
        Canceled,
        
        ReadyToFinish,
        
        Updated
    }
}