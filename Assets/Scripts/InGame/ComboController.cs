using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes

public class ComboController : MonoBehaviour {

	// Constant vars
	private RectTransform _bar;			// Reference to the bar game object
	private GameController _gc;			// Reference to the game controller
	// UI
	private const float LERP_THRESHOLD = 1f;			// Threshold for lerp animation
	private Text _multText;				// Reference to UI text for multiplier

	// Dynamic vars
	private int _currMultiplier;		// Current combo multiplier
	private int _currIndex;				// Current index out of 3 on bar
	private float _indexWidth;			// Reference to width of index
	private float _lerpTo;				// Position to lerp the bar to

	// On instantiation
	void Start () {
		InitVars();
	}
	
	// Runs every frame
	void Update () {
		if(Mathf.Abs(_bar.sizeDelta.x - _lerpTo) > LERP_THRESHOLD) {
			float width = Mathf.Lerp(_bar.sizeDelta.x, _lerpTo, Time.deltaTime * 10f);
			_bar.sizeDelta = new Vector2(width, _bar.sizeDelta.y);
		}else 
		if(_currIndex == 3) {
			_gc.ComboBonus(_currMultiplier);
			UpdateMultiplier();
		}
	}

/// -----------------------------------------------------------------------------------------------
/// Public methods --------------------------------------------------------------------------------

	// Move bar to next index
	public void AddIndex() {
		_currIndex++;
		_lerpTo = _indexWidth * _currIndex;
	}

	// Reset bar
	public void ResetIndex() {
		UpdateMultiplier(0);
	}

/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_gc = GameObject.Find("GameController").GetComponent<GameController>();
		_bar = gameObject.transform.Find("Bar").gameObject.GetComponent<RectTransform>();
		_multText = gameObject.transform.Find("Num").Find("Combo").gameObject.GetComponent<Text>();
		_indexWidth = 400;
		_currIndex = 0;
		_lerpTo = _indexWidth * _currIndex;
		_currMultiplier = 1;
	}

	// Updates multiplier and UI
	private void UpdateMultiplier(int mult = -1) {
		_currMultiplier = (mult == 0)? 1 : Mathf.Min(_currMultiplier + 1, GameController.MAX_COMBO_MULT);
		_multText.text = _currMultiplier.ToString();

		_currIndex = 0;
		_lerpTo = _indexWidth * _currIndex;
	}

}
