using System.Collections.Generic;
using MaskTransitions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Menu.UI
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;

        [Header("Menu UI Settings")]
        [Space(8)]

        public AudioSource musicAudioSource;
        private float defaultMusicVolume = -1;

        public AudioSource button2DAudioSource;
        public List<AudioClip> buttonSelectSounds = new List<AudioClip>();
        public float buttonSelectSound_Volume = 0.85f;
        public bool randomizePitchButtonSelectSound = true;
        public float minPitchButtonSelectSound = 0.92f;
        public float maxPitchButtonSelectSound = 1.09f;

        public List<AudioClip> buttonPressSounds = new List<AudioClip>();
        public float buttonPressSound_Volume = 0.85f;
        public bool randomizePitchButtonPressSound = true;
        public float minPitchButtonPressSound = 0.92f;
        public float maxPitchButtonPressSound = 1.09f;

        [HideInInspector] public bool isKeyboardAndMouse;
        [Space]

        [Header("Settings Components")]
        [Space(8)]
        [Tooltip("TMP Dropdown for selecting screen resolution.")]
        public TMP_Dropdown resolutionDropdown;

        [Tooltip("Toggle for setting fullscreen mode.")]
        public Toggle fullScreenToggle;

        [Tooltip("Slider for setting the master volume.")]
        public Slider masterVolumeSlider;

        [Header("Audio")]
        [Tooltip("Audio Mixer with an exposed parameter (e.g., 'MasterVolume').")]
        public AudioMixer audioMixer;

        // List to store available resolutions.
        private Resolution[] availableResolutions;

        // Currently selected resolution index.
        private int currentResolutionIndex = 0;
        [Space]
        [Header("Default Buttons for each Pages")]
        [Space(8)]

        public GameObject defaultButtonMainMenu;
        public GameObject defaultButtonSettings;
        public GameObject defaultButtonExtras;
        public GameObject defaultButtonQuitMessageMenu;

        private void Awake()
        {
            // Singleton
            if (MenuManager.Instance == null)
            {
                Instance = this;
                Debug.Log($"Assigning MenuManager singleton for '{this.gameObject.name}'");
            }
            else
            {
                Debug.Log($"Destroying MenuManager singleton from '{this.gameObject.name}', because there's two Instances!");
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            // Set up music
            // ..
            defaultMusicVolume = musicAudioSource.volume; // Get the default value first
            FadeIn(musicAudioSource, 1.5f, defaultMusicVolume); // Then fade in

            Application.runInBackground = true; // Makes game not to be frozen when you press windows key

            // Add event to the input system, to check for the joystick
            InputSystem.onActionChange += InputActionChangeCallback;

            // Fade out transition
            TransitionManager.Instance.PlayEndHalfTransition(0.50f);

            // SET UP SETTINGS
            // Populate the resolution dropdown list with available resolutions.
            availableResolutions = Screen.resolutions;
            List<string> options = new List<string>();

            for (int i = 0; i < availableResolutions.Length; i++)
            {
                Resolution res = availableResolutions[i];
                string option = res.width + " x " + res.height;
                options.Add(option);

                // Check if this resolution is the current one.
                if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            // Clear existing options and add our list.
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            // Set the fullscreen toggle based on the current setting.
            fullScreenToggle.isOn = Screen.fullScreen;

            // Add listeners to UI components.
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

            // Optionally, initialize the slider value to a current volume setting.
            // For example, if your mixer uses a value between -80 and 0 dB.
            // Here we assume 0 dB (normalized to 1 on the slider) is the default.
            masterVolumeSlider.value = 1f;
        }

        /// <summary>
        /// Called when the resolution dropdown value changes.
        /// </summary>
        /// <param name="resolutionIndex">Index of the selected resolution.</param>
        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex < 0 || resolutionIndex >= availableResolutions.Length)
            {
                Debug.LogWarning("Resolution index out of range.");
                return;
            }

            Resolution res = availableResolutions[resolutionIndex];
            // Use the current fullscreen setting from the toggle.
            Screen.SetResolution(res.width, res.height, fullScreenToggle.isOn);
        }

        /// <summary>
        /// Called when the fullscreen toggle value changes.
        /// </summary>
        /// <param name="isFullScreen">True if fullscreen is enabled.</param>
        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;

            // Optionally, reapply the current resolution so that the setting takes immediate effect.
            Resolution res = availableResolutions[resolutionDropdown.value];
            Screen.SetResolution(res.width, res.height, isFullScreen);
        }

        /// <summary>
        /// A parameterless function that toggles between fullscreen and windowed mode.
        /// This function can be called by other scripts or UI events.
        /// </summary>
        public void ToggleFullScreen()
        {
            bool newFullScreen = !Screen.fullScreen;
            Screen.fullScreen = newFullScreen;

            // Update the toggle UI to reflect the new state.
            fullScreenToggle.isOn = newFullScreen;

            // Reapply the current resolution with the updated fullscreen setting.
            Resolution res = availableResolutions[resolutionDropdown.value];
            Screen.SetResolution(res.width, res.height, newFullScreen);
        }

        /// <summary>
        /// Called when the master volume slider value changes.
        /// </summary>
        /// <param name="volume">Slider value (assumed range from 0 to 1).</param>
        public void SetMasterVolume(float volume)
        {
            // Convert the slider value to decibels. This assumes that a slider value of 1 corresponds to 0 dB,
            // and a slider value of 0 corresponds to -80 dB (silence). Adjust as needed.
            float dB;
            if (volume > 0)
                dB = Mathf.Log10(volume) * 20;
            else
                dB = -80f;

            audioMixer.SetFloat("MasterVolume", dB);
        }

        public void PlayButtonSoundOnce(bool pressSfx = true)
        {
            // Select random clip and play it once
            // ..
            if (pressSfx)
            {
                // Press button sound
                if (randomizePitchButtonPressSound)
                {
                    // Randomize pitch for random sound
                    button2DAudioSource.pitch = UnityEngine.Random.Range(minPitchButtonPressSound, maxPitchButtonPressSound);
                }
                else
                {
                    // Reset pitch
                    button2DAudioSource.pitch = 1.0f;
                }

                button2DAudioSource.PlayOneShot(buttonPressSounds[UnityEngine.Random.Range(0, buttonPressSounds.Count - 1)], buttonPressSound_Volume);
            }
            else
            {
                // Select button sound
                if (randomizePitchButtonSelectSound)
                {
                    // Randomize pitch for random sound
                    button2DAudioSource.pitch = UnityEngine.Random.Range(minPitchButtonSelectSound, maxPitchButtonSelectSound);
                }
                else
                {
                    // Reset pitch
                    button2DAudioSource.pitch = 1.0f;
                }

                button2DAudioSource.PlayOneShot(buttonSelectSounds[UnityEngine.Random.Range(0, buttonSelectSounds.Count - 1)], buttonSelectSound_Volume);
            }
        }

        public void OnResizeWindow()
        {
            ToggleFullScreen();
        }

        public void OnMainMenuPage()
        {
            StartCoroutine(SetDefaultSelected(defaultButtonMainMenu));
        }

        public void OnSettingsPage()
        {
            StartCoroutine(SetDefaultSelected(defaultButtonSettings));
        }

        public void OnExtrasPage()
        {
            StartCoroutine(SetDefaultSelected(defaultButtonExtras));
        }

        public void OnQuitMessagePage()
        {
            StartCoroutine(SetDefaultSelected(defaultButtonQuitMessageMenu));
        }

        public void OnQuitButton()
        {
            Application.Quit();
        }

        public void OnNewGame()
        {
            // EDIT THIS METHOD LATER BASED ON YOUR GAME
            // ..
            StartCoroutine(LoadGameLevel());
        }

        private IEnumerator LoadGameLevel()
        {
            Debug.Log("Loading game scene with transition");

            // Fade music (musicAudioSource)
            FadeOut(musicAudioSource, 0.5f);

            // Transition with asset
            // ..
            TransitionManager.Instance.PlayStartHalfTransition(0.5f);

            yield return new WaitForSeconds(0.6f);

            SceneManager.LoadScene("Game"); // Enter your scene name
            TransitionManager.Instance.PlayEndHalfTransition(0.5f);
        }

        private IEnumerator SetDefaultSelected(GameObject newDefaultButton)
        {
            // Wait until end of frame to ensure the UI is updated.
            yield return new WaitForEndOfFrame();

            // Set the default selected button.
            EventSystem.current.SetSelectedGameObject(null); // Clear current selection.
            EventSystem.current.SetSelectedGameObject(newDefaultButton);
        }

        /// <summary>
        /// Fades the volume of an AudioSource to the targetVolume over the specified duration.
        /// </summary>
        /// <param name="audioSource">The AudioSource to fade.</param>
        /// <param name="targetVolume">The target volume (typically between 0 and 1).</param>
        /// <param name="duration">Duration in seconds over which the fade occurs.</param>
        public IEnumerator FadeAudioSource(AudioSource audioSource, float targetVolume, float duration)
        {
            if (audioSource == null)
            {
                yield break;
            }

            float startVolume = audioSource.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                yield return null;
            }

            // Ensure the final volume is exactly the target volume.
            audioSource.volume = targetVolume;
        }

        /// <summary>
        /// Fades in an AudioSource over a given duration to a target volume.
        /// </summary>
        /// <param name="audioSource">The AudioSource to fade in.</param>
        /// <param name="duration">Duration in seconds for the fade in.</param>
        /// <param name="targetVolume">The target volume to reach (default is 1f).</param>
        public void FadeIn(AudioSource audioSource, float duration, float targetVolume = 1f)
        {
            StartCoroutine(FadeAudioSource(audioSource, targetVolume, duration));
        }

        /// <summary>
        /// Fades out an AudioSource over a given duration.
        /// </summary>
        /// <param name="audioSource">The AudioSource to fade out.</param>
        /// <param name="duration">Duration in seconds for the fade out.</param>
        public void FadeOut(AudioSource audioSource, float duration)
        {
            StartCoroutine(FadeAudioSource(audioSource, 0f, duration));
        }

        private void InputActionChangeCallback(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                InputAction receivedInputAction = (InputAction)obj;
                InputDevice lastDevice = receivedInputAction.activeControl.device;

                isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
                // If needed we could check for "XInputControllerWindows" or "DualShock4GamepadHID"
                // Maybe if it Contains "controller" could be xbox layout and "gamepad" sony? More investigation needed
            }
        }
    }
}