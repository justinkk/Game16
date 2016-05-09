using UnityEngine;
using System.Collections;

/* This object gives the first player who grabs it one point,
 * triggers a Bonus event, and then disappears. */
public class OneBonus : OverlapObject {
    public OneBonus() : base(1, true, "Bonus")
    {
    }
}
