using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private MemoryCard originalCard;
    [SerializeField] private Sprite[] cardImages;
    [SerializeField] private TMP_Text scoreText;

    [Header("Timers")]
    [SerializeField] private float moveTime = 0.2f; //Time to move and rotate card
    [SerializeField] private float waitToSpawn = 0.1f; //Time to spawn cards to grid
    [SerializeField] private float waitForCheckMatch = 0.3f; //How much time needed for cards to unreavel
    [SerializeField] private float randomCardRotation = 90; //Random card rotation between this value
    private int score = 0;

    private MemoryCard firstRevealed;
    private MemoryCard secondRevealed;

    private AudioSource audioSource;

    public bool canReveal
    {
        get { return secondRevealed == null; }
    }

    public const int gridRows = 2;
    public const int gridColumns = 4;
    public const float offsetX = 2.5f;
    public const float offsetY = 3f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine(PlaceCards());
    }

    IEnumerator PlaceCards()
    {
        Vector3 startPos = originalCard.transform.position;

        int[] numbers = { 0, 0, 1, 1, 2, 2, 3, 3 };
        numbers = ShuffleArray(numbers);

        for(int x= 0; x <gridColumns; x++)
        {
            for(int y = 0; y <gridRows; y++)
            {
                MemoryCard card;
                if(x==0 & y== 0)
                    card = originalCard;
                else
                {
                    card = Instantiate(originalCard) as MemoryCard;

                    card.transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Random.Range(-randomCardRotation, randomCardRotation));

                }
                audioSource.Play();

                int index = y * gridColumns + x;
                int id = numbers[index];
                card.SetCard(id, cardImages[id]);

                float posX = (offsetX * x) + startPos.x;
                float posY = -(offsetY * y) + startPos.y;

                StartCoroutine(AnimationCoroutine(card.transform, new Vector3(posX, posY, startPos.z), moveTime));

                yield return new WaitForSeconds(waitToSpawn);

            }
        }
    }

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];

        for (int i = 0; i < newArray.Length; i++)
        {
            int tmp = newArray[i];
            int random = Random.Range(0, newArray.Length);
            newArray[i] = newArray[random];
            newArray[random] = tmp;
        }
        return newArray;
    }

    public void CardRevealed(MemoryCard card)
    {
        if(firstRevealed == null)
        {
            firstRevealed = card;
        }
        else
        {
            secondRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        if(firstRevealed.Id == secondRevealed.Id)
        {
            score++;
            scoreText.text = $"Score: {score}";
        }
        else
        {
            yield return new WaitForSeconds(waitForCheckMatch);

            firstRevealed.Unreveal();
            secondRevealed.Unreveal();
        }

        firstRevealed = null;
        secondRevealed = null;
    }


    private IEnumerator AnimationCoroutine(Transform cardTransform,Vector3 tartgetPosition,float lerpTime)
    {
        float elapsedTime = 0;
        Vector3 startPosition = cardTransform.position;

        Quaternion startRotation = cardTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);

        while (elapsedTime <= 1)
        {
            elapsedTime += Time.deltaTime /lerpTime;

            cardTransform.position = Vector3.Lerp(startPosition, tartgetPosition, elapsedTime);
            cardTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);

            yield return null;
        }
        cardTransform.position = tartgetPosition;
    }
}
