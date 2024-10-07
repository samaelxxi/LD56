using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Oatium : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("Oatium is here!");
        transform.DOLocalMoveY(transform.localPosition.y + 1, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        var rot = transform.rotation.eulerAngles;
        rot.y += 360;
        transform.DOLocalRotate(rot, 6f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pig"))
        {
            StartCoroutine(BeEaten());
        }
    }

    IEnumerator BeEaten()
    {
        transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack);
        float time = Time.time;
        var pig = Game.Instance.QuatumPig.transform;
        Game.Instance.CollectOatium();
        while (Time.time - time < 1.5f)
        {
            transform.position = Vector3.Lerp(transform.position, pig.position, Time.deltaTime * 5);
            yield return null;
        }
        Destroy(gameObject);
    }
}
