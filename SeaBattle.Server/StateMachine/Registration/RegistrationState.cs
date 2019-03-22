namespace SeaBattle.Server.StateMachine.Registration
{
    public enum RegistrationState
    {
        Started,
        
        WaitingForName,
        
        WaitingForStrategy,
        
        ReadyToFinish,
        
        Registered,
        
        Canceled
    }
}