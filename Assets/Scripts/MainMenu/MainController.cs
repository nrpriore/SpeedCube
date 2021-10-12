using UnityEngine;

// Entry point of the application - Initializes app and settings
public class MainController : MonoBehaviour {

	public NavController NavController;

	// Entry point of the application
	void Awake() {
		SetQualitySettings();
		InitializeControllers();
	}


	// Sets relevant quality settings
	private void SetQualitySettings() {
		QualitySettings.vSyncCount = 0;  // VSync must be disabled to set targetFrameRate
		Application.targetFrameRate = 60;
	}

	// Initializes relevant Controller classes
	private void InitializeControllers() {
		NavController.Initialize();
	}
}
