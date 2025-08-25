using System;
using System.Collections.Generic;
using StateMachineKit.Core.Interfaces;
using StateMachineKit.Core.Reference;
using Xunit;

namespace StateMachineKit.Tests;

public class FiniteStateMachineTests
{
    private class AttackState : IState<StateOwner>
    {
        public void OnEnter(StateOwner ctx, IState<StateOwner>? from = null)
        {
            throw new NotImplementedException();
        }

        public void OnExit(StateOwner ctx)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(StateOwner ctx, IStateMachine<StateOwner> stateMachine, float deltaTime = 0)
        {
            throw new NotImplementedException();
        }

        public void OnFixedUpdate(StateOwner ctx, IStateMachine<StateOwner> stateMachine, float deltaTime = 0)
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public void OwnerInitialize_SetsDataCorrectly()
    {
        var owner = new StateOwner("TestOwner");
        Assert.Equal("TestOwner", owner.Name);
        Assert.Equal(100, owner.Health);
    }
    
    [Fact]
    public void StateMachine_InitializesWithOwnerAndState()
    {
        var fsm = new FiniteStateMachine();
        fsm.Initialize<IdleState>();

        Assert.NotNull(fsm.Context);
        Assert.Equal(100, fsm.Context.Health);
        
        Assert.NotNull(fsm.CurrentState);
        Assert.IsType<IdleState>(fsm.CurrentState);
    }
    
    [Fact]
    public void StateMachine_ChangesStateCorrectly()
    {
        var fsm = new FiniteStateMachine();
        fsm.Initialize<IdleState>();

        Assert.IsType<IdleState>(fsm.CurrentState);
        
        fsm.ChangeState<WalkState>();
        Assert.IsType<WalkState>(fsm.CurrentState);
        
        fsm.ChangeState<RunState>();
        Assert.IsType<RunState>(fsm.CurrentState);
    }

    [Fact]
    public void StateMachine_TryChangeState_ReturnsFalseIfInvalid()
    {
        var fsm = new FiniteStateMachine();
        fsm.Initialize<IdleState>();

        Assert.IsType<IdleState>(fsm.CurrentState);
        
        var result = fsm.TryChangeState<AttackState>();
        Assert.False(result);
        Assert.IsType<IdleState>(fsm.CurrentState);
        
        result = fsm.TryChangeState<WalkState>();
        Assert.True(result);
        Assert.IsType<WalkState>(fsm.CurrentState);
    }

    [Fact]
    public void StateMachine_ChangeState_ThrowsOnInvalidState()
    {
        var fsm = new FiniteStateMachine();
        fsm.Initialize<IdleState>();
        
        Assert.IsType<IdleState>(fsm.CurrentState);
        Assert.Throws<KeyNotFoundException>(() => fsm.ChangeState<AttackState>());
    }
    
    [Fact]
    public void StateMachine_Tick_CallsUpdateOnCurrentState()
    {
        var fsm = new FiniteStateMachine();
        fsm.Initialize<IdleState>();

        Assert.IsType<IdleState>(fsm.CurrentState);
        
        fsm.Tick();
    }
}