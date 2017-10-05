using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes

public class MultiplierController : MonoBehaviour {

	// Constant vars
	private float _barYMax;				// Height of Timer bar (max height of handle)
	private RectTransform _bar; 		// Reference to the bar's recttransform
	private Text _multText;				// Reference to the text showing the multiplier
	private GameController _gc;			// Reference to the game controller
	private const float LERP_THRESHOLD = 1f;			// Threshold for lerp animation

	// Dynamic vars
	private float _lerpRatio;			// % of bar height from bot to top
	private int _botScore;				// Score at bottom of bar for current multiplier
	private int _topScore;				// Score at top of bar for current multiplier

	// On instantiation
	void Awake() {
		InitVars();
	}
	
	// Runs every frame
	void Update() {
		float lerpVal = _lerpRatio * _barYMax;
		if(Mathf.Abs(_bar.sizeDelta.y - lerpVal) > LERP_THRESHOLD) {
			Vector2 lerpVector = new Vector2(_bar.sizeDelta.x, lerpVal);
			_bar.sizeDelta = Vector2.Lerp(_bar.sizeDelta, lerpVector, Time.deltaTime * 10f);
		}else
		if(_lerpRatio >= 1f) {
			NextMultiplier();
		}
	}

/// -----------------------------------------------------------------------------------------------
/// Public methods --------------------------------------------------------------------------------

	// Sets the UI (First set is run from GameController, hence public)
	public void SetMultiplier() {
		NextMultiplier();
	}

	// Updates the bar height based on new score, to a max of 1
	public void UpdateBar(int score) {
		_lerpRatio = (score >= _topScore)? 1f : (float)(score - _botScore) / (float)(_topScore - _botScore);
	}

/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_gc = GameObject.Find("GameController").GetComponent<GameController>();
		_bar = gameObject.transform.Find("MultBar").gameObject.GetComponent<RectTransform>();
		_multText = gameObject.transform.Find("Multiplier").gameObject.GetComponent<Text>();
		_barYMax = 2000f;
	}

	// Sets the variables for the next multiplier
	private void NextMultiplier() {
		int _mult = _gc.Multiplier();
		_multText.text = _mult + "x";

		Vector3 size = _gc.GetMultiplierConfig();
		_botScore = (int)size.x;
		_topScore = (int)size.y;

		UpdateBar((int)size.z);
	}

}
