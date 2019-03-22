namespace SeaBattle.Server.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using StateMachine;
    using StateMachine.UpdateStrategy;
    using Telegram.Bot.Types;

    public class UpdateStrategyService : IServiceWithState<UpdateStrategyState>
    {
        private readonly IServiceProvider _serviceProvider;
        
        private readonly ConcurrentDictionary<int, IStateMachine<UpdateStrategyState>> _stateMachines;
        
        public UpdateStrategyService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _stateMachines = new ConcurrentDictionary<int, IStateMachine<UpdateStrategyState>>();
        }
        
        public async Task MoveMext(Update update)
        {
            if (_stateMachines.TryGetValue(update.Message.From.Id, out var machine))
            {
                await machine.MoveNext(update);
                switch (machine.State)
                {
                    case UpdateStrategyState.Canceled:
                        _stateMachines.TryRemove(update.Message.From.Id, out _);
                        break;
                    case UpdateStrategyState.ReadyToFinish:
                        await machine.Finish(update);
                        _stateMachines.TryRemove(update.Message.From.Id, out _);
                        break;
                }

                return;
            }
            
            machine = _serviceProvider.GetRequiredService<IStateMachine<UpdateStrategyState>>();
            _stateMachines.TryAdd(update.Message.From.Id, machine);

            await machine.MoveNext(update);

            if (machine.State == UpdateStrategyState.Canceled)
            {
                _stateMachines.TryRemove(update.Message.From.Id, out _);
            }
        }

        public bool IsActive(int userId)
        {
            return _stateMachines.ContainsKey(userId) &&
                   _stateMachines[userId].State != UpdateStrategyState.Updated &&
                   _stateMachines[userId].State != UpdateStrategyState.Canceled;
        }
    }
}