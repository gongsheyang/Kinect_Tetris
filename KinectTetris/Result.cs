using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace KinectTetris
{
    /// <summary>
    /// 记录分数和级别
    /// </summary>
    class Result : INotifyPropertyChanged
    {
        Result()
        {
            Score = 0;
            Level = 1;
        }


        //单例模式
        private static Result instance;
        private static readonly object syncRoot = new object();
        public static Result GetInstance()
        {
            if (instance == null)
            {

                lock (syncRoot)
                {

                    if (instance == null)
                    {
                        instance = new Result();
                    }
                }
            }
            return instance;
        }


        int score;
        int level;

        public int Score
        {
            get { return score; }
            set { score = value; Notify("Score"); }
        }
        public int Level
        {
            get { return level; }
            set { level = value; Notify("Level"); }
        }

        public void CalculateScore(int Lines)
        {
            switch (Lines)
            {
                case 1: Score += 5;
                    break;
                case 2: Score += 15;
                    break;
                case 3: Score += 30;
                    break;
                case 4: Score += 50;
                    break;
                default: Score += 0;
                    break;
            }

            if (Score < 20) Level = 1;
            else if (Score < 100) Level = 2;
            else if (Score < 300) Level = 3;
            else if (Score < 500) Level = 4;
            else if (Score < 1000) Level = 5;
            else if (Score < 3000) Level = 6;
            else if (Score < 5000) Level = 7;
            else Level = 8;

        }

        void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
