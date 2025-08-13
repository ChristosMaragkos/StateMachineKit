using StateMachineKit.Core.Interfaces;

namespace StateMachineKit.Core.Tests;

public class CoreTests
{
    // Test double for an owner.
    private class TestStateOwner : IStateOwner
    {
        public string Name { get; set; }
        public int Health { get; set; }
        private bool _isDestroyed;
        public bool IsDestroyed => _isDestroyed;

        public TestStateOwner(string name)
        {
            Name = name;
            Initialize();
        }

        public void Initialize()
        {
            Health = 1;
            _isDestroyed = false;
        }

        public void Destroy()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;
            Console.WriteLine($"{Name} destroyed!");
        }
    }

    // State that applies damage then (on Update) may transition to DieTestState.
    private class TakeDamageTestState : IState<TestStateOwner>
    {
        public TakeDamageTestState(IStateMachine<TestStateOwner> stateMachine, TestStateOwner owner)
        {
            StateMachine = stateMachine;
            Owner = owner;
        }

        public void Enter(IState<TestStateOwner>? previousState = null)
        {
            Owner.Health -= 1;
            Console.WriteLine($"{Owner.Name} took damage! Health: {Owner.Health}");
        }

        public void Update()
        {
            if (Owner.Health <= 0)
            {
                StateMachine.ChangeState(new DieTestState(StateMachine, Owner));
            }
        }

        public void PhysicsUpdate(float deltaTime = 0)
        {
            // Not needed for these tests.
        }

        public IStateMachine<TestStateOwner> StateMachine { get; set; }
        public TestStateOwner Owner { get; set; }
    }

    // Terminal state.
    private class DieTestState : IState<TestStateOwner>
    {
        public DieTestState(IStateMachine<TestStateOwner> stateMachine, TestStateOwner owner)
        {
            StateMachine = stateMachine;
            Owner = owner;
        }

        public void Enter(IState<TestStateOwner>? previousState = null)
        {
            if (previousState is TakeDamageTestState)
            {
                Console.WriteLine($"{Owner.Name} is dying after taking damage!");
            }
            Owner.Destroy();
        }

        public void Update() { }

        public void PhysicsUpdate(float deltaTime = 0) { }

        public IStateMachine<TestStateOwner> StateMachine { get; set; }
        public TestStateOwner Owner { get; set; }
    }

    // Minimal test state machine implementing expected operations.
    private class TestStateMachine : IStateMachine<TestStateOwner>
    {
        public TestStateMachine(TestStateOwner owner)
        {
            Owner = owner;
        }

        public void Initialize(IState<TestStateOwner> initialState)
        {
            CurrentState = initialState ?? throw new ArgumentNullException(nameof(initialState));
            CurrentState.Enter();
        }

        public void ChangeState(IState<TestStateOwner> newState)
        {
            ArgumentNullException.ThrowIfNull(newState);
            var previous = CurrentState;
            CurrentState = newState;
            CurrentState.Enter(previous);
        }

        public void Update() => CurrentState.Update();

        public IStateOwner Owner { get; }
        public IState<TestStateOwner> CurrentState { get; set; } = null!;
    }

    [Fact]
    public void TakeDamageState_ReducesHealth_OnEnter()
    {
        var owner = new TestStateOwner("Owner1") { Health = 2 }; // Override initial health.
        var sm = new TestStateMachine(owner);
        var damage = new TakeDamageTestState(sm, owner);

        sm.Initialize(damage);

        Assert.Equal(1, owner.Health);
        Assert.IsType<TakeDamageTestState>(sm.CurrentState);
    }

    [Fact]
    public void Update_Transitions_To_DieState_When_Health_Depleted()
    {
        var owner = new TestStateOwner("Owner2"); // Starts at 1, Enter will reduce to 0.
        var sm = new TestStateMachine(owner);
        var damage = new TakeDamageTestState(sm, owner);

        sm.Initialize(damage);
        sm.Update(); // Triggers transition.

        Assert.IsType<DieTestState>(sm.CurrentState);
        Assert.True(owner.IsDestroyed);
    }

    [Fact]
    public void No_Transition_When_Health_Remains_Positive()
    {
        var owner = new TestStateOwner("Owner3") { Health = 3 };
        var sm = new TestStateMachine(owner);
        var damage = new TakeDamageTestState(sm, owner);

        sm.Initialize(damage); // Health becomes 2.
        sm.Update(); // Still > 0, no transition.

        Assert.IsType<TakeDamageTestState>(sm.CurrentState);
        Assert.False(owner.IsDestroyed);
    }

    [Fact]
    public void DieState_Calls_Destroy_On_Enter()
    {
        var owner = new TestStateOwner("Owner4");
        var sm = new TestStateMachine(owner);
        var damage = new TakeDamageTestState(sm, owner);

        sm.Initialize(damage);
        sm.Update(); // Transition to DieTestState.

        Assert.IsType<DieTestState>(sm.CurrentState);
        Assert.True(owner.IsDestroyed);
    }
}