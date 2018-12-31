using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DisplayGuessInfo : MonoBehaviour
{
    public Guess guess;

    public Text numberHolder;
    public Text bullHolder;
    public Text cowHolder;


    // Use this for initialization
    void Start()
    {
        updateInfo();
    }
    void updateInfo()
    {
        numberHolder.text = guess.gGuess;
        bullHolder.text = guess.gBulls.ToString();
        cowHolder.text = guess.gCows.ToString();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
