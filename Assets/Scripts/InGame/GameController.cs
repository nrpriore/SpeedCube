using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes
using System.Collections.Generic;		// To access Lists
using UnityEngine.SceneManagement;		// To change scenes

public class GameController : MonoBehaviour {

	// Constant vars
	public const string BASE_HEX 	= "C4EAF2FF";		// Base hex color of input tokens
	public const string CLICK_HEX 	= "FDFF4BFF";		// Hex color of tokens when selected
	public const string DISABLE_HEX = "C8C8C880";		// Hex color of tokens when disabled
	private const int BASE_COMBO_BONUS	= 100;			// Combo bonus points at x1 mult
	public const int MAX_COMBO_MULT 	= 10;			// Max combo multipler
	public static Transform GameCanvas;					// Reference to transform of the Canvas
	private InputController _ic;		// Reference to the input controller
	private ComboController _cc;		// Reference to the combo controller
	private MultiplierController _mc;	// Reference to the multiplier controller
	private ScoreTextController _sc;	// Reference to the score text controller
	private float _maxTime;				// The starting point of the timer
	private Text _numPatText;			// Reference to the num patterns text
	private int[] _multScores;			// The scores at which the multipler goes up

	// Dynamic vars
	private List<int> _inputs;			// List of currently selected inputs
	private List<int> _pattern;			// List containing desired pattern
	private float _timer;				// Current value of time left
	private float _prevTimer;			// Time at which pattern started - to calculate score
	private int _difficulty;			// Reference to current difficulty/multiplier
	private float _timeAdded;			// The time added upon completion of a pattern
	private float _timeTaken;			// The time removed when the wrong token is clicked
	private bool _active;				// Specifies whether the game is active

	// Statistics
	private int _score;					// Score of current game
	private int _numPatterns;			// Number of correct patterns this game
	private bool _flawlessPattern;		// Is the current pattern flawless


	// On instantiation
	void Start() {
		InitVars();
		_ic.NextBoard();
	}

	// Runs every frame
	void Update() {
		if(InPlay()) {
			if(CorrectPattern()) {
				NextPattern();
			}
			else {
				_timer -= Time.deltaTime;
			}
		}else 
		if(_active) {
			Lose();
		}
	}

/// -----------------------------------------------------------------------------------------------
/// Public methods --------------------------------------------------------------------------------

	// Sets the pattern
	public void SetPattern(List<int> pattern) {
		_pattern = pattern;
	}

	// Add token to input
	public void AddInput(int index) {
		_inputs.Add(index);
	}

	// Called from incorrectly clicked token
	public void IncorrectClick() {
		_timer = Mathf.Max(_timer - _timeTaken, 0);
		_flawlessPattern = false;
		_cc.ResetIndex();
	}

	// Checks whether given input appears in pattern
	public bool IsInPattern(int index) {
		return _pattern.Contains(index);
	}

	// Returns int for UI display
	public int Timer() {
		return (int)_timer;
	}
	// Returns the % of current time/max time for UI display
	public float TimerPercent() {
		return _timer/_maxTime;
	}
	// Returns whether the game is active
	public bool InPlay() {
		return _timer > 0f && _active;
	}

	// Adjusts and returns difficulty marker based on score
	public int Multiplier() {
		_difficulty = 0;
		for(int index = 0; index < _multScores.Length; index++) {
			if(_score < _multScores[index]) {
				_difficulty = index;
				break;
			}
		}
		if(_difficulty == 0) {
			_difficulty = _multScores.Length;
		}
		_timeAdded = 1.2f + (0.05f * _difficulty);
		return _difficulty;
	}

	// Returns the relevant multiplier config for current difficulty
	// The x value is the bottom score of current multiplier
	// The y value is the top score of current multiplier
	// The z value is the current score
	public Vector3 GetMultiplierConfig() {
		return (_difficulty < _multScores.Length)?
		new Vector3(_multScores[_difficulty-1], _multScores[_difficulty], _score) :
		new Vector3(_multScores[_difficulty-2], _multScores[_difficulty-1], _score);
	} 

	// DEVELOPMENT - RESTARTS GAME
	public void Restart() {
		SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
	}

/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_ic = GameObject.Find("Input").GetComponent<InputController>();
		_cc = GameObject.Find("ComboBonus").GetComponent<ComboController>();
		_mc = GameObject.Find("MultGauge").GetComponent<MultiplierController>();
		_sc = GameObject.Find("ScoreText").GetComponent<ScoreTextController>();
		GameCanvas = GameObject.Find("Canvas").transform;
		_numPatText = GameObject.Find("NumPatterns").GetComponent<Text>();
		_inputs = new List<int>();
		_pattern = new List<int>();

		// Game config
		_maxTime = 15f;
		_prevTimer = _timer = _maxTime - 5f;
		_timeTaken = 0.2f;
		_multScores = new [] {0,1500,8000,16000,26000,42000,50000,60000,75000,100000}; // Needs 10 length

		// Init stats
		_score = 0;
		_numPatterns = 0;
		_flawlessPattern = true;

		// Init UI
		_mc.SetMultiplier();

		// Start game
		_active = true;
	}

	// Creates the next pattern upon completion of the previous one
	private void NextPattern() {
		_inputs.Clear();
		_pattern.Clear();

		bool reset = !_flawlessPattern;
		UpdateScore();

		_timer = Mathf.Min(_timer + _timeAdded, _maxTime);
		_prevTimer = _timer;
		_ic.NextBoard(reset);
	}

	// Update score and stats
	private void UpdateScore() {
		int patternScore = 0;
		int comboScore = 0;
		patternScore = (100 - (int)((_prevTimer - _timer) * 10));
		_numPatterns++;
		if(_flawlessPattern) {
			Vector2 comboInfo = _cc.AddIndex();
			if((int)comboInfo.x == 3) {
				comboScore = BASE_COMBO_BONUS * (int)comboInfo.y;
			}
		}
		else {
			_flawlessPattern = true;
		}
		// Multiply by _difficulty (score multiplier)
		patternScore *= _difficulty;
		comboScore *= _difficulty;
		int patternBar = _score + patternScore;
		int comboBar = patternBar + comboScore;

		int scoreToAdd = patternScore + comboScore;
		_score += scoreToAdd;


		// Update UI
		_numPatText.text = _numPatterns.ToString();
		_sc.UpdateScore(_score, scoreToAdd);
		_mc.UpdateMultiplier(_score, patternBar, comboBar);
	}

	// Called when combo bonus hits
	private int ComboBonus() {
		return BASE_COMBO_BONUS * _difficulty;
	}

	// Runs when the timer has run out
	private void Lose() {
		_active = false;
		_ic.DisableTokens();
	}

/// Private when conditions -------------------------------------------------------------------------------

	// Returns true the frame the inputs match the pattern
	private bool CorrectPattern() {
		return Functions.ListEquals(_inputs, _pattern);
	}
	
}
