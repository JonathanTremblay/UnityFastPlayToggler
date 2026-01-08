# Unity Fast Play Toggler

![Unity Version](https://img.shields.io/badge/unity-2021.3_--_6000.2-000000.svg?style=flat-square&logo=unity)
![License](https://img.shields.io/badge/license-MIT-green.svg?style=flat-square)

**Fast Play Toggler** adds a "Fast  ▶️" button (or a toggle checkbox) next to the play button in the Unity Editor. It allows to quickly enter **Fast Play mode** without having to manually change the Project Settings every time.

It acts as a shortcut to Unity's built-in **Enter Play Mode Options**. It disables Domain and Scene reloading to drastically reduce the time it takes to enter Play Mode.

> **⚠️ Warning:** Disabling Domain Reload means **static variables are not reset** between play sessions. Ensure your code handles static variable initialization correctly (see [Handling Static Variables](#handling-static-variables)).


## Table of Contents

1. [Features](#features)
2. [Getting Started](#getting-started)
3. [Compatibility](#compatibility)
4. [Handling Static Variables](#handling-static-variables)
5. [Known Issues](#known-issues)
6. [Contact](#contact)
7. [Version History](#version-history)
8. [License](#license)

## Features

*   **Fast Play Button:** A dedicated "Fast ▶️" button to start a single play session with Fast Play settings (Domain/Scene reload disabled), automatically reverting to safe settings afterwards.
*   **Toggle Checkbox (Optional):** A persistent checkbox to keep Fast Play enabled for standard Play button clicks.

## Getting Started

1.  **Install:** Import this package into your Unity project.
2.  **Use:**
    *   Click the **Fast ▶️** button in the toolbar to start playing immediately without reloading Domain/Scene.
    *   *Alternatively*, use the classic toggle checkbox if configured.

## Compatibility

*   **Supported Unity Versions:** From 2021.3 to 6000.2.
*   **Note for Unity 6.3+:** If you are using Unity 6000.3 or newer, it is recommended to use the newer **[Fast Play](https://github.com/JonathanTremblay/UnityFastPlay)** package instead, which uses the modern <kbd>**UnityEditor.Toolbars**</kbd> API.

⠀
## Handling Static Variables

When Domain Reload is disabled (which Fast Play does), **static variables are not reset** between play sessions. This is standard Unity behavior for "Fast Play" modes.

You must manually reset your static variables to ensure your game logic works correctly when restarting. The best way to do this is using the <kbd>**[RuntimeInitializeOnLoadMethod]**</kbd> attribute with <kbd>**SubsystemRegistration**</kbd>.

**Example:**

```csharp
public class MyScoreManager : MonoBehaviour
{
    public static int Score = 0;

    // This method runs before the scene loads, resetting the static variable.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Score = 0;
    }
}
```

⠀
## Known Issues

*   **Static Variables:** As mentioned above, static fields persist. Ensure you reset them.
*   **Unexpected Behavior:** Fast Play mode may lead to unexpected behavior in some third-party assets that rely on full domain reloading.
*   **Recommendation:** Use Fast Play for rapid iteration on gameplay logic, but frequently test with the regular Play button to ensure your game works correctly with a fresh start.
*   **Reporting:** Issues can be reported on GitHub: [https://github.com/JonathanTremblay/UnityFastPlayToggler/issues](https://github.com/JonathanTremblay/UnityFastPlayToggler/issues)

## Contact

**Jonathan Tremblay**  
Teacher, Cegep de Saint-Jerome  
jtrembla@cstj.qc.ca

Project Repository: [https://github.com/JonathanTremblay/UnityFastPlayToggler](https://github.com/JonathanTremblay/UnityFastPlayToggler)

## Version History

* 0.9.3
    * Added a dedicated "Fast Play" button for one-off fast sessions.
    * Added warnings for Unity 6.3+ users recommending the new FastPlay package.
* 0.9.2
    * Added French localization for messages.
* 0.9.1
    * Renamed asmdef file to match namespace. 
    * Prevented status messages from repeating.
* 0.9.0
    * First public version.

## License

* This project is licensed under the MIT License - see the [LICENSE](https://github.com/JonathanTremblay/UnityFastPlayToggler/blob/main/LICENSE) file for details.
* This package includes code from [Unity Toolbar Extender](https://github.com/marijnz/unity-toolbar-extender), a project created by Marijn Zwemmer, which is also under the MIT License.