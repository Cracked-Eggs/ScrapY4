using System.Collections.Generic;
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
        public TMP_Dropdown resolutionDropdown;

        public Toggle fullScreenToggle;

        public Slider masterVolumeSlider;

        [Header("Audio")]
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
            defaultMusicVolume = musicAudioSource.volume; // Get the default value first
            FadeIn(musicAudioSource, 1.5f, defaultMusicVolume); // Then fade in

            Application.runInBackground = true; // Makes game not to be frozen when you press windows key

            // Add event to the input system, to check for the joystick
            InputSystem.onActionChange += InputActionChangeCallback;


            // SET UP SETTINGS
            availableResolutions = Screen.resolutions;
            List<string> options = new List<string>();

            for (int i = 0; i < availableResolutions.Length; i++)
            {
                Resolution res = availableResolutions[i];
                string option = res.width + " x " + res.height;
                options.Add(option);

                if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            fullScreenToggle.isOn = Screen.fullScreen;

            resolutionDropdown.onValueChanged.AddListener(SetResolution);
            fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

            masterVolumeSlider.value = 1f;
        }

        public void SetResolution(int resolutionIndex)
        {
            if (resolutionIndex < 0 || resolutionIndex >= availableResolutions.Length)
            {
                Debug.LogWarning("Resolution index out of range.");
                return;
            }

            Resolution res = availableResolutions[resolutionIndex];
            Screen.SetResolution(res.width, res.height, fullScreenToggle.isOn);
        }

        public void SetFullScreen(bool isFullScreen)
        {
            Screen.fullScreen = isFullScreen;

            Resolution res = availableResolutions[resolutionDropdown.value];
            Screen.SetResolution(res.width, res.height, isFullScreen);
        }

        public void ToggleFullScreen()
        {
            bool newFullScreen = !Screen.fullScreen;
            Screen.fullScreen = newFullScreen;

            fullScreenToggle.isOn = newFullScreen;

            Resolution res = availableResolutions[resolutionDropdown.value];
            Screen.SetResolution(res.width, res.height, newFullScreen);
        }

        public void SetMasterVolume(float volume)
        {
            float dB;
            if (volume > 0)
                dB = Mathf.Log10(volume) * 20;
            else
                dB = -80f;

            audioMixer.SetFloat("MasterVolume", dB);
        }

        public void PlayButtonSoundOnce(bool pressSfx = true)
        {
            if (pressSfx)
            {
                if (randomizePitchButtonPressSound)
                {
                    button2DAudioSource.pitch = UnityEngine.Random.Range(minPitchButtonPressSound, maxPitchButtonPressSound);
                }
                else
                {
                    button2DAudioSource.pitch = 1.0f;
                }

                button2DAudioSource.PlayOneShot(buttonPressSounds[UnityEngine.Random.Range(0, buttonPressSounds.Count - 1)], buttonPressSound_Volume);
            }
            else
            {
                if (randomizePitchButtonSelectSound)
                {
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
            FadeOut(musicAudioSource, 0.5f);
            SceneManager.LoadScene("Main"); // Enter your scene name
        }

        private IEnumerator LoadGameLevel()
        {
            // Fade music (musicAudioSource)

            yield return new WaitForSeconds(0f);

        }

        private IEnumerator SetDefaultSelected(GameObject newDefaultButton)
        {
            // Wait until end of frame to ensure the UI is updated.
            yield return new WaitForEndOfFrame();

            // Set the default selected button.
            EventSystem.current.SetSelectedGameObject(null); // Clear current selection.
            EventSystem.current.SetSelectedGameObject(newDefaultButton);
        }

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

            audioSource.volume = targetVolume;
        }

        public void FadeIn(AudioSource audioSource, float duration, float targetVolume = 1f)
        {
            StartCoroutine(FadeAudioSource(audioSource, targetVolume, duration));
        }

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
            }
        }
    }
}