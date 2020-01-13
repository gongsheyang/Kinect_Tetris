﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace KinectTetris
{

    class Box_I : Box
    {
		/// <summary>
		/// 竖直形状
		/// </summary>
		/// <param name="grid"></param>
        public Box_I(ref Grid grid)
        {
            this.grid = grid;
            for (int i = 0; i < 4; i++) rectangles[i].Fill = new SolidColorBrush(Colors.Red);
        }
		/// <summary>
		/// 显示位置
		/// </summary>
		/// <param name="P"></param>
		/// <param name="grid"></param>
        private void ShowAt(Position P, ref Grid grid)
        {
            rectangles[0].SetValue(Grid.ColumnProperty, P.x + 0);
            rectangles[0].SetValue(Grid.RowProperty, P.y + 0);

            rectangles[1].SetValue(Grid.ColumnProperty, P.x + 0);
            rectangles[1].SetValue(Grid.RowProperty, P.y + 1);

            rectangles[2].SetValue(Grid.ColumnProperty, P.x + 0);
            rectangles[2].SetValue(Grid.RowProperty, P.y + 2);

            rectangles[3].SetValue(Grid.ColumnProperty, P.x + 0);
            rectangles[3].SetValue(Grid.RowProperty, P.y + 3);

            for (int i = 0; i < 4; i++) grid.Children.Add(rectangles[i]);
        }

        public override void ShowWating(ref Grid WaingGrid)
        {
            ShowAt(new Position(2, 0), ref WaingGrid);
        }


        public override void Ready()
        {
            ShowAt(new Position(5, 0), ref grid);

			
            ActivityStatus = new Status();
            ActivityStatus.NextRelativeposition.Add(new Position(-1, 1));
            ActivityStatus.NextRelativeposition.Add(new Position(0, 0));
            ActivityStatus.NextRelativeposition.Add(new Position(1, -1));
            ActivityStatus.NextRelativeposition.Add(new Position(2, -2));
            ActivityStatus.NeedCheck.Add(true);
            ActivityStatus.NeedCheck.Add(true);
            ActivityStatus.NeedCheck.Add(false);
            ActivityStatus.NeedCheck.Add(true);
            ActivityStatus.Next = new Status();

            ActivityStatus.Next.NextRelativeposition.Add(new Position(1, -1));
            ActivityStatus.Next.NextRelativeposition.Add(new Position(0, 0));
            ActivityStatus.Next.NextRelativeposition.Add(new Position(-1, 1));
            ActivityStatus.Next.NextRelativeposition.Add(new Position(-2, 2));
            ActivityStatus.Next.NeedCheck.Add(true);
            ActivityStatus.Next.NeedCheck.Add(true);
            ActivityStatus.Next.NeedCheck.Add(false);
            ActivityStatus.Next.NeedCheck.Add(true);
            ActivityStatus.Next.Next = ActivityStatus;
        }

    }
}
