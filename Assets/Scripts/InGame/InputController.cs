using UnityEngine;						// To inherit from Monobehaviour
using System.Collections.Generic;		// To access Lists
using UnityEngine.UI;					// To access Unity UI classes

public class InputController : MonoBehaviour {

	// Constant vars
	private float _width;				// Width of input board
	private GameController _gc;			// Reference to the game controller
	// Tokens
	private GameObject _token;			// Reference to token prefab
	private GameObject _tokenTL;		// Reference to the top left token prefab
	private GameObject _tokenTR;		// Reference to the top right token prefab
	// Pattern rotation
	private const float ROT_THRESHOLD = 0.01f;				//  Threshold for rotation animation
	private RectTransform _patternRT;	// Reference to pattern recttransform
	private int _prevPatternRot;		// Reference to previous rotation of pattern object
	private RectTransform _inputRT;		// Reference to input recttransform
	private int _patternRot;			// Intended rotation of pattern
	private int _inputRot;				// Intended roation of input

	// Dynamic vars
	private GameObject _pattern;		// Reference to pattern game object
	

	// On instantiation
	void Awake() {
		InitVars();
	}

	// Runs every frame
	void Update() {
		if(Mathf.Abs(_patternRT.rotation.eulerAngles.z - _patternRot) > ROT_THRESHOLD) {
			_patternRT.rotation = Quaternion.Lerp(_patternRT.rotation, Quaternion.Euler(0, 0, _patternRot), Time.deltaTime * 10f);
		}
		if(Mathf.Abs(_inputRT.rotation.eulerAngles.z - _inputRot) > ROT_THRESHOLD) {
			_inputRT.rotation = Quaternion.Lerp(_inputRT.rotation, Quaternion.Euler(0, 0, _inputRot), Time.deltaTime * 10f);
		}
	}

/// -----------------------------------------------------------------------------------------------
/// Public methods --------------------------------------------------------------------------------

	// Called from the game controller to create a new board
	public void NextBoard(bool reset = false) {
		SwitchBoard(reset);
	}

	// Called when the player loses
	public void DisableTokens() {
		foreach(Transform token in gameObject.transform) {
			token.gameObject.GetComponent<Image>().color = Functions.HexToColor(GameController.DISABLE_HEX);
		}
		foreach(Transform token in _pattern.transform) {
			token.gameObject.GetComponent<Image>().color = Functions.HexToColor(GameController.DISABLE_HEX);
		}
	}

/// -----------------------------------------------------------------------------------------------
/// Private methods -------------------------------------------------------------------------------

	// Initialize game variables
	private void InitVars() {
		_gc = GameObject.Find("GameController").GetComponent<GameController>();
		_width = gameObject.GetComponent<RectTransform>().sizeDelta.x;
		_token = Resources.Load<GameObject>("InGame/Square/Main");
		_tokenTL = Resources.Load<GameObject>("InGame/Square/TopLeft");
		_tokenTR = Resources.Load<GameObject>("InGame/Square/TopRight");
		_inputRT = gameObject.GetComponent<RectTransform>();

		_patternRot = _prevPatternRot = 0;
	}

	// Switches the active game board
	private void SwitchBoard(bool reset = false) {
		// Clear relevant game objects
		for(int cnt = gameObject.transform.childCount-1; cnt >= 0; cnt--) {
			DestroyImmediate(gameObject.transform.GetChild(cnt).gameObject);
		}
		if(_pattern != null) {
			Destroy(_pattern);
		}

		int difficulty = _gc.Multiplier();
		// Create and assign board
		switch(difficulty) {
			case 1: case 2: case 3: case 4: case 5:
				CreateSquare(3, difficulty, reset);
				break;
			case 6: case 7: case 8: case 9: case 10:
			difficulty -= 5;
				CreateSquare(4, difficulty, reset);
				break;
			default:
				CreateSquare(10);
				break;
		}
	}

	// Creates and assigns the desired pattern
	private void AssignPattern(int numTokens, int difficulty) {
		// Create pattern gameobject
		_pattern = Instantiate<GameObject>(gameObject, GameController.GameCanvas);
		_patternRT = _pattern.GetComponent<RectTransform>();
		_pattern.GetComponent<InputController>().enabled = false;
		foreach(Transform child in _pattern.transform) {
			child.GetComponent<Button>().interactable = false;
		}

		// Edit pattern gameobject
		_pattern.name = "Pattern";
		RectTransform rect = _pattern.GetComponent<RectTransform>();
		rect.localScale = new Vector2(0.75f, 0.75f);
		rect.anchoredPosition = new Vector2(0, 550f);
		rect.rotation = Quaternion.Euler(0, 0, _prevPatternRot);

		// Determine how many tokens to light up and which ones
		List<int> pattern = new List<int>();
		int numToPick = (difficulty < 4)? 3 : 4;
		while(pattern.Count < numToPick) {
			int index = Mathf.FloorToInt(Random.value * (numTokens - 0.001f));
			while(pattern.Contains(index)) {
				index = Mathf.FloorToInt(Random.value * (numTokens - 0.001f));
			}
			pattern.Add(index);
		}

		foreach(int index in pattern) {
			_pattern.transform.GetChild(index).gameObject.GetComponent<Token>().AddPattern();
		}

		_gc.SetPattern(pattern);
	}

	// Creates a square of paramterized size
	private void CreateSquare(int size, int difficulty = -1, bool reset = false) {
		float margin = 15;		// Margin between input tokens
		float sizeFloat = (_width - ((size + 1) * margin)) / size;
		float posFloat = sizeFloat + margin;
		float initialPosFloat = -posFloat * (size/2f - 0.5f);

		int numTokens = size * size;
		int TLIndex = numTokens - size;
		int TRIndex = numTokens - 1;
		GameObject tempToken;
		for(int i = 0; i < numTokens; i++) {
			// Choose what token prefab to create
			tempToken = _token;
			if(i == TLIndex) {
				tempToken = _tokenTL;
			}else 
			if(i == TRIndex) {
				tempToken = _tokenTR;
			}
			RectTransform token = Instantiate<GameObject>(tempToken, gameObject.transform).GetComponent<RectTransform>();
			Vector2 position = new Vector2(initialPosFloat + (i % size) * posFloat,initialPosFloat + (i / size) * posFloat);

			float tokenScale = sizeFloat / token.sizeDelta.x;
			token.localScale = new Vector3(tokenScale, tokenScale, 1);
			token.anchoredPosition = position;
			token.name = i.ToString();
		}

		if(reset) {
			ResetObjects();
		}
		else {
			RotateObjects(difficulty);
		}
		AssignPattern(numTokens, difficulty);
	}

	// Determines if objects should rotate and picks how much
	private void RotateObjects(int difficulty) {
		// Don't rotate on first difficulty
		if(difficulty == 1) {
			ResetObjects();
			return;
		}

		// Pattern rotation
		float randPattern = Random.value;
		int rotPattern = 0;
		// Difficulty goes from 2 - 5 (since 1 has no pattern rotation) so...
		// Chance of rotating 180 degrees is 10% -> 25%
		// Chance of rotating 90 degrees is 30% -> 45%
		// I.e. total chance of any pattern rotation goes from 40% -> 70% (each difficulty adds 10%)
		float patternCheck2 = 1f - (0.05f * difficulty);
		float patternCheck1 = patternCheck2 - 0.2f - (0.05f * difficulty);
		if(randPattern > patternCheck2) {
			rotPattern = 2;
		}else
		if(randPattern > patternCheck1) {
			rotPattern = 1; 
		}

		RotatePattern((Random.value > 0.5f ? 1 : -1) * rotPattern);

		// Input rotation
		if(difficulty == 2) {
			return;
		}
		float randInput = Random.value;
		int rotInput = 0;
		// Difficulty goes from 3 - 5 (since 1 & 2 have no input rotation) so...
		// Chance of rotating 180 degrees is 10% -> 30%
		// Chance of rotating 90 degrees is 30% -> 40%
		// I.e. total chance of any input rotation goes from 40% -> 70% (each difficulty adds 10%)
		float inputCheck2 = 1.2f - (0.1f * difficulty);
		float inputCheck1 = inputCheck2 - 0.15f - (0.05f * difficulty);
		if(randInput >= inputCheck2) {
			rotInput = 2;
		}else
		if(randInput >= inputCheck1) {
			rotInput = 1; 
		}

		RotateInput((Random.value > 0.5f ? 1 : -1) * rotInput);
	}

	// Rotates the pattern to a specified multiple of 90
	// 30% chance to override the rotation and just return to 0 rotation
	private void RotatePattern(int index) {
		_prevPatternRot = _patternRot;
		_patternRot += index * 90;
		if(Random.value >= 0.7) {
			_patternRot = 0;
		}
	}

	// Rotates the input to a specified multiple of 90
	// 30% chance to override the rotation and just return to 0 rotation
	private void RotateInput(int index) {
		_inputRot += index * 90;
		if(Random.value >= 0.7) {
			_inputRot = 0;
		}
	}

	// Resets rotation on objects
	private void ResetObjects() {
		_prevPatternRot = _patternRot;
		_patternRot = (_patternRot / 360) * 360;
		_inputRot = (_inputRot / 360) * 360;
	}

}
