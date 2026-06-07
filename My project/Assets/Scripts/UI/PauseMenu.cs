using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;

    private void Awake()
    {
        // 1. Kiểm tra nếu người dùng đã gán trong Inspector chưa
        if (pauseMenuUI == null)
        {
            // 2. Thử tìm kiếm Panel có tên "PausePanel" trong các con của Canvas
            Transform existingPanel = transform.Find("PausePanel");
            if (existingPanel != null)
            {
                pauseMenuUI = existingPanel.gameObject;
                Debug.Log("PauseMenu: Đã tìm thấy PausePanel có sẵn trong Canvas.");
            }
            else
            {
                // 3. Nếu chưa có, tự động tạo một giao diện PausePanel mặc định tại runtime
                Debug.Log("PauseMenu: Không tìm thấy PausePanel được gán hoặc có sẵn. Tiến hành tự động tạo tại runtime...");
                CreateDefaultPauseMenuUI();
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Nếu tải scene Menu chính, tự hủy Canvas chơi game và các đối tượng chơi game khác để giải phóng bộ nhớ và reset trạng thái
        if (scene.name == "Menu")
        {
            Debug.Log("PauseMenu: Đã phát hiện scene Menu chính. Giải phóng các đối tượng gameplay cũ...");
            
            if (PlayerController.Instance != null) Destroy(PlayerController.Instance.gameObject);
            if (CameraController.Instance != null) Destroy(CameraController.Instance.gameObject);
            if (SceneManagement.Instance != null) Destroy(SceneManagement.Instance.gameObject);
            if (ActiveInventory.Instance != null) Destroy(ActiveInventory.Instance.gameObject);
            if (EnemyManager.Instance != null) Destroy(EnemyManager.Instance.gameObject);

            Destroy(gameObject); // Hủy UICanvas này
        }
    }

    private void Start()
    {
        Debug.Log("PauseMenu: Script đang chạy trên đối tượng: " + gameObject.name);
        if (pauseMenuUI != null)
        {
            SetupButtonListeners(pauseMenuUI.transform);
            pauseMenuUI.SetActive(false);
        }
        GameIsPaused = false;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        bool escapePressed = false;

        // 1. Thử kiểm tra bằng hệ thống Input System mới
        try
        {
            if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                escapePressed = true;
            }
        }
        catch
        {
            // Bỏ qua lỗi nếu không tìm thấy Input System mới
        }

        // 2. Thử kiểm tra bằng Input cũ (legacy)
        if (!escapePressed)
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    escapePressed = true;
                }
            }
            catch
            {
                // Bỏ qua lỗi nếu Input cũ bị vô hiệu hóa trong Player Settings (chỉ dùng New Input System)
            }
        }

        if (escapePressed)
        {
            Debug.Log("PauseMenu: Đã bấm phím Escape! Trạng thái GameIsPaused hiện tại: " + GameIsPaused);
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Debug.Log("PauseMenu: Tiếp tục game (Resume)");
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("PauseMenu: pauseMenuUI chưa được gán hoặc khởi tạo!");
        }
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        Debug.Log("PauseMenu: Tạm dừng game (Pause)");
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("PauseMenu: pauseMenuUI chưa được gán hoặc khởi tạo!");
        }
        Time.timeScale = 0f;
        GameIsPaused = true;
        
        // Đảm bảo con trỏ chuột hiển thị để tương tác với các nút
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;

        // Ẩn panel tạm dừng ngay lập tức
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        // Hủy các đối tượng gameplay cũ để tránh lỗi nhân đôi và giữ lại trạng thái cũ khi load lại cảnh
        if (PlayerController.Instance != null) Destroy(PlayerController.Instance.gameObject);
        if (CameraController.Instance != null) Destroy(CameraController.Instance.gameObject);
        if (SceneManagement.Instance != null) Destroy(SceneManagement.Instance.gameObject);
        if (ActiveInventory.Instance != null) Destroy(ActiveInventory.Instance.gameObject);
        if (EnemyManager.Instance != null) Destroy(EnemyManager.Instance.gameObject);

        // Hủy luôn UICanvas cũ, scene mới sẽ tự sinh ra UICanvas sạch sẽ từ scene file
        Destroy(gameObject);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    private void CreateDefaultPauseMenuUI()
    {
        // 1. Tạo GameObject chính cho PausePanel
        GameObject panelGo = new GameObject("PausePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image));
        panelGo.transform.SetParent(this.transform, false);
        
        RectTransform panelRect = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Đặt màu nền mờ tối (đen trong suốt)
        UnityEngine.UI.Image panelImage = panelGo.GetComponent<UnityEngine.UI.Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.75f);
        
        // 2. Thử tìm font asset hiện có trong Canvas để đồng bộ font chữ của game
        TMPro.TMP_FontAsset customFont = null;
        TMPro.TextMeshProUGUI[] allTexts = transform.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
        foreach (var txt in allTexts)
        {
            if (txt.font != null)
            {
                customFont = txt.font;
                break;
            }
        }

        // 3. Tạo Tiêu đề "TẠM DỪNG"
        GameObject titleGo = new GameObject("PauseTitle", typeof(RectTransform), typeof(CanvasRenderer), typeof(TMPro.TextMeshProUGUI));
        titleGo.transform.SetParent(panelGo.transform, false);
        
        RectTransform titleRect = titleGo.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.8f);
        titleRect.anchoredPosition = new Vector2(0f, 0f);
        titleRect.sizeDelta = new Vector2(600f, 100f);
        
        TMPro.TextMeshProUGUI titleText = titleGo.GetComponent<TMPro.TextMeshProUGUI>();
        if (customFont != null) titleText.font = customFont;
        titleText.text = "TẠM DỪNG";
        titleText.fontSize = 64;
        titleText.fontStyle = TMPro.FontStyles.Bold;
        titleText.color = Color.white;
        titleText.alignment = TMPro.TextAlignmentOptions.Center;
        
        // 4. Tạo Container chứa các Nút bấm
        GameObject containerGo = new GameObject("ButtonsContainer", typeof(RectTransform), typeof(UnityEngine.UI.VerticalLayoutGroup));
        containerGo.transform.SetParent(panelGo.transform, false);
        
        RectTransform containerRect = containerGo.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0.4f);
        containerRect.anchorMax = new Vector2(0.5f, 0.4f);
        containerRect.anchoredPosition = new Vector2(0f, -30f);
        containerRect.sizeDelta = new Vector2(350f, 320f);
        
        UnityEngine.UI.VerticalLayoutGroup layout = containerGo.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        layout.spacing = 15f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlHeight = false;
        layout.childControlWidth = false;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = false;
        
        // 5. Tạo các nút điều hướng
        CreateMenuButton(containerGo.transform, "ResumeButton", "Tiếp Tục", Resume, customFont);
        CreateMenuButton(containerGo.transform, "RestartButton", "Chơi Lại", Restart, customFont);
        CreateMenuButton(containerGo.transform, "MainMenuButton", "Menu Chính", LoadMainMenu, customFont);
        CreateMenuButton(containerGo.transform, "QuitButton", "Thoát Game", QuitGame, customFont);
        
        pauseMenuUI = panelGo;
    }

    private void CreateMenuButton(Transform parent, string name, string textLabel, UnityEngine.Events.UnityAction onClickAction, TMPro.TMP_FontAsset font)
    {
        // Tạo Button GameObject
        GameObject btnGo = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Image), typeof(UnityEngine.UI.Button));
        btnGo.transform.SetParent(parent, false);
        
        RectTransform btnRect = btnGo.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(300f, 55f);
        
        UnityEngine.UI.Image btnImage = btnGo.GetComponent<UnityEngine.UI.Image>();
        btnImage.color = new Color(0.18f, 0.18f, 0.18f, 0.95f);
        
        // Tạo Text bên trong nút
        GameObject textGo = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TMPro.TextMeshProUGUI));
        textGo.transform.SetParent(btnGo.transform, false);
        
        RectTransform textRect = textGo.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMPro.TextMeshProUGUI btnText = textGo.GetComponent<TMPro.TextMeshProUGUI>();
        if (font != null) btnText.font = font;
        btnText.text = textLabel;
        btnText.fontSize = 24;
        btnText.fontStyle = TMPro.FontStyles.Bold;
        btnText.color = Color.white;
        btnText.alignment = TMPro.TextAlignmentOptions.Center;
        
        // Gán hành động khi click nút
        UnityEngine.UI.Button btn = btnGo.GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(onClickAction);
        
        // Cấu hình đổi màu khi rê chuột (Hover) và nhấn (Pressed)
        btn.transition = UnityEngine.UI.Selectable.Transition.ColorTint;
        UnityEngine.UI.ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        colors.highlightedColor = new Color(0.35f, 0.35f, 0.35f, 1f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        colors.selectedColor = new Color(0.35f, 0.35f, 0.35f, 1f);
        btn.colors = colors;
    }

    private void SetupButtonListeners(Transform panel)
    {
        UnityEngine.UI.Button[] buttons = panel.GetComponentsInChildren<UnityEngine.UI.Button>(true);
        foreach (UnityEngine.UI.Button btn in buttons)
        {
            string nameLower = btn.gameObject.name.ToLower();
            
            // Tìm chữ hiển thị trên nút (để hỗ trợ nếu tên GameObject không trùng khớp)
            string textContent = "";
            TMPro.TextMeshProUGUI tmpText = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            if (tmpText != null)
            {
                textContent = tmpText.text.ToLower();
            }
            else
            {
                UnityEngine.UI.Text legacyText = btn.GetComponentInChildren<UnityEngine.UI.Text>(true);
                if (legacyText != null)
                {
                    textContent = legacyText.text.ToLower();
                }
            }

            // Kết nối sự kiện tương ứng
            if (nameLower.Contains("resume") || nameLower.Contains("tiep") || nameLower.Contains("tiếp") ||
                textContent.Contains("resume") || textContent.Contains("tiếp") || textContent.Contains("tiep") || textContent.Contains("tục") || textContent.Contains("tuc"))
            {
                btn.onClick.RemoveListener(Resume);
                btn.onClick.AddListener(Resume);
                Debug.Log("PauseMenu: Đã kết nối tự động nút Resume: " + btn.gameObject.name);
            }
            else if (nameLower.Contains("restart") || nameLower.Contains("choi") || nameLower.Contains("chơi") || nameLower.Contains("lai") || nameLower.Contains("lại") ||
                     textContent.Contains("restart") || textContent.Contains("chơi") || textContent.Contains("choi") || textContent.Contains("lại") || textContent.Contains("lai") || textContent.Contains("reset"))
            {
                btn.onClick.RemoveListener(Restart);
                btn.onClick.AddListener(Restart);
                Debug.Log("PauseMenu: Đã kết nối tự động nút Restart: " + btn.gameObject.name);
            }
            else if (nameLower.Contains("menu") || nameLower.Contains("main") || nameLower.Contains("chinh") || nameLower.Contains("chính") ||
                     textContent.Contains("menu") || textContent.Contains("chính") || textContent.Contains("chinh") || textContent.Contains("trang chu") || textContent.Contains("trang chủ"))
            {
                btn.onClick.RemoveListener(LoadMainMenu);
                btn.onClick.AddListener(LoadMainMenu);
                Debug.Log("PauseMenu: Đã kết nối tự động nút MainMenu: " + btn.gameObject.name);
            }
            else if (nameLower.Contains("quit") || nameLower.Contains("exit") || nameLower.Contains("thoat") || nameLower.Contains("thoát") ||
                     textContent.Contains("quit") || textContent.Contains("thoát") || textContent.Contains("thoat") || textContent.Contains("exit") || textContent.Contains("out"))
            {
                btn.onClick.RemoveListener(QuitGame);
                btn.onClick.AddListener(QuitGame);
                Debug.Log("PauseMenu: Đã kết nối tự động nút QuitGame: " + btn.gameObject.name);
            }
        }
    }
}
