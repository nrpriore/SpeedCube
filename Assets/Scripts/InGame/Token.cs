using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes


public class Token : MonoBehaviour {

	// Constant vars
	private Image _image;				// Reference to to image component
	private GameController _gc;			// Reference to the game controller
	private int _index;					// Reference to position in tokens array
	// UI
	private const float MAX_TIME = 0.15f;				// Total time for UI animations
	private float _baseRed;				// Amt of red in normal token hex
	private float _redDiff;				// Amt to lerp red value on incorrect click

	// Dynamic vars
	private bool _selected;				// Is the token currently selected
	private float _timer;				// Timer to control UI animations for token


	// On instantiation
	void Start() {
		InitVars();
	}

	// Runs every frame
	void Update() {
		if(_timer > 0f) {
			_timer -= Time.deltaTime;

			byte r = (byte)(_baseRed + (Mathf.Min(_timer, MAX_TIME - _timer) / (MAX_TIME / 2f) * _redDiff));
			byte g = 80;
			byte b = 80;
			byte a = 250;
			_image.color = new Color32(r, g, b, a);
		}else 
		if(_image.color.a < 1) {
			_image.color = Functions.HexToColor(GameController.BASE_HEX);
		}
	}

/// -----------------------------------------------------------------------------------------------
/// Public methods --------------------------------------------------------------------------------

	// Adds token to input
	public void AddInput() {
		if(_gc.IsInPattern(_index)) {
			if(!_selected) {
				VibrationManager.Vibrate(5);
				UpdateInput();
				ClickUI();
			}
		}
		else {
			VibrationManager.Vibrate(200);
			IncorrectClick();
		}
	}

	// Adds token to pattern
	public void AddPattern() {
		gameObject.GetComponent<Image>().color = Functions.HexToColor(GameController.CLICK_HEX);
	}

/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_gc = GameObject.Find("GameController").GetComponent<GameController>();
		_image = gameObject.GetComponent<Image>();
		_baseRed = 255f * _image.color.r;
		_redDiff = 255f - _baseRed;
		_index = int.Parse(name);
		_selected = false;
		_timer = 0;
	}

	// Updates token UI on click
	private void ClickUI() {
		_selected = true;
		_image.color = Functions.HexToColor(GameController.CLICK_HEX);
	}

	// Updates the game controller input
	private void UpdateInput() {
		_gc.AddInput(_index);
	}

	// Updates when token not in pattern is clicked
	private void IncorrectClick() {
		_gc.IncorrectClick();
		_timer = MAX_TIME;
	}
	
}
