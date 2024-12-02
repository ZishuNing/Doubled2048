using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextScene : MonoBehaviour
{
    public Image panel;
    public void LoadNextScene()
    {
        panel.DOColor(new Color(0, 0, 0, 1), 1f).OnComplete(
            () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            });
    }
}
