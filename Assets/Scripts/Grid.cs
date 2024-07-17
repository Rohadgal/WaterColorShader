using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    public float _res = .5f;
    public float _width = 100, _height = 100;
    private int _columns, _rows;

    private float[,] _pointsArray;
    private Cell[,] cellsArray;

    private int numCircles = 6;

    private Circle[] _circles ;

    private Vector2 [,] pointPosArray;
    
    Circle circle = new Circle(new Vector2(25f, 25f), 5f);
    struct Cell
    {
        public Vector2 top;
        public Vector2 right;
        public Vector2 left;
        public Vector2 bottom;
    }

    struct Circle
    {
        public Vector2 centerPos { get; set;}
        public float radius { get; set; }
        public Vector2 vel { get; set; }

        public Circle(Vector2 t_center, float t_radius)
        {
            centerPos = t_center;
            radius = t_radius;
            vel = new Vector2(0, 0);
        }

        public void MoveDir(Vector2 t_vel)
        {
            vel = t_vel;
            centerPos += vel * Time.deltaTime;
        }
    }

    struct Line
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        
        public Line(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // create circles
        _circles = new Circle[numCircles];
        for (int i = 0; i < _circles.Length; i++)
        {
            Vector2 centerPos = new Vector2(Random.Range(5f, _width - 5f), Random.Range(5f, _height - 10f));
            _circles[i] = new Circle(centerPos, Random.Range(3, 10));
            _circles[i].MoveDir(new Vector2(Random.Range(-0.001f, 0.001f), Random.Range(-0.001f,0.001f)));
        }
        
        // create grid
        _columns = Mathf.CeilToInt(_width / _res) + 1;
        _rows = Mathf.CeilToInt(_height / _res) + 1;
        _pointsArray = new float[_columns, _rows];
        cellsArray = new Cell[_columns, _rows];
        pointPosArray = new Vector2[_columns, _rows];

        for (int i = 0; i < _columns; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                //_pointsArray[i, j] = Mathf.FloorToInt(Random.Range(0, 2));
                _pointsArray[i, j] = 0f;
                pointPosArray[i, j] = new Vector2(i * _res, j * _res);
                
                cellsArray[i, j].top = Vector2.zero;
                cellsArray[i, j].bottom = Vector2.zero;
                cellsArray[i, j].right = Vector2.zero;
                cellsArray[i, j].left = Vector2.zero;

            }
        }

        for (var i = 0; i < cellsArray.GetLength(0) - 1; i++)
        {
            for (var j = 0; j < cellsArray.GetLength(1) - 1; j++)
            {

                float x = i * _res;
                float y = j * _res;
                cellsArray[i, j].top = new Vector2(x + _res * 0.5f, y + _res);
                cellsArray[i, j].right = new Vector2(x + _res, y + _res * 0.5f);
                cellsArray[i, j].left = new Vector2(x, y + _res * 0.5f);
                cellsArray[i, j].bottom = new Vector2(x + _res * 0.5f, y);
            }
        }

    }

    private void Update()
    {
        for (var i = 0; i < _columns; i++)
        {
            for (var j = 0; j < _rows; j++)
            {

                float val = 0;
                for (int k = 0; k < _circles.Length; k++)
                {
                    val += Mathf.Pow(_circles[k].radius, 2) /
                           (Mathf.Pow(pointPosArray[i, j].x - _circles[k].centerPos.x, 2) +
                            Mathf.Pow(pointPosArray[i, j].y - _circles[k].centerPos.y, 2));

                    _circles[k].MoveDir(_circles[k].vel);

                    if (_circles[k].centerPos.x + _circles[k].radius >= _width ||
                        _circles[k].centerPos.x - _circles[k].radius <= 0)
                    {
                        _circles[k].MoveDir(new Vector2(-_circles[k].vel.x, _circles[k].vel.y));
                    }

                    if (_circles[k].centerPos.y + _circles[k].radius >= _height ||
                        _circles[k].centerPos.y - _circles[k].radius <= 0)
                    {
                        _circles[k].MoveDir(new Vector2(_circles[k].vel.x, -_circles[k].vel.y));
                    }

                }

                _pointsArray[i, j] = val;
            }
        }

        for (var i = 0; i < _columns-1; i++)
        {
            for (var j = 0; j < _rows-1; j++)
            {
                cellsArray[i,j].right.y = pointPosArray[i + 1,j + 1].y + (pointPosArray[i + 1, j].y - pointPosArray[i + 1, j + 1].y) * ((1 - _pointsArray[i + 1, j + 1]) / (_pointsArray[i + 1, j] - _pointsArray[i + 1, j + 1]));
                cellsArray[i,j].left.y = pointPosArray[i, j + 1].y + (pointPosArray[i, j].y - pointPosArray[i, j + 1].y) * ((1 - _pointsArray[i, j + 1]) / (_pointsArray[i, j] - _pointsArray[i, j + 1]));
                cellsArray[i,j].top.x = pointPosArray[i, j + 1].x + (pointPosArray[i + 1, j + 1].x - pointPosArray[i, j + 1].x) * ((1 - _pointsArray[i, j + 1]) / (_pointsArray[i + 1 , j + 1] - _pointsArray[i, j + 1]));
                cellsArray[i,j].bottom.x = pointPosArray[i, j].x + (pointPosArray[i + 1, j].x - pointPosArray[i, j].x) * ((1 - _pointsArray[i, j]) / (_pointsArray[i + 1, j] - _pointsArray[i, j]));
            }
        }

    }

    int GetState(float t_a, float t_b, float t_c, float t_d)
    {
        // int a = Mathf.FloorToInt(t_a);
        // int b = Mathf.FloorToInt(t_b);
        // int c = Mathf.FloorToInt(t_c);
        // int d = Mathf.FloorToInt(t_d);
        int a = 0;
        int b = 0;
        int c = 0;
        int d = 0;
        
        a = (t_a >= 1) ? 1 : 0;
        b = (t_b >= 1) ? 1 : 0;
        c = (t_c >= 1) ? 1 : 0;
        d = (t_d >= 1) ? 1 : 0;
        
        // int[] bits = new[]
        //     { Mathf.FloorToInt(t_d), Mathf.FloorToInt(t_c), Mathf.FloorToInt(t_b), Mathf.FloorToInt(t_a) };
        // int binaryCase = 0;
        // for (var i = 0; i < bits.Length; i++)
        // {
        //     bits[i] = (bits[i] < 1) ? 0 : 1;
        //     binaryCase = (binaryCase << 1) | bits[i];
        // }

        return a + b * 2 + c * 4 + d * 8;
    }

    // void DrawState(int t_value, Cell t_cell)
    // {
    //     //
    //     // Dictionary<int, Line> _drawResult = new Dictionary<int, Line>()
    //     // {
    //     //     { 0b0001, new Line (new Vector2(t_cell.left.x, t_cell.left.y), new Vector2(t_cell.bottom.x, t_cell.bottom.y)) },
    //     //     { 0b0010, new Line(new Vector2(t_cell.right.x, t_cell.right.y), new Vector2(t_cell.bottom.x, t_cell.bottom.y)) },
    //     //     { 0b0011, new Line(new Vector2(t_cell.left.x, t_cell.left.y), new Vector2(t_cell.right.x, t_cell.left.y)) },
    //     //     { 0b0100, new Line(new Vector2(t_cell.right.x, t_cell.right.y), new Vector2(t_cell.top.x, t_cell.top.y)) },
    //     //     
    //     //     { 0b0101, new Line (new Vector2(t_cell.left.x, t_cell.left.y), new Vector2(t_cell.bottom.x, t_cell.bottom.y)) },
    //     //     { 0b0110, new Line(new Vector2(t_cell.right.x, t_cell.right.y), new Vector2(t_cell.bottom.x, t_cell.bottom.y)) },
    //     //     { 0b0111, new Line(new Vector2(t_cell.left.x, t_cell.left.y), new Vector2(t_cell.right.x, t_cell.left.y)) },
    //     //     { 0b1000, new Line(new Vector2(t_cell.right.x, t_cell.right.y), new Vector2(t_cell.top.x, t_cell.top.y)) }
    //     // };
    //     //
    //     // if (_drawResult.TryGetValue(t_binary, out Line line))
    //     // {
    //     //     return line;
    //     // }
    //     // else
    //     // {
    //     //     throw new KeyNotFoundException($"Key {t_binary} not found in the dictionary.");
    //     // }
    // }

    void OnDrawGizmos()
    {
           
        for (var i = 0; i < _columns; i++)
        {
            for (var j = 0; j < _rows; j++)
            {
                Gizmos.color = Color.green;
                var pointPos = new Vector2(i * _res, j * _res);
                if (_pointsArray[i,j] < 1f)
                {
                    Gizmos.color = Color.gray;
                }
                if (_pointsArray[i,j] < 0.5f)
                {
                    Gizmos.color = Color.red;
                }
                if (_pointsArray[i,j] < 0.2f)
                {
                    Gizmos.color = Color.black;
                }
                if (_pointsArray[i,j] > 1.5f)
                {
                    Gizmos.color = Color.blue;
                }
                if (_pointsArray[i,j] > 2.5f)
                {
                    Gizmos.color = Color.magenta;
                }
                if (_pointsArray[i,j] > 4f)
                {
                    Gizmos.color = Color.yellow;
                }
                if (_pointsArray[i,j] > 10f)
                {
                    Gizmos.color = Color.white;
                }
                Gizmos.DrawSphere(pointPos, .5f);
            }
        }
        
        for (var i = 0; i < _columns-1; i++)
        {
            for (var j = 0; j < _rows - 1; j++)
            {
                Gizmos.color = Color.black;
                //Gizmos.DrawLine(cellsArray[i,j].top, cellsArray[i,j].left);
                int num = GetState(_pointsArray[i, j], _pointsArray[i + 1, j],_pointsArray[i + 1, j +1],_pointsArray[i, j + 1]);
                
                switch (num)
                {
                    case 1:
                        Line line = new Line(new Vector2(cellsArray[i,j].left.x, cellsArray[i,j].left.y),
                            new Vector2(cellsArray[i,j].bottom.x, cellsArray[i,j].bottom.y));
                        Gizmos.DrawLine(line.Start,line.End);
                        break;
                    case 2:
                        Line line2 = new Line(new Vector2(cellsArray[i,j].right.x, cellsArray[i,j].right.y),
                            new Vector2(cellsArray[i,j].bottom.x, cellsArray[i,j].bottom.y));
                        Gizmos.DrawLine(line2.Start,line2.End);
                        break;
                    case 3:
                        Line line3 = new Line(new Vector2(cellsArray[i,j].right.x, cellsArray[i,j].right.y),
                            new Vector2(cellsArray[i,j].left.x, cellsArray[i,j].left.y));
                        Gizmos.DrawLine(line3.Start,line3.End);
                        break;
                    case 4:
                        Line line4 = new Line(new Vector2(cellsArray[i,j].right.x, cellsArray[i,j].right.y),
                            new Vector2(cellsArray[i,j].top.x, cellsArray[i,j].top.y));
                        Gizmos.DrawLine(line4.Start,line4.End);
                        break;
                    case 5:
                        Line line5 = new Line(new Vector2(cellsArray[i,j].left.x, cellsArray[i,j].left.y),
                            new Vector2(cellsArray[i,j].top.x, cellsArray[i,j].top.y));
                        Line line6 = new Line(new Vector2(cellsArray[i,j].right.x, cellsArray[i,j].right.y),
                            new Vector2(cellsArray[i,j].bottom.x, cellsArray[i,j].bottom.y));
                        Gizmos.DrawLine(line5.Start,line5.End);
                        Gizmos.DrawLine(line6.Start, line6.End);
                        break;
                    case 6:
                        Line line7 = new Line(new Vector2(cellsArray[i,j].bottom.x, cellsArray[i,j].bottom.y),
                            new Vector2(cellsArray[i,j].top.x, cellsArray[i,j].top.y));
                        Gizmos.DrawLine(line7.Start,line7.End);
                        break;
                    case 7:
                        Line line8 = new Line(new Vector2(cellsArray[i,j].left.x, cellsArray[i,j].left.y),
                            new Vector2(cellsArray[i,j].top.x, cellsArray[i,j].top.y));
                        Gizmos.DrawLine(line8.Start,line8.End);
                        break;
                    case 8:
                        Line line9 = new Line(new Vector2(cellsArray[i,j].left.x, cellsArray[i,j].left.y),
                            new Vector2(cellsArray[i,j].top.x, cellsArray[i,j].top.y));
                        Gizmos.DrawLine(line9.Start,line9.End);
                        break;
                    case 9:
                        Line line10 = new Line(new Vector2(cellsArray[i,j].bottom.x, cellsArray[i,j].bottom.y),
                            new Vector2(cellsArray[i,j].top.x, cellsArray[i,j].top.y));
                        Gizmos.DrawLine(line10.Start,line10.End);
                        break;
                    case 10:
                        Line line11 = new Line(new Vector2(cellsArray[i,j].left.x, cellsArray[i,j].left.y),
                            new Vector2(cellsArray[i,j].bottom.x, cellsArray[i,j].bottom.y));
                        Line line12 = new Line(new Vector2(cellsArray[i,j].right.x, cellsArray[i,j].right.y),
                            new Vector2(cellsArray[i,j].top.x, cellsArray[i,j].top.y));
                        Gizmos.DrawLine(line11.Start,line11.End);
                        Gizmos.DrawLine(line12.Start, line12.End);
                        break;
                    case 11:
                        Line line13 = new Line(new Vector2(cellsArray[i,j].right.x, cellsArray[i,j].right.y),
                            new Vector2(cellsArray[i,j].top.x, cellsArray[i,j].top.y));
                        Gizmos.DrawLine(line13.Start,line13.End);
                        break;
                    case 12:
                        Line line14 = new Line(new Vector2(cellsArray[i,j].right.x, cellsArray[i,j].right.y),
                            new Vector2(cellsArray[i,j].left.x, cellsArray[i,j].left.y));
                        Gizmos.DrawLine(line14.Start,line14.End);
                        break;
                    case 13:
                        Line line15 = new Line(new Vector2(cellsArray[i,j].right.x, cellsArray[i,j].right.y),
                            new Vector2(cellsArray[i,j].bottom.x, cellsArray[i,j].bottom.y));
                        Gizmos.DrawLine(line15.Start,line15.End);
                        break;
                    case 14:
                        Line line16 = new Line(new Vector2(cellsArray[i,j].left.x, cellsArray[i,j].left.y),
                            new Vector2(cellsArray[i,j].bottom.x, cellsArray[i,j].bottom.y));
                        Gizmos.DrawLine(line16.Start,line16.End);
                        break;
                    default: break;
                }
            }
        }
    }
}
