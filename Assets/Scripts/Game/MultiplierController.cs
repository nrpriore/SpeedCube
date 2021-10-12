using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes

public class MultiplierController : MonoBehaviour {

	// Constant vars
	private float _barYMax;				// Height of Timer bar (max height of handle)
	private Image _lerpBar; 			// Reference to the lerp bar's Image
	private Image _patternBar;			// Reference to the score bar's Image
	private Image _comboBar;			// Reference to the combo bar's Image
	private RectTransform _comboText;	// Reference to the combo bar's text recttransform
	private Text _multText;				// Reference to the text showing the multiplier
	private GameController _gc;			// Reference to the game controller
	private const float LERP_THRESHOLD = .0001f;			// Threshold for lerp animation

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
		if(Mathf.Abs(_lerpBar.fillAmount - _lerpRatio) > LERP_THRESHOLD) {
			_lerpBar.fillAmount = Mathf.Lerp(_lerpBar.fillAmount, _lerpRatio, Time.deltaTime * 5f);
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
		_lerpBar = gameObject.transform.Find("LerpBar").gameObject.GetComponent<Image>();
		_patternBar = gameObject.transform.Find("PatternBar").gameObject.GetComponent<Image>();
		_comboBar = gameObject.transform.Find("ComboBar").gameObject.GetComponent<Image>();
		_comboText = _comboBar.gameObject.transform.Find("Text").gameObject.GetComponent<RectTransform>();
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

		float comboY = comboRatio * _barYMax;

		_patternBar.fillAmount 	= patternRatio;
		_comboBar.fillAmount 	= comboRatio;
		_comboText.sizeDelta 	= new Vector2((comboY - 50 > 50)? (comboY - 50) : -50, 180);
		_comboText.localPosition = new Vector3(-6, comboY - 30,0);
	}

}
