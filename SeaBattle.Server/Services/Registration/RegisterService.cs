namespace SeaBattle.Server.Services.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using StateMachine;
    using StateMachine.Registration;
    using Telegram.Bot.Types;

    public class RegisterService : IServiceWithState<RegistrationState>
    {
        private readonly IServiceProvider _serviceProvider;
        
        private readonly ConcurrentDictionary<int, IStateMachine<RegistrationState>> _stateMachines;
        
        public RegisterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _stateMachines = new ConcurrentDictionary<int, IStateMachine<RegistrationState>>();
        }

        public async Task MoveMext(Update update)
        {
            if (_stateMachines.TryGetValue(update.Message.From.Id, out var machine))
            {
                await machine.MoveNext(update);
                switch (machine.State)
                {
                    case RegistrationState.Canceled:
                        _stateMachines.TryRemove(update.Message.From.Id, out _);
                        break;
                    case RegistrationState.ReadyToFinish:
                        await machine.Finish(update);
                        _stateMachines.TryRemove(update.Message.From.Id, out _);
                        break;
                }

                return;
            }
            
            machine = _serviceProvider.GetRequiredService<IStateMachine<RegistrationState>>();
            _stateMachines.TryAdd(update.Message.From.Id, machine);

            await machine.MoveNext(update);

            if (machine.State == RegistrationState.Canceled)
            {
                _stateMachines.TryRemove(update.Message.From.Id, out _);
            }
        }

        public bool IsActive(int userId)
        {
            return _stateMachines.ContainsKey(userId) &&
                   _stateMachines[userId].State != RegistrationState.Registered &&
                   _stateMachines[userId].State != RegistrationState.Canceled;
        }
    }
}