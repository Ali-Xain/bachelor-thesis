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
    public abstract class CustomIColorSkinDetector
    {

        //TCOLOR>>Color mode.....
        //Bit depth/Color depth>>no of bits used to show the color of a pixel
        //nbit then 2^n colors
        //pixel stored in three dimesional array
        //img.data ll access that arary
       public abstract Image<Gray, Byte> DetectSkin(Image<Bgr, Byte> Img, IColor min, IColor max);


       
        
    }
}
