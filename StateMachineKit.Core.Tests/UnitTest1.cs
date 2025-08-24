using StateMachineKit.Core.Interfaces;

namespace StateMachineKit.Core.Tests;

public class CoreTests
{
    // Test double for an owner.
    private class TestStateOwner : IStateOwner
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public bool IsDestroyed { get; private set; }

        public TestStateOwner(string name)
        {
            Name = name;
            Initialize();
        }

        public void Initialize()
        {
            Health = 1;
            IsDestroyed = false;
        }

        public void Destroy()
        {
            if (IsDestroyed) return;
            IsDestroyed = true;
            Console.WriteLine($"{Name} destroyed!");
        }
    }

    // State that applies damage then (on Update) may transition to DieTestState.
    private class TakeDamageTestState : IState<TestStateOwner>
    {
        public void Enter(IState<TestStateOwner>? previousState = null)
        {
            Owner.Health -= 1;
            Console.WriteLine($"{Owner.Name} took damage! Health: {Owner.Health}");
        }

        public void Update()
        {
            if (Owner.Health <= 0)
            {
                StateMachine.ChangeState(typeof(TakeDamageTestState));
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
    
    // Minimal representation of a state machine for testing.
    private class TestStateMachine : IStateMachine<TestStateOwner>
    {
        public Dictionary<Type, IState<TestStateOwner>> States { get; } = new();
        public TestStateOwner Owner { get; set; }
        public TestStateOwner FindOwner()
        {
            return new TestStateOwner("TestOwner" + Guid.NewGuid());
        }

        public IState<TestStateOwner> CurrentState { get; set; }
        public List<IState<TestStateOwner>> GetAllStates()
        {
            return [
                new TakeDamageTestState(),
                new DieTestState()
            ];
        }
    }
    
    [Fact]
    public void TestStateMachineInitialization()
    {
        var stateMachine = new TestStateMachine();
        stateMachine.Initialize(new TakeDamageTestState());
        
        Assert.NotNull(stateMachine.CurrentState);
        Assert.Contains(typeof(TakeDamageTestState), stateMachine.States.Keys);
    }
}