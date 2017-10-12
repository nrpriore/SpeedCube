using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes

public class TimerController : MonoBehaviour {

	// Constant vars
	private const int GRADIENT_DIF = 160;				// How much color gradient can go down from max
	private const int GRADIENT_MAX = 200;				// Max value used for color gradient
	private Image _radial; 				// Reference to the radials image to lerp
	private Image _back;				// Reference to backs image for color
	private Text _text;					// Reference to text for timer
	private GameController _gc;			// Reference to the game controller

	// Dynamic vars

	// On instantiation
	void Awake() {
		InitVars();
	}
	
	// Runs every frame
	void Update() {
		_radial.fillAmount = _gc.TimerPercent();
		UpdateTimerColor();
		_text.text = _gc.Timer().ToString();
	}

/// -----------------------------------------------------------------------------------------------
/// Public methods --------------------------------------------------------------------------------



/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_gc = GameObject.Find("GameController").GetComponent<GameController>();
		_radial = gameObject.transform.Find("Radial").gameObject.GetComponent<Image>();
		_back 	= gameObject.transform.Find("Back").gameObject.GetComponent<Image>();
		_text 	= gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
	}

	// Sets the color of the timer based on remaining time percentage
	// Need red value to go from 40 --> 200 and green to go from 200 --> 40 as timer goes down
	// The math doesn't look intuitive, but it works LOL
	private void UpdateTimerColor() {
		float r = ((GRADIENT_MAX - (GRADIENT_DIF * Mathf.Max( 2f * _gc.TimerPercent() - 1f, 0))) / 255f);
		float g = ((GRADIENT_MAX - (GRADIENT_DIF * Mathf.Max(-2f * _gc.TimerPercent() + 1f, 0))) / 255f);

		_back.color = Functions.UpdateColor(_back.color, r, g);
	}

}

/*
using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes

public class TimerController : MonoBehaviour {

	// Constant vars
	private const int GRADIENT_DIF = 160;				// How much color gradient can go down from max
	private const int GRADIENT_MAX = 200;				// Max value used for color gradient
	private float _handleYMax;			// Height of Timer bar (max height of handle)
	private RectTransform _handle; 		// Reference to the handle's recttransform
	private Image _timerBar;			// Reference to timer's image for color
	private GameController _gc;			// Reference to the game controller

	// Dynamic vars

	// On instantiation
	void Awake() {
		InitVars();
	}
	
	// Runs every frame
	void Update() {
		_handle.anchoredPosition = new Vector2(0, _gc.TimerPercent() * _handleYMax);
		_timerBar.color = TimerColor();
	}

/// -----------------------------------------------------------------------------------------------
/// Public methods --------------------------------------------------------------------------------



/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_gc = GameObject.Find("GameController").GetComponent<GameController>();
		_handle = gameObject.transform.Find("Handle").gameObject.GetComponent<RectTransform>();
		_timerBar = gameObject.GetComponent<Image>();
		_handleYMax = gameObject.GetComponent<RectTransform>().sizeDelta.y;
	}

	// Returns the color of the timer based on remaining time percentage
	private Color TimerColor() {
		byte r = (byte)(GRADIENT_MAX - (GRADIENT_DIF * Mathf.Max( 2f * _gc.TimerPercent() - 1f, 0)));
		byte g = (byte)(GRADIENT_MAX - (GRADIENT_DIF * Mathf.Max(-2f * _gc.TimerPercent() + 1f, 0)));

		return new Color32(r, g, 40, 255);
	}

}
*/