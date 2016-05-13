using UnityEngine;
using System.Collections;

/* This class takes away one point from the first player who grabs it,
 triggers a Hazard event, then disappears. */
public class OneHazard : OverlapObject
{
    public OneHazard() : base(-1, true, "Hazard")
    {
    }
}
