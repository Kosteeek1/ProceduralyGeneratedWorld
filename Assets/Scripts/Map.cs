using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _mapTilesParent;
    [SerializeField] private BiomePreset[] _biomes;
    [SerializeField] private GameObject _tilePrefab;

    [Header("Dimensions")] [SerializeField]
    private int _width = 50;

    [SerializeField] private int _height = 50;
    [SerializeField] private float _scale = 1.0f;
    [SerializeField] private Vector2 _offset;

    [Header("Height Map")] [SerializeField]
    private Wave[] _heightWaves;

    private float[,] _heightMap;

    [Header("Moisture Map")] [SerializeField]
    private Wave[] _moistureWaves;

    private float[,] _moistureMap;

    [Header("Heat Map")] [SerializeField] private Wave[] _heatWaves;

    private float[,] _heatMap;

    private readonly List<GameObject> _mapTiles = new();

    private void Start()
    {
        GenerateMap();
    }

    [ContextMenu("DeleteMap")]
    private void DeleteMap()
    {
        foreach (var tile in _mapTiles)
        {
            Destroy(tile);
        }

        _mapTiles.Clear();
    }

    [ContextMenu("GenerateMap")]
    private void GenerateMap()
    {
        _heightMap = PerlinNoiseGenerator.Generate(_width, _height, _scale, _heightWaves, _offset);
        _moistureMap = PerlinNoiseGenerator.Generate(_width, _height, _scale, _moistureWaves, _offset);
        _heatMap = PerlinNoiseGenerator.Generate(_width, _height, _scale, _heatWaves, _offset);

        for (var x = 0; x < _width; ++x)
        {
            for (var y = 0; y < _height; ++y)
            {
                var tile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity, _mapTilesParent);
                _mapTiles.Add(tile);
                tile.GetComponent<SpriteRenderer>().sprite =
                    GetBiome(_heightMap[x, y], _moistureMap[x, y], _heatMap[x, y]).GetSprite();
            }
        }

        _mapTilesParent.position = new Vector3(-_width / 2f, -_height / 2f);
        _mainCamera.orthographicSize = _width / 2f;
    }

    private class BiomeTempData
    {
        public readonly BiomePreset Biome;

        public BiomeTempData(BiomePreset preset)
        {
            Biome = preset;
        }

        public float GetDiffValue(float height, float moisture, float heat)
        {
            return (height - Biome.MinimalHeight) + (moisture - Biome.MinimalMoisture) + (heat - Biome.MinimalHeat);
        }
    }

    BiomePreset GetBiome(float height, float moisture, float heat)
    {
        BiomePreset biomeToReturn = null;
        var biomeTemp = new List<BiomeTempData>();

        foreach (var biome in _biomes)
        {
            if (biome.CheckIfBiomeConditionsMet(height, moisture, heat))
            {
                biomeTemp.Add(new BiomeTempData(biome));
            }
        }

        var curVal = 0.0f;

        foreach (var biome in biomeTemp)
        {
            if (biomeToReturn == null)
            {
                biomeToReturn = biome.Biome;
                curVal = biome.GetDiffValue(height, moisture, heat);
            }
            else
            {
                if (!(biome.GetDiffValue(height, moisture, heat) < curVal)) continue;
                biomeToReturn = biome.Biome;
                curVal = biome.GetDiffValue(height, moisture, heat);
            }
        }

        if (biomeToReturn == null)
            biomeToReturn = _biomes[0];

        return biomeToReturn;
    }
}