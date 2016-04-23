using UnityEngine;
using System.Collections.Generic;

public class MinigameManager : MonoBehaviour {

    void Awake() {
        DontDestroyOnLoad(gameObject);
        GameManager.instance.startMinigame(this);
    }

    // Use this for initialization
    virtual public  void Start () { }
	
	// Update is called once per frame
	virtual public void Update () { }

    virtual public void tick() { }

    virtual public List<int> getWinners() { return new List<int>(); }
}
