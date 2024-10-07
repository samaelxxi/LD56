using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using CarterGames.Assets.AudioManager;


public class DialogWindow : MonoBehaviour
{
    [Serializable]
    public class TextData
    {
        public TMPro.TMP_Text Text;
        public GameObject Bubble;
    }

    [SerializeField] TextData[] _texts;


    [SerializeField] TMPro.TMP_Text _text;


    bool _isTalking = false;

    [SerializeField] float _waitBeforeStart = 0.5f;
    [SerializeField] float _letterTime = 0.07f;
    [SerializeField] float _spaceTime = 0.03f;
    [SerializeField] float _dotTime = 0.3f;
    [SerializeField] float _commaTime = 0.15f;
    [SerializeField] float _endTime = 1;

    int dialogIdx = 0;

    Coroutine _coroutine;

    Coroutine _talkCor;


    public void GoGoGo()
    {
        foreach (var text in _texts)
        {
            text.Bubble.SetActive(false);
            text.Bubble.transform.parent.gameObject.SetActive(false);
        }

        StartCoroutine(DialogCor());

    }

    IEnumerator DialogCor()
    {
        dialogIdx = 0;


        while (dialogIdx < _texts.Length)
        {
            Debug.Log(dialogIdx);
            string toShow = _texts[dialogIdx].Text.text;
            _text = _texts[dialogIdx].Text;
            _text.text = "";
            _texts[dialogIdx].Bubble.SetActive(true);
            _texts[dialogIdx].Bubble.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            AudioManager.PlayGroup("oink", pitch: 1f.WithVariation(0.1f));
            yield return StartCoroutine(ShowTextInternal(toShow));
            float waitTime = dialogIdx == _texts.Length - 1 ? 3 : 1;
            if (dialogIdx == 2 || dialogIdx == 0)
                waitTime = 3;
            yield return new WaitForSeconds(waitTime);

            _text.text = "";
            _texts[dialogIdx].Bubble.SetActive(false);
            dialogIdx++;
        }
    }
    IEnumerator ShowText(string text, Action onComplete=null)
    {
        yield return new WaitForSeconds(_waitBeforeStart);
        yield return StartCoroutine(ShowTextInternal(text));
        StopTalking(_endTime / 2);
        yield return new WaitForSeconds(_endTime);
        onComplete?.Invoke();
        Clean();
        Debug.Log("2");
    }


    void StopTalking(float duration = 0)
    {
        if (_talkCor != null)
            StopCoroutine(_talkCor);
        _talkCor = null;
    }

    public IEnumerator Show(string text, bool force)
    {
        if (_isTalking && !force)
            yield break;

        Prepare(force);
        _coroutine = StartCoroutine(ShowText(text));
        yield return _coroutine;
    }

    public IEnumerator Show(List<string> texts, bool force)
    {
        if (_isTalking && !force)
            yield break;

        Prepare(force);
        _coroutine = StartCoroutine(ShowTexts(texts));
        yield return _coroutine;
    }

    public void Show(string text, bool force, Action onComplete)
    {
        if (_isTalking && !force)
            return;

        Prepare(force);
        _coroutine = StartCoroutine(ShowText(text, onComplete));
    }

    public void Show(List<string> texts, bool force, Action onComplete)
    {
        if (texts.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        if (_isTalking && !force)
            return;

        Prepare(force);
        _coroutine = StartCoroutine(ShowTexts(texts, onComplete));
    }

    void Prepare(bool force)
    {
        if (force && _coroutine != null)
            StopCoroutine(_coroutine);

        StopTalking();
        _text.text = "";
        _isTalking = true;
        // Debug.Log($"DialogWindow Prepare {gameObject == null} ");
        gameObject.SetActive(true);
    }

    IEnumerator ShowTexts(List<string> texts, Action onComplete=null)
    {
        yield return new WaitForSeconds(_waitBeforeStart);
        
        foreach (var text in texts)
        {
            yield return StartCoroutine(ShowTextInternal(text));
            _text.text = "";
        }
        StopTalking();
        yield return new WaitForSeconds(_endTime);
        onComplete?.Invoke();
        Clean();
    }


    void Clean()
    {
        _text.text = "";
        _isTalking = false;
        gameObject.SetActive(false);
    }

    IEnumerator ShowTextInternal(string text)
    {
        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '<' && (i > 0 && text[i-1] != '\\'))
            {
                int startIdx = i;
                while (text[i] != '>') i++;
                _text.text += text[startIdx..(i+1)];
                i++;
            }
            if (i < text.Length)
                yield return StartCoroutine(ShowSymbol(text[i++]));
        }
        float waitTime = Mathf.Clamp(text.Length * 0.07f, 0.5f, 1.2f);
        yield return new WaitForSeconds(waitTime);
    }

    IEnumerator ShowSymbol(char symbol)
    {
        _text.text += symbol;
        if (symbol == '.')
        {
            yield return new WaitForSeconds(_dotTime);
        }
        else if (symbol == ',')
        {
            yield return new WaitForSeconds(_commaTime);
        }
        else if (symbol == ' ')
        {
            yield return new WaitForSeconds(_spaceTime);
        }
        else
            yield return new WaitForSeconds(_letterTime);
    }
}
