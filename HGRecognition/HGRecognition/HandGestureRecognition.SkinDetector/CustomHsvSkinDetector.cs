/*     Author: M.ALI(17-10278)
 * 
 * 
 *   */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace HandGestureRecognition.SkinDetector
{
    public class CustomHsvSkinDetector:CustomIColorSkinDetector
    {
                

        //return skin if thall falls in range (HSV COLOR MODEL);
        public override Image<Gray, byte> DetectSkin(Image<Bgr, byte> Img, IColor min, IColor max)
        {
            Image<Hsv, Byte> currentHsvFrame = Img.Convert<Hsv, Byte>();
            Image<Gray, byte> skin = new Image<Gray, byte>(Img.Width, Img.Height);
            skin = currentHsvFrame.InRange((Hsv)min,(Hsv)max);
            return skin;
        }
    }
}
