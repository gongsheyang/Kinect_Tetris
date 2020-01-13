using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectTetris
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
		//定义kinectSensor对象
		private KinectSensor kinectSensor = null;
		//定义人数
        int count = 0;
		//定义colorFrameReader对象
		private ColorFrameReader colorFrameReader = null;
		//定义colorBitmap对象
		private WriteableBitmap colorBitmap = null;
		//定义记录骨骼节点的数组
        Body[] bodies;
		//定义MultiSourceFrameReader
		MultiSourceFrameReader msfr;

        /// <summary>
        /// 用于延时的计数器
        /// </summary>
        int times = 0;

		//是否暂停的变量
        bool isPause = false;

        public MainWindow()
        {
			//初始化
            InitializeComponent();
            Container.grid = Grid1;
            Container.waitingGrid = grid3;
            grid4.DataContext = Result.GetInstance();
            Container.OnGameover += OnGameover;

			//记录的6个骨骼点
            bodies = new Body[6];
            this.kinectSensor = KinectSensor.GetDefault();//打开Kinect传感器
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

            msfr = kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Color);
            
        }

        private void msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame msf = e.FrameReference.AcquireFrame();
            if (msf != null)
            {
                using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
                {
                    using (ColorFrame colorFrame = msf.ColorFrameReference.AcquireFrame())
                    {
                        if (bodyFrame != null && colorFrame != null)
                        {
                            FrameDescription colorFrameDescription = colorFrame.FrameDescription;
                            using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                            {
                                this.colorBitmap.Lock();
                                if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                                {
                                    colorFrame.CopyConvertedFrameDataToIntPtr(this.colorBitmap.BackBuffer, (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4), ColorImageFormat.Bgra);
                                    this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                                }
                                this.colorBitmap.Unlock();
                                bodyFrame.GetAndRefreshBodyData(bodies);
                                count = 0;

                                for (int i = 0; i < bodies.Length; i++)
                                {
                                    //如果追踪到body
                                    if (bodies[i].IsTracked)
                                    {//记录骨骼节点
                                        Joint headJoint = bodies[i].Joints[JointType.Head];
                                        Joint neck = bodies[i].Joints[JointType.Neck];
                                        Joint handLeft = bodies[i].Joints[JointType.HandLeft];
                                        Joint handRight = bodies[i].Joints[JointType.HandRight];
                                        Joint shoulderLeft = bodies[i].Joints[JointType.ShoulderLeft];
                                        Joint shoulderRight = bodies[i].Joints[JointType.ShoulderRight];
                                        if (headJoint.TrackingState == TrackingState.Tracked)
                                        {
											//每捕捉到100次骨骼节点触发一次对应的事件
                                            if (times == 0)
                                            {
                                                if (handLeftUp(headJoint, handLeft) == true)//旋转
                                                {
                                                    Container.ActivityBox.ChangeShape();
                                                }
                                                if (handLeftLeft(headJoint, handLeft, handRight) == true)//左移
                                                {
                                                    Container.ActivityBox.MoveLeft();
                                                }
                                                if (handRightRight(headJoint, handLeft, handRight) == true)//右移
                                                {
                                                    Container.ActivityBox.MoveRight();
                                                }
                                                if (handRightUp(headJoint, handRight) == true)//加速向下
                                                {
                                                    Container.ActivityBox.FastDown();
                                                }
                                                if (leftrighttrack(handLeft,handRight) == true)//暂停
                                                {
                                                    if(isPause)//暂停游戏的条件
													{
                                                        Container.ActivityBox.UnPause();
                                                        isPause = false;
                                                    }
                                                    else
                                                    {
                                                        isPause = true;
                                                        Container.ActivityBox.Pause();
                                                    }
                                                }
                                                times++;
                                            }
                                            else if (times >= 10)
                                            {
                                                times = 0;
                                            }
                                            else
                                            {
                                                times++;
                                            }

                                            count++;
                                        }
                                    }
                                    else
                                    {
                                        //未检测到身体
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 双手交叉
        /// </summary>
        /// <param name="handLeft"></param>
        /// <param name="handRight"></param>
        /// <returns></returns>
        private bool leftrighttrack(Joint handLeft, Joint handRight)
        {
            if (Math.Abs(handLeft.Position.Y - handRight.Position.Y) <0.1 && Math.Abs(handLeft.Position.X - handRight.Position.X) < 0.1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 左手举过头顶
        /// </summary>
        /// <param name="headJoint"></param>
        /// <param name="handLeft"></param>
        /// <returns></returns>
        private bool handLeftUp(Joint headJoint, Joint handLeft)
        {
            if (handLeft.Position.Y - headJoint.Position.Y > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 左手向左
        /// </summary>
        /// <param name="headJoint"></param>
        /// <param name="handLeft"></param>
        /// <returns></returns>
        private bool handLeftLeft(Joint headJoint, Joint handLeft, Joint handRight)
        {
            if ((handLeft.Position.X < headJoint.Position.X - 0.45) && (handRight.Position.X <= headJoint.Position.X + 0.45))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 右手向右
        /// </summary>
        /// <param name="headJoint"></param>
        /// <param name="handLeft"></param>
        /// <param name="handRight"></param>
        /// <returns></returns>
        private bool handRightRight(Joint headJoint, Joint handLeft, Joint handRight)
        {
            if ((handRight.Position.X > headJoint.Position.X + 0.45) && (handLeft.Position.X >= headJoint.Position.X - 0.45))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 右手举过头顶
        /// </summary>
        /// <param name="headJoint"></param>
        /// <param name="handRight"></param>
        /// <returns></returns>
        private bool handRightUp(Joint headJoint, Joint handRight)
        {
            if (handRight.Position.Y - headJoint.Position.Y > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


		/// <summary>
		/// 键盘控制事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up: Container.ActivityBox.ChangeShape();
                    break;
                case Key.Left: Container.ActivityBox.MoveLeft();
                    break;
                case Key.Right: Container.ActivityBox.MoveRight();
                    break;
                case Key.Down: Container.ActivityBox.MoveDown();
                    break;
                case Key.Space: Container.ActivityBox.FastDown();
                    break;
                default: break;
            }
        }

		/// <summary>
		/// 重新开始游戏
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (Container.ActivityBox == null) Container.NewBoxReadyToDown();
            else
            {
                Container.Pause();

                if (MessageBox.Show("当前游戏还在进行中，您是否重新开始新游戏?", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Container.Stop();
                    button2.Content = "暂停";
                    Container.NewBoxReadyToDown();
                }
                else
                {
                  if(button2.Content.ToString()=="暂停")  Container.UnPause();
                }
            }
            msfr.MultiSourceFrameArrived += msfr_MultiSourceFrameArrived;
            this.kinectSensor.Open();
        }
		/// <summary>
		/// 暂停游戏
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.ToString() == "暂停")
            {
                Container.Pause();
                (sender as Button).Content = "取消暂停";
            }
            else
            {
                Container.UnPause();
                (sender as Button).Content = "暂停";
            }
        }
		/// <summary>
		/// 结束游戏
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Container.Pause();
            if (MessageBox.Show("您是否结束游戏?", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Container.Stop();
                button2.Content = "暂停";
            }
            else
            {
                if (button2.Content.ToString() == "暂停") Container.UnPause();
            }

            closeFunc();
        }

        static private void OnGameover(object sender, EventArgs e)
        {
            MessageBox.Show("游戏结束！");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closeFunc();
        }
		/// <summary>
		/// 释放资源
		/// </summary>
        private void closeFunc()
        {
            if (this.colorFrameReader != null)
            {
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
    }

}
