//using ShootBalls;
using ShootBalls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Balls
{
    public partial class Balls : Control
    {
        //размеры игрового поля (в шариках)
        public const int RowCount = 19;
        public const int ColCount = 12;
        //матрица шариков
        private int[,] BallsMatrix;
        //список цветов шариков
        public List<Color> ColorsArr { get; set; }
        Random rnd = new Random();
        //размер шарика
        public int _ballWidth = 25;
        //Координаты летящего шарика
        private int ShotX, ShotY;
        //private int ShotY; //= Height - _ballWidth;
        private double k, d;
        System.Windows.Forms.Timer MyTimer;
        //флаг, отвечающий, был произведен выстрел или нет
        private bool BallShot;
        Brush CurrentBrush;
        //переменная, обозначающая смещение шарика, которое зависит от ряда, в который прилетел шарик
        int Offset;
        Point CurrentCircleCords;
        int NextBallColor;
        //шаг полета шарика
        public int Step = 5;

        public List<Ball> BallsList;

        public delegate void ScoreHandler();
        public event ScoreHandler Notify;
        //переменная, отвечающая за счет
        public int Score { get; private set; }
        
        public int ballWidth
        {
            get
            {
                return _ballWidth;
            }
            set
            {
                if (value != _ballWidth)
                    if (value >= 5 && value <= 25)
                        _ballWidth = value;
            }
        }

        //пассивный конструктор
        public Balls()
        {
            //создание экземпляра
            ColorsArr = new List<Color>();
            ColorsArr.Add(Color.Transparent);
            ColorsArr.Add(Color.Black);
            ColorsArr.Add(Color.Pink);
            ColorsArr.Add(Color.Red);
            ColorsArr.Add(Color.Blue);
            ColorsArr.Add(Color.Yellow);
            StartGame();
            DoubleBuffered = true;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            int OldShotX = ShotX;
            ShotX = k != 0 ? (int)((ShotY - d) / k) : CurrentCircleCords.X;
            ShotY -= Step;
            BallShot = true;
            int Row = ShotY / _ballWidth;
            //int Col0 = (ShotX - 3 * _ballWidth / 4) / _ballWidth;
            int Col1 = ShotX / _ballWidth;
            int Col2 = (ShotX + 3 * _ballWidth / 4) / _ballWidth;

            if ((ShotX <= Step / 2) && (ShotX < OldShotX))
            {
                d = d - k * ShotX;
                k = -k;
            }
            if ((ShotX >= Width - _ballWidth) && (ShotX > OldShotX))
            {
                d = d - k * ShotX;
                k = -k;
            }


            if ((Row > 0) && (((Row % 2 != 0) && (BallsMatrix[Row - 1, Col2] != 0)) || ((Row % 2 == 0) && (BallsMatrix[Row - 1, Col1] != 0))))
            //(((Row % 2 == 0) && (BallsMatrix[Row - 1, Col2] != 0)) || ((Row % 2 == 0) && (BallsMatrix[Row - 1, Col2] != 0))))
            //(((Row % 2 != 0) && (BallsMatrix[Row - 1, Col2] != 0)) || ((Row % 2 == 0) && (BallsMatrix[Row - 1, Col1] != 0)))
            //(((Row % 2 != 0) && (BallsMatrix[Row - 1, Col2 + 1] != 0)) || ((Row % 2 == 0) && (BallsMatrix[Row - 1, Col1 + 1] != 0)))
            //  || BallsMatrix[Row - 1, Col - 1] != 0) &&
            //(BallsMatrix[Row - 1, Col] != 0 || BallsMatrix[Row - 1, Col + 1] != 0))
            {
                int Column;
                if (Row < 0) Row = 0;
                if (Row % 2 != 0)
                {
                    BallsMatrix[Row, Col1] = NextBallColor;
                    Column = Col1;

                }
                else
                {
                    BallsMatrix[Row, Col2] = NextBallColor;
                    Column = Col2;
                }
                (sender as System.Windows.Forms.Timer).Stop();
                BallShot = false;
                OuterProc(new Ball(Row, Column));
                NextBallColor = rnd.Next(1, ColorsArr.Count);
            }
            Invalidate();
        }

        public void StartGame()
        {
            BallsMatrix = new int[RowCount, ColCount];
            int LastCol;
            //первоначальное заполнение стакана шариками
            for (int Row = 0; Row < 7; Row++)
            {
                //проверка, нужно ли заполнять шарик в последней колонке или нет
                if (Row % 2 != 0) LastCol = BallsMatrix.GetLength(1) - 1; else LastCol = BallsMatrix.GetLength(1);
                for (int Col = 0; Col < LastCol; Col++)
                    BallsMatrix[Row, Col] = rnd.Next(1, ColorsArr.Count);
            }
            MyTimer = new System.Windows.Forms.Timer();
            MyTimer.Interval = 5;
            MyTimer.Enabled = true;
            MyTimer.Tick += TimerTick;
            MyTimer.Stop();
            CurrentBrush = new SolidBrush(ColorsArr[rnd.Next(1, ColorsArr.Count)]);
            Size = new Size(ColCount * _ballWidth, 500);
            //Width = ColCount * _ballWidth;
            MaximumSize = new Size(ColCount * _ballWidth, 500);
            BallsList = new List<Ball>();
            NextBallColor = 1;
            Score = 0;
            //Invalidate();
        }

        public bool EndGame()
        {
            int cnt = 0;
            for (int Row = 0; Row < 7; Row++)
                for (int Col = 0; Col < BallsMatrix.GetLength(1); Col++)
                    if (BallsMatrix[Row, Col] > 0) { cnt++; }
            if (cnt == 0) return true; else return false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Brush b;
            b = new SolidBrush(Color.Gray);
            e.Graphics.FillRectangle(b, 0, 0, Width, Height);
            if (ColorsArr != null)
            {
                for (int Row = 0; Row < BallsMatrix.GetLength(0); Row++)
                {
                    int LastCol;
                    if (Row % 2 == 0)
                    {
                        LastCol = BallsMatrix.GetLength(1);
                        Offset = 0;
                    }
                    else
                    {
                        LastCol = BallsMatrix.GetLength(1) - 1;
                        Offset = _ballWidth / 2;
                    }
                    for (int Col = 0; Col < LastCol; Col++)
                    {
                        CurrentBrush = new SolidBrush(ColorsArr[BallsMatrix[Row, Col]]);
                        if (BallsMatrix[Row, Col] != 0) e.Graphics.FillEllipse(CurrentBrush, Col * _ballWidth + Offset, Row * _ballWidth, _ballWidth, _ballWidth);
                    }
                }
            }
            CurrentBrush = new SolidBrush(ColorsArr[NextBallColor]);
            if (BallShot)
            {
                e.Graphics.FillEllipse(CurrentBrush, ShotX, ShotY, _ballWidth, _ballWidth);
            }
            else
            {
                e.Graphics.FillEllipse(CurrentBrush, Width / 2 - _ballWidth / 2, Height - _ballWidth, _ballWidth, _ballWidth);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!BallShot) Flight(e.Location);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!BallShot)
                if (e.KeyCode == Keys.Space)
                {
                    NextBallColor = rnd.Next(1, ColorsArr.Count);
                    Invalidate();
                }
        }

        protected void Flight(Point CurrentCircleCords)
        {
            CurrentCircleCords.X -= _ballWidth / 2;
            CurrentCircleCords.Y -= _ballWidth / 2;
            ShotX = Width / 2 - _ballWidth / 2;
            ShotY = Height - _ballWidth;
            BallShot = true;
            if (CurrentCircleCords.X != ShotX)
            {
                k = 1.0 * (CurrentCircleCords.Y - ShotY) / (CurrentCircleCords.X - ShotX);
                d = ShotY - k * ShotX;
            }
            else
            {
                k = 0;
            }
            MyTimer.Start();
        }

        private List<Ball> DeleteNullBalls(List<Ball> List)
        {
            for (int item = List.Count - 1; item >= 0; item--)
            {
                if (BallsMatrix[List[item].Row, List[item].Col] == 0)
                {
                    List.Remove(List[item]);
                }
            }
            return List;
        }

        private List<Ball> FoundBalls(int startRow, int startCol)
        {
            BallsList.Clear();
            if (startRow >= 0 && startRow < BallsMatrix.GetLength(0) && startCol >= 0 && startCol < BallsMatrix.GetLength(1))
            {

                if (startCol > 0)
                {
                    BallsList.Add(new Ball(startRow, startCol - 1));
                }
                if (startCol < BallsMatrix.GetLength(1) - 1)
                {
                    BallsList.Add(new Ball(startRow, startCol + 1));
                }
                if (startRow > 0)
                {
                    if (startRow % 2 == 0)
                    {
                        if (startCol > 0) BallsList.Add(new Ball(startRow - 1, startCol - 1));
                        if (startCol < BallsMatrix.GetLength(1) - 1) BallsList.Add(new Ball(startRow - 1, startCol));
                    }
                    else
                    {
                        BallsList.Add(new Ball(startRow - 1, startCol));
                        if (startCol < BallsMatrix.GetLength(1) - 1) BallsList.Add(new Ball(startRow - 1, startCol + 1));
                    }
                }
                if (startRow < BallsMatrix.GetLength(0) - 1)
                {
                    if (startRow % 2 == 0)
                    {
                        if (startCol > 0) BallsList.Add(new Ball(startRow + 1, startCol - 1));
                        if (startCol < BallsMatrix.GetLength(1) - 1) BallsList.Add(new Ball(startRow + 1, startCol));
                    }
                    else
                    {
                        BallsList.Add(new Ball(startRow + 1, startCol));
                        if (startCol < BallsMatrix.GetLength(1) - 1) BallsList.Add(new Ball(startRow + 1, startCol + 1));
                    }
                }
            }
            DeleteNullBalls(BallsList);
            return BallsList;
        }

        private void ClearList(List<Ball> List)
        {
            foreach (Ball item in List)
            {
                BallsMatrix[item.Row, item.Col] = 0;
                Score += 5;
            }
            List.Clear();
        }

        private List<Ball> FoundIslands(Ball startBall)
        {
            List<Ball> FoundIsland = new List<Ball> { startBall };
            int i = 0;
            do
            {
                foreach (Ball item in FoundBalls(FoundIsland[i].Row, FoundIsland[i].Col))
                {
                    if (!FoundIsland.Exists(P => P.Row == item.Row && P.Col == item.Col))
                    {
                        FoundIsland.Add(item);
                    }
                }
                i++;
            } while (i < FoundIsland.Count);
            foreach (Ball item in FoundIsland)
            {
                if (item.Row == 0)
                {
                    return new List<Ball>();
                }
            }
            return FoundIsland;
        }

        private void DeleteIslands()
        {
            for (int Row = 0; Row < BallsMatrix.GetLength(0); Row++)
            {
                for (int Col = 0; Col < BallsMatrix.GetLength(1); Col++)
                {
                    List<Ball> Island = FoundIslands(new Ball(Row, Col));
                    if (Island.Count > 0)
                    {
                        ClearList(Island);  
                    }
                }
            }
        }

        private void OuterProc(Ball startBall)
        {
            List<Ball> DeleteBalls = new List<Ball>();
            int i = 0;
            DeleteBalls.Add(startBall);
            do
            {
                foreach (Ball item in FoundBalls(DeleteBalls[i].Row, DeleteBalls[i].Col))
                {
                    if ((BallsMatrix[DeleteBalls[0].Row, DeleteBalls[0].Col] == BallsMatrix[item.Row, item.Col]) &&
                        !DeleteBalls.Exists(P => P.Row == item.Row && P.Col == item.Col))
                    {
                        DeleteBalls.Add(item);
                    }
                }
            } while (++i < DeleteBalls.Count);
            if (DeleteBalls.Count > 2) ClearList(DeleteBalls);
            DeleteBalls.Clear();
            Invalidate();
            DeleteIslands();
            if (EndGame())
            {
                MessageBox.Show("Конец игры!\n" + "Ваш счет: " + Score);
                StartGame();
            }
        }


    }
}