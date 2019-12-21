/*     Author: M.ALI(17-10278)
 * 
 * 
 *   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;

namespace HandGestureRecognition.SkinDetector
{
    public class YCrCbSkinDetector:CustomIColorSkinDetector
    {
        public override Image<Gray, byte> DetectSkin(Image<Bgr, byte> Img, IColor min, IColor max)
        {
            Image<Ycc, Byte> currentYCrCbFrame = Img.Convert<Ycc, Byte>();
            Image<Gray, byte> skin = new Image<Gray, byte>(Img.Width, Img.Height);
            skin = currentYCrCbFrame.InRange((Ycc)min,(Ycc) max);


            // Create a structuring element of the specific type

            //Erodes the source image using the specified structuring element that determines the shape of a pixel neighborhood over 
            
            
            




                 StructuringElementEx rect_12 = new StructuringElementEx(12, 12, 6, 6, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            //iterating only once
            //src,des,element,iterations
            CvInvoke.cvErode(skin, skin, rect_12, 1);
            StructuringElementEx rect_6 = new StructuringElementEx(6, 6, 3, 3, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            //iterating twice
            CvInvoke.cvDilate(skin, skin, rect_6, 2);
            return skin;
        }
        
    }
}
