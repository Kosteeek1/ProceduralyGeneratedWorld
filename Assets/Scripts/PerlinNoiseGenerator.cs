using UnityEngine;

public class PerlinNoiseGenerator
{
    public static float[,] Generate(int width, int height, float scale, Wave[] waves, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];

        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                //sample positions calculation
                float samplePositionX = (float)x * scale + offset.x;
                float sapmplePositionY = (float)y * scale + offset.y;

                float normalization = 0.0f;

                foreach (Wave wave in waves)
                {
                    noiseMap[x, y] += wave.amplitude * Mathf.PerlinNoise(samplePositionX * wave.frequency + wave.seed,
                        sapmplePositionY * wave.frequency + wave.seed);
                    normalization += wave.amplitude;
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class Wave
{
    public float seed; //amount of the noise offsetting
    public float frequency; //scale of the noisemap
    public float amplitude; //size and intensity
}