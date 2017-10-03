using UnityEngine;						// To inherit from Monobehaviour
using UnityEngine.UI;					// To access Unity UI classes
using System.Collections.Generic;		// To access Lists
using UnityEngine.SceneManagement;		// To change scenes

public class GameController : MonoBehaviour {

	// Constant vars
	public const string BASE_HEX 	= "C4EAF2FF";		// Base hex color of input tokens
	public const string CLICK_HEX 	= "FDFF4BFF";		// Hex color of tokens when selected
	private const float TIME_ADD 	= 1f;				// Time added when a pattern is correct
	private const int BASE_COMBO_BONUS	= 200;			// Combo bonus points at x1 mult
	public const int MAX_COMBO_MULT 	= 10;			// Max combo multipler
	public static Transform GameCanvas;					// Reference to transform of the Canvas
	private InputController _ic;		// Reference to the input controller
	private ComboController _cc;		// Reference to the combo controller
	private float _maxTime;				// The starting point of the timer
	private Text _scoreText;			// Reference to the score text
	private Text _numPatText;			// Reference to the num patterns text

	// Dynamic vars
	private List<int> _inputs;			// List of currently selected inputs
	private List<int> _pattern;			// List containing desired pattern
	private float _timer;				// Current value of time left
	private float _prevTimer;			// Time at which pattern started - to calculate score
	private int _difficulty;			// Reference to current difficulty/multiplier
	private float _timeAdded;			// The time added upon completion of a pattern
	private float _timeTaken;			// The time removed when the wrong token is clicked

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
		}
		else {
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

	// Called when combo bonus hits
	public void ComboBonus(int multiplier) {
		_score += BASE_COMBO_BONUS * multiplier;
		UpdateScoreUI();
	}

	// Checks whether given input appears in pattern
	public bool IsInPattern(int index) {
		return _pattern.Contains(index);
	}

	// Returns the % of current time/max time for UI display
	public float TimerPercent() {
		return _timer/_maxTime;
	}

	// Adjusts and returns difficulty marker based on score
	public int Multiplier() {
		if(_score >= 80000) {
			_timeAdded = 5.8f;
			_difficulty = 9;
		}else 
		if(_score >= 65000) {
			_timeAdded = 5.4f;
			_difficulty = 8;
		}else 
		if(_score >= 55000) {
			_timeAdded = 5f;
			_difficulty = 7;
		}else 
		if(_score >= 42000) {
			_timeAdded = 4.3f;
			_difficulty = 6;
		}else 
		if(_score >= 32000) {
			_timeAdded = 3.9f;
			_difficulty = 5;
		}else 
		if(_score >= 25000) {
			_timeAdded = 3.5f;
			_difficulty = 4;
		}else 
		if(_score >= 8000) {
			_timeAdded = 2.8f;
			_difficulty = 3;
		}else 
		if(_score >= 1500) {
			_timeAdded = 2.4f;
			_difficulty = 2;
		}
		else {
			_timeAdded = 2f;
			_difficulty = 1;
		}
		return _difficulty;
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
		GameCanvas = GameObject.Find("Canvas").transform;
		_scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
		_numPatText = GameObject.Find("NumPatterns").GetComponent<Text>();
		_inputs = new List<int>();
		_pattern = new List<int>();

		// Game config
		_maxTime = 15f;
		_prevTimer = _timer = _maxTime - 5f;
		_timeTaken = 0.2f;

		// Init stats
		_score = 0;
		_numPatterns = 0;
		_flawlessPattern = true;
	}

	// Creates the next pattern upon completion of the previous one
	private void NextPattern() {
		_inputs.Clear();
		_pattern.Clear();

		bool reset = !_flawlessPattern;
		UpdateScore();
		UpdateScoreUI();

		_timer = Mathf.Min(_timer + _timeAdded, _maxTime);
		_prevTimer = _timer;
		_ic.NextBoard(reset);
	}

	// Update score and stats
	private void UpdateScore() {
		_score += _difficulty * (100 - (int)((_prevTimer - _timer) * 10));
		_numPatterns++;
		if(_flawlessPattern) {
			_cc.AddIndex();
		}
		else {
			_flawlessPattern = true;
		}
	}

	//Update score and stat UI
	private void UpdateScoreUI() {
		_scoreText.text = _score.ToString(); 
		_numPatText.text = _numPatterns.ToString();
	}

	// Runs when the timer has run out
	private void Lose() {
		
	}

/// When conditions -------------------------------------------------------------------------------

	// Returns true the frame the inputs match the pattern
	private bool CorrectPattern() {
		return Functions.ListEquals(_inputs, _pattern);
	}

	// Returns whether the game is active
	private bool InPlay() {
		return _timer > 0;
	}
	
}
