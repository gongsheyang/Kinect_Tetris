using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KinectTetris
{
    class BoxFactory
    {
        /// <summary>
        /// 随机方块工厂
        /// </summary>
        static public Box GetRandomBox(ref Grid grid)
        {
            //return new Box_Z(ref grid);
            Random ran = new Random();
            int index = ran.Next(7);
            switch (index)
            {
                case 0: return new Box_S(ref grid);
                case 1: return new Box_Z(ref grid);
                case 2: return new Box_J(ref grid);
                case 3: return new Box_L(ref grid);
                case 4: return new Box_I(ref grid);
                case 5: return new Box_O(ref grid);
                case 6: return new Box_T(ref grid);
                default: return null;
            }
        }
    }
}
