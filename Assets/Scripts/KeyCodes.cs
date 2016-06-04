using UnityEngine;
using System.Collections;

public static class KeyCodes {
    /* 
     * Returns a KeyCode representing the A button for the given controller
     */
    public static KeyCode GetA(int controllerIndex) {
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor) {
            switch (controllerIndex) {
                case 2:
                    return KeyCode.Joystick2Button0;
                case 3:
                    return KeyCode.Joystick3Button0;
                case 4:
                    return KeyCode.Joystick4Button0;
                case 1:
                default:
                    return KeyCode.Joystick1Button0;

            }
        }
        switch (controllerIndex) {
            case 2:
                return KeyCode.Joystick2Button4;
            case 3:
                return KeyCode.Joystick3Button4;
            case 4:
                return KeyCode.Joystick4Button4;
            case 1:
            default:
                return KeyCode.Joystick1Button4;
        }
    }

    /* 
     * Returns a KeyCode representing the Z button for the given controller
     */
    public static KeyCode GetZ(int controllerIndex) {
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
            Application.platform == RuntimePlatform.WindowsEditor) {
            switch (controllerIndex) {
                case 2:
                    return KeyCode.Joystick2Button3;
                case 3:
                    return KeyCode.Joystick3Button3;
                case 4:
                    return KeyCode.Joystick4Button3;
                case 1:
                default:
                    return KeyCode.Joystick1Button3;
            }
        }
        switch (controllerIndex) {
            case 2:
                return KeyCode.Joystick2Button12;
            case 3:
                return KeyCode.Joystick3Button12;
            case 4:
                return KeyCode.Joystick4Button12;
            case 1:
            default:
                return KeyCode.Joystick1Button12;
        }
    }
}