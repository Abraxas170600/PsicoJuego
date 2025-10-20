using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour
{
    [Header("Config inicial")]
    public List<CardSO> StartingDeck;
    public CombatSettingsSO Settings;
    public bool InputLocked { get; private set; }
    private bool _cardPlayedThisTurn;
    public MonsterCombatant Monster;

    [SerializeField] private int handSizeCap = 3;
    [SerializeField] private Sprite monsterPortrait;
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] private CombatantPanelView playerPanel;
    [SerializeField] private CombatantPanelView monsterPanel;

    // Runtime
    public PlayerCombatant Player { get; private set; }
    public DeckService Deck { get; private set; }
    public HandService Hand { get; private set; }
    public DiscardService Discard { get; private set; }
    public BattleStateMachine FSM { get; private set; }
    public EventBus Events { get; private set; }
    public bool PlayerHasEndedTurn { get; private set; }

    private RngService _rng;

    void Awake()
    {
        _rng = new RngService(seed: System.Environment.TickCount);
        Events = new EventBus();
        Player = new PlayerCombatant
        {
            Name = "John",
            Portrait = playerPortrait,
            Stats = new Stats { MaxHP = 50, HP = 50, Attack = 0, Defense = 0 }
            
        };
        Monster = new MonsterCombatant
        {
            Name = "Ira",
            Portrait = monsterPortrait,
            Stats = new Stats { MaxHP = 50, HP = 50, Attack = 0, Defense = 0 }
        };

        Hand = new HandService();
        Discard = new DiscardService();
        Deck = new DeckService(StartingDeck, _rng);
        FSM = new BattleStateMachine(this);

        // UI puede suscribirse a Events.*
        playerPanel.Bind(Player, Events);
        monsterPanel.Bind(Monster, Events);
    }

    void Update()
    {
        FSM.Tick();
    }

    public void DrawAtStartOfPlayerTurn(int count)
    {
        _cardPlayedThisTurn = false;
        InputLocked = false;
        PlayerHasEndedTurn = false;
        Player.ResetTurnTemps();

        DrawToHandCap();
        Events.Log($"Inicio turno: mano={Hand.Hand.Count}, mazo={Deck.Count}");
    }

    private void DrawToHandCap()
    {
        int need = Mathf.Clamp(handSizeCap - Hand.Hand.Count, 0, Deck.Count);
        for (int i = 0; i < need; i++)
        {
            var c = Deck.Draw();
            if (c != null) Hand.Add(c);
        }
        Events.EmitHandChanged();
    }

    public void PlayCard(CardSO card)
    {
        // Solo durante la fase de acción del jugador y si no jugó ya.
        if (_cardPlayedThisTurn || InputLocked) return;
        if (!Hand.Hand.Contains(card)) return;

        _cardPlayedThisTurn = true;
        InputLocked = true;                   // bloquea más clicks
        StartCoroutine(ResolvePlayerCardSequence(card));
    }

    private System.Collections.IEnumerator ResolvePlayerCardSequence(CardSO card)
    {
        // Señal: empieza anim carta / panel jugador
        Events.EmitPlayerCardPlayStarted(card);
        yield return new WaitForSeconds(Settings.playerCardWindup);

        // Resolver lógicamente la carta
        var ctx = new EffectContext { Player = Player, Monster = Monster, Card = card, Events = Events };
        if (HasStatus(Player, s => s.Definition.doubleNextCardNegative)) ctx.DoubleNextCardNegative = true;

        EffectPipeline.ResolveCard(ctx);

        Hand.Remove(card);
        Discard.Add(card);
        Events.EmitHandChanged();

        // Señal: termina la resolución (números en pantalla ya mostrados)
        Events.EmitPlayerCardResolved();
        yield return new WaitForSeconds(Settings.playerResolvePause);

        // Fin automático del turno del jugador
        TickStatusesPlayerEnd();
        yield return new WaitForSeconds(Settings.betweenPhases);

        // Pasa al turno del monstruo
        PlayerHasEndedTurn = true; // la FSM detecta y avanza
    }


    public void EndPlayerTurn()
    {
        PlayerHasEndedTurn = true;
    }

    // Estados: ticks y efectos de inicio/fin de turno del jugador
    public void TickStatusesPlayerStart() => StatusSystem.Tick(Player, StatusTiming.OnTurnStartPlayer, Events);
    public void TickStatusesPlayerEnd() => StatusSystem.Tick(Player, StatusTiming.OnTurnEndPlayer, Events);
    public void TickStatusesMonsterEnd() => StatusSystem.Tick(Monster, StatusTiming.OnTurnEndMonster, Events);

    public void MonsterAct()
    {
        StartCoroutine(MonsterActSequence());
    }

    private System.Collections.IEnumerator MonsterActSequence()
    {
        Events.EmitMonsterAttackStarted();                       // anticipación/pose
        yield return new WaitForSeconds(Settings.monsterWindup);

        // Cálculo de daño
        int baseDamage = Monster.Stats.Attack + 6;               // o según nivel/tabla
        int before = Player.Stats.HP;
        Player.ApplyDamage(baseDamage);
        int dealt = before - Player.Stats.HP;

        Events.EmitDamage(Monster, Player, dealt);               // ya dispara OnHpChanged
        Events.EmitMonsterAttackHit(dealt);                      // numeritos/flashes
        yield return new WaitForSeconds(Settings.monsterHitPause);

        //var miedo = ScriptableObject.CreateInstance<StatusEffectSO>();
        //miedo.Id = "fear";
        //miedo.DisplayName = "Miedo";
        //miedo.Sign = StatusSign.Negative;
        //miedo.Timing = StatusTiming.OnTurnEndPlayer;
        //miedo.Stackable = false;
        //miedo.MaxStacks = 1;
        //miedo.flatAttackDelta = -2;
        //// Si en el futuro se añade flatDefenseDelta, asignarlo aquí.

        //Player.Statuses.Add(new RuntimeStatus(miedo, 2));
        //Events.EmitStatusApplied(Player, miedo, 2);
        //Events.EmitMonsterStatusShown(miedo.DisplayName, 2);

        yield return new WaitForSeconds(Settings.monsterStatusPause);

        TickStatusesMonsterEnd();
        yield return new WaitForSeconds(Settings.betweenPhases);
    }

    public bool IsBattleOver() => Player.Stats.HP <= 0 || Monster.Stats.HP <= 0;

    public void EndBattle()
    {
        Events.Log(Player.Stats.HP <= 0 ? "Derrota" : "Victoria");
        enabled = false; // o dispara evento de fin
    }

    private static bool HasStatus(Combatant c, System.Func<RuntimeStatus, bool> predicate)
    {
        foreach (var s in c.Statuses) if (predicate(s)) return true;
        return false;
    }
}
