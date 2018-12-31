using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour {

    public Transform LoadingBar;
    public Transform TextIndicator;
    [SerializeField] public static float currentAmount = 100;
    [SerializeField] private float speed;

    public void Restart()
    {
        currentAmount = 100;
    }

	// Update is called once per frame
	void Update () {
        if (GameManager.GM.timeToggle == 1) {
            if (currentAmount > 0)
            {
                currentAmount -= speed * Time.deltaTime;
                //TextIndicator.GetComponent<Text>().text = ((int)currentAmount).ToString() + "%";
            }
            else
            {
                GameManager.GM.timeToggle = 0;
                GameManager.GM.timeOut();
                //TextIndicator.GetComponent<Text>().text = ("Done");
            }
            LoadingBar.GetComponent<Image>().fillAmount = currentAmount / 100;
        }
    }
}
