using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskForPlayerName : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject askForName;
    public string username;
    // Start is called before the first frame update
    void Start()
    {
        askForName.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterName()
    {
        askForName.SetActive(false);
        menuPanel.SetActive(true);
    }
}
