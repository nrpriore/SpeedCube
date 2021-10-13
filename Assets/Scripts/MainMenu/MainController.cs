using UnityEngine;
using UnityEngine.SceneManagement;

// Entry point of the application - Initializes app and settings
public class MainController : MonoBehaviour {

	public NavController NavController;
	public ScaleController ScaleController;

	// Entry point of the application
	void Awake() {
		SetQualitySettings();
		InitializeControllers();
	}

	// DEV
	public void StartGame() {
		SceneManager.LoadScene("Game");
	}


	// Sets relevant quality settings
	private void SetQualitySettings() {
		QualitySettings.vSyncCount = 0;  // VSync must be disabled to set targetFrameRate
		Application.targetFrameRate = 60;
	}

	// Initializes relevant Controller classes
	private void InitializeControllers() {
		NavController.Initialize();
		ScaleController.SetMainMenuUI();
	}
}
