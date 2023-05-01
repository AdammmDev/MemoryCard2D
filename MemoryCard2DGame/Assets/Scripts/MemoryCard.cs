using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;

public class MemoryCard : MonoBehaviour
{
    [SerializeField] private GameObject cardBack;
    [SerializeField] private float revealCardSpeed = 0.7f;
    [SerializeField] private AudioSource audioSource;

    private SpriteRenderer spritRenderer;

    [SerializeField] private SceneController controller;
    private int _id;
    public int Id { get { return _id; } }


    private void Start()
    {
        controller = FindObjectOfType<SceneController>();
        spritRenderer = cardBack.GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }
    public void OnMouseDown()
    {
        if (controller.canReveal)
        {
            StopAllCoroutines();

            StartCoroutine(RevealCardRoutine());
            controller.CardRevealed(this);
        }
    }

    public void Unreveal()
    {
        StopAllCoroutines();
        StartCoroutine(UnRevealCardRoutine());
    }

    public void SetCard(int id, Sprite cardImage)
    {
        _id = id;
        GetComponent<SpriteRenderer>().sprite = cardImage;
    }

    private IEnumerator RevealCardRoutine()
    {
        float elapsedTime = 0;
        audioSource.Play();
        while(spritRenderer.color.a >= 0)
        {
            elapsedTime += Time.deltaTime;
            float lerpT = elapsedTime / revealCardSpeed;

            float newAlpha = Mathf.Lerp(1, 0, lerpT);
            spritRenderer.color = new Color(spritRenderer.color.r, spritRenderer.color.g, spritRenderer.color.b, newAlpha);

            yield return null;
        }
        cardBack.SetActive(false);

    }
    private IEnumerator UnRevealCardRoutine()
    {
        cardBack.SetActive(true);

        float elapsedTime = 0;
        while (spritRenderer.color.a <= 1)
        {
            elapsedTime += Time.deltaTime;
            float lerpT = elapsedTime / revealCardSpeed;

            float newAlpha = Mathf.Lerp(0, 1, lerpT);
            spritRenderer.color = new Color(spritRenderer.color.r, spritRenderer.color.g, spritRenderer.color.b, newAlpha);

            yield return null;
        }

    }
}
