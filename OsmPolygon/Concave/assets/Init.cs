
// using UnityEngine;


namespace ConcaveHull
{
    public class Init //: MonoBehaviour
    {
        System.Collections.Generic.List<Node> dot_list = new System.Collections.Generic.List<Node>(); //Used only for the demo

        public string seed;
        public int scaleFactor;
        public int number_of_dots;
        public double concavity;

        void Start() {
            setDots(number_of_dots); //Used only for the demo
            generateHull();
        }

        public void generateHull() {
            Hull.setConvexHull(dot_list);
            Hull.setConcaveHull(concavity, scaleFactor);
        }

        public static System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> foo(System.Collections.Generic.IEnumerable<NetTopologySuite.Geometries.Coordinate> mydots)
        {
            System.Collections.Generic.List<Node> ls = new System.Collections.Generic.List<Node>();

            int index = 0;
            foreach (var coords in mydots)
            {
                ls.Add(new Node(coords.X,coords.Y, index));
                index++;
            }
          
            //Delete nodes that share same position
            for (int pivot_position = 0; pivot_position < ls.Count; pivot_position++) 
            {
                for (int position = 0; position < ls.Count; position++) 
                {
                    if (ls[pivot_position].x == ls[position].x && ls[pivot_position].y == ls[position].y
                                                               && pivot_position != position) 
                    {
                        ls.RemoveAt(position);
                        position--;
                    }
                } 
            }
            
            
            int scaleFactor = 100;
            double concavity = 0.5;
            
            Hull.setConvexHull(ls);
            Hull.setConcaveHull(concavity, scaleFactor);
            
            
            System.Collections.Generic.List<OSM.API.v0_6.GeoPoint> lsUnionPolygonPoints = 
                new System.Collections.Generic.List<OSM.API.v0_6.GeoPoint>();
            
            
            for (int i = 0; i < Hull.hull_concave_edges.Count; i++)
            {
                double x = Hull.hull_concave_edges[i].nodes[0].x;
                double y = Hull.hull_concave_edges[i].nodes[0].y;
                
                lsUnionPolygonPoints.Add(new OSM.API.v0_6.GeoPoint(x, y));
            }

            return lsUnionPolygonPoints;
        }



        public void setDots(int number_of_dots) {
            // This method is only used for the demo!
            System.Random pseudorandom = new System.Random(seed.GetHashCode());
            for (int x = 0; x < number_of_dots; x++) {
                dot_list.Add(new Node(pseudorandom.Next(0, 100), pseudorandom.Next(0, 100), x));
            }
            //Delete nodes that share same position
            for (int pivot_position = 0; pivot_position < dot_list.Count; pivot_position++) {
                for (int position = 0; position < dot_list.Count; position++) {
                    if (dot_list[pivot_position].x == dot_list[position].x && dot_list[pivot_position].y == dot_list[position].y
                        && pivot_position != position) {
                        dot_list.RemoveAt(position);
                        position--;
                    }
                } 
            }
        }

        // Unity demo visualization
        void OnDrawGizmos() {
            // Convex hull
            // Gizmos.color = Color.yellow;
            for (int i = 0; i < Hull.hull_edges.Count; i++) {
                // Vector2 left = new Vector2((float)Hull.hull_edges[i].nodes[0].x, (float)Hull.hull_edges[i].nodes[0].y);
                // Vector2 right = new Vector2((float)Hull.hull_edges[i].nodes[1].x, (float)Hull.hull_edges[i].nodes[1].y);
                // Gizmos.DrawLine(left, right);
            }

            // Concave hull
            // Gizmos.color = Color.blue;
            for (int i = 0; i < Hull.hull_concave_edges.Count; i++) {
                // Vector2 left = new Vector2((float)Hull.hull_concave_edges[i].nodes[0].x, (float)Hull.hull_concave_edges[i].nodes[0].y);
                // Vector2 right = new Vector2((float)Hull.hull_concave_edges[i].nodes[1].x, (float)Hull.hull_concave_edges[i].nodes[1].y);
                // Gizmos.DrawLine(left, right);
            }

            // Dots
            // Gizmos.color = Color.red;
            for (int i = 0; i < dot_list.Count; i++) {
                //  Gizmos.DrawSphere(new Vector3((float)dot_list[i].x, (float)dot_list[i].y, 0), 0.5f);
            }            
        }
        
        
    }

    
}
