using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Important class, it is in charge of interpreting the messages coming from the process. Also of maintaining a reference state of the cube(centers)
/// and creating a queue of the moves read by the cube
/// </summary>
public enum FACES
{
    R = 'R',
    L = 'L',
    U = 'U',
    D = 'D',
    F = 'F',
    B = 'B',
    M = 'M',
    E = 'E',
    S = 'S',
    Null
}

[DefaultExecutionOrder(3)]
public class CubeInputs : MonoBehaviour

{
    private CubeConectionManager _conection;
    private MovesQueue _moves;
    private List<char> _validationFaces; //list with the possible incomes that are meant to be moves
    public bool isActive = false;
    private Color topColor = Color.clear, FrontColor = Color.clear;

    private Color[,] centers = new Color[3, 4]
    {
        { Color.white, Color.clear, Color.clear, Color.clear },
        {
            Color.green, Color.red, Color.blue,
            new Color(1f, 0.59f, 0.18f)
        },
        { Color.yellow, Color.clear, Color.clear, Color.clear }
    };

    // Start is called before the first frame update
    private void Awake()
    {
        _validationFaces = new List<char>(new[] { 'R', 'L', 'U', 'D', 'F', 'B' });
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        _conection = FindObjectOfType<CubeConectionManager>();
        _moves = GetComponent<MovesQueue>();
        topColor = centers[0, 0];
        FrontColor = centers[1, 0];
    }

    public void ProcessMessages(MovesQueue movesQueue)
    {
        if (isActive && movesQueue.HasMessages())
        {
            Move move = movesQueue.Dequeue();
            //check if connection lost
            if (move.msg == "connection failed")
                _conection.ReEstablish();
            else if (move.msg == "Device not found")
                _conection.Refresh();
            else if (move.msg == "connection successful")
                _conection.Connected();
            else
            {
                //check movements
                if (move.msg != "" && ValidateMoves(move.msg))
                {
                    //double moves
                    move = DoubleMove(move);
                    move.color = GetColor(move.face);
                    _moves.Enqueue(move);
                    _moves.lastMove = move;
                }
            }
        }
    }

    private Color GetColor(FACES faces)
    {
        switch (faces)
        {
            case FACES.F:
                return centers[1, 0];
            case FACES.R:
                return centers[1, 1];
            case FACES.L:
                return centers[1, 3];
            case FACES.B:
                return centers[1, 2];
            case FACES.D:
                return centers[2, 0];
            case FACES.U:
                return centers[0, 0];
            default:
                return Color.clear;
        }
    }

    private Move ApplyOffset(Move move)
    {
        FACES face = move.face;
        if (topColor == Color.white)
        {
            if (FrontColor == Color.green) return move;
            if (FrontColor == Color.red)
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.F, FACES.B, FACES.U, FACES.D);
            }
            else if (FrontColor == Color.blue)
            {
                move.face = FaceSwap(face, FACES.B, FACES.F, FACES.L, FACES.R, FACES.U, FACES.D);
            }
            else
            {
                move.face = FaceSwap(face, FACES.R, FACES.L, FACES.B, FACES.F, FACES.U, FACES.D);
            }
        }
        else if (topColor == Color.yellow)
        {
            if (FrontColor == Color.green)
            {
                move.face = FaceSwap(face, FACES.F, FACES.B, FACES.L, FACES.R, FACES.D, FACES.U);
            }
            else if (FrontColor == Color.red)
            {
                move.face = FaceSwap(face, FACES.R, FACES.L, FACES.F, FACES.B, FACES.D, FACES.U);
            }
            else if (FrontColor == Color.blue)
            {
                move.face = FaceSwap(face, FACES.B, FACES.F, FACES.R, FACES.L, FACES.D, FACES.U);
            }
            else
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.B, FACES.F, FACES.D, FACES.U);
            }
        }
        else if (topColor == Color.green)
        {
            if (FrontColor == Color.white)
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.F, FACES.B, FACES.U, FACES.D);
            }
            else if (FrontColor == Color.red)
            {
                move.face = FaceSwap(face, FACES.U, FACES.D, FACES.F, FACES.B, FACES.R, FACES.L);
            }
            else if (FrontColor == Color.yellow)
            {
                move.face = FaceSwap(face, FACES.U, FACES.D, FACES.R, FACES.L, FACES.B, FACES.F);
            }
            else
            {
                move.face = FaceSwap(face, FACES.U, FACES.D, FACES.B, FACES.F, FACES.L, FACES.R);
            }
        }
        else if (topColor == Color.blue)
        {
            if (FrontColor == Color.white)
            {
                move.face = FaceSwap(face, FACES.D, FACES.U, FACES.R, FACES.L, FACES.F, FACES.B);
            }
            else if (FrontColor == Color.red)
            {
                move.face = FaceSwap(face, FACES.D, FACES.U, FACES.F, FACES.B, FACES.L, FACES.R);
            }
            else if (FrontColor == Color.yellow)
            {
                move.face = FaceSwap(face, FACES.D, FACES.U, FACES.L, FACES.R, FACES.B, FACES.F);
            }
            else
            {
                move.face = FaceSwap(face, FACES.D, FACES.U, FACES.B, FACES.F, FACES.R, FACES.L);
            }
        }
        else if (topColor == Color.red)
        {
            if (FrontColor == Color.white)
            {
                move.face = FaceSwap(face, FACES.R, FACES.L, FACES.U, FACES.D, FACES.F, FACES.B);
            }
            else if (FrontColor == Color.green)
            {
                move.face = FaceSwap(face, FACES.F, FACES.B, FACES.U, FACES.D, FACES.L, FACES.R);
            }
            else if (FrontColor == Color.yellow)
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.U, FACES.D, FACES.B, FACES.F);
            }
            else
            {
                move.face = FaceSwap(face, FACES.B, FACES.F, FACES.D, FACES.U, FACES.R, FACES.L);
            }
        }
        else
        {
            if (FrontColor == Color.white)
            {
                move.face = FaceSwap(face, FACES.L, FACES.R, FACES.D, FACES.U, FACES.F, FACES.B);
            }
            else if (FrontColor == Color.green)
            {
                move.face = FaceSwap(face, FACES.F, FACES.B, FACES.D, FACES.U, FACES.R, FACES.L);
            }
            else if (FrontColor == Color.yellow)
            {
                move.face = FaceSwap(face, FACES.R, FACES.L, FACES.D, FACES.U, FACES.B, FACES.F);
            }
            else
            {
                move.face = FaceSwap(face, FACES.B, FACES.F, FACES.D, FACES.U, FACES.L, FACES.R);
            }
        }

        return move;
    }

    private FACES FaceSwap(FACES face, FACES F, FACES B, FACES R, FACES L, FACES U, FACES D)
    {
        if (face == FACES.F) return F;
        if (face == FACES.B) return B;
        if (face == FACES.R) return R;
        if (face == FACES.L) return L;
        if (face == FACES.U) return U;
        if (face == FACES.D) return D;
        return FACES.Null;
    }

    private Move DoubleMove(Move move)
    {
        Move move1 = GetFace(move);
        if (_moves.lastMove == null) _moves.lastMove = move;
        Move move2 = _moves.lastMove;

        if (Math.Abs(move1.time.TotalMilliseconds - move2.time.TotalMilliseconds) < 500)
        {
            //could be a double move
            if (move1.IsMiddleLayer(move2))
            {
                switch (move1.face)
                {
                    case FACES.L or FACES.R:
                        if (move1.face is FACES.L)
                            move1.direction *= -1;
                        move1.face = FACES.M;
                        offsetCentersX(move1.direction);
                        break;
                    case FACES.U or FACES.D:
                        if (move1.face is FACES.U)
                            move1.direction *= -1;
                        move1.face = FACES.E;
                        offsetCentersY(move1.direction);
                        break;
                    case FACES.F or FACES.B:
                        if (move1.face is FACES.B)
                            move1.direction *= -1;
                        move1.face = FACES.S;
                        offsetCentersZ(move1.direction);
                        break;
                }
            }
        }

        return move1;
    }

    private void offsetCentersY(int direction)
    {
        Color[] aux = new Color[4]
            { centers[1, 0], centers[1, 1], centers[1, 2], centers[1, 3] };
        if (direction < 0)
        {
            centers[1, 0] = aux[3];
            centers[1, 1] = aux[0];
            centers[1, 2] = aux[1];
            centers[1, 3] = aux[2];
        }
        else
        {
            centers[1, 0] = aux[1];
            centers[1, 1] = aux[2];
            centers[1, 2] = aux[3];
            centers[1, 3] = aux[0];
        }

        UpdateColors();
    }

    private void offsetCentersX(int direction)
    {
        Color[] aux = new Color[4]
            { centers[0, 0], centers[1, 0], centers[2, 0], centers[1, 2] };

        if (direction > 0)
        {
            centers[0, 0] = aux[3];
            centers[1, 0] = aux[0];
            centers[2, 0] = aux[1];
            centers[1, 2] = aux[2];
        }
        else
        {
            centers[0, 0] = aux[1];
            centers[1, 0] = aux[2];
            centers[2, 0] = aux[3];
            centers[1, 2] = aux[0];
        }

        UpdateColors();
    }

    private void offsetCentersZ(int direction)
    {
        Color[] aux = new Color[4]
            { centers[0, 0], centers[1, 1], centers[2, 0], centers[1, 3] };
        if (direction > 0)
        {
            centers[0, 0] = aux[3];
            centers[1, 1] = aux[0];
            centers[2, 0] = aux[1];
            centers[1, 3] = aux[2];
        }
        else
        {
            centers[0, 0] = aux[1];
            centers[1, 1] = aux[2];
            centers[2, 0] = aux[3];
            centers[1, 3] = aux[0];
        }

        UpdateColors();
    }

    private void UpdateColors()
    {
        topColor = centers[0, 0];
        FrontColor = centers[1, 0];
    }

    private Move GetFace(Move move)
    {
        char[] chars = move.msg.ToCharArray();

        move.direction = chars.Length > 1 ? -1 : 1;

        switch (chars[0])
        {
            case 'R':
                move.face = FACES.R;
                break;
            case 'L':
                move.face = FACES.L;
                break;
            case 'F':
                move.face = FACES.F;
                break;
            case 'B':
                move.face = FACES.B;
                break;
            case 'U':
                move.face = FACES.U;
                break;
            case 'D':
                move.face = FACES.D;
                break;
        }

        return ApplyOffset(move);
    }

    private bool ValidateMoves(string move)
    {
        char[] chars = move.ToCharArray();
        return _validationFaces.Contains(chars[0]);
    }


    #region User methods

    public Color GetFrontColor()
    {
        return FrontColor;
    }

    public Color GetTopColor()
    {
        return topColor;
    }

    public void SetInputsActive(bool value)
    {
        isActive = value;
    }

    #endregion
}