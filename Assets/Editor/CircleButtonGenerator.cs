using UnityEngine;
using UnityEditor;
using System.IO;

public class CircleButtonGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Circle Button")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<CircleButtonGenerator>("Circle Button Generator");
    }
    
    private int size = 256;
    private int borderWidth = 4;
    private Color fillColor = Color.white;
    private Color borderColor = Color.black;
    
    private void OnGUI()
    {
        GUILayout.Label("원형 버튼 생성기", EditorStyles.boldLabel);
        
        size = EditorGUILayout.IntSlider("텍스처 크기", size, 64, 512);
        borderWidth = EditorGUILayout.IntSlider("테두리 두께", borderWidth, 1, 10);
        fillColor = EditorGUILayout.ColorField("내부 색상", fillColor);
        borderColor = EditorGUILayout.ColorField("테두리 색상", borderColor);
        
        if (GUILayout.Button("생성"))
        {
            GenerateCircleButton();
        }
    }
    
    private void GenerateCircleButton()
    {
        // 텍스처 생성
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        
        // 초기화 (투명)
        Color transparent = new Color(0, 0, 0, 0);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, transparent);
            }
        }
        
        // 원 그리기
        float radius = size / 2.0f - 2.0f;
        Vector2 center = new Vector2(size / 2.0f, size / 2.0f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    // 테두리 영역인지 확인
                    if (distance > radius - borderWidth)
                    {
                        texture.SetPixel(x, y, borderColor);
                    }
                    else
                    {
                        texture.SetPixel(x, y, fillColor);
                    }
                }
            }
        }
        
        texture.Apply();
        
        // 저장 경로 확인 및 폴더 생성
        string resourcesPath = "Assets/Resources";
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }
        
        // 저장
        string path = "Assets/Resources/CircleButton.png";
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        
        // 에셋 데이터베이스 갱신
        AssetDatabase.Refresh();
        
        // 텍스처 임포트 설정
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            
            // 에디터에서만 필요한 설정
            SerializedObject serializedImporter = new SerializedObject(importer);
            SerializedProperty textureSettingsProp = serializedImporter.FindProperty("m_TextureSettings");
            if (textureSettingsProp != null)
            {
                SerializedProperty filterModeProp = textureSettingsProp.FindPropertyRelative("m_FilterMode");
                if (filterModeProp != null)
                {
                    filterModeProp.intValue = (int)FilterMode.Bilinear;
                }
            }
            serializedImporter.ApplyModifiedProperties();
            
            // 설정 적용
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            
            // 확인 메시지
            Debug.Log($"원형 버튼이 생성되었습니다: {path}");
            EditorUtility.DisplayDialog("완료", "원형 버튼이 Resources 폴더에 생성되었습니다.", "확인");
        }
        else
        {
            Debug.LogError("텍스처 임포터를 가져오는데 실패했습니다.");
        }
    }
    
    [MenuItem("Tools/Generate All Color Buttons")]
    public static void GenerateAllColorButtons()
    {
        // 5개 색상 버튼 생성 (빨강, 초록, 파랑, 노랑, 보라)
        Color[] colors = new Color[] 
        {
            new Color(1, 0, 0, 1),     // 빨강
            new Color(0, 1, 0, 1),     // 초록
            new Color(0, 0, 1, 1),     // 파랑
            new Color(1, 1, 0, 1),     // 노랑
            new Color(1, 0, 1, 1)      // 보라
        };
        
        string[] colorNames = { "Red", "Green", "Blue", "Yellow", "Purple" };
        
        // Resources 폴더 확인 및 생성
        string resourcesPath = "Assets/Resources";
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }
        
        for (int i = 0; i < colors.Length; i++)
        {
            GenerateColoredButton(colorNames[i], colors[i], 256, 4, Color.black);
        }
        
        // 기본 흰색 버튼도 생성
        GenerateColoredButton("CircleButton", Color.white, 256, 4, Color.black);
        
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("완료", "모든 색상의 버튼이 Resources 폴더에 생성되었습니다.", "확인");
    }
    
    private static void GenerateColoredButton(string name, Color fillColor, int size = 256, int borderWidth = 4, Color borderColor = default)
    {
        if (borderColor == default)
        {
            borderColor = Color.black;
        }
        
        // 텍스처 생성
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        
        // 초기화 (투명)
        Color transparent = new Color(0, 0, 0, 0);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                texture.SetPixel(x, y, transparent);
            }
        }
        
        // 원 그리기
        float radius = size / 2.0f - 2.0f;
        Vector2 center = new Vector2(size / 2.0f, size / 2.0f);
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                if (distance <= radius)
                {
                    // 테두리 영역인지 확인
                    if (distance > radius - borderWidth)
                    {
                        texture.SetPixel(x, y, borderColor);
                    }
                    else
                    {
                        texture.SetPixel(x, y, fillColor);
                    }
                }
            }
        }
        
        texture.Apply();
        
        // 저장
        string path = $"Assets/Resources/{name}.png";
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
        
        // 텍스처 임포트 설정
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
            
            // 설정 적용
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            
            Debug.Log($"버튼 스프라이트 생성 완료: {path}");
        }
        else
        {
            Debug.LogError($"텍스처 임포터를 가져오는데 실패했습니다: {path}");
        }
    }
} 