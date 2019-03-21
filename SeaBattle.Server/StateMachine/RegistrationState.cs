namespace SeaBattle.Server.StateMachine
{
    public enum RegistrationState
    {
        Started,
        
        WaitingForName,
        
        WaitingForStrategy,
        
        Registered,
        
        Canceled
    }
}