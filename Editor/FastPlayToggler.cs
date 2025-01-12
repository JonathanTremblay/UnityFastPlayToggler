#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using FastPlayToggler.ToolbarCallback;

namespace FastPlayToggler
{
	/// <summary>
	/// This tool adds a 'Fast Play' checkbox next to the play button in the Unity Editor.
	/// It allows to quickly toggle Fast Play mode (without having to go to Project Settings > Editor > Enter Play Mode Settings).
	/// 
	/// Keep in mind that Fast Play mode will prevent the Domain and/or Scene from being reloaded when entering Play.
	/// This can lead to unexpected behavior, so don't forget to make frequent tests without the Fast Play mode!
	/// 
	/// Created by Jonathan Tremblay, teacher at Cegep de Saint-Jerome.
	/// This project is available for distribution and modification under the MIT License.
	/// https://github.com/JonathanTremblay/UnityFastPlayToggler
	/// 
	/// This package includes code from 'Unity Toolbar Extender', a project created by Marijn Zwemmer.
	/// </summary>
	[InitializeOnLoad]
	public class FastPlayToggler
	{
		const string VERSION = "Version 0.9.1 (2025-01-12)";
		const string PREF_NAME = "FastPlayMode";
		const string SESSION_MESSAGE_KEY = "FastPlayTogglerMessage";
		public enum MessageKey { FastPlay, TooltipOn, TooltipOff, MoreOptions, IsDisabled, IsFastest, IsSceneOnly, IsDomainOnly, IsSceneOnlyLabel, IsDomainOnlyLabel, About }
		static readonly Dictionary<MessageKey, string> _messages = new()
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
			{ MessageKey.About, $"\n<size=10>** Fast Play Toggler is free and open source. For updates and feedback, visit https://github.com/JonathanTremblay/UnityFastPlayToggler. {VERSION} **</size>" }
		};
		static string _labelText = _messages[MessageKey.FastPlay];
		static string _currentStateText = _messages[MessageKey.IsDisabled];
		static EnterPlayModeOptions _lastPlayModeOptions;
		static EnterPlayModeOptions _lastPlayModeOptionsWhenEnabled;
		static bool _isFastPlayMode;
		static bool _lastFastPlayMode;
		static bool _isPlacedOnLeft = false; // Change it to true to place the checkbox on the left side of the Play button
		static string _tooltipText = _messages[MessageKey.TooltipOff];

		static FastPlayToggler()
		{
			if (_isPlacedOnLeft) ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI); // To put it on the left side
			else ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI); // To put it on the right side (default)
		}

		/// <summary>
		/// Draws the checkbox and manages the fast play mode.
		/// </summary>
		static void OnToolbarGUI()
		{
			UpdatePlayModeState();

			if (_isPlacedOnLeft) GUILayout.FlexibleSpace(); // Required if the checkbox is on the left side
			GUILayout.BeginVertical();
			ManageVerticalAlign();

			bool isChecked = GUILayout.Toggle(_isFastPlayMode, new GUIContent(_labelText, _tooltipText));
			EditorPrefs.SetBool(PREF_NAME, isChecked);

			GUILayout.EndVertical();

			// If Event.current is not 'Used', return (no need to process the event if Repaint or Layout):
			ManagePlayModeOptions(isChecked);
		}

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
			string previousMessage = SessionState.GetString(SESSION_MESSAGE_KEY, ""); // Get the previous message
			// If the message is different than the previous message:
			if (message != previousMessage) 
			{
				// If the previous message is not empty OR fast play mode is enabled, display the message in the console:
				if (previousMessage != "" || _isFastPlayMode) Debug.Log(message);
			}
			SessionState.SetString(SESSION_MESSAGE_KEY, message); // Save the message for the next time
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