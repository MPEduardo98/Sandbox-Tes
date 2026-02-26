using UnityEngine;

/// <summary>
/// Genera una textura de cuadrícula por código y se la asigna al material del terreno.
/// Así puedes ver claramente que te estás moviendo.
/// Pon este script en el GameObject ChunkManager.
/// </summary>
public class GridTextureGenerator : MonoBehaviour
{
    [Header("Configuración del Grid")]
    [Tooltip("Tamaño de la textura en píxeles (potencia de 2: 64, 128, 256)")]
    [SerializeField] private int textureSize = 256;

    [Tooltip("Grosor de las líneas del grid en píxeles")]
    [SerializeField] private int lineThickness = 4;

    [Tooltip("Color del suelo")]
    [SerializeField] private Color groundColor = new Color(0.45f, 0.65f, 0.35f); // verde pasto

    [Tooltip("Color de las líneas")]
    [SerializeField] private Color lineColor = new Color(0.25f, 0.45f, 0.20f); // verde oscuro

    [Header("Referencia")]
    [Tooltip("Arrastra aquí el TerrainMaterial")]
    [SerializeField] private Material terrainMaterial;

    void Awake()
    {
        // Generamos la textura al iniciar y se la asignamos al material
        Texture2D gridTexture = GenerateGridTexture();
        terrainMaterial.mainTexture = gridTexture;

        // Hacemos que la textura se repita (tile) en el terreno
        // Sin esto, la textura se estiraría en todo el chunk
        terrainMaterial.mainTextureScale = new Vector2(1, 1);
    }

    /// <summary>
    /// Crea una textura cuadrada con líneas de grid pintadas píxel por píxel.
    /// </summary>
    private Texture2D GenerateGridTexture()
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        texture.filterMode = FilterMode.Point; // Sin blur, líneas nítidas

        Color[] pixels = new Color[textureSize * textureSize];

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                // Si el píxel está cerca del borde de la textura, es una línea
                bool isLine = x < lineThickness || y < lineThickness;

                pixels[y * textureSize + x] = isLine ? lineColor : groundColor;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}