using System;
using Microsoft.Xna.Framework.Input;

namespace PathTracer.Core.Source.Control;

/* Base Class for Key Controls */
public abstract class KeyControl<T> {
	Keys _key;
	T _value;

	public KeyControl(Keys key, T value) {
		_key = key;
		_value = value;
	}
	
	public Keys Key => _key;
	public string KeyName => Key.ToString();
	public T Value { get => _value; set => _value = value; }
	public void Lambda (Action<T> action) {
		action(Value);
	}
	public abstract bool IsActive(KeyboardState prev, KeyboardState curr);
}

public class KeyToggle<T>(Keys key, T value) : KeyControl<T>(key, value) {
	public override bool IsActive(KeyboardState prev, KeyboardState curr) {
		/** If key was Toggled **/
		return (prev.IsKeyDown(Key) && curr.IsKeyUp(Key));
	}

}

public class KeyPress(Keys key, float value = 0.0f, float step = 0.01f) : KeyControl<float>(key, value) {

	readonly float? _min = null;
	readonly float? _max = null;

	public KeyPress(Keys key, float value, float step, float minValue, float maxValue) : this(key, value, step) {
		_min = minValue;
		_max = maxValue;
	}

	public bool IsClamped => _min.HasValue && _max.HasValue;

	public void Press() {
		if (IsClamped)
			Math.Clamp(Value += step, _min.Value, _max.Value);
		else
			Value += step;
	}

	public override bool IsActive(KeyboardState prev, KeyboardState curr) {
		/** If key was Pressed & Held **/
		return (prev.IsKeyDown(Key) && curr.IsKeyDown(Key));
	}
}

public class KeyPair((Keys Increase, Keys Decrease) keys, float value = 1.0f, float step = 0.01f) {
	
	readonly float? _min = null;
	readonly float? _max = null;
	readonly float _initial = value;

	public KeyPair((Keys Increase, Keys Decrease) keys, (float lower, float upper) bounds, float value = 1.0f, float step = .01f)
		: this(keys, value, step) {
		_min = bounds.lower;
		_max = bounds.upper;
	}

	public float Value => value;
	public bool IsClamped => _min.HasValue && _max.HasValue;
	public void Lambda(Action<float> action) {
		action(Value);
	}

	public bool FirstActive(KeyboardState prev, KeyboardState curr) {
		return (prev.IsKeyDown(keys.Increase) && curr.IsKeyDown(keys.Increase));
	}

	public bool SecondActive(KeyboardState prev, KeyboardState curr) {
		return (prev.IsKeyDown(keys.Decrease) && curr.IsKeyDown(keys.Decrease));
	}

	public bool ComboActive(KeyboardState prev, KeyboardState curr) {
		return (FirstActive(prev, curr) && SecondActive(prev, curr));
	}

	public void valueIncrease() {
		if (IsClamped)
			value = Math.Clamp(value + step, _min.Value, _max.Value);
		else
			value += step;
	}
	public void valueDecrease() {
		if (IsClamped)
			value = Math.Clamp(value - step, _min.Value, _max.Value);
		else
			value -= step;
	}

	public void Reset() {
		value = _initial;
	}
}

[Obsolete("ControlOption is deprecated, use KeyToggle or KeyPress instead.", false)]
public enum ControlOption { TOGGLE, PRESS };
[Obsolete("KeyAction is deprecated, use KeySelect or KeyToggle w/ Lambda() through a ControlHandler.", false)]
public class KeyAction<ActionType> : KeyControl<Action<ActionType>> {

	private ActionType _input;
	private ControlOption _control;

	public KeyAction(Keys key, Action<ActionType> value, ActionType input, ControlOption control = ControlOption.TOGGLE) : base(key, value) {
		_input = input;
		_control = control;
	}

	public void Act() => Value(_input);

	public override bool IsActive(KeyboardState prev, KeyboardState curr) {
		/** If key was Toggled **/
		if (_control == ControlOption.TOGGLE)
			return (prev.IsKeyDown(Key) && curr.IsKeyUp(Key));
		/** Else key was Pressed **/
		else
			return (prev.IsKeyDown(Key) && curr.IsKeyDown(Key));
	}
}
