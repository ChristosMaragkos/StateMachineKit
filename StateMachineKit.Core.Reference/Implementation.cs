using System;
using System.Collections.Generic;
using System.Linq;
using StateMachineKit.Core.Attributes;
using StateMachineKit.Core.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace StateMachineKit.Core.Reference
{
    public class StateOwner : IStateOwner
    {
        private const int DefaultHealth = 100;

        public StateOwner(string name)
        {
            Name = name;
            Initialize();
        }

        public string Name { get; }
        public int Health { get; set; }

        public void Initialize()
        {
            Health = DefaultHealth;
        }

        public void Destroy()
        {
        }
    }
    
    public class FiniteStateMachine : IStateMachine<StateOwner>
    {
        public StateOwner Context { get; private set; }
        public IState<StateOwner>? CurrentState { get; private set; }

        private readonly Dictionary<Type, IState<StateOwner>> _states = new
            Dictionary<Type, IState<StateOwner>>();

        private FiniteStateMachine(StateOwner context, IState<StateOwner> currentState)
        {
            Context = context;
            CurrentState = currentState;
        }
        
        public FiniteStateMachine()
        {
            Context = FindOwner();
        }

        public void AttachOwner(StateOwner owner)
        {
            Context = owner;
            Context.Initialize();
        }

        private StateOwner FindOwner()
        {
            return new StateOwner(Guid.NewGuid().ToString());
        }

        public void Initialize<TState>() where TState : class, IState<StateOwner>
        {
            FindAllStates();
            AttachOwner(FindOwner());

            TryChangeState<TState>();
        }

        /// <summary>
        /// Scans all loaded assemblies for types implementing <see cref="IState{StateOwner}"/>
        /// and registers those marked with DiscoverableStateAttribute.
        /// </summary>
        private void FindAllStates()
        {
            _states.Clear();
            var stateType = typeof(IState<StateOwner>);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes()
                         .Where(t => stateType.IsAssignableFrom(t) 
                                     && t is { IsInterface: false, IsAbstract: false })
                         .Where(t => Attribute.IsDefined(t, typeof(DiscoverableStateAttribute))))
                if (Activator.CreateInstance(type) is IState<StateOwner> instance)
                    _states[type] = instance;
        }

        public void ChangeState<TState>() where TState : class, IState<StateOwner>
        {
            CurrentState?.OnExit(Context);
            var prev = CurrentState;
            CurrentState = _states[typeof(TState)] ?? throw new KeyNotFoundException(
                $"[{Context.Name.ToUpper()}] State of type {typeof(TState).Name} does not exist.");
            CurrentState.OnEnter(Context, prev);
        }

        public bool TryChangeState<TState>() where TState : class, IState<StateOwner>
        {
            if (!_states.ContainsKey(typeof(TState)))
            {
                Console.WriteLine($"State of type {typeof(TState).Name} does not exist.");
                return false;
            }

            ChangeState<TState>();
            return true;
        }
    }

    [DiscoverableState]
    public sealed class WalkState : IState<StateOwner>
    {
        public void OnEnter(StateOwner ctx, IState<StateOwner>? from = null)
        {
            Console.WriteLine($"{ctx.Name} is now walking.");
        }

        public void OnExit(StateOwner ctx)
        {
            Console.WriteLine($"{ctx.Name} stopped walking.");
        }

        public void OnUpdate(StateOwner ctx, IStateMachine<StateOwner> stateMachine,
            float deltaTime = 0)
        {
            Console.WriteLine("According to all known laws of aviation, " +
                              "there is no way a bee should be able to fly. " +
                              "Its wings are too small to get its fat little body off the ground. " +
                              "The bee, of course, flies anyway because bees don't care what " +
                              "humans think is impossible.");
        }

        public void OnFixedUpdate(StateOwner ctx, IStateMachine<StateOwner> stateMachine,
            float deltaTime = 0)
        {
            Console.WriteLine($"{ctx.Name} is now walking.");
        }
    }

    [DiscoverableState]
    public sealed class RunState : IState<StateOwner>
    {
        public void OnEnter(StateOwner ctx, IState<StateOwner>? from = null)
        {
            Console.WriteLine($"{ctx.Name} is now running.");
        }

        public void OnExit(StateOwner ctx)
        {
            Console.WriteLine($"{ctx.Name} stopped running.");
        }

        public void OnUpdate(StateOwner ctx, IStateMachine<StateOwner> stateMachine,
            float deltaTime = 0)
        {
            Console.WriteLine("According to all known laws of aviation, " +
                              "there is no way a bee should be able to fly. " +
                              "Its wings are too small to get its fat little body off the ground. " +
                              "The bee, of course, flies anyway because bees don't care what " +
                              "humans think is impossible.");
        }

        public void OnFixedUpdate(StateOwner ctx, IStateMachine<StateOwner> stateMachine,
            float deltaTime = 0)
        {
            Console.WriteLine($"{ctx.Name} is now running.");
        }
    }

    [DiscoverableState]
    public sealed class IdleState : IState<StateOwner>
    {
        public void OnEnter(StateOwner ctx, IState<StateOwner>? from = null)
        {
            Console.WriteLine($"{ctx.Name} is now idle.");
        }

        public void OnExit(StateOwner ctx)
        {
            Console.WriteLine($"{ctx.Name} stopped idling.");
        }

        public void OnUpdate(StateOwner ctx, IStateMachine<StateOwner> stateMachine,
            float deltaTime = 0)
        {
            Console.WriteLine("According to all known laws of aviation, " +
                              "there is no way a bee should be able to fly. " +
                              "Its wings are too small to get its fat little body off the ground. " +
                              "The bee, of course, flies anyway because bees don't care what " +
                              "humans think is impossible.");
        }

        public void OnFixedUpdate(StateOwner ctx, IStateMachine<StateOwner> stateMachine,
            float deltaTime = 0)
        {
            Console.WriteLine($"{ctx.Name} is now idle.");
        }
    }
}