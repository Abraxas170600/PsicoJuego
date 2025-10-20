using UnityEngine;

public class RngService
{
    private System.Random _rng;
    public RngService(int seed) { _rng = new System.Random(seed); }
    public int Range(int minInclusive, int maxExclusive) => _rng.Next(minInclusive, maxExclusive);
}
