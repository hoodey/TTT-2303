using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerOption
{
    NONE, //0
    X, // 1
    O // 2
}

public class TTT : MonoBehaviour
{
    public int Rows;
    public int Columns;
    [SerializeField] BoardView board;
    [SerializeField] TMP_Text text;
    [SerializeField] Button switchButton;

    PlayerOption currentPlayer = PlayerOption.X;
    Cell[,] cells;

    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[Columns, Rows];

        board.InitializeBoard(Columns, Rows);

        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {
                cells[j, i] = new Cell();
                cells[j, i].current = PlayerOption.NONE;
            }
        }
    }

    public void MakeOptimalMove()
    {
        #region First Moves

        //If going first put X in corner
        for (int i =0; i < Rows; i++)
        {
            for (int j= 0; j < Columns;j++)
            {
                if (cells[j, i].current == PlayerOption.NONE)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            if (cells[0,0].current == PlayerOption.NONE)
            {
                ChooseSpace(0, 0);
                return;
            }
        }

        //If X goes middle
        if (cells[1, 1].current == PlayerOption.X)
        {
            if (cells[0, 0].current == PlayerOption.NONE)
            {
                ChooseSpace(0, 0);
                return;
            }
        }

        //If X goes corner
        if (cells[0, 0].current == PlayerOption.X || cells[2,0].current == PlayerOption.X || cells[0,2].current == PlayerOption.X || cells[2,2].current == PlayerOption.X)
        {
            if (cells[1, 1].current == PlayerOption.NONE)
            {
                ChooseSpace(1, 1);
                return;
            }
        }

        //If X goes side/top
        if (cells[0,1].current == PlayerOption.X || cells[1,0].current == PlayerOption.X)
        {
            if (cells[0, 0].current == PlayerOption.NONE)
            {
                ChooseSpace(0, 0);
                return;
            }
        }
        else if (cells[1,2].current == PlayerOption.X || cells[2,1].current == PlayerOption.X)
        {
            if (cells[2, 2].current == PlayerOption.NONE)
            {
                ChooseSpace(2, 2);
                return;
            }
        }

        #endregion

        PlayWinningCell();

    }

    public void ChooseSpace(int column, int row)
    {
        // can't choose space if game is over
        if (GetWinner() != PlayerOption.NONE)
            return;

        // can't choose a space that's already taken
        if (cells[column, row].current != PlayerOption.NONE)
            return;

        // set the cell to the player's mark
        cells[column, row].current = currentPlayer;

        // update the visual to display X or O
        board.UpdateCellVisual(column, row, currentPlayer);

        // if there's no winner, keep playing, otherwise end the game
        if(GetWinner() == PlayerOption.NONE)
            EndTurn();
        else
        {
            Debug.Log("GAME OVER!");
        }
    }

    public void EndTurn()
    {
        // increment player, if it goes over player 2, loop back to player 1
        currentPlayer += 1;
        if ((int)currentPlayer > 2)
            currentPlayer = PlayerOption.X;
        UpdatePlayerTurnText();
    }

    public void UpdatePlayerTurnText()
    {
        text.text = "Player's Turn: " + currentPlayer;
    }

    public void PlayWinningCell()
    {
        int sum = 0;
        int num1 = 0;
        int num2 = 0;
        
        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;
                else if (cells[j,i].current == PlayerOption.NONE)
                {
                    num1 = j;
                    num2 = i;
                }
                    

                sum += value;
            }

            if (sum == 2)
            {
                ChooseSpace(num1, num2);
            }
            else if (sum == -2)
                ChooseSpace(num1, num2);

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;
                else if (cells[j, i].current == PlayerOption.NONE)
                {
                    num1 = j;
                    num2 = i;
                }

                sum += value;
            }

            if (sum == 2)
                ChooseSpace(num1, num2);
            else if (sum == -2)
                ChooseSpace(num1, num2);

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;
            else if (cells[i, i].current == PlayerOption.NONE)
            {
                num1 = i;
                num2 = i;
            }

            sum += value;
        }

        if (sum == 2)
            ChooseSpace(num1, num2);
        else if (sum == -2)
            ChooseSpace(num1, num2);

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.NONE)
            {
                num1 = Columns - 1 - i;
                num2 = i;
            }

            sum += value;
        }

        if (sum == 2)
            ChooseSpace(num1, num2);
        else if (sum == -3)
            ChooseSpace(num1, num2);

        Debug.Log("We found no winning squares.");
    }

    public PlayerOption GetWinner()
    {
        // sum each row/column based on what's in each cell X = 1, O = -1, blank = 0
        // we have a winner if the sum = 3 (X) or -3 (O)
        int sum = 0;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for(int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        return PlayerOption.NONE;
    }
}
