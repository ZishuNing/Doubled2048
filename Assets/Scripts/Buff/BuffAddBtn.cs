using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffAddBtn : MonoBehaviour
{
    public BuffType buff;
    private Button btn;

    // Start is called before the first frame update
    void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(AddBuff);
    }

    private void AddBuff()
    {
        BuffManager.Instance.AddBuff(buff);
        Events.Instance.EnemyDeadChooseEnd();
    }
}
