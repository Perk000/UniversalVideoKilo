using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class NavigationMenuController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject toggleButtonGO;
    public float slideDuration = 0.5f;
    public float bounceAmount = 20f;
    public int bounceCount = 2;
    public bool startOpen = false; // Set to false to start closed, only open on toggle

    private bool isOpen = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private RectTransform panelRect;
    private Button toggleButton;
    private Toggle toggleToggle;

    void Start()
    {
        if (menuPanel == null)
        {
            Debug.LogError("NavigationMenuController: menuPanel is not assigned!");
            return;
        }
        panelRect = menuPanel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            Debug.LogError("NavigationMenuController: menuPanel does not have RectTransform!");
            return;
        }
        closedPosition = panelRect.anchoredPosition;
        openPosition = new Vector3(closedPosition.x, -350f, closedPosition.z); // Open to y = -350

        if (toggleButtonGO != null)
        {
            toggleButton = toggleButtonGO.GetComponent<Button>();
            toggleToggle = toggleButtonGO.GetComponent<Toggle>();

            if (toggleButton)
            {
                toggleButton.onClick.AddListener(ToggleMenu);
                Debug.Log("NavigationMenuController: Using Button component for toggle.");
            }
            else if (toggleToggle)
            {
                toggleToggle.onValueChanged.AddListener((isOn) => ToggleMenu());
                Debug.Log("NavigationMenuController: Using Toggle component for toggle.");
            }
            else
            {
                Debug.LogWarning("NavigationMenuController: toggleButtonGO does not have Button or Toggle component!");
            }
        }
        else
        {
            Debug.LogWarning("NavigationMenuController: toggleButtonGO is not assigned!");
        }

        // Start based on startOpen setting
        if (startOpen)
        {
            OpenMenu();
        }
        else
        {
            // Start closed
            panelRect.anchoredPosition = closedPosition;
            menuPanel.SetActive(false);
        }
    }

    public void ToggleMenu()
    {
        if (isOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        if (menuPanel == null)
        {
            Debug.LogError("NavigationMenuController: Cannot open menu, menuPanel is not assigned!");
            return;
        }
        menuPanel.SetActive(true);
        StartCoroutine(SlideAndBounce(openPosition, true));
        isOpen = true;
    }

    private void CloseMenu()
    {
        if (menuPanel == null)
        {
            Debug.LogError("NavigationMenuController: Cannot close menu, menuPanel is not assigned!");
            return;
        }
        StartCoroutine(SlideAndBounce(closedPosition, false));
        isOpen = false;
    }

    private IEnumerator SlideAndBounce(Vector3 targetPos, bool opening)
    {
        Vector3 startPos = panelRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            t = Mathf.SmoothStep(0, 1, t); // Ease in out
            panelRect.anchoredPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        panelRect.anchoredPosition = targetPos;

        if (opening)
        {
            // Bounce effect
            for (int i = 0; i < bounceCount; i++)
            {
                yield return StartCoroutine(Bounce());
            }
        }
        else
        {
            menuPanel.SetActive(false);
        }
    }

    private IEnumerator Bounce()
    {
        Vector3 originalPos = panelRect.anchoredPosition;
        Vector3 bouncePos = originalPos + Vector3.down * bounceAmount;

        // Down
        yield return MoveTo(bouncePos, 0.1f);
        // Up
        yield return MoveTo(originalPos, 0.1f);
    }

    private IEnumerator MoveTo(Vector3 target, float duration)
    {
        Vector3 start = panelRect.anchoredPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            panelRect.anchoredPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }
        panelRect.anchoredPosition = target;
    }
}