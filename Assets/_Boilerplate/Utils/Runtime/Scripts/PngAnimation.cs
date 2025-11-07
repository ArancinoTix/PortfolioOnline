using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PngAnimation : MonoBehaviour
{
    [SerializeField] private Image display;
    [SerializeField] Sprite[] frames;
    [SerializeField] float fps = 24.0f;
    [SerializeField] bool loop = false;
    [SerializeField] int loopStartFrame = 0;
    private int cnt = 0;
    private float playbackPosition;

    Coroutine routine;

    public float PlaybackPosition { get => playbackPosition; }

    public void PlayAnimation(bool shouldLoop = false)
    {
        loop = shouldLoop;
        StopAnimation();
        routine = StartCoroutine(DoAnimation());
    }

    public void StopAnimation()
    {
        if (routine != null)
            StopCoroutine(routine);
        cnt = 0;
        display.sprite = frames[cnt];
    }

    IEnumerator DoAnimation()
    {
        while (cnt < frames.Length)
        {
            yield return new WaitForSeconds(1 / fps);
            display.sprite = frames[cnt];
            playbackPosition = (float)cnt / (float)frames.Length;
            cnt++;

            if (loop)
            {
                if (cnt >= frames.Length)
                    cnt = loopStartFrame;
            }
        }
    }
}
