/*
 * Author: Muhammad ALI(17-10278)   
 * 
 * 
 * 
 * 
 */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.CvEnum;
//add Skin detector
using HandGestureRecognition.SkinDetector;

using System.Runtime.InteropServices;//user32.dll

namespace HandGestureRecognition
{
      //partial just to split the def of class to multiple souce file or source setions.
    public partial class Form1 : Form
    {

        //Global Declarations
        bool button_pressed = false;                                       
        Contour<Point> biggestContour = null;
        //store intermediate data
        MemStorage storage = new MemStorage();
        
        //global declaration storage..........defects error is still there  (Light issue)
        
        CustomIColorSkinDetector skinDetector;

        Image<Bgr, Byte> currentFrame;
        Image<Bgr, Byte> currentFrameCopy;
                
        Capture customGrabber;
        AdaptiveSkinDetector opencvCustomdetector;
        
        int customframeWidth;
        int customframeHeight;
        
        
        Hsv hsv_min_threshHold;
        Hsv hsv_max_threshHold;
        Ycc YCrCb_min_threshHold;
        Ycc YCrCb_max_threshHold;
        
        Seq<Point> hull;
        Seq<Point> filteredHull;
        Seq<MCvConvexityDefect> customdefects;
        MCvConvexityDefect[] customdefectArray;
        Rectangle handRect;
        MCvBox2D custombox;
        Ellipse customellip;

        
        public Form1()
        {
            InitializeComponent();
            //integrated webcam(default)
            customGrabber = new Emgu.CV.Capture(0);
            customGrabber.QueryFrame();//each frame out of video
            customframeWidth = customGrabber.Width;
            customframeHeight = customGrabber.Height;
            opencvCustomdetector = new AdaptiveSkinDetector(1, AdaptiveSkinDetector.MorphingMethod.NONE);
            hsv_min_threshHold = new Hsv(0, 45, 0);
            hsv_max_threshHold = new Hsv(20, 255, 255);
            YCrCb_min_threshHold = new Ycc(0, 131, 80);
            YCrCb_max_threshHold = new Ycc(255, 185, 135);
            custombox = new MCvBox2D();
            customellip = new Ellipse();


            //callback
            Application.Idle += new EventHandler(FrameGrabber);                        
        }

        void FrameGrabber(object sender, EventArgs e)
        {
            currentFrame = customGrabber.QueryFrame();  //keep querying frames
            if (currentFrame != null)
            {
                currentFrameCopy = currentFrame.Copy();//save the copy

                // for opencv adaptive skin detector
                //Image<Gray,Byte> skin = new Image<Gray,byte>(currentFrameCopy.Width,currentFrameCopy.Height);                
                //opencvCustomdetector.Process(currentFrameCopy, skin);                

                skinDetector = new YCrCbSkinDetector(); //new CustomHsvSkinDetector();//new CustomYCrCbSkinDetector();


                Image<Gray, Byte> skin = skinDetector.DetectSkin(currentFrameCopy, YCrCb_min_threshHold, YCrCb_max_threshHold);

                //Image<Gray, Byte> skin = skinDetector.DetectSkin(currentFrameCopy, hsv_min_threshHold, hsv_max_threshHold);
                ExtractContourAndHull(skin);
                                
                DrawAndComputeFingersNum();

                imageBoxSkin.Image = skin;
                imageBoxFrameGrabber.Image = currentFrame;
            }
        }
               
        private void ExtractContourAndHull(Image<Gray, byte> skin)
        {
            //using (MemStorage storage = new MemStorage())
            {

               
                Contour<Point> contours = skin.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage);
                
                //Contour<Point> biggestContour = null;

                Double Result1 = 0;
                Double Result2 = 0;
                while (contours != null)
                {
                    Result1 = contours.Area;
                    if (Result1 > Result2)
                    {
                        Result2 = Result1;
                        biggestContour = contours;
                    }
                    // next contour (v links)
                    contours = contours.HNext;
                }

                if (biggestContour != null)
                {

                    /*

                    Retrieves contours from the binary image and returns the number of retrieved contours. 
                    It will contain pointer to the first most outer contour , if no contours is detected (if the image is completely black).
                    Other contours may be reached from firstContour using h_next and v_next links.     
                    

                    */

                    //currentFrame.Draw(biggestContour, new Bgr(Color.DarkViolet), 2);
                    Contour<Point> currentContour = biggestContour.ApproxPoly(biggestContour.Perimeter * 0.0025, storage);
                    currentFrame.Draw(currentContour, new Bgr(Color.LimeGreen), 2);
                    biggestContour = currentContour;

                    //Contours can be also used for shape analysis and object recognition 

                    hull = biggestContour.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                    //defects = biggestContour.GetConvexityDefacts(storage, Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                    custombox = biggestContour.GetMinAreaRect();
                    PointF[] points = custombox.GetVertices();
                    
                    //handRect = box.MinAreaRect();
                    //currentFrame.Draw(handRect, new Bgr(200, 0, 0), 1);

                    Point[] ps = new Point[points.Length];
                    for (int i = 0; i < points.Length; i++)
                        ps[i] = new Point((int)points[i].X, (int)points[i].Y);

                    currentFrame.DrawPolyline(hull.ToArray(), true, new Bgr(200, 125, 75), 2);
                    currentFrame.Draw(new CircleF(new PointF(custombox.center.X, custombox.center.Y), 3), new Bgr(200, 125, 75), 2);

                    //customellip.MCvBox2D= CvInvoke.cvFitEllipse2(biggestContour.Ptr);
                    //currentFrame.Draw(new Ellipse(customellip.MCvBox2D), new Bgr(Color.LavenderBlush), 3);

                    PointF center;
                    float radius;
                    
                    //debugging and checking;

                    //CvInvoke.cvMinEnclosingCircle(biggestContour.Ptr, out  center, out  radius);
                    //currentFrame.Draw(new CircleF(center, radius), new Bgr(Color.Gold), 2);

                    //currentFrame.Draw(new CircleF(new PointF(customellip.MCvBox2D.center.X, customellip.MCvBox2D.center.Y), 3), new Bgr(100, 25, 55), 2);
                    //currentFrame.Draw(customellip, new Bgr(Color.DeepPink), 2);

                    //CvInvoke.cvEllipse(currentFrame, new Point((int)customellip.MCvBox2D.center.X, (int)customellip.MCvBox2D.center.Y), new System.Drawing.Size((int)customellip.MCvBox2D.size.Width, (int)customellip.MCvBox2D.size.Height), customellip.MCvBox2D.angle, 0, 360, new MCvScalar(120, 233, 88), 1, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, 0);
                    //currentFrame.Draw(new Ellipse(new PointF(custombox.center.X, custombox.center.Y), new SizeF(custombox.size.Height, custombox.size.Width), custombox.angle), new Bgr(0, 0, 0), 2);



                    //convex hull too mnay close points.so filter it
                    filteredHull = new Seq<Point>(storage);
                    for (int i = 0; i < hull.Total; i++)
                    {
                        if (Math.Sqrt(Math.Pow(hull[i].X - hull[i + 1].X, 2) + Math.Pow(hull[i].Y - hull[i + 1].Y, 2)) > custombox.size.Width / 10)
                        {
                            filteredHull.Push(hull[i]);
                        }
                    }

                    //change
                    customdefects = biggestContour.GetConvexityDefacts(storage, Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                    //MessageBox.Show("j");

                    customdefectArray = customdefects.ToArray();
                }
            }
        }

        private void DrawAndComputeFingersNum()
        {
            int fingerNum = 0;

            #region hull drawing
            /*
            for (int i = 0; i < filteredHull.Total; i++)
            {
                PointF hullPoint = new PointF((float)filteredHull[i].X,
                                              (float)filteredHull[i].Y);
                CircleF hullCircle = new CircleF(hullPoint, 4);
                currentFrame.Draw(hullCircle, new Bgr(Color.Aquamarine), 2);
            }*/

            #endregion

            #region defects drawing
            for (int i = 0; i < customdefects.Total; i++)
            {
                PointF startPoint = new PointF((float)customdefectArray[i].StartPoint.X,
                                                (float)customdefectArray[i].StartPoint.Y);

                PointF depthPoint = new PointF((float)customdefectArray[i].DepthPoint.X,
                                                (float)customdefectArray[i].DepthPoint.Y);

                PointF endPoint = new PointF((float)customdefectArray[i].EndPoint.X,
                                                (float)customdefectArray[i].EndPoint.Y);

                LineSegment2D startDepthLine = new LineSegment2D(customdefectArray[i].StartPoint, customdefectArray[i].DepthPoint);

                LineSegment2D depthEndLine = new LineSegment2D(customdefectArray[i].DepthPoint, customdefectArray[i].EndPoint);

                CircleF startCircle = new CircleF(startPoint, 5f);

                CircleF depthCircle = new CircleF(depthPoint, 5f);

                CircleF endCircle = new CircleF(endPoint, 5f);

                //Custom heuristic based on some experiment, double check it before use
                if ((startCircle.Center.Y < custombox.center.Y || depthCircle.Center.Y < custombox.center.Y) && (startCircle.Center.Y < depthCircle.Center.Y) && (Math.Sqrt(Math.Pow(startCircle.Center.X - depthCircle.Center.X, 2) + Math.Pow(startCircle.Center.Y - depthCircle.Center.Y, 2)) > custombox.size.Height / 6.5))
                {
                    fingerNum++;
                    currentFrame.Draw(startDepthLine, new Bgr(Color.Green), 2);
                    //currentFrame.Draw(depthEndLine, new Bgr(Color.Magenta), 2);
                }

                                
                currentFrame.Draw(startCircle, new Bgr(Color.Red), 2);
                currentFrame.Draw(depthCircle, new Bgr(Color.Yellow), 5);
                //currentFrame.Draw(endCircle, new Bgr(Color.DarkBlue), 4);
            }
            #endregion

            MCvFont font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_DUPLEX, 5d, 5d);
            currentFrame.Draw(fingerNum.ToString(), ref font, new Point(50, 150), new Bgr(Color.White));



            /*
             *  control cursor .............................................................
             * */


            
            MCvMoments moment = new MCvMoments();               // a new MCvMoments object

            





            try
            {
                moment = biggestContour.GetMoments();           // Moments of biggestContour
            }
            catch (NullReferenceException except)
            {
                //label3.Text = except.Message;
                return;
            }

            
            
            CvInvoke.cvMoments(biggestContour, ref moment, 0);




/*           
            double m_00 = CvInvoke.cvGetSpatialMoment(ref moment, 0, 0);
            double m_10 = CvInvoke.cvGetSpatialMoment(ref moment, 1, 0);
            double m_01 = CvInvoke.cvGetSpatialMoment(ref moment, 0, 1);
            */                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      


            double m_00 = CvInvoke.cvGetSpatialMoment(ref moment, 0, 0);
            double m_10 = CvInvoke.cvGetSpatialMoment(ref moment, 1, 0);
            double m_01 = CvInvoke.cvGetSpatialMoment(ref moment, 0, 1);




            
            // whole edivide by 10 to spped up.
            int current_X = Convert.ToInt32(m_10 / m_00)/10;      // X location of centre of contour              
            int current_Y = Convert.ToInt32(m_01 / m_00)/10;      // Y location of center of contour
            


            /*
             * not worked
            int current_X = Convert.ToInt32(m_10 / m_00) ;      // X location of centre of contour              
            int current_Y = Convert.ToInt32(m_01 / m_00);      // Y location of center of contour


            CvInvoke.cvGetNormalizedCentralMoment(ref moment, current_X, current_Y);
            */

            if (button_pressed)
            {

                // move cursor to center of contour only if Finger count is 1 
                // i.e. 0=palm is closed

                if (fingerNum == 1 )
                {
                    Cursor.Position = new Point(current_X*20  , current_Y*20 );


                    //speeed up   //change
                    //Cursor.Position = new Point(current_X *20, current_Y*20);

                }

                // Leave the cursor where it was and Do mouse click and other mouse events, if finger count >= 2


                if (fingerNum == 2)
                {
                      DoLeftMouseClick();                     // function clicks mouse left button
                }

                if (fingerNum == 3)
                {
                    

                    DoRightMouseClick();                     // function clicks mouse right button





                }



                if (fingerNum == 4)
                {

                    Process.Start("taskmgr.exe");

                    //formMinimize();
                    

                    
                }


                if (fingerNum == 5)
                {

                    Application.Exit();
                    

                }

















            }//BTN ENDS


    









            
        }//fingercount method end







        


        // function for mouse clicks

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData,
         int dwExtraInfo);

        
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;          // mouse left button pressed 
        private const int MOUSEEVENTF_LEFTUP = 0x04;            // mouse left button unpressed
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;         // mouse right button pressed
        private const int MOUSEEVENTF_RIGHTUP = 0x10;           // mouse right button unpressed

        //this function will click the mouse using the parameters assigned to it
        public void DoLeftMouseClick()
        {
            //Call the imported function with the cursor's current position
            uint X = Convert.ToUInt32(Cursor.Position.X);
            uint Y =Convert.ToUInt32(Cursor.Position.Y);


            

            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
           

        }

        public void DoRightMouseClick()
        {
            uint X = Convert.ToUInt32(Cursor.Position.X);
            uint Y = Convert.ToUInt32(Cursor.Position.Y);
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);

        }

        public void formMinimize()
        {


            Form1 myform = new Form1();
              if (myform.WindowState != FormWindowState.Minimized)
                myform.WindowState = FormWindowState.Minimized;




        }
    












        


        private void imageBoxSkin_Click(object sender, EventArgs e)
        {

        }

        private void imageBoxFrameGrabber_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

           
            if (!button_pressed)
            {
                button_pressed = true;                                              // change Flag
                button1.Text = "Shift Mouse Control to Mouse Device";       // Change button text
                 FrameGrabber(sender,e);

            }
            else
            {
                button1.Text = "Shift Mouse Control to Webcam";             // Change text
                button_pressed = false;                                             // change flag
                
            }


    




        }//btn click end

        private void button2_Click(object sender, EventArgs e)
        {
            

            


        }
    }//class

    

}//namespace

