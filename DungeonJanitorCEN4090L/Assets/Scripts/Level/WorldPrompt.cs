using UnityEngine;
using TMPro;

public class WorldPrompt : MonoBehaviour
{
    private TextMeshPro textMesh;

    void Awake()
    {
        textMesh = gameObject.AddComponent<TextMeshPro>();
        textMesh.fontSize = 4;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = Color.yellow;
    }

    public void SetText(string message)
    {
        if (textMesh != null)
            textMesh.text = message;
    }

    void LateUpdate()
    {
        // Force text to face camera
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
    }
}
