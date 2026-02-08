using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject aboutPanel;

    [Header("Animator")]
    [SerializeField] private Animator aboutAnimator;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button backButton;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI aboutText;

    private bool isTransitioning;
    private Coroutine colorRoutine;


    [Header("Fade Transition")]
    [SerializeField] private GameObject fadePanelObject;
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 0.6f;

    void Start()
    {
        fadePanelObject.SetActive(true);
        fadeCanvas.alpha = 1;

        StartCoroutine(FadeOut());

        if (aboutPanel != null)
            aboutPanel.SetActive(false);
    }




    // -------------------------------------------------
    // PLAY GAME
    // -------------------------------------------------
    public void PlayGame()
    {
        if (isTransitioning) return;

        StartCoroutine(PlayRoutine());
    }

    IEnumerator PlayRoutine()
    {
        isTransitioning = true;

        yield return FadeIn();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }



    // -------------------------------------------------
    // ABOUT PANEL OPEN
    // -------------------------------------------------
    public void AboutGame()
    {
        if (isTransitioning) return;

        StartCoroutine(OpenAbout());
    }

    IEnumerator OpenAbout()
    {
        isTransitioning = true;

        aboutPanel.SetActive(true);

        if (aboutAnimator != null)
            aboutAnimator.SetTrigger("Open");

        yield return null;

        if (aboutText != null)
            colorRoutine = StartCoroutine(ColorWave());

        yield return new WaitForSeconds(0.35f);

        mainMenuPanel.SetActive(false);

        isTransitioning = false;
    }

    // -------------------------------------------------
    // BACK TO MENU
    // -------------------------------------------------
    public void BackToMenu()
    {
        if (isTransitioning) return;

        StartCoroutine(CloseAbout());
    }

    IEnumerator CloseAbout()
    {
        isTransitioning = true;

        mainMenuPanel.SetActive(true);

        if (aboutAnimator != null)
            aboutAnimator.SetTrigger("Close");

        if (colorRoutine != null)
        {
            StopCoroutine(colorRoutine);
            colorRoutine = null;
        }

        yield return new WaitForSeconds(0.35f);

        aboutPanel.SetActive(false);

        isTransitioning = false;
    }

    // -------------------------------------------------
    // EXIT GAME
    // -------------------------------------------------
    public void ExitGame()
    {
        if (isTransitioning) return;

        StartCoroutine(QuitRoutine());
    }

    IEnumerator QuitRoutine()
    {
        isTransitioning = true;

        yield return FadeIn();

#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    // -------------------------------------------------
    // TEXT COLOR WAVE
    // -------------------------------------------------
    IEnumerator ColorWave()
    {
        while (aboutPanel.activeInHierarchy)
        {
            if (aboutText == null)
                yield break;

            aboutText.ForceMeshUpdate();

            TMP_TextInfo textInfo = aboutText.textInfo;

            if (textInfo.characterCount == 0)
                yield return null;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

                if (vertexColors == null || vertexColors.Length == 0)
                    continue;

                Color32 color = Random.ColorHSV();

                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;
            }

            aboutText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            yield return new WaitForSeconds(0.3f);
        }
    }




    IEnumerator FadeIn()
    {
        fadePanelObject.SetActive(true);
        fadeCanvas.alpha = 0;

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = time / fadeDuration;
            yield return null;
        }

        fadeCanvas.alpha = 1;
    }
    IEnumerator FadeOut()
    {
        fadeCanvas.blocksRaycasts = true;

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeCanvas.alpha = 1 - (time / fadeDuration);
            yield return null;
        }

        fadeCanvas.alpha = 0;
        fadeCanvas.blocksRaycasts = false;

        fadePanelObject.SetActive(false);
    }

}
