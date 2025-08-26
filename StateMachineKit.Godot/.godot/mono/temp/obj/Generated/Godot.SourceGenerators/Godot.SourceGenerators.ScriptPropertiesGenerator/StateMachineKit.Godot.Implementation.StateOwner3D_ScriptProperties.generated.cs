using Godot;
using Godot.NativeInterop;

namespace StateMachineKit.Godot.Implementation {

partial class StateOwner3D
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::Godot.CharacterBody3D.PropertyName {
        /// <summary>
        /// Cached name for the 'StateOwnerName' property.
        /// </summary>
        public new static readonly global::Godot.StringName @StateOwnerName = "StateOwnerName";
        /// <summary>
        /// Cached name for the 'MakeNameUnique' field.
        /// </summary>
        public new static readonly global::Godot.StringName @MakeNameUnique = "MakeNameUnique";
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
    {
        if (name == PropertyName.@MakeNameUnique) {
            this.@MakeNameUnique = global::Godot.NativeInterop.VariantUtils.ConvertTo<bool>(value);
            return true;
        }
        return base.SetGodotClassPropertyValue(name, value);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
    {
        if (name == PropertyName.@StateOwnerName) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<string>(this.@StateOwnerName);
            return true;
        }
        if (name == PropertyName.@MakeNameUnique) {
            value = global::Godot.NativeInterop.VariantUtils.CreateFrom<bool>(this.@MakeNameUnique);
            return true;
        }
        return base.GetGodotClassPropertyValue(name, out value);
    }
    /// <summary>
    /// Get the property information for all the properties declared in this class.
    /// This method is used by Godot to register the available properties in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::Godot.Bridge.PropertyInfo> GetGodotPropertyList()
    {
        var properties = new global::System.Collections.Generic.List<global::Godot.Bridge.PropertyInfo>();
        properties.Add(new(type: (global::Godot.Variant.Type)4, name: PropertyName.@StateOwnerName, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::Godot.Variant.Type)1, name: PropertyName.@MakeNameUnique, hint: (global::Godot.PropertyHint)0, hintString: "", usage: (global::Godot.PropertyUsageFlags)4102, exported: true));
        return properties;
    }
#pragma warning restore CS0109
}

}
