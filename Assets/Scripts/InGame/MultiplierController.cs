using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes

public class MultiplierController : MonoBehaviour {

	// Constant vars
	private float _barYMax;				// Height of Timer bar (max height of handle)
	private RectTransform _lerpBar; 	// Reference to the lerp bar's recttransform
	private RectTransform _patternBar;	// Reference to the score bar's recttransform
	private RectTransform _comboBar;	// Reference to the combo bar's recttransform
	private RectTransform _comboText;	// Reference to the combo bar's text recttransform
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
		if(Mathf.Abs(_lerpBar.sizeDelta.y - lerpVal) > LERP_THRESHOLD) {
			Vector2 lerpVector = new Vector2(_lerpBar.sizeDelta.x, lerpVal);
			_lerpBar.sizeDelta = Vector2.Lerp(_lerpBar.sizeDelta, lerpVector, Time.deltaTime * 5f);
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

	// updates all 3 bars of the multiplier
	public void UpdateMultiplier(int score, int pattern, int combo) {
		UpdateSpecificBars(pattern, combo);
		UpdateLerpBar(score);
	}

/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_gc = GameObject.Find("GameController").GetComponent<GameController>();
		_lerpBar = gameObject.transform.Find("LerpBar").gameObject.GetComponent<RectTransform>();
		_patternBar = gameObject.transform.Find("PatternBar").gameObject.GetComponent<RectTransform>();
		_comboBar = gameObject.transform.Find("ComboBar").gameObject.GetComponent<RectTransform>();
		_comboText = _comboBar.Find("Text").gameObject.GetComponent<RectTransform>();
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

		UpdateSpecificBars(_botScore, _botScore);
		UpdateLerpBar((int)size.z);
	}

	// Updates the lerp bar height based on new score, to a max of 1
	private void UpdateLerpBar(int score) {
		_lerpRatio = (score >= _topScore)? 1f : (float)(score - _botScore) / (float)(_topScore - _botScore);
	}

	// Updates the score-specific bars instantly
	private void UpdateSpecificBars(int patternbar, int combobar) {
		float patternRatio = (patternbar >= _topScore)? 1f : (float)(patternbar - _botScore) / (float)(_topScore - _botScore);
		float comboRatio = (combobar >= _topScore)? 1f : (float)(combobar - _botScore) / (float)(_topScore - _botScore);

		float patternY = patternRatio * _barYMax;
		float comboY = comboRatio * _barYMax;

		_patternBar.sizeDelta 	= new Vector2(_patternBar.sizeDelta.x, patternY);
		_comboBar.sizeDelta 	= new Vector2(_comboBar.sizeDelta.x, comboY);
		_comboText.sizeDelta 	= new Vector2(comboY - 20, 100);
	}

}
