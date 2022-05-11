using ColorLibrary.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorLibrary
{
    public class ColorModel
    {
        public ColorModel()
        {

        }
        private HsbaColor selectColor = null;

        public HsbaColor SelectColor { get => selectColor; set => selectColor = value; }


    }
}
