using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RetryBtn : MonoBehaviour
{
    public Image panel;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(Retry);
    }

    public void Retry()
    {
        panel.DOColor(new Color(0, 0, 0, 1f), 0.5f).OnComplete(
                       () =>
                       {
                           SceneManager.LoadScene("2048");
                       });
    }
}
