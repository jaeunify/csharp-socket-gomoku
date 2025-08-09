using System;
using System.Collections.Generic;
using GomokuPacket;

public class GameState : ReduxState
{
    public Rock[,] Board { get; private set; } = default!;
    public Rock MyRock { get; private set; } = Rock.Empty;
    public Rock OtherRock { get; set; } = Rock.Empty;
    private Rock Turn = Rock.Black; // 흑 선
    private (int X, int Y) LastActPosition;
    public event Action OnChange;
    public GameState()
    {
        Board = new Rock[GameOption.BoardSize, GameOption.BoardSize];
    }

    public (bool success, string? errorMsg) SetRock(bool isMyAction, int x, int y)
    {
        if (MyRock == Rock.Empty || OtherRock == Rock.Empty)
        {
            return (false, "Impossible situation. 아직 게임 시작 전입니다.");
        }

        var rock = isMyAction ? MyRock : OtherRock;

        if (Turn != rock)
        {
            return (false, $"Impossible situation. {Turn}의 차례인데, {rock}이 수를 놓았습니다...");
        }

        Board[y, x] = rock;

        LastActPosition = (x, y);

        ChangeTurn();

        OnChange?.Invoke();

        return (true, null);
    }

    public void SetMyRock(bool amIFirst)
    {
        if (amIFirst)
        {
            MyRock = Rock.Black;
            OtherRock = Rock.White;
        }
        else
        {
            MyRock = Rock.White;
            OtherRock = Rock.Black;
        }

        // ui 반영할 건 없는 것 같다.
    }

    public void HandleError()
    {
        ChangeTurn();
        Board[LastActPosition.Y, LastActPosition.X] = Rock.Empty;

        OnChange?.Invoke();
    }

    private void ChangeTurn()
    {
        if (Turn == Rock.White)
        {
            Turn = Rock.Black;
        }
        else if (Turn == Rock.Black)
        {
            Turn = Rock.White;
        }
    }
}

