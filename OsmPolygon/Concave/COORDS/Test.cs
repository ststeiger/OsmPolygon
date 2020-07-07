
namespace OsmPolygon.Concave.COORDS
{


    class TestCoordinateConverion
    {


        public static void DrawPoint(System.Drawing.Graphics g2, System.Drawing.Pen pen, Vector3D v)
        {
            // Plane planeX = new Plane(new Vector3D(1, 0, 0));
            Plane planeX = new Plane(new Vector3D(1, 0, 1));
            Plane planeY = new Plane(new Vector3D(0, 1, 0)); // Must be orthogonal plane of planeX

            double worldUnitX = 100; // (x + offsetX) * 128.0D;
            double worldUnitY = 100; // (y + offsetY) * 128.0D;


            

            g2.DrawLine(pen, 0, 0,
                    (int)(worldUnitX * planeX.getOffset(v)),
                    (int)(worldUnitY * planeY.getOffset(v))
            );
        }


        public static void PaintComponent(System.Drawing.Graphics g)
        {
            System.Drawing.Pen redPen = System.Drawing.Pens.Red;
            System.Drawing.Pen greenPen = System.Drawing.Pens.Green;
            System.Drawing.Pen bluePen = System.Drawing.Pens.Blue;
            System.Drawing.Pen blackPen = System.Drawing.Pens.Black;


            DrawPoint(g, redPen, new Vector3D(2, 1, 0));
            DrawPoint(g, greenPen, new Vector3D(0, 2, 0));
            DrawPoint(g, bluePen, new Vector3D(0, 0, 2));
            DrawPoint(g, blackPen, new Vector3D(1, 1, 1));
        }


        public static void Test()
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1000, 1000))
            {

                using (System.Drawing.Graphics gfx = System.Drawing.Graphics.FromImage(bmp))
                {
                    gfx.Clear(System.Drawing.Color.White);
                    PaintComponent(gfx);
                }

                bmp.Save(@"D:\foo.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            System.Console.WriteLine("Drawn !");


            // https://math.stackexchange.com/questions/2305792/3d-projection-on-a-2d-plane-weak-maths-ressources
            // https://stackoverflow.com/questions/724219/how-to-convert-a-3d-point-into-2d-perspective-projection
            // https://stackoverflow.com/questions/15079764/3d-to-2d-point-conversion
            // https://github.com/Fylax/Apache-Commons-Math3-C-
            // https://github.com/mrdefnerd/Arrav-Server-Provider/blob/dce7fae5f9f8ccec0f795c1cbe486ef570c41273/org/rev317/api/methods/Calculations.java

        }


    }


}
