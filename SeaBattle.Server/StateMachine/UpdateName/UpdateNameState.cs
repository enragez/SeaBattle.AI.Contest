namespace SeaBattle.Server.StateMachine.UpdateName
{
    public enum UpdateNameState
    {
        Started,
        
        WaitingForName,
        
        Canceled,
        
        ReadyToFinish,
        
        Updated
    }
}