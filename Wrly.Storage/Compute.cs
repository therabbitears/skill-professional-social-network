using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Storage
{
    public class Compute
    {
        public static Size Dimensions(Size objOriginalSize, int intTargetSize)
        {
            Size newSize = new Size();
            if (objOriginalSize.Width > objOriginalSize.Height)
            {
                if (objOriginalSize.Width > intTargetSize)
                {
                    newSize.Width = intTargetSize;
                    newSize.Height = (int)(objOriginalSize.Height * (float)intTargetSize / (float)objOriginalSize.Width);
                }
                else
                {
                    newSize.Height = (int)(objOriginalSize.Height * (float)objOriginalSize.Width / (float)objOriginalSize.Width);
                    newSize.Width = objOriginalSize.Width;
                }
            }
            else
            {
                if (objOriginalSize.Height > intTargetSize)
                {
                    newSize.Width = (int)(objOriginalSize.Width * (float)intTargetSize / (float)objOriginalSize.Height);
                    newSize.Height = intTargetSize;
                }
                else
                {
                    newSize.Width = (int)(objOriginalSize.Width * (float)objOriginalSize.Height / (float)objOriginalSize.Height);
                    newSize.Height = objOriginalSize.Height;
                }
            }
            return newSize;
        }

    }
}
