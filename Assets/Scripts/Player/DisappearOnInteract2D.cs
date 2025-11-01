using System.Collections;
using UnityEngine;

public class DisappearOnInteract2D : MonoBehaviour
{
    [Header("Paramètres du carré")]
    public float delayBeforeDisappear = 2f;
    public Color highlightColor = Color.yellow;
    public Vector2 hitboxSize = new Vector2(1.5f, 1.5f);

    [Header("Barre de progression")]
    public Transform progressBar;
    public GameObject barBackground;

    [Header("Apparence")]
    public Sprite[] staticSprites; // cadavre_1 à cadavre_5
    public Animator animator;       // Animator pour la 6e option

    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 initialScale;
    private bool playerIsNear = false;
    private bool isDisappearing = false;
    private PlayerMovement playerMovement;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;

        // Collider
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.size = hitboxSize;
            collider.isTrigger = true;
        }

        // Barre
        if (progressBar != null)
        {
            initialScale = progressBar.localScale;
            progressBar.gameObject.SetActive(false);
        }
        if (barBackground != null)
            barBackground.SetActive(false);

        // Référence PlayerMovement
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        // Apparence aléatoire
        int index = Random.Range(0, 6);
        if (index < 5)
        {
            sr.sprite = staticSprites[index];
            if (animator != null)
                animator.enabled = false; // désactive l'anim si c'était actif
        }
        else
        {
            if (animator != null)
                animator.enabled = true;  // lance l'animation 6e option
        }
    }

    void Update()
    {
        if (playerIsNear && Input.GetKey(KeyCode.Space) && !isDisappearing)
        {
            if (!playerMovement.IsMoving())
            {
                StartCoroutine(DisappearAfterDelay());
            }
            else
            {
                ResetProgressBar();
            }
        }

        if (isDisappearing && !Input.GetKey(KeyCode.Space))
        {
            ResetProgressBar();
        }
    }

    private IEnumerator DisappearAfterDelay()
    {
        isDisappearing = true;
        float elapsed = 0f;

        if (barBackground != null)
            barBackground.SetActive(true);

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.localScale = new Vector3(0f, initialScale.y, initialScale.z);
        }

        while (elapsed < delayBeforeDisappear)
        {
            if (playerMovement.IsMoving() || !Input.GetKey(KeyCode.Space))
            {
                ResetProgressBar();
                yield break;
            }

            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / delayBeforeDisappear);

            if (progressBar != null)
                progressBar.localScale = new Vector3(progress * initialScale.x, initialScale.y, initialScale.z);

            yield return null;
        }

        GameManager.Instance.AddScore(10);
        Destroy(gameObject);
    }

    private void ResetProgressBar()
    {
        isDisappearing = false;
        if (progressBar != null)
            progressBar.localScale = new Vector3(0f, initialScale.y, initialScale.z);

        if (progressBar != null)
            progressBar.gameObject.SetActive(false);

        if (barBackground != null)
            barBackground.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = true;
            if (sr != null)
                sr.color = highlightColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsNear = false;
            if (sr != null)
                sr.color = originalColor;

            ResetProgressBar();
        }
    }
}
