using UnityEngine;

public enum BattleState { PlayerDraw, PlayerAction, MonsterAction, ResolveEnd }
public class BattleStateMachine
{
    public BattleState State { get; private set; } = BattleState.PlayerDraw;
    private readonly BattleController _controller;

    public BattleStateMachine(BattleController controller) { _controller = controller; }

    public void Tick()
    {
        switch (State)
        {
            case BattleState.PlayerDraw:
                _controller.DrawAtStartOfPlayerTurn(3);
                _controller.TickStatusesPlayerStart();
                State = BattleState.PlayerAction;
                break;

            case BattleState.PlayerAction:
                if (_controller.PlayerHasEndedTurn)
                    State = BattleState.MonsterAction;
                break;

            case BattleState.MonsterAction:
                // La corrutina ya corre; aquí basta con esperar a que termine un frame
                _controller.MonsterAct();
                State = BattleState.ResolveEnd;
                break;

            case BattleState.ResolveEnd:
                if (_controller.IsBattleOver()) _controller.EndBattle();
                else State = BattleState.PlayerDraw;
                break;
        }
    }
}
