using UnityEngine;

[CreateAssetMenu(fileName = "BiomePreset", menuName = "New Biome Preset")]
public class BiomePreset : ScriptableObject
{
    public Sprite[] Tiles;
    public float MinimalHeight;
    public float MinimalMoisture;
    public float MinimalHeat;

    public Sprite GetSprite()
    {
        return Tiles[Random.Range(0, Tiles.Length)];
    }

    public bool CheckIfBiomeConditionsMet(float height, float moisture, float heat)
    {
        return height >= MinimalHeight && moisture >= MinimalMoisture && heat >= MinimalHeat;
    }
}