
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Normal {

    float mean;
    float std;

    public Normal(float mean, float std)
    {
        this.mean = mean;
        this.std = std;
    }

    public static float NextGaussian() {
        float v1, v2, s;
        do {
            v1 = 2.0f * Random.Range(0f,1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f,1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);

        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
    
        return v1 * s;
    }

    public static float NextGaussian(float mean, float standard_deviation)
    {
        return mean + NextGaussian() * standard_deviation;
    }

    public static float NextGaussian (float mean, float standard_deviation, float min, float max) 
    {
        float x;
        do {
            x = NextGaussian(mean, standard_deviation);
        } while (x < min || x > max);
        return x;
    }    

    public float Sample() 
    {
        return NextGaussian(mean, std);
    }

}