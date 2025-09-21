using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class MenuFasesCreator
{
    [MenuItem("Tools/Criar MenuFases")]
    public static void CriarCenaMenuFases()
    {
        // Cria nova cena
        var cena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Canvas
        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // EventSystem
        GameObject eventSystem = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem), typeof(UnityEngine.EventSystems.StandaloneInputModule));

        // Fundo (Panel)
        GameObject panel = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvasGO.transform, false);
        Image panelImage = panel.GetComponent<Image>();
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Menu ilustração.png");
        if (bgSprite != null)
        {
            panelImage.sprite = bgSprite;
            panelImage.preserveAspect = true;
        }
        RectTransform panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;

        // Função auxiliar para criar botões
        GameObject CriarBotao(string nome, string spritePath, Vector2 anchoredPos)
        {
            GameObject botaoGO = new GameObject(nome, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            botaoGO.transform.SetParent(canvasGO.transform, false);

            RectTransform rt = botaoGO.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 80);
            rt.anchoredPosition = anchoredPos;

            Image img = botaoGO.GetComponent<Image>();
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sp != null)
            {
                img.sprite = sp;
                img.preserveAspect = true;
            }

            return botaoGO;
        }

        // Cria os botões
        GameObject fase1Btn = CriarBotao("fase1Button", "Assets/fase 1.png", new Vector2(-300, 100));
        GameObject fase2Btn = CriarBotao("fase2Button", "Assets/fase 2.png", new Vector2(0, 100));
        GameObject fase3Btn = CriarBotao("fase3Button", "Assets/fase 3.png", new Vector2(300, 100));
        GameObject creditosBtn = CriarBotao("creditosButton", "Assets/creditos.png", new Vector2(0, -100));

        // GameController
        GameObject controller = new GameObject("GameController");
        var script = controller.AddComponent<MenuFases>();

        // Liga os botões ao script
        script.fase1Button = fase1Btn.GetComponent<Button>();
        script.fase2Button = fase2Btn.GetComponent<Button>();
        script.fase3Button = fase3Btn.GetComponent<Button>();
        script.creditosButton = creditosBtn.GetComponent<Button>();

        // Cria pasta Scenes se não existir
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        // Salva cena
        string scenePath = "Assets/Scenes/MenuFases.unity";
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);

        // Adiciona aos Build Settings
        var scenes = EditorBuildSettings.scenes;
        var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
        for (int i = 0; i < scenes.Length; i++) newScenes[i] = scenes[i];
        newScenes[scenes.Length] = new EditorBuildSettingsScene(scenePath, true);
        EditorBuildSettings.scenes = newScenes;

        Debug.Log("Cena MenuFases criada em: " + scenePath);
    }
}
