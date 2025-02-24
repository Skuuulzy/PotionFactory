using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShortcutsController : MonoBehaviour
{
    [SerializeField] private GameObject _shortcutsLabel;
    [SerializeField] private GameObject _shortcutsPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayShortcuts(true);
        }
        else if(Input.GetKeyUp(KeyCode.Tab))
        {
            DisplayShortcuts(false);
        }
    }

    private void DisplayShortcuts(bool value)
    {
        _shortcutsLabel.SetActive(!value);
        _shortcutsPanel.SetActive(value);
    }
}
