namespace SeaBattle.Server.Services.UpdateNameService
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using StateMachine;
    using StateMachine.UpdateName;
    using Telegram.Bot.Types;

    public class UpdateNameService : IServiceWithState<UpdateNameState>
    {
        private readonly IServiceProvider _serviceProvider;
        
        private readonly ConcurrentDictionary<int, IStateMachine<UpdateNameState>> _stateMachines;
        
        public UpdateNameService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _stateMachines = new ConcurrentDictionary<int, IStateMachine<UpdateNameState>>();
        }
        
        public async Task MoveMext(Update update)
        {
            if (_stateMachines.TryGetValue(update.Message.From.Id, out var machine))
            {
                await machine.MoveNext(update);
                switch (machine.State)
                {
                    case UpdateNameState.Canceled:
                        _stateMachines.TryRemove(update.Message.From.Id, out _);
                        break;
                    case UpdateNameState.ReadyToFinish:
                        await machine.Finish(update);
                        _stateMachines.TryRemove(update.Message.From.Id, out _);
                        break;
                }

                return;
            }
            
            machine = _serviceProvider.GetRequiredService<IStateMachine<UpdateNameState>>();
            _stateMachines.TryAdd(update.Message.From.Id, machine);

            await machine.MoveNext(update);

            if (machine.State == UpdateNameState.Canceled)
            {
                _stateMachines.TryRemove(update.Message.From.Id, out _);
            }
        }

        public bool IsActive(int userId)
        {
            return _stateMachines.ContainsKey(userId) &&
                   _stateMachines[userId].State != UpdateNameState.Updated &&
                   _stateMachines[userId].State != UpdateNameState.Canceled;
        }
    }
}