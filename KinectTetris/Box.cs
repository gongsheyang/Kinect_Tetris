using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;

namespace KinectTetris
{
    abstract class Box
    {
		/// <summary>
		/// 定义Box父类
		/// </summary>
        public Box()
        {
            for (int i = 0; i < 4; i++)
            {
                rectangles.Add(new Rectangle());
                rectangles[i].Width = 24.0;
                rectangles[i].Height = 24.0;
            }

            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Tick += new EventHandler(Timer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(450 - Result.GetInstance().Level * 50);
        }


        protected Grid grid;

        /// <summary>
        /// 定义由四个方格组成的方块
        /// </summary>
        protected IList<Rectangle> rectangles = new List<Rectangle>(4);

        DispatcherTimer dispatcherTimer;

        /// <summary>
        /// 当方块到达底部时触发的事件句柄
        /// </summary>
        public event EventHandler OnBottom;

        /// <summary>
        /// 判断x行y列是否包含方格
        /// </summary>
        protected bool Existence(int x, int y)
        {
            foreach (var r in grid.Children)
            {
                if (r is Rectangle)
                {
                    if (this.rectangles.Contains(r as Rectangle)) return false;
                    if (Convert.ToInt32((r as Rectangle).GetValue(Grid.ColumnProperty)) == x && Convert.ToInt32((r as Rectangle).GetValue(Grid.RowProperty)) == y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 当前方块是否与其他方块重叠
        /// </summary>
        public bool ISOverlapping()
        {
            foreach (var r in rectangles)
            {
                int x = Convert.ToInt32((r as Rectangle).GetValue(Grid.ColumnProperty));
                int y = Convert.ToInt32((r as Rectangle).GetValue(Grid.RowProperty));
                if (Existence(x, y)) return true;
            }
            return false;
        }




        /// <summary>
        /// 判断由数值 x,y标示的行列值是否在Grid范围内
        /// </summary>
        protected bool InGrid(int x, int y)
        {
            if (x >= 12 || y >= 24 || x < 0 || y < 0) return false;
            return true;
        }

        /// <summary>
        /// 定义活动方块自动下降
        /// </summary>
        public void AtuoDown()
        {
            dispatcherTimer.Start();
        }
		
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!MoveDown())
            {
                dispatcherTimer.Stop();
                OnBottom(this, null);
            }
        }


        public abstract void Ready();
        public abstract void ShowWating(ref Grid WaingGrid);


        protected bool Move(int _x, int _y)
        {
            if (IsPause) return false;
            for (int i = 0; i < 4; i++)
            {
                int x = Convert.ToInt32(rectangles[i].GetValue(Grid.ColumnProperty)) + _x;
                int y = Convert.ToInt32(rectangles[i].GetValue(Grid.RowProperty)) + _y;
                if (Existence(x, y)) return false;
                if (!InGrid(x, y)) return false;
            }
            for (int i = 0; i < 4; i++)
            {
                rectangles[i].SetValue(Grid.ColumnProperty, Convert.ToInt32(rectangles[i].GetValue(Grid.ColumnProperty)) + _x);
                rectangles[i].SetValue(Grid.RowProperty, Convert.ToInt32(rectangles[i].GetValue(Grid.RowProperty)) + _y);
            }
            return true;
        }

        public bool MoveUp()
        {
            return Move(0, -1);
        }

        public bool MoveDown()
        {
            return Move(0, 1);
        }

        public bool MoveLeft()
        {
            return Move(-1, 0);
        }

        public bool MoveRight()
        {
            return Move(1, 0);
        }

        /// <summary>
        /// 快速下降
        /// </summary>
        public bool FastDown()
        {
            if (IsPause) return false;
            bool mark = false;
            int j = 0;
            while (!mark)
            {
                j++;
                for (int i = 0; i < 4; i++)
                {
                    int x = Convert.ToInt32(rectangles[i].GetValue(Grid.ColumnProperty));
                    int y = Convert.ToInt32(rectangles[i].GetValue(Grid.RowProperty)) + j;
                    if (Existence(x, y) || !InGrid(x, y))
                    {
                        j--;
                        mark = true;
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                rectangles[i].SetValue(Grid.RowProperty, Convert.ToInt32(rectangles[i].GetValue(Grid.RowProperty)) + j);
            }

            //OnBottom(this, null);
            return mark;
        }

        /// <summary>
        /// 当前方块是否处于暂停状态
        /// </summary>
        private bool IsPause = false;
        public void Pause()
        {
            dispatcherTimer.IsEnabled = false;
            IsPause = true;
        }

        public void UnPause()
        {
            dispatcherTimer.IsEnabled = true;
            IsPause = false;
        }

        public void StopAction()
        {
            dispatcherTimer.Stop();
        }


        /// <summary>
        /// 当前变形状态
        /// </summary>
        protected Status ActivityStatus;

        /// <summary>
        /// 变形
        /// </summary>
        public bool ChangeShape()
        {
            if (IsPause) return false;
            IList<Position> P = new List<Position>(4);
            for (int i = 0; i < 4; i++) P.Add(new Position());

            P[0].x = Convert.ToInt32(rectangles[0].GetValue(Grid.ColumnProperty)) + ActivityStatus.NextRelativeposition[0].x;
            P[0].y = Convert.ToInt32(rectangles[0].GetValue(Grid.RowProperty)) + ActivityStatus.NextRelativeposition[0].y;
            if (ActivityStatus.NeedCheck[0]) if (Existence(P[0].x, P[0].y) || !InGrid(P[0].x, P[0].y)) return false;

            P[1].x = Convert.ToInt32(rectangles[1].GetValue(Grid.ColumnProperty)) + ActivityStatus.NextRelativeposition[1].x;
            P[1].y = Convert.ToInt32(rectangles[1].GetValue(Grid.RowProperty)) + ActivityStatus.NextRelativeposition[1].y;
            if (ActivityStatus.NeedCheck[1]) if (Existence(P[1].x, P[1].y) || !InGrid(P[1].x, P[1].y)) return false;

            P[2].x = Convert.ToInt32(rectangles[2].GetValue(Grid.ColumnProperty)) + ActivityStatus.NextRelativeposition[2].x;
            P[2].y = Convert.ToInt32(rectangles[2].GetValue(Grid.RowProperty)) + ActivityStatus.NextRelativeposition[2].y;
            if (ActivityStatus.NeedCheck[2]) if (Existence(P[2].x, P[2].y) || !InGrid(P[2].x, P[2].y)) return false;

            P[3].x = Convert.ToInt32(rectangles[3].GetValue(Grid.ColumnProperty)) + ActivityStatus.NextRelativeposition[3].x;
            P[3].y = Convert.ToInt32(rectangles[3].GetValue(Grid.RowProperty)) + ActivityStatus.NextRelativeposition[3].y;
            if (ActivityStatus.NeedCheck[3]) if (Existence(P[3].x, P[3].y) || !InGrid(P[3].x, P[3].y)) return false;

            for (int i = 0; i < 4; i++)
            {
                rectangles[i].SetValue(Grid.ColumnProperty, P[i].x);
                rectangles[i].SetValue(Grid.RowProperty, P[i].y);
            }
            ActivityStatus = ActivityStatus.Next;

            return true;
        }
    }

    /// <summary>
    /// 定义一个方格坐标点
    /// </summary>
    class Position
    {
        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Position()
        {
        }

       public int x { get; set; }
       public int y { get; set; }
    }


    /// <summary>
    /// 定义方块形状状态循环链表，标记变形状态
    /// </summary>
    class Status
    {
        /// <summary>
        /// 方格[四个方块]下一次变形将要去的相对位置
        /// </summary>
        public List<Position> NextRelativeposition = new List<Position>(4);

        /// <summary>
        /// 是否需要检查方格[每个方块]到这个位置的可行性
        /// </summary>
        public List<bool> NeedCheck = new List<bool>(4);

        /// <summary>
        /// 指向下一状态
        /// </summary>
        public Status Next;
    }

}
