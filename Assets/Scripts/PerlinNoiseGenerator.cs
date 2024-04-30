using System;
using System.Linq;
using UnityEngine;

public static class PerlinNoiseGenerator
{
    public static float[,] Generate(int width, int height, float scale, Wave[] waves, Vector2 offset)
    {
        var noiseMap = new float[width, height];
        var normalization = waves.Sum(wave => wave.Amplitude);

        for (var x = 0; x < width; ++x)
        {
            for (var y = 0; y < height; ++y)
            {
                //sample positions calculation
                var samplePositionX = x * scale + offset.x;
                var samplePositionY = y * scale + offset.y;

                noiseMap[x, y] = 0.0f;

                foreach (var wave in waves)
                {
                    noiseMap[x, y] += wave.Amplitude * Mathf.PerlinNoise(samplePositionX * wave.Frequency + wave.Seed,
                        samplePositionY * wave.Frequency + wave.Seed);
                }

                noiseMap[x, y] /= normalization;
            }
        }

        return noiseMap;
    }
}

[Serializable]
public class Wave
{
    public float Seed; //amount of the noise offsetting
    public float Frequency; //scale of the noisemap
    public float Amplitude; //size and intensity
}