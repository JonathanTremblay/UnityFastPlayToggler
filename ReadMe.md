# Unity Fast Play Toggler

This tool adds a 'Fast Play' checkbox next to the play button in the Unity Editor.
It allows to quickly toggle Fast Play mode (without having to go to Project Settings > Editor > Enter Play Mode Settings).

Keep in mind that Fast Play mode will prevent the Domain and/or Scene from being reloaded when entering Play.
This can lead to unexpected behavior, so don't forget to make frequent tests without the Fast Play mode!

## Table of Contents

1. [Getting Started](#getting-started)
2. [Compatibility](#compatibility)
3. [Known Issues](#known-issues)
4. [About the Project](#about-the-project)
5. [Contact](#contact)
6. [Version History](#version-history)
7. [License](#license)

## Getting Started

* Import this lightweight package to your project.
* The 'Fast Play' checkbox will appear next to the play button in the Unity Editor.
* Click the checkbox to enable or disable Fast Play mode.
* Advanced shortcuts:
  - ALT+Click: Reload Domain only
  - CTRL+Click: Reload Scene only
  - SHIFT+Click: Reload nothing

## Compatibility

* Tested on Windows and MacOS with Unity versions 2022.3, 2023.2 and 6.0.

## Known Issues

* Fast Play mode may lead to unexpected behavior due to the lack of Domain and/or Scene reloading.
* Make sure to frequently test your project without Fast Play mode to ensure stability.
* (Issues can be reported on GitHub: [https://github.com/JonathanTremblay/UnityFastPlayToggler/issues](https://github.com/JonathanTremblay/UnityFastPlayToggler/issues))

## About the Project

* This tool doesn't do any magic! It is only a shortcut to the options available in Project Settings > Editor > Enter Play Mode Settings.
* Those settings are nice, but they may change the way the project is behaving, so Fast Play Toggler is a tool to make toggling faster! 

## Contact

**Jonathan Tremblay**  
Teacher, Cegep de Saint-Jerome  
jtrembla@cstj.qc.ca

Project Repository: [https://github.com/JonathanTremblay/UnityFastPlayToggler](https://github.com/JonathanTremblay/UnityFastPlayToggler)

## Version History

* 0.9.0
    * First public version.

## License

* This project is licensed under the MIT License - see the [LICENSE](https://github.com/JonathanTremblay/UnityFastPlayToggler/blob/main/LICENSE) file for details.
* This package includes code from [Unity Toolbar Extender](https://github.com/marijnz/unity-toolbar-extender), a project created by Marijn Zwemmer, which is also under the MIT License.