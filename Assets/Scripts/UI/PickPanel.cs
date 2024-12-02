using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickPanel : MonoBehaviour
{
    private Transform Btns;
    // Start is called before the first frame update
    void Start()
    {
        Btns = transform.Find("Btns");
        Btns.gameObject.SetActive(false);
        Events.Instance.OnEnemyDead += ShowPickPanel;
        Events.Instance.OnEnemyDeadChooseEnd += HidePickPanel;
    }

    private void OnDestroy()
    {
        Events.Instance.OnEnemyDead -= ShowPickPanel;
        Events.Instance.OnEnemyDeadChooseEnd -= HidePickPanel;
    }

    private void HidePickPanel()
    {
        Btns.gameObject.SetActive(false);
    }

    private void ShowPickPanel()
    {
        Btns.gameObject.SetActive(true);
    }
}
