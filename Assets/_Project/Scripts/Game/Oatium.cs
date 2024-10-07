using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Oatium : MonoBehaviour
{
    Tween _tween;

    public void Start()
    {
        Debug.Log("Oatium is here!");
        _tween = transform.DOLocalMoveY(transform.localPosition.y + 1, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
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
        _tween.Kill();
        transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBack);
        float time = Time.time;
        var pig = Game.Instance.QuatumPig.transform;
        Game.Instance.CollectOatium();
        float t = 0;
        float flyTime = 1.5f;
        while (Time.time - time < flyTime)
        {
            var target = pig.position;
            t += Time.deltaTime / flyTime;
            var control = transform.position + (target - transform.position) * 0.5f + Vector3.up * Mathf.Sqrt(Vector3.Distance(transform.position, target));
            var pos = MathExtensions.Bezier(transform.position, control, target, t);

            transform.position = pos;
            yield return null;
        }
        Destroy(gameObject);
    }
}
