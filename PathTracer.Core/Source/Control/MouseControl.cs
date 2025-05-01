using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PathTracer.Core.Source.Control;

public enum MouseButtonType { LEFT, RIGHT, WHEEL };
public abstract class MouseControl<T> {

	MouseButtonType _buttonType;
	T _sens;
	T _value;

	public MouseControl(T sensitivity) {
		_sens = sensitivity;
	}

	public MouseControl(T value, T sensitivity): this (sensitivity) {
		_value = value;
	}
	
	public MouseControl(T value, T sensitivity, MouseButtonType buttonType) : this(value, sensitivity) {
		_buttonType = buttonType;
	}

	public T Value { get { return _value; } set { _value = value; } }
	public T Sensitivity { get { return _sens; } set { _sens = value; } }
	public MouseButtonType ButtonType { get { return _buttonType; } }
	public string ButtonTypeName => nameof(_buttonType);
	public abstract bool IsActive(MouseState state);
	public void Lambda(Action<T> action) {
		action(Value);
	}
}

public class MouseDrag: MouseControl<Vector2> {
	
	// private Vector2 _value = new(0.0f, 0.0f);
	public MouseDrag(Vector2 sensitivity, MouseButtonType buttonType): base(sensitivity) {
		Value = new(0f, 0f);
	}

	public MouseDrag(Vector2 sensitivity, MouseButtonType buttonType, Vector2 value) : base(value, sensitivity, buttonType) { }

	public float X => Value.X;
	public float Y => Value.Y;
	public override bool IsActive(MouseState state) {
		switch (ButtonType) {
			case MouseButtonType.LEFT:
				return (state.LeftButton == ButtonState.Pressed);
			case MouseButtonType.WHEEL:
				return (state.MiddleButton == ButtonState.Pressed);
			case MouseButtonType.RIGHT:
				return (state.RightButton == ButtonState.Pressed);
			default: return false; // FIXME
		}
	}

	public void Drag(float deltaX, float deltaY) {
		float _x = Value.X + (deltaX * Sensitivity.X);
		float _y = Value.Y + (deltaY * Sensitivity.Y);
		Value = new Vector2(_x, _y);
	}
}

public class MouseScroll(float sensitivity, float value = 0.0f) : MouseControl<float>(sensitivity, value) {

	readonly MouseButtonType buttonType = MouseButtonType.WHEEL;
	readonly float? _min;
	readonly float? _max;

	public MouseScroll(float sensitivity, float value, float minValue, float maxValue) : this(sensitivity, value) {
		Sensitivity = sensitivity;
		Value = value;
		_min = minValue;
		_max = maxValue;
	}

	public override bool IsActive(MouseState state) {
		throw new NotImplementedException();
	}

	public bool IsClamped => _min.HasValue && _max.HasValue;

	public void Scroll(float delta) {
		if (_max.HasValue && _min.HasValue)
			Value = Math.Clamp(Value + (delta * Sensitivity), _min.Value, _max.Value);
		else
			Value += (delta * Sensitivity);
	}
}
