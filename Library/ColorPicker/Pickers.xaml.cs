using ColorPicker.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColorPicker
{

    /// <summary>
    /// ColorPicker.xaml 的交互逻辑
    /// </summary>
    public partial class Pickers : UserControl
    {
        private const string PATTERN = @"^[0-9A-Fa-f]+$";

        private double ViewSelectColor_X = 0;
        private double ViewSelectColor_Y = 0;

        public ColorModel ColorData { get; private set; }

        #region delegate event
        /// <summary>
        /// 声明路由事件
        /// 参数:要注册的路由事件名称，路由事件的路由策略，事件处理程序的委托类型(可自定义)，路由事件的所有者类类型
        /// </summary>
        public static readonly RoutedEvent ChanageColorEvent = EventManager.RegisterRoutedEvent("ChanageColorEvent", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventArgs<Object>), typeof(Pickers));

        /// <summary>
        /// 处理各种路由事件的方法 
        /// </summary>
        public event RoutedEventHandler ChanageColor
        {
            add { AddHandler(ChanageColorEvent, value); }
            remove { RemoveHandler(ChanageColorEvent, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public HsbaColor SelectColor
        {
            get { return (HsbaColor)GetValue(SelectColorProperty); }
            set { SetValue(SelectColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectColorProperty =
            DependencyProperty.Register("SelectColor", typeof(string), typeof(Pickers), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion


        private int R = 255;
        private int G = 255;
        private int _B = 255;
        private int A = 255;

        private double H = 0;
        private double S = 1;
        private double B = 1;

        public Pickers()
        {
            InitializeComponent();
        }

        private void ColorsThumb_ValueChanged(double xpercent, double ypercent)
        {
            Debug.WriteLine("ThumbPro X:" + xpercent);
            Debug.WriteLine("ThumbPro Y:" + ypercent);
            H = 360 * xpercent;
            HsbaColor Hcolor = new HsbaColor(H, 1, 1, 1);
            viewSelectColor.Fill = Hcolor.SolidColorBrush;

            Hcolor = new HsbaColor(H, S, B, 1);
            ColorChange(Hcolor.RgbaColor);

            if (ColorData != null)
            {
                ColorData.SelectColor = Hcolor;
            }
            ViewSelectThumb_ValueChanged(ViewSelectColor_X, ViewSelectColor_Y);
            OnColorChanageEvent();
        }


        private void ViewSelectThumb_ValueChanged(double xpercent, double ypercent)
        {
            Debug.WriteLine("ThumbSB X:" + xpercent);
            Debug.WriteLine("ThumbSB Y:" + ypercent);
            S = xpercent;
            B = 1 - ypercent;
            HsbaColor Hcolor = new HsbaColor(H, S, B, 1);

            Color selectColor = Hcolor.Color;
            transparentColor.Color = new HsbaColor(selectColor.R, selectColor.G, selectColor.B, 30).Color;
            gradientColor.Color = selectColor;

            ColorChange(Hcolor.RgbaColor);

            ViewSelectColor_X = xpercent;
            ViewSelectColor_Y = ypercent;

            if (ColorData != null)
            {
                ColorData.SelectColor = Hcolor;
                TransparentThum.SetTopLeftByPercent(Hcolor.A, 1);
            }
            OnColorChanageEvent();
        }

        private void TransparentThum_ValueChanged(double xpercent, double ypercent)
        {
            Debug.WriteLine("Transparent X:" + xpercent);
            Debug.WriteLine("Transparent Y:" + ypercent);
            if (xpercent < 0.3)
            {
                xpercent = 0.3;
            }
            HsbaColor Hcolor = new HsbaColor(H, S, B, xpercent);

            Color selectColor = Hcolor.Color;
            ColorChange(Hcolor.RgbaColor);

            if (ColorData != null)
            {
                ColorData.SelectColor = Hcolor;
            }
            OnColorChanageEvent();
        }


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string text = textBox.Text;
            if (!int.TryParse(TextR.Text, out int Rvalue) || (Rvalue > 255 || Rvalue < 0))
            {
                TextR.Text = R.ToString();
                return;
            }
            if (!int.TryParse(TextG.Text, out int Gvalue) || (Gvalue > 255 || Gvalue < 0))
            {
                TextG.Text = G.ToString();
                return;
            }
            if (!int.TryParse(TextB.Text, out int Bvalue) || (Bvalue > 255 || Bvalue < 0))
            {
                TextB.Text = _B.ToString();
                return;
            }
            if (!int.TryParse(TextA.Text, out int Avalue) || (Avalue > 255 || Avalue < 0))
            {
                TextA.Text = A.ToString();
                return;
            }
            R = Rvalue; G = Gvalue; _B = Bvalue; A = Avalue;

            HsbaColor Hcolor = new HsbaColor(R, G, _B, A);
            TextHex.Text = Hcolor.HexString;
            SetHabaColorToView(Hcolor);
            if (ColorData != null)
            {
                ColorData.SelectColor = Hcolor;
            }
            OnColorChanageEvent();
        }

        private void HexTextLostFocus(object sender, RoutedEventArgs e)
        {
            HsbaColor Hcolor = null;
            try
            {
                if (IsIllegalHexadecimal(TextHex.Text))
                {
                    Hcolor = new HsbaColor(TextHex.Text);
                    Color color = Hcolor.Color;
                    TextR.Text = color.R.ToString();
                    TextG.Text = color.G.ToString();
                    TextB.Text = color.B.ToString();
                    TextA.Text = color.A.ToString();
                    TextHex.Text = Hcolor.HexString;

                    SetHabaColorToView(Hcolor);
                    OnColorChanageEvent();
                }
                else
                {
                    if (ColorData != null)
                    {
                        TextHex.Text = ColorData.SelectColor.HexString;
                    }
                }
            }
            catch
            {
                if (ColorData != null)
                {
                    TextHex.Text = ColorData.SelectColor.HexString;
                }
            }
            finally
            {
                if (ColorData != null && Hcolor != null)
                {
                    ColorData.SelectColor = Hcolor;
                }

            }
        }

        /// <summary>
        /// 判断十六进制字符串hex是否正确
        /// </summary>
        /// <param name="hex">十六进制字符串</param>
        /// <returns>true：不正确，false：正确</returns>
        private bool IsIllegalHexadecimal(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return false;
            }
            if (!hex.StartsWith("#"))
            {
                return false;
            }
            string hexText = hex.Replace("#", "");
            return System.Text.RegularExpressions.Regex.IsMatch(hexText, PATTERN);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hcolor"></param>
        private void SetHabaColorToView(HsbaColor Hcolor)
        {
            if (Hcolor != null)
            {
                H = Hcolor.H;
                viewSelectColor.Fill = new HsbaColor(H, 1, 1, 1).SolidColorBrush;

                Color selectColor = Hcolor.Color;
                S = Hcolor.S;
                B = Hcolor.B;
                transparentColor.Color = new HsbaColor(selectColor.R, selectColor.G, selectColor.B, 30).Color;
                gradientColor.Color = selectColor;

                ColorsThumb.SetTopLeftByPercent(H / 360, 1);
                TransparentThum.SetTopLeftByPercent(Hcolor.A, 1);
                ViewSelectThumb.SetTopLeftByPercent(S, 1 - B);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hcolor"></param>
        private void ColorChange(RgbaColor Hcolor)
        {
            TextR.Text = Hcolor.R.ToString();
            TextG.Text = Hcolor.G.ToString();
            TextB.Text = Hcolor.B.ToString();
            TextA.Text = Hcolor.A.ToString();
            TextHex.Text = Hcolor.HexString;
        }

        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            HsbaColor hsbaColor = new HsbaColor(R, G, _B, A);
            ColorData = new ColorModel();
            ColorData.SelectColor = hsbaColor;
        }

        public void SetColorToPicker(ColorModel colorModel)
        {
            if (colorModel != null)
            {
                //SetHabaColorToView(colorModel.SelectColor);
                ColorChange(colorModel.SelectColor.RgbaColor);
            }
        }

        private void OnColorChanageEvent()
        {
            if (ColorData != null)
            {
                this.RaiseEvent(new RoutedEventArgs(ChanageColorEvent, ColorData));
            }
        }

        private void TransparentThum_Loaded(object sender, RoutedEventArgs e)
        {
            TransparentThum.Left = 200;
        }
    }


}
