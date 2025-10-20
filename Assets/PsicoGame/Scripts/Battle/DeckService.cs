using System.Collections.Generic;
using UnityEngine;

public class DeckService
{
    private readonly List<CardSO> _drawPile = new();
    private readonly RngService _rng;

    public DeckService(IEnumerable<CardSO> deck, RngService rng)
    {
        _drawPile.AddRange(deck);
        _rng = rng;
        Shuffle();
    }

    public void Shuffle()
    {
        for (int i = _drawPile.Count - 1; i > 0; i--)
        {
            int j = _rng.Range(0, i + 1);
            (_drawPile[i], _drawPile[j]) = (_drawPile[j], _drawPile[i]);
        }
    }

    public CardSO Draw()
    {
        if (_drawPile.Count == 0) return null;
        var c = _drawPile[^1];
        _drawPile.RemoveAt(_drawPile.Count - 1);
        return c;
    }

    public int Count => _drawPile.Count;
}

public class HandService
{
    public readonly List<CardSO> Hand = new();
    public void Add(CardSO c) { if (c != null) Hand.Add(c); }
    public void Remove(CardSO c) { Hand.Remove(c); }
}

public class DiscardService
{
    public readonly List<CardSO> Discard = new();
    public void Add(CardSO c) { if (c != null) Discard.Add(c); }
}
