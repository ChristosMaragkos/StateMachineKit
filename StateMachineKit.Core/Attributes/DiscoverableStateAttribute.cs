using System;

namespace StateMachineKit.Core.Attributes
{
    /// <summary>
    /// Attribute to mark a class as a discoverable state.
    /// You can mark state classes with this attribute and use reflection
    /// to find and register them automatically in your state machine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DiscoverableStateAttribute : Attribute {}
}