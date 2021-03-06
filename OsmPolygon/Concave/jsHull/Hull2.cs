﻿
using System.Linq;


namespace OsmPolygon.Concave
{


    public class FlatQueue<TValue>
        where TValue : System.IComparable<TValue>
    {

        private int Length;
        private System.Collections.Generic.List<TValue> values;
        public System.Collections.Generic.List<int> ids;


        public FlatQueue()
        {
                     
            this.ids = new System.Collections.Generic.List<int>();
            this.values = new System.Collections.Generic.List<TValue>();
            this.Length = 0;
        }



        public void Clear()
        {
            this.ids.Clear();
            this.values.Clear();
            this.Length = 0;
        }

        public void Push(int id, TValue value)
        {
            this.ids.Add(id);
            this.values.Add(value);

            int pos = this.Length++;
            while (pos > 0)
            {
                int parent = (pos - 1) >> 1;
                TValue parentValue = this.values[parent];

                // if (value >= parentValue) break;
                if (value.CompareTo(parentValue) > -1) 
                    break;

                this.ids[pos] = this.ids[parent];
                this.values[pos] = parentValue;
                pos = parent;
            }

            this.ids[pos] = id;
            this.values[pos] = value;
        }



        public int? Pop()
        {
            if (this.Length == 0) return null;

            int top = this.ids[0];
            this.Length--;

            if (this.Length > 0)
            {
                int id = this.ids[0] = this.ids[this.Length];
                TValue value = this.values[0] = this.values[this.Length];
                int halfLength = this.Length >> 1;
                int pos = 0;

                while (pos < halfLength)
                {
                    int left = (pos << 1) + 1;
                    int right = left + 1;
                    int bestIndex = this.ids[left];
                    TValue bestValue = this.values[left];
                    TValue rightValue = this.values[right];

                    // if (right < this.length && rightValue < bestValue)
                    if (right < this.Length && rightValue.CompareTo(bestValue) == -1)
                    {
                        left = right;
                        bestIndex = this.ids[right];
                        bestValue = rightValue;
                    }

                    // if (bestValue >= value) break;
                    if (bestValue.CompareTo(value) >= 0)
                        break;

                    this.ids[pos] = bestIndex;
                    this.values[pos] = bestValue;
                    pos = left;
                }

                this.ids[pos] = id;
                this.values[pos] = value;
            }

            this.ids.RemoveAt(this.ids.Count - 1);
            this.values.RemoveAt(this.values.Count - 1);
            
            return top;
        }

        public int Peek()
        {
            return this.ids[0];
        }

        public TValue PeekValue()
        {
            return this.values[0];
        }
    }


    public class Circumcircles
    {
        public float[] x;
        public float[] y;
        public float[] r;
    }
    
        
    public class BoundingBox
    {
        public float minX;
        public float minY;
        public float maxX;
        public float maxY;
    }
    

    class Hull2
    {



        //const {triangles, halfedges} = delaunay;

        public static Circumcircles ComputeCircumcircles(DelaunatorSharp.IPoint[] points, DelaunatorSharp.Delaunator delaunay)
        {
            int n = delaunay.Halfedges.Length;
            float[] x = new float[n];
            float[] y = new float[n];
            float[] r = new float[n];

            for (int i = 0; i < n; i++)
            {
                DelaunatorSharp.IPoint pt1 = points[delaunay.Triangles[i - i % 3 + 0]];
                float ax = (float)pt1.X;
                float ay = (float)pt1.Y;


                DelaunatorSharp.IPoint pt2 = points[delaunay.Triangles[i - i % 3 + 1]];
                float bx = (float)pt2.X;
                float by = (float)pt2.Y;

                DelaunatorSharp.IPoint pt3 = points[delaunay.Triangles[i - i % 3 + 2]];
                float cx = (float)pt3.X;
                float cy = (float)pt3.Y;

                float dx = bx - ax;
                float dy = by - ay;
                float ex = cx - ax;
                float ey = cy - ay;
                float bl = dx * dx + dy * dy;
                float cl = ex * ex + ey * ey;
                float d = 0.5f / (dx * ey - dy * ex);
                float x2 = (ey * bl - dy * cl) * d;
                float y2 = (dx * cl - ex * bl) * d;
                x[i] = ax + x2;
                y[i] = ay + y2;
                r[i] = (float)System.Math.Sqrt(x2 * x2 + y2 * y2);
            }

            return new Circumcircles() { x = x, y = y, r = r };
        }




        public static void addToQueue(FlatQueue<float> queue, DelaunatorSharp.Delaunator delauny
            , DelaunatorSharp.IPoint[] points, Circumcircles circumcircles, bool[] onEdge, bool[] visited, int i)
        {
            onEdge[i] = true;

            visited[delauny.Triangles[i]] = true;
            
            DelaunatorSharp.IPoint p0 = points[delauny.Triangles[i]];
            DelaunatorSharp.IPoint p1 = points[delauny.Triangles[i % 3 == 2 ? i - 2 : i + 1]];

            float r = circumcircles.r[i];
            float x = circumcircles.x[i];
            float y = circumcircles.y[i];


            float x0 = (float)p0.X;
            float y0 = (float)p0.Y;

            float x1 = (float)p1.X;
            float y1 = (float)p1.Y;
            
            // var [x0, y0] = p0;
            // var[x1, y1] = p1;
            
            float area = (y - y0) * (x1 - x) - (x - x0) * (y1 - y);
            float cx = (x0 + x1) / 2;
            float cy = (y0 + y1) / 2;
            float d = (float)System.Math.Sqrt(System.Math.Pow(cx - x, 2) + System.Math.Pow(cy - y, 2));

            // if the center of a circumcircle on the edge lies outside the advancing polygon, 
            // or inside but less than 0.2 circumradiuses away from the edge, queue it for collapsing
            if (area >= 0f || d / r < 0.2f)
                queue.Push(i, -r);
        }
        
        
        public static BoundingBox GetBbox(DelaunatorSharp.IPoint[] points)
        {
            float minX = float.PositiveInfinity;
            float minY = float.PositiveInfinity;
            float maxX = float.NegativeInfinity;
            float maxY = float.NegativeInfinity;

            foreach (DelaunatorSharp.IPoint pt in points)
            {
                minX = (float)System.Math.Min(pt.X, minX);
                minY = (float)System.Math.Min(pt.Y, minY);
                maxX = (float)System.Math.Max(pt.X, maxX);
                maxY = (float)System.Math.Max(pt.Y, maxY);
            }

            // return [0      1     2       3]
            // return [minX, minY, maxX, maxY]
            return new BoundingBox() { minX = minX, minY = minY, maxX = maxX, maxY = maxY };
        }


        public static float projX(float x, float padding, float scale, BoundingBox bbox)
        {
            return padding + (x - bbox.minX) * scale;
        }


        public static float projY(float y, float padding, float scale, BoundingBox bbox)
        {
            return padding + (y - bbox.minY) * scale;
        }



        public static void Draw(
              DelaunatorSharp.Delaunator delaunay
            , DelaunatorSharp.IPoint[] points
            , BoundingBox bbox
            , Circumcircles circumcircles
            , System.Drawing.Graphics ctx
            , float padding
            , float scale
            , FlatQueue<float> queue
            , bool[] onEdge
        )
        {
            // ctx.clearRect(0, 0, width, height);  
            ctx.Clear(System.Drawing.Color.White);


            System.Drawing.Drawing2D.GraphicsPath path1 = new System.Drawing.Drawing2D.GraphicsPath();
            
            int[] t = delaunay.Triangles;
            for (int i = 0; i < t.Length; i += 3)
            {
                DelaunatorSharp.IPoint pt1 = points[t[i + 0]];
                float ax = (float)pt1.X;
                float ay = (float)pt1.Y;


                DelaunatorSharp.IPoint pt2 = points[t[i + 1]];
                float bx = (float)pt2.X;
                float by = (float)pt2.Y;


                DelaunatorSharp.IPoint pt3 = points[t[i + 2]];
                float cx = (float)pt3.X;
                float cy = (float)pt3.Y;

                path1.StartFigure();
                path1.AddLine(projX(ax, padding, scale, bbox), projY(ay, padding, scale, bbox), projX(bx, padding, scale, bbox), projY(by, padding, scale, bbox));
                path1.AddLine(projX(bx, padding, scale, bbox), projY(by, padding, scale, bbox), projX(cx, padding, scale, bbox), projY(cy, padding, scale, bbox));
                path1.CloseFigure();
            }

            System.Drawing.Pen trianglePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb((int)(0.4*255),0,200, 0));
            trianglePen.Width = 0.5f;
            trianglePen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
            ctx.DrawPath(trianglePen, path1);
            
            System.Drawing.Drawing2D.GraphicsPath path2 = new System.Drawing.Drawing2D.GraphicsPath();
            

            // for (const [x, y] of points) 
            foreach (DelaunatorSharp.IPoint thisPoint in points)
            {
                float sx = projX((float)thisPoint.X, padding, scale, bbox);
                float sy = projY((float)thisPoint.Y, padding, scale, bbox);
                float r = 1.5f;
                r = 5;

                path2.StartFigure();
                path2.AddArc(sx - r / 2.0f, sy - r / 2.0f, r, r, 0.0f, 360.0f);
                path2.CloseFigure();
            }

            System.Drawing.Brush blackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            ctx.FillPath(blackBrush, path2);
             

            
            // ctx.beginPath(); 
            System.Drawing.Drawing2D.GraphicsPath path3 = new System.Drawing.Drawing2D.GraphicsPath();
            // path3.StartFigure();
            for (int i = 0; i < onEdge.Length; i++)
            {
                if (!onEdge[i])
                    continue;
                
                DelaunatorSharp.IPoint pt1 = points[t[i]];
                float ax = (float)pt1.X;
                float ay = (float)pt1.Y;

                DelaunatorSharp.IPoint pt2 = points[t[i % 3 == 2 ? i - 2 : i + 1]];
                float bx = (float)pt2.X;
                float by = (float)pt2.Y;

                path3.StartFigure();
                path3.AddLine(projX(ax, padding, scale, bbox), projY(ay, padding, scale, bbox), projX(bx, padding, scale, bbox), projY(by, padding, scale, bbox));
                path3.CloseFigure();
                // ctx.moveTo(projX(ax), projY(ay));
                // ctx.lineTo(projX(bx), projY(by));
                // ctx.closePath();  
            }
            // path3.CloseFigure();
            System.Drawing.Pen hullPen = new System.Drawing.Pen(System.Drawing.Color.Blue);
            hullPen.Width = 2.0f;
            ctx.DrawPath(hullPen, path3);
            
            
            
            System.Drawing.Drawing2D.GraphicsPath path4 = new System.Drawing.Drawing2D.GraphicsPath();
            foreach (int i in queue.ids)
            {
                // ctx.beginPath();  
                path4.StartFigure();
                float sr = circumcircles.r[i] * scale;
                float sx = projX(circumcircles.x[i], padding, scale, bbox);
                float sy = projY(circumcircles.y[i], padding, scale, bbox);
                //ctx.moveTo(sx + sr, sy);    
                //ctx.arc(sx, sy, sr, 0, Math.PI* 2, false);    
                //ctx.strokeStyle = 'rgba(200,0,0,1)';    
                //ctx.lineWidth = 1;    
                //ctx.stroke();    
                //ctx.fillStyle = 'rgba(255,255,0,0.2)';    
                //ctx.fill();  
                
                path4.AddArc(sx - sr / 2.0f, sy - sr / 2.0f, sr, sr, 0.0f, 360.0f);
                path4.CloseFigure();
            }
            
            System.Drawing.SolidBrush circleBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb((int)(0.2 * 255), 255, 255, 0));
            System.Drawing.Pen redCircle = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255,200,0,0));
            redCircle.Width = 1;
            redCircle.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
            ctx.FillPath(circleBrush, path4);
            ctx.DrawPath(redCircle, path4);
        }



        // https://api.observablehq.com/document/44b78126ef9b422b@596
        /*
        var points = [[168, 180], [168, 178], [168, 179], [168, 181], [168, 183], [167, 183], [167, 184], [165, 184], [162, 186], [164, 188], [161, 188], [160, 191], [158, 193], [156, 193], [152, 195], [152, 198], [150, 198], [147, 198], [148, 205], [150, 210], [148, 210], [148, 208], [145, 206], [142, 206], [140, 206], [138, 206], [135, 206], [135, 209], [131, 209], [131, 211], [127, 211], [124, 210], [120, 207], [120, 204], [120, 202], [124, 201], [123, 201], [125, 198], [125, 194], [127, 194], [127, 191], [130, 191], [132, 189], [134, 189], [134, 186], [136, 184], [134, 182], [134, 179], [134, 176], [136, 174], [139, 174], [141, 177], [142, 176], [144, 176], [147, 178], [148, 176], [151, 178], [154, 178], [153, 175], [152, 174], [152, 170], [152, 168], [150, 166], [148, 166], [147, 165], [145, 162], [146, 160], [146, 157], [146, 155], [144, 155], [142, 152], [140, 150], [138, 150], [138, 148], [140, 145], [140, 142], [140, 138], [139, 138], [137, 138], [135, 138], [133, 135], [132, 132], [129, 132], [128, 132], [124, 132], [124, 130], [123, 130], [118, 126], [116, 124], [112, 122], [109, 122], [105, 122], [102, 124], [100, 124], [97, 124], [95, 126], [92, 127], [89, 127], [88, 130], [85, 132], [80, 134], [72, 134], [69, 134], [65, 138], [64, 138], [58, 137], [56, 133], [52, 133], [51, 133], [48, 133], [44, 133], [41, 131], [38, 130], [35, 130], [32, 127], [30, 127], [27, 127], [24, 127], [24, 126], [23, 124], [20, 122], [17, 122], [16, 118], [15, 116], [15, 110], [18, 108], [20, 102], [24, 97], [28, 102], [28, 98], [26, 97], [28, 94], [27, 85], [29, 79], [32, 76], [39, 70], [44, 66], [48, 65], [53, 61], [53, 58], [51, 54], [54, 54], [52, 48], [51, 43], [48, 42], [49, 38], [48, 34], [51, 30], [53, 33], [58, 30], [61, 30], [60, 27], [64, 26], [68, 24], [74, 24], [80, 24], [85, 26], [92, 26], [96, 29], [103, 32], [109, 33], [112, 37], [116, 37], [120, 37], [124, 35], [126, 35], [128, 38], [132, 38], [134, 41], [138, 38], [140, 36], [142, 40], [144, 43], [145, 41], [149, 41], [155, 41], [159, 41], [161, 46], [165, 46], [164, 42], [164, 39], [164, 34], [167, 30], [173, 24], [178, 24], [184, 24], [189, 26], [195, 21], [195, 20], [199, 20], [203, 20], [207, 17], [211, 17], [216, 17], [218, 16], [222, 22], [225, 27], [228, 31], [226, 34], [224, 34], [226, 39], [228, 43], [230, 46], [236, 46], [242, 46], [243, 50], [245, 50], [247, 54], [247, 56], [248, 60], [248, 65], [253, 66], [255, 64], [260, 64], [264, 67], [268, 71], [272, 66], [275, 66], [281, 61], [285, 66], [286, 70], [292, 74], [294, 74], [296, 74], [296, 71], [301, 74], [307, 74], [311, 78], [315, 74], [315, 77], [319, 77], [322, 82], [328, 82], [331, 81], [331, 84], [333, 86], [333, 90], [330, 95], [326, 98], [328, 99], [332, 98], [333, 101], [331, 104], [329, 104], [327, 106], [329, 111], [332, 116], [333, 119], [333, 122], [332, 126], [332, 130], [327, 130], [321, 130], [317, 130], [315, 134], [312, 134], [308, 138], [306, 138], [306, 144], [306, 149], [306, 152], [301, 152], [297, 154], [295, 154], [292, 154], [292, 158], [288, 158], [283, 162], [281, 164], [279, 163], [276, 163], [273, 166], [272, 169], [268, 168], [265, 170], [260, 172], [256, 176], [252, 176], [248, 181], [246, 182], [246, 189], [246, 194], [248, 197], [250, 198], [252, 200], [252, 203], [254, 205], [260, 205], [264, 202], [267, 202], [269, 202], [272, 199], [280, 199], [278, 202], [278, 207], [278, 211], [276, 211], [272, 213], [268, 213], [265, 213], [264, 211], [262, 210], [260, 210], [257, 212], [257, 214], [255, 217], [253, 217], [253, 221], [249, 220], [247, 220], [243, 222], [240, 223], [239, 226], [234, 231], [229, 231], [224, 231], [219, 227], [220, 227], [222, 224], [222, 222], [222, 219], [224, 217], [222, 214], [220, 212], [217, 210], [215, 210], [211, 209], [208, 206], [202, 209], [202, 205], [206, 202], [211, 198], [216, 195], [220, 192], [224, 192], [221, 186], [218, 186], [214, 185], [208, 185], [204, 186], [200, 186], [193, 183], [190, 182], [188, 182], [190, 178], [186, 178], [184, 174], [182, 171], [178, 171], [173, 174], [169, 174], [169, 175], [169, 179], [167, 182], [164, 186], [160, 192], [155, 195], [152, 198], [150, 198], [148, 198], [148, 202], [151, 208], [148, 210], [146, 208], [144, 205], [140, 205], [137, 208], [132, 208], [132, 210], [127, 210], [124, 210], [120, 206], [120, 202], [123, 202], [124, 201], [124, 198], [128, 195], [131, 191], [133, 187], [135, 183], [130, 203], [129, 208], [123, 203], [129, 203], [129, 198], [133, 198], [136, 200], [142, 200], [143, 199], [143, 197], [137, 196], [136, 194], [133, 194], [136, 186], [136, 182], [141, 186], [144, 186], [150, 186], [150, 190], [155, 190], [159, 188], [156, 182], [151, 182], [144, 182], [164, 176], [161, 177], [157, 177], [166, 176], [168, 165], [175, 167], [180, 167], [188, 159], [195, 164], [195, 162], [187, 162], [178, 163], [173, 166], [168, 170], [156, 170], [157, 165], [164, 165], [164, 161], [170, 159], [167, 158], [159, 154], [149, 151], [145, 145], [145, 138], [152, 138], [152, 146], [159, 146], [165, 153], [176, 153], [180, 153], [187, 153], [194, 153], [202, 153], [202, 158], [197, 158], [193, 158], [193, 142], [180, 142], [171, 142], [163, 135], [176, 135], [186, 139], [201, 139], [206, 139], [205, 147], [205, 160], [198, 160], [206, 174], [205, 178], [196, 178], [196, 182], [202, 182], [206, 181], [209, 181], [215, 181], [222, 181], [230, 177], [238, 175], [241, 175], [237, 175], [237, 168], [237, 161], [232, 156], [231, 162], [225, 166], [217, 169], [210, 173], [224, 173], [227, 173], [235, 175], [237, 178], [228, 192], [222, 199], [216, 199], [211, 204], [205, 206], [219, 207], [222, 211], [229, 214], [236, 214], [244, 211], [247, 211], [268, 206], [277, 201], [279, 201], [281, 202], [278, 202], [242, 178], [236, 170], [236, 162], [255, 162], [251, 156], [240, 156], [253, 152], [261, 152], [277, 157], [268, 151], [255, 143], [260, 142], [267, 145], [271, 149], [273, 154], [258, 146], [257, 131], [256, 134], [248, 137], [260, 137], [260, 134], [271, 137], [276, 138], [276, 144], [289, 144], [285, 150], [294, 150], [298, 149], [301, 145], [292, 145], [282, 134], [276, 134], [283, 127], [282, 116], [277, 113], [283, 113], [288, 106], [296, 106], [297, 113], [297, 118], [298, 118], [310, 122], [310, 128], [300, 130], [300, 140], [292, 129], [292, 114], [283, 122], [289, 122], [299, 122], [299, 134], [294, 134], [288, 124], [314, 121], [311, 113], [308, 110], [304, 96], [299, 90], [299, 82], [305, 87], [309, 94], [311, 101], [312, 102], [314, 107], [320, 112], [320, 115], [326, 116], [323, 109], [321, 102], [321, 94], [321, 90], [328, 90], [328, 88], [316, 88], [316, 84], [307, 84], [290, 77], [289, 88], [289, 97], [278, 97], [268, 106], [268, 110], [261, 105], [255, 103], [244, 103], [252, 100], [252, 91], [252, 82], [242, 78], [252, 78], [259, 78], [264, 87], [267, 92], [272, 91], [272, 83], [264, 83], [260, 79], [276, 79], [283, 84], [283, 94], [289, 94], [284, 86], [272, 77], [253, 110], [248, 110], [239, 110], [234, 114], [222, 125], [219, 127], [219, 131], [219, 138], [219, 141], [224, 139], [224, 135], [225, 130], [232, 136], [240, 138], [237, 131], [237, 118], [248, 120], [256, 122], [262, 127], [255, 118], [245, 110], [207, 129], [199, 134], [195, 134], [188, 130], [180, 130], [165, 129], [156, 129], [165, 128], [173, 125], [185, 126], [193, 126], [201, 124], [204, 123], [208, 116], [214, 114], [207, 114], [196, 114], [183, 121], [183, 111], [189, 117], [196, 112], [172, 126], [164, 126], [159, 114], [174, 106], [186, 106], [192, 105], [184, 105], [184, 96], [173, 96], [163, 111], [159, 110], [152, 110], [168, 110], [171, 106], [183, 98], [193, 101], [219, 96], [225, 97], [225, 104], [232, 92], [240, 92], [237, 86], [229, 86], [216, 88], [214, 79], [203, 79], [203, 75], [212, 75], [221, 75], [229, 80], [230, 89], [217, 88], [217, 77], [228, 77], [228, 69], [235, 71], [240, 71], [244, 66], [236, 54], [236, 62], [232, 68], [229, 61], [216, 61], [212, 58], [212, 47], [212, 39], [214, 28], [215, 48], [225, 55], [236, 55], [202, 65], [202, 54], [202, 44], [202, 24], [198, 32], [199, 38], [192, 38], [185, 38], [174, 42], [174, 48], [178, 51], [184, 51], [194, 55], [191, 68], [182, 68], [174, 69], [167, 67], [153, 59], [153, 49], [147, 49], [152, 58], [152, 74], [154, 83], [161, 83], [165, 88], [153, 97], [153, 89], [152, 82], [168, 88], [168, 101], [156, 102], [156, 119], [173, 110], [184, 110], [177, 106], [160, 106], [145, 125], [137, 122], [131, 120], [124, 120], [122, 118], [113, 118], [114, 111], [129, 111], [140, 110], [143, 106], [137, 102], [127, 102], [119, 98], [126, 93], [139, 93], [139, 99], [141, 95], [128, 89], [118, 74], [128, 76], [135, 76], [141, 83], [141, 71], [137, 61], [137, 50], [129, 50], [118, 50], [109, 52], [112, 61], [123, 60], [134, 60], [129, 76], [121, 67], [124, 76], [123, 76], [111, 74], [128, 73], [109, 83], [109, 94], [105, 103], [102, 118], [92, 113], [98, 105], [99, 93], [94, 93], [94, 81], [99, 81], [100, 73], [100, 89], [100, 60], [100, 55], [105, 37], [101, 34], [93, 37], [90, 37], [90, 49], [99, 49], [88, 68], [80, 68], [78, 64], [88, 62], [86, 77], [76, 89], [71, 91], [71, 106], [78, 106], [82, 118], [84, 110], [71, 104], [76, 103], [76, 91], [78, 83], [85, 89], [83, 103], [83, 119], [76, 130], [62, 130], [68, 127], [74, 126], [83, 123], [62, 123], [56, 123], [59, 129], [59, 120], [49, 110], [46, 106], [56, 100], [62, 94], [62, 109], [72, 112], [67, 112], [57, 112], [61, 122], [60, 102], [52, 125], [44, 121], [36, 114], [32, 110], [20, 110], [22, 118], [35, 118], [44, 124], [32, 119], [22, 111], [44, 96], [36, 106], [36, 94], [32, 94], [35, 83], [44, 91], [52, 91], [52, 80], [59, 80], [62, 76], [62, 70], [47, 78], [55, 75], [64, 71], [64, 60], [58, 53], [58, 43], [65, 43], [65, 60], [76, 52], [73, 38], [76, 36], [93, 48], [89, 39], [99, 40], [98, 50], [94, 63], [117, 63], [131, 67], [131, 74], [142, 78], [140, 61], [124, 58], [124, 48], [136, 55], [236, 200], [228, 200], [226, 192], [232, 198], [238, 210], [248, 210], [236, 220], [230, 223], [230, 213], [175, 32], [172, 32], [171, 38], [184, 30]];

        var foo = [];

        for(let i = 0; i < points.length; ++i)
        {
           foo.push("new Point(" + points[i][0] + ", " + points[i][1] + "),");
        }

        console.log(foo.join("\r\n"));
        */



        public static DelaunatorSharp.IPoint[] EnhancePointDensity( DelaunatorSharp.IPoint[] points, int numPointsOnLine)
        {
            System.Collections.Generic.List<DelaunatorSharp.IPoint> newList =
                new System.Collections.Generic.List<DelaunatorSharp.IPoint>();
            
            for (int i = 0; i < points.Length; ++i)
            {
                DelaunatorSharp.IPoint thisPoint = points[i];
                newList.Add(thisPoint);

                if (i < points.Length - 1)
                {
                    DelaunatorSharp.IPoint nextPoint = points[i + 1];

                    double deltaX = nextPoint.X - thisPoint.X;
                    double deltaY = nextPoint.Y - thisPoint.Y;

                    deltaX = deltaX / numPointsOnLine;
                    deltaY = deltaY / numPointsOnLine;

                    for (int j = 0; j < numPointsOnLine; ++j)
                    {
                        DelaunatorSharp.IPoint newPoint = new DelaunatorSharp.Point(thisPoint.X + j * deltaX, thisPoint.Y + j * deltaY);
                        newList.Add(newPoint);
                    }
                }
            }

            return newList.ToArray();
        }


        // https://observablehq.com/@mourner/adaptive-concave-hull
        // https://mapbox.github.io/delaunator/
        public static void ComputeHull()
        {
            string jsonFile = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "DelaunatorPoints.json");
            jsonFile = System.IO.Path.GetFullPath(jsonFile);
            
            string json = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);
            
            // DelaunatorSharp.IPoint[] points = Newtonsoft.Json.JsonConvert.DeserializeObject<DelaunatorSharp.IPoint[]>(json);
            DelaunatorSharp.IPoint[] points = Newtonsoft.Json.JsonConvert.DeserializeObject<DelaunatorSharp.Point[]>(json).Cast<DelaunatorSharp.IPoint>().ToArray();
            points = EnhancePointDensity(points, 10);
            // string json = Newtonsoft.Json.JsonConvert.SerializeObject(points);
            // System.IO.File.WriteAllText(jsonFile, json,System.Text.Encoding.UTF8);
            
            BoundingBox bbox = GetBbox(points);

            // float width = bbox.maxX - bbox.minX;
            float width = bbox.maxX*6;
            float padding = (float)System.Math.Round(width * 0.02);
            float scale = (width - 2 * padding) / (bbox.maxX - bbox.minX);
            float height = (float)System.Math.Ceiling(scale * (bbox.maxY - bbox.minY)) + 2 * padding;
            width = width + 2 * padding;


            DelaunatorSharp.Delaunator delauny = new DelaunatorSharp.Delaunator(points);
            Circumcircles circumcircles = ComputeCircumcircles(points, delauny);

            FlatQueue<float> queue = new FlatQueue<float>();
            bool[] onEdge = new bool[delauny.Halfedges.Length];
            bool[] visited = new bool[points.Length];
            
            
            for (int i = 0; i < delauny.Halfedges.Length; i++) 
            {
                if (delauny.Halfedges[i] == -1)
                    addToQueue(queue, delauny, points, circumcircles, onEdge, visited, i); // start with convex hull edges
            }
            
            // var ctx = DOM.context2d(width, height);
            
            int imageCount = 0;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap((int) width, (int)height);
            
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                // Draw(delauny, points, bbox, circumcircles, g, padding, scale, queue, onEdge);
                // bmp.Save(@"D:\TestImage" + imageCount + ".png", System.Drawing.Imaging.ImageFormat.Png);
                // imageCount++;
                
                while (true)
                {
                    int? i = queue.Pop();
                    if (i == null)
                        break;

                    int i1 = delauny.Halfedges[i.Value % 3 == 2 ? i.Value - 2 : i.Value + 1];
                    int i2 = delauny.Halfedges[i.Value % 3 == 0 ? i.Value + 2 : i.Value - 1];
                    if (i1 != -1 && i2 != -1 && !visited[delauny.Triangles[i1]])
                    {
                        addToQueue(queue, delauny, points, circumcircles, onEdge, visited, i1);
                        addToQueue(queue, delauny, points, circumcircles, onEdge, visited, i2);
                        onEdge[i.Value] = false;
                    }
                    
                    // yield draw(ctx, queue, onEdge);
                    // Draw(delauny, points, bbox, circumcircles, g, padding, scale, queue, onEdge);
                    
                    // if(System.Environment.OSVersion.Platform == PlatformID.Unix)
                    //     bmp.Save(@"TestImage" + imageCount + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    // else
                    //     bmp.Save(@"D:\TestImage" + imageCount + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    
                    // imageCount++;
                } // Whend 
                
                Draw(delauny, points, bbox, circumcircles, g, padding, scale, queue, onEdge);
                    
                if(System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                    bmp.Save(@"TestImage.png", System.Drawing.Imaging.ImageFormat.Png);
                else
                    bmp.Save(@"D:\TestImage.png", System.Drawing.Imaging.ImageFormat.Png);
            } // End Using g 
            
            System.Console.Write("finished");
        }


    }


}
