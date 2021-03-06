using FluentState.Machine;
using System;
using System.Collections.Generic;

namespace FluentState.Builder
{
    public class StateBuilder<TStateMachine, TState, TStimulus> : IStateBuilder<TStateMachine, TState, TStimulus>
        where TStateMachine : IStateMachine<TState, TStimulus>
        where TState : struct
        where TStimulus : struct
    {
        private readonly IStateMachineBuilder<TStateMachine, TState, TStimulus> _machineBuilder;
        private readonly TState _state;

        public StateBuilder(IStateMachineBuilder<TStateMachine, TState, TStimulus> machineBuilder, TState state)
        {
            _machineBuilder = machineBuilder;
            _state = state;

        }

        public IStateBuilder<TStateMachine, TState, TStimulus> WithTransitionTo(
            TState enteringState,
            TStimulus reason,
            IEnumerable<Func<TState, TState, TStimulus, bool>>? guards = null,
            IEnumerable<Action<TState, TState, TStimulus>>? enterActions = null,
            IEnumerable<Action<TState, TState, TStimulus>>? leaveActions = null
        )
        {
            var machine = _machineBuilder.Machine;

            machine.AddTransition(enteringState, _state, reason);
            if (enterActions != null)
            {
                foreach(var action in enterActions)
                {
                    machine.AddStateEnterAction(enteringState, _state, reason, action);
                }
            }

            if (leaveActions != null)
            {
                foreach (var action in leaveActions)
                {
                    machine.AddStateLeaveAction(enteringState, _state, reason, action);
                }
            }

            if (guards != null)
            {
                foreach (var guard in guards)
                {
                    machine.AddTransitionGuard(enteringState, _state, reason, guard);
                }
            }

            return this;
        }

        public IStateBuilder<TStateMachine, TState, TStimulus> WithEnterAction(Action<TState, TState, TStimulus> action)
        {
            _machineBuilder.Machine.AddStateEnterAction(_state, action);
            return this;
        }

        public IStateBuilder<TStateMachine, TState, TStimulus> WithLeaveAction(Action<TState, TState, TStimulus> action)
        {
            _machineBuilder.Machine.AddStateLeaveAction(_state, action);
            return this;
        }

        public IStateBuilder<TStateMachine, TState, TStimulus> WithEnterAction(TState leavingState, TStimulus reason, Action<TState, TState, TStimulus> action)
        {
            _machineBuilder.Machine.AddStateEnterAction(_state, leavingState, reason, action);
            return this;
        }

        public IStateBuilder<TStateMachine, TState, TStimulus> WithLeaveAction(TState enteringState, TStimulus reason, Action<TState, TState, TStimulus> action)
        {
            _machineBuilder.Machine.AddStateLeaveAction(enteringState, _state, reason, action);
            return this;
        }

        public IStateMachineBuilder<TStateMachine, TState, TStimulus> Build()
        {
            return _machineBuilder;
        }
    }
}
