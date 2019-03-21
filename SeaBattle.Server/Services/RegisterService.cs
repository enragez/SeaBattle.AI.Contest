namespace SeaBattle.Server.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using StateMachine;
    using Telegram.Bot.Types;

    public class RegisterService : IRegisterService
    {
        private readonly IServiceProvider _serviceProvider;
        
        private readonly Dictionary<int, IRegistrationStateMachine> _stateMachines;
        
        
        public RegisterService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _stateMachines = new Dictionary<int, IRegistrationStateMachine>();
        }

        public async Task MoveMext(Update update)
        {
            if (_stateMachines.TryGetValue(update.Message.From.Id, out var machine))
            {
                await machine.MoveNext(update);
                if (machine.State == RegistrationState.Registered)
                {
                    var model = machine.Register();
                    _stateMachines.Remove(update.Message.From.Id);
                    // todo finish registration
                }
                return;
            }
            
            machine = _serviceProvider.GetRequiredService<IRegistrationStateMachine>();
            _stateMachines.Add(update.Message.From.Id, machine);

            await machine.MoveNext(update);
        }

        public bool UserRegistering(int userId)
        {
            return _stateMachines.ContainsKey(userId) &&
                   _stateMachines[userId].State != RegistrationState.Registered &&
                   _stateMachines[userId].State != RegistrationState.Canceled;
        }
    }
}