using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes

public class ScoreTextController : MonoBehaviour {

	// Constant vars
	private Text _score; 				// Reference to score text
	private Text _addedScore;			// Reference to text of added score popup
	private const float LERP_TIME = 1f; // Time of lerp animation

	// Dynamic vars
	private float _timer;				// Current time of lerp animation
	private int _lerpToScore;			// Score to lerp to
	private int _baseScore;				// Original score before lerp
	private int _currScore;				// Current value of score text

	// On instantiation
	void Awake() {
		InitVars();
	}
	
	// Runs every frame
	void Update() {
		if(_currScore < _lerpToScore) {
			_timer -= Time.deltaTime;

			// Update score value
			float lerpRatio = Mathf.Min((LERP_TIME - _timer) / LERP_TIME, 1f);
			_currScore = Mathf.Min(_baseScore + (int)(lerpRatio * (_lerpToScore - _baseScore)), _lerpToScore);
			_score.text = _currScore.ToString();

			// Update added score alpha
			float alphaRatio = Mathf.Min(1f, 2f - (2f * (_currScore - _baseScore) / (_lerpToScore - _baseScore)));
			_addedScore.color = Functions.UpdateColor(_addedScore.color, a: alphaRatio);
		}
	}

/// -----------------------------------------------------------------------------------------------
/// Public methods --------------------------------------------------------------------------------

	// Starts the updatescore process
	public void UpdateScore(int score, int scoreAdded) {
		_addedScore.text = "+" + scoreAdded;
		_baseScore = _currScore;
		_lerpToScore = score;
		_addedScore.color = Functions.UpdateColor(_addedScore.color, a: 1f);

		_timer = LERP_TIME;
	}

/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_score = gameObject.GetComponent<Text>();
		_addedScore = gameObject.transform.Find("AddedScore").gameObject.GetComponent<Text>();
		_timer = 0f;
		_currScore = _baseScore = _lerpToScore = 0;
		_addedScore.color = Functions.UpdateColor(_addedScore.color, a: 0f);
	}

}
