#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using FastPlayToggler.ToolbarCallback;

namespace FastPlayToggler
{
	/// <summary>
	/// This tool adds a 'Fast ▶️' button next to the regular play button in the Unity Editor.
	/// It allows to quickly activate Fast Play mode (without having to go to Project Settings > Editor > Enter Play Mode Settings).
	/// 
	/// The tool can still display a checkbox instead of a button, like in previous versions.
	/// To get the checkbox instead of the button, use the '_useNewButtonInsteadOfToggle' variable below.
	/// 
	/// Keep in mind that Fast Play mode will prevent the Domain and/or Scene from being reloaded when entering Play.
	/// This can lead to unexpected behavior, so don't forget to make frequent tests without the Fast Play mode!
	/// 
	/// Created by Jonathan Tremblay, teacher at Cegep de Saint-Jerome.
	/// This project is available for distribution and modification under the MIT License.
	/// https://github.com/JonathanTremblay/UnityFastPlayToggler
	/// 
	/// This package includes code from 'Unity Toolbar Extender', a project created by Marijn Zwemmer.
	/// 
	/// If you are using Unity 6.3 or newer, please use my new and improved 'Fast Play ⚡' tool instead:
	/// https://github.com/JonathanTremblay/UnityFastPlay
	/// </summary>
	[InitializeOnLoad]
	public class FastPlayToggler
	{
		const string _VERSION = "Version 0.9.3 (2026-01-08)";
		const string _PREF_NAME = "FastPlayMode";
		const string _SESSION_MESSAGE_KEY = "FastPlayTogglerMessage";
		const int BUTTON_WIDTH = 60;

		// Set this to false if you want to use the old toggle checkbox instead of the new button:
		static readonly bool _useNewButtonInsteadOfToggle = true;

		enum MessageKey { FastPlay, TooltipOn, TooltipOff, MoreOptions, IsDisabled, IsFastest, IsSceneOnly, IsDomainOnly, IsSceneOnlyLabel, IsDomainOnlyLabel, About, AboutBtn, WarningWithButton, WarningTitle, YesRecommended, No }
		static readonly Dictionary<MessageKey, string> _messagesEn = new()
		{
			{ MessageKey.FastPlay, "Fast Play" },
			{ MessageKey.TooltipOn, "Fast Play is enabled."},
			{ MessageKey.TooltipOff, "Fast Play is disabled." },
			{ MessageKey.MoreOptions, " <size=10>(Options: ALT+Click reloads Domain only, CTRL+Click reloads Scene only, SHIFT+Click reloads nothing.)</size>" },
			{ MessageKey.IsDisabled, "<b>[ <color=#BB7777>Fast Play Disabled:</color> Reload Domain and Scene ]</b>" },
			{ MessageKey.IsFastest, "<b>[ <color=#44CC44>Fast Play Enabled:</color> Do not reload Domain and Scene ]</b>" },
			{ MessageKey.IsSceneOnly, "<b>[ <color=#EECC22>Fast Play Partially Enabled:</color> Reload Scene only ]</b>" },
			{ MessageKey.IsDomainOnly, "<b>[ <color=#EECC22>Fast Play Partially Enabled:</color> Reload Domain only ]</b>" },
			{ MessageKey.IsSceneOnlyLabel, " (Reload Scene only)" },
			{ MessageKey.IsDomainOnlyLabel, " (Reload Domain only)" },
			{ MessageKey.About, $"\n<size=10>** Fast Play Toggler is free and open source. For updates and feedback, visit https://github.com/JonathanTremblay/UnityFastPlayToggler. {_VERSION} **</size>" },
			{ MessageKey.AboutBtn, $"<b><color=#539AEF>FAST ▶️</color></b> Remember that static variables persist between plays. Ensure they are reset in your scripts.\n <size=10>** Fast Play Toggler is free and open source – For updates and feedback, visit <a href=\"https://github.com/JonathanTremblay/UnityFastPlayToggler\">https://github.com/JonathanTremblay/UnityFastPlayToggler</a> – " + _VERSION + " **</size>" },
			{ MessageKey.WarningWithButton, "Your current Enter Play Mode Settings are set to \"Do not reload Domain or Scene\". This is not recommended with Fast Play. After this play session, do you want to reset these options back to \"Reload Domain and Scene\"?" },
			{ MessageKey.WarningTitle, "Enter Play Mode Settings Warning" },
			{ MessageKey.YesRecommended, "Yes (recommended)" },
			{ MessageKey.No, "No" }
		};
		static readonly Dictionary<MessageKey, string> _messagesFr = new()
		{
			{ MessageKey.FastPlay, "Fast Play" },
			{ MessageKey.TooltipOn, "Fast Play est activé."},
			{ MessageKey.TooltipOff, "Fast Play est désactivé." },
			{ MessageKey.MoreOptions, " <size=10>(Options: ALT+Clic recharge seulement le domaine, CTRL+Clic recharge seulement la scène, SHIFT+Clic ne recharge rien.)</size>" },
			{ MessageKey.IsDisabled, "<b>[ <color=#BB7777>Fast Play désactivé:</color> Recharge le domaine et la scène ]</b>" },
			{ MessageKey.IsFastest, "<b>[ <color=#44CC44>Fast Play activé:</color> Ne recharge ni le domaine ni la scène ]</b>" },
			{ MessageKey.IsSceneOnly, "<b>[ <color=#EECC22>Fast Play partiellement activé:</color> Recharge seulement la scène ]</b>" },
			{ MessageKey.IsDomainOnly, "<b>[ <color=#EECC22>Fast Play partiellement activé:</color> Recharge seulement le domaine ]</b>" },
			{ MessageKey.IsSceneOnlyLabel, " (Reload Scene only)" },
			{ MessageKey.IsDomainOnlyLabel, " (Reload Domain only)" },
			{ MessageKey.About, $"\n<size=10>** Fast Play Toggler est gratuit et open source. Pour les mises à jour et les commentaires, visitez https://github.com/JonathanTremblay/UnityFastPlayToggler. {_VERSION} **</size>" },
			{ MessageKey.AboutBtn, $"<b><color=#539AEF>FAST ▶️</color></b> N'oubliez pas que les variables statiques persistent entre les sessions de jeu. Assurez-vous qu'elles sont réinitialisées dans vos scripts.\n <size=10>** Fast Play Toggler est gratuit et open source – Pour les mises à jour et les commentaires, visitez <a href=\"https://github.com/JonathanTremblay/UnityFastPlayToggler\">https://github.com/JonathanTremblay/UnityFastPlayToggler</a> – " + _VERSION + " **</size>" },
			{ MessageKey.WarningWithButton, "Vos paramètres actuels d'entrée en mode Play sont configurés à \"Do not reload Domain or Scene\". Ceci n'est pas recommandé avec Fast Play. Après cette session de jeu, souhaitez-vous réinitialiser ces options à \"Reload Domain and Scene\" ?" },
			{ MessageKey.WarningTitle, "Avertissement : Paramètres du mode Play" },
			{ MessageKey.YesRecommended, "Oui (recommandé)" },
			{ MessageKey.No, "Non" }
		};
		// The dictionary to use for messages, depending on the current language:
		static Dictionary<MessageKey, string> _messages = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr" ? _messagesFr : _messagesEn;

		static string _labelText = _messages[MessageKey.FastPlay];
		static string _currentStateText = _messages[MessageKey.IsDisabled];
		static EnterPlayModeOptions _lastPlayModeOptions;
		static EnterPlayModeOptions _lastPlayModeOptionsWhenEnabled;
		static bool _isFastPlayMode;
		static bool _lastFastPlayMode;
		static bool _isPlacedOnLeft = true; // Change it to false to place the checkbox or button on the right side of the Play button
		static string _tooltipText = _messages[MessageKey.TooltipOff];
		static Color _buttonColorOn = new(100f / 255f, 200f / 255f, 255f / 255f);
		static Color _textColorOn = new(1, 1, 1);
		static Color _textColorOff = new(0, 0, 0);

		// Fields for the Fast Play Button logic
		static bool _isFastPlayButtonActive = false;
		static bool _wasStartedViaFastPlayButton = false;
		static EnterPlayModeOptions _restorePlayModeOptions;

		static FastPlayToggler()
		{
#if UNITY_6000_3_OR_NEWER
			Debug.LogWarning("FastPlayToggler: You are using Unity 6.3+. Please use my new and improved <b>Fast Play ⚡</b> tool: <color=white><a href=\"https://github.com/JonathanTremblay/UnityFastPlay\">https://github.com/JonathanTremblay/UnityFastPlay</a></color> **</size>");
#endif
			if (_useNewButtonInsteadOfToggle)
			{
				if (_isPlacedOnLeft) ToolbarExtender.LeftToolbarGUI.Add(OnToolbarButtonGUI); // To put it on the left side
				else ToolbarExtender.RightToolbarGUI.Add(OnToolbarButtonGUI); // To put it on the right side (default)
			}
			else
			{
				if (_isPlacedOnLeft) ToolbarExtender.LeftToolbarGUI.Add(OnToolbarToggleGUI); // To put it on the left side
				else ToolbarExtender.RightToolbarGUI.Add(OnToolbarToggleGUI); // To put it on the right side (default)
			}

			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		/// <summary>
		/// Draws the checkbox and manages the fast play mode.
		/// </summary>
		static void OnToolbarToggleGUI()
		{
			UpdatePlayModeState();

			if (_isPlacedOnLeft) GUILayout.FlexibleSpace(); // Required if the checkbox is on the left side

			GUILayout.BeginVertical();
			ManageVerticalAlign();

			bool isChecked = GUILayout.Toggle(_isFastPlayMode, new GUIContent(_labelText, _tooltipText));
			EditorPrefs.SetBool(_PREF_NAME, isChecked);

			GUILayout.EndVertical();

			GUILayout.Space(2);

			// If Event.current is not 'Used', return (no need to process the event if Repaint or Layout):
			ManagePlayModeOptions(isChecked);
		}

		/// <summary>
		/// Draws the Fast Play button next to the Play button.
		/// </summary>
		static void OnToolbarButtonGUI()
		{
			if (_isPlacedOnLeft) GUILayout.FlexibleSpace(); // Required if the checkbox is on the left side

			GUILayout.BeginVertical();
			//ManageVerticalAlign();

			GUIStyle boldStyle = new GUIStyle(EditorStyles.toolbarButton);
			boldStyle.fontStyle = FontStyle.Bold;
			var oldColor = GUI.contentColor;
			if (EditorApplication.isPlaying && _wasStartedViaFastPlayButton)
			{
				GUI.contentColor = _textColorOn;
				//also change the button background color
				var oldBgColor = GUI.backgroundColor;
				GUI.backgroundColor = _buttonColorOn;
			}
			else if (EditorApplication.isPlaying && !_wasStartedViaFastPlayButton)
			{
				GUI.contentColor = _textColorOff;
				//make the button not interactable (no rollover effect)
				GUI.enabled = false;
			}
			if (GUILayout.Button("Fast ▶️", boldStyle, GUILayout.Width(BUTTON_WIDTH)))
			{
				FastPlayButtonClick();
			}

			GUI.contentColor = oldColor;
			GUILayout.EndVertical();
		}

		static void FastPlayButtonClick()
		{
			if (EditorApplication.isPlaying && !_wasStartedViaFastPlayButton)
			{
				return; // Already in regular play mode, do nothing
			}
			else if (EditorApplication.isPlaying && _wasStartedViaFastPlayButton)
			{
				EditorApplication.isPlaying = false;
				return; // Stop play mode if it was started via Fast Play button
			}
			else // Not in play mode, start Fast Play mode
			{
				_restorePlayModeOptions = EditorSettings.enterPlayModeOptions;
				_isFastPlayButtonActive = true;
				_wasStartedViaFastPlayButton = true;

				EditorSettings.enterPlayModeOptionsEnabled = true;
				EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;

				EditorApplication.isPlaying = true;

				bool isAlreadyFastPlay = _restorePlayModeOptions == (EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload);
                if (isAlreadyFastPlay)
                {
                    bool shouldReset = EditorUtility.DisplayDialog(_messages[MessageKey.WarningTitle], _messages[MessageKey.WarningWithButton], _messages[MessageKey.YesRecommended], _messages[MessageKey.No]);
                    if (shouldReset)
					{
						_restorePlayModeOptions = EnterPlayModeOptions.None;
						Debug.Log("Fast ▶️: Enter Play Mode Settings will be reset after this play session.");
					}
                }
			}
		}

		static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (_isFastPlayButtonActive && state == PlayModeStateChange.EnteredPlayMode)
			{
				RestorePlayModeOptions();
				_isFastPlayButtonActive = false;
				Debug.Log(_messages[MessageKey.AboutBtn]);
			}
			else if (state == PlayModeStateChange.ExitingPlayMode)
			{
				_isFastPlayButtonActive = false;
				_wasStartedViaFastPlayButton = false;
			}
		}

        /// <summary>
        /// Restores the last saved Play Mode options (as soon as Play Mode is entered).
        /// </summary>
        static void RestorePlayModeOptions() => EditorSettings.enterPlayModeOptions = _restorePlayModeOptions;
		
		/// <summary>
		/// Checks the current fast play mode, and then updates all display elements.
		/// </summary>
		private static void UpdatePlayModeState()
		{
			EnterPlayModeOptions currentPlayModeOptions = EditorSettings.enterPlayModeOptions;
			_isFastPlayMode = EditorSettings.enterPlayModeOptionsEnabled;
			if (currentPlayModeOptions != _lastPlayModeOptions || _isFastPlayMode != _lastFastPlayMode)
			{

				if (_isFastPlayMode) _lastPlayModeOptionsWhenEnabled = currentPlayModeOptions;

				_lastPlayModeOptions = currentPlayModeOptions;

				UpdatePlayModeText();
			}
		}

		/// <summary>
		/// Updates the tooltip and displays the current state in the console.
		/// </summary>
		private static void UpdatePlayModeText()
		{
			_tooltipText = (_isFastPlayMode) ? _messages[MessageKey.TooltipOn] : _messages[MessageKey.TooltipOff];

			bool isDomainReloadDisabled = EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableDomainReload);
			bool isSceneReloadDisabled = EditorSettings.enterPlayModeOptions.HasFlag(EnterPlayModeOptions.DisableSceneReload);

			_labelText = _messages[MessageKey.FastPlay];
			if (!EditorSettings.enterPlayModeOptionsEnabled) _currentStateText = _messages[MessageKey.IsDisabled];
			else if (isDomainReloadDisabled && !isSceneReloadDisabled)
			{
				_labelText += _messages[MessageKey.IsSceneOnlyLabel];
				_currentStateText = _messages[MessageKey.IsSceneOnly];
			}
			else if (isSceneReloadDisabled && !isDomainReloadDisabled)
			{
				_labelText += _messages[MessageKey.IsDomainOnlyLabel];
				_currentStateText = _messages[MessageKey.IsDomainOnly];
			}
			else _currentStateText = _messages[MessageKey.IsFastest];

			string message = _currentStateText + _messages[MessageKey.MoreOptions] + _messages[MessageKey.About];
			string previousMessage = SessionState.GetString(_SESSION_MESSAGE_KEY, ""); // Get the previous message
																					   // If the message is different than the previous message:
			if (message != previousMessage)
			{
				// If the previous message is not empty OR fast play mode is enabled, display the message in the console:
				if (previousMessage != "" || _isFastPlayMode) Debug.Log(message);
			}
			SessionState.SetString(_SESSION_MESSAGE_KEY, message); // Save the message for the next time
		}

		/// <summary>
		/// Manages the play mode options when the checkbox is toggled (only if the checkbox has changed).
		/// </summary>
		/// <param name="isChecked">The new state of the checkbox.</param>
		static void ManagePlayModeOptions(bool isChecked)
		{
			// If SHIFT, CTRL or ALT is pressed, force the checkbox to be checked:
			if ((Event.current.type == EventType.Used) && (Event.current.shift || Event.current.control || Event.current.alt))
			{
				isChecked = true; // Force the checkbox to be checked
				_lastFastPlayMode = false; // Will force the next update
			}

			// If fast play mode is enabled, set Enter Play Mode Settings to true (in the Editor Settings):
			EditorSettings.enterPlayModeOptionsEnabled = isChecked;

			if (isChecked != _lastFastPlayMode) // If the checkbox has changed
			{
				if (isChecked)
				{
					// If _lastPlayModeOptionsWhenEnabled is NONE, or SHIFT is pressed, set it to disable Domain and Scene Reload:
					if (_lastPlayModeOptionsWhenEnabled == EnterPlayModeOptions.None || Event.current.shift)
					{
						_lastPlayModeOptionsWhenEnabled = EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
					}
					else if (Event.current.control) // CTRL is pressed, set it to disable Domain Reload:
					{
						_lastPlayModeOptionsWhenEnabled = EnterPlayModeOptions.DisableDomainReload;
					}
					else if (Event.current.alt) // ALT is pressed, set it to disable Scene Reload:
					{
						_lastPlayModeOptionsWhenEnabled = EnterPlayModeOptions.DisableSceneReload;
					}
					EditorSettings.enterPlayModeOptions = _lastPlayModeOptionsWhenEnabled;
				}
				_lastFastPlayMode = _isFastPlayMode;
			}
		}

		/// <summary>
		/// Manages the vertical alignment of the checkbox (needed in Unity versions prior to 6).
		/// </summary>
		static void ManageVerticalAlign()
		{
#if UNITY_6000_0_OR_NEWER
			// Nothing, no space needed in Unity 6
#else
			GUILayout.Space(2); // Add space to move the checkbox down (needed in previous Unity versions) 
#endif
		}
	}
}
#endif