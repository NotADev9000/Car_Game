using UnityEngine;

public abstract class MenuManager : MonoBehaviour
{
    [SerializeField] protected GameObject _menuOverlay;
    [SerializeField][Tooltip("UI Elements that should be hidden in WebGL builds")] private GameObject[] _NonWebGlUIElements;

    protected virtual void Start()
    {
        HideElementsFromWebGL();
    }

    private void HideElementsFromWebGL()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            for (int i = 0; i < _NonWebGlUIElements.Length; i++)
            {
                _NonWebGlUIElements[i].SetActive(false);
            }
        }
    }

    protected void ChangeOverlayUI(bool show)
    {
        if (_menuOverlay != null)
        {
            _menuOverlay.SetActive(show);
        }
    }

    public void OnQuitButtonPressedUI()
    {
        Application.Quit();
        Debug.Log("Quit Pressed");
    }
}
