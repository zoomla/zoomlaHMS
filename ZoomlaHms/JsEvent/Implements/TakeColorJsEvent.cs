using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZoomlaHms.JsEvent.Implements
{
    public class TakeColorJsEvent : IJsEvent
    {
        private ZPoint.POINT lastPoint;
        private bool taking = false;
        private Color color = Color.FromArgb(255, 255, 255, 255);
        private byte[] shortcut = null;

        public TakeColorJsEvent()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(50);
                    if (!taking)
                    { continue; }

                    ZPoint.POINT point;
                    ZPoint.GetCursorPos(out point);
                    if (lastPoint.X != point.X || lastPoint.Y != point.Y)
                    {
                        lastPoint = point;
                        color = ScreenColor.GetPixelColor(point.X, point.Y);
                    }

                    int sw = (int)SystemParameters.PrimaryScreenWidth;
                    int sh = (int)SystemParameters.PrimaryScreenHeight;
                    int tw = 100;
                    int th = 100;

                    int rectX = Math.Max(point.X - tw / 2, 0);
                    int rectY = Math.Max(point.Y - th / 2, 0);

                    if (rectX + tw > sw)
                    { rectX = sw - tw; }
                    if (rectY + th > sh)
                    { rectY = sh - th; }


                    Bitmap screenBlock = new Bitmap(tw, th);
                    using (var g = Graphics.FromImage(screenBlock))
                    {
                        g.Clear(Color.White);
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                        g.CopyFromScreen(rectX, rectY, 0, 0, new System.Drawing.Size(screenBlock.Width, screenBlock.Height), CopyPixelOperation.SourceCopy);
                    }
                    using (var ms = new System.IO.MemoryStream())
                    {
                        screenBlock.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        shortcut = ms.ToArray();
                    }
                }
            });
        }

        public string GetScreenBlockBase64Image()
        {
            string def = "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";

            if (shortcut == null)
            {
                return def;
            }

            try
            {
                return Convert.ToBase64String(shortcut);
            }
            catch (Exception)
            {
                return def;
            }
        }

        public string GetHexColor()
        {
            return ColorTranslator.ToHtml(color).TrimStart('#');
        }

        public string GetRgbColor()
        {
            return string.Join(",", color.R, color.G, color.B);
        }

        public void StartTake()
        {
            taking = true;
        }

        public void StopTake()
        {
            taking = false;
        }
    }
}
