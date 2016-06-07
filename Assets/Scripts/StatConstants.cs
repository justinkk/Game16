using UnityEngine;
using System.Collections;

public static class StatConstants {
    public const int NUM_STATS = 4;

    public const int SPEED = 0;
    public const int ACCELERATION = 1;
    public const int BRAKES = 2;
    public const int BOOST = 3;

    public static readonly string[] NAMES = { "Speed", "Acceleration", "Brakes", "Boost" };
}
