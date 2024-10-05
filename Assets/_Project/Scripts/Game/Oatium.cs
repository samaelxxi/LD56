using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Oatium : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("Oatium is here!");
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 3f).SetEase(Ease.OutElastic);
    }

    void OnTriggerEnter(Collider other)
    {
        transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}
