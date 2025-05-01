using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace PathTracer.Core.Source.Control;
public record struct MouseInput(MouseState prev, MouseState curr);
public record struct KeyboardInput(KeyboardState prev, KeyboardState curr);
public class ControlHandler() {

	private MouseInput _mouse = new();
	private KeyboardInput _keyboard = new();

	//public MouseState MouseCurrent => _mouse.curr;
	//public MouseState MousePrevious => _mouse.prev;
	//public KeyboardState KeyboardCurrent => _keyboard.curr;
	//public KeyboardState KeyboardPrevious => _keyboard.prev;

	public void MouseCurrent(MouseState state) => _mouse.curr = state;
	public void MousePrevious(MouseState state) => _mouse.prev = state;
	public void KeyboardCurrent(KeyboardState state) => _keyboard.curr = state;
	public void KeyboardPrevious(KeyboardState state) => _keyboard.prev = state;

	[Obsolete("PollKeyAction is deprecated, use Poll function alternatives", false)]
	public void PollKeyAction<E>(KeyAction<E> key) {
		if (key.IsActive(_keyboard.prev, _keyboard.curr)) {
			key.Act();
		}
	}

	public void PollToggle(KeyToggle<bool> key) {
		if (key.IsActive(_keyboard.prev, _keyboard.curr)) {
			key.Value = !key.Value;
		}
	}

	public void PollToggle(KeyToggle<bool> key, Action<bool> onToggle) {
		if (key.IsActive(_keyboard.prev, _keyboard.curr)) {
			key.Value = !key.Value;
			key.Lambda(onToggle);
		}
	}

	public void PollSelect<T>(KeyToggle<T> key, Action<T> onSelect) {
		if (key.IsActive(_keyboard.prev, _keyboard.curr)) {
			key.Lambda(onSelect);
		}
	}

	public void PollAction<T>(KeyToggle<Action<T>> key, T input) {
		if (key.IsActive(_keyboard.prev, _keyboard.curr)) {
			key.Value(input);
		}
	}

	public void PollKeyPair(KeyPair keys) {
		if (keys.FirstActive(_keyboard.prev, _keyboard.curr))
			keys.valueIncrease();

		else if (keys.SecondActive(_keyboard.prev, _keyboard.curr))
			keys.valueDecrease();
	}

	public void PollKeyPair(KeyPair keys, Action<float> onActive) {
		if (keys.FirstActive(_keyboard.prev, _keyboard.curr)) {
			keys.valueIncrease();
			keys.Lambda(onActive);
		}

		else if (keys.SecondActive(_keyboard.prev, _keyboard.curr)) {
			keys.valueDecrease();
			keys.Lambda(onActive);
		}
	}

	public void PollKeyCombo(KeyPair keys) {
		if (keys.ComboActive(_keyboard.prev, _keyboard.curr))
			keys.valueDecrease();

		else if (keys.FirstActive(_keyboard.prev, _keyboard.curr))
			keys.valueIncrease();
	}

	public void PollKeyCombo(KeyPair keys, Action<float> onActive) {
		if (keys.ComboActive(_keyboard.prev, _keyboard.curr)) {
			keys.valueDecrease();
			keys.Lambda(onActive);
		}

		else if (keys.FirstActive(_keyboard.prev, _keyboard.curr)) {
			keys.valueIncrease();
			keys.Lambda(onActive);
		}
	}

	public void PollMouseDrag(MouseDrag mouseButton) {

		if (mouseButton.IsActive(_mouse.curr) && mouseButton.IsActive(_mouse.prev)) {
			mouseButton.Drag(_mouse.curr.X - _mouse.prev.X, _mouse.curr.Y - _mouse.prev.Y);
		}

	}

	public void PollMouseDrag(MouseDrag mouseButton, Action<Vector2> onDrag) {
		if (mouseButton.IsActive(_mouse.curr) && mouseButton.IsActive(_mouse.prev)) {
			mouseButton.Drag(_mouse.curr.X - _mouse.prev.X, _mouse.curr.Y - _mouse.prev.Y);
			mouseButton.Lambda(onDrag);
		}
	}

	public void PollMouseScroll(MouseScroll mouseButton) {

		if (_mouse.curr.ScrollWheelValue != _mouse.prev.ScrollWheelValue) {
			mouseButton.Scroll(_mouse.curr.ScrollWheelValue - _mouse.prev.ScrollWheelValue);
		}

	}

	public void PollMouseScroll(MouseScroll mouseButton, Action<float> onScroll) {

		if (_mouse.curr.ScrollWheelValue != _mouse.prev.ScrollWheelValue) {
			mouseButton.Scroll(_mouse.curr.ScrollWheelValue - _mouse.prev.ScrollWheelValue);
			mouseButton.Lambda(onScroll);
		}

	}
}
