
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using NetTopologySuite.Triangulate;
using NetTopologySuite.Triangulate.QuadEdge;


namespace NetTopologySuite.Hull
{
    public class ConcaveHull
    {
        private readonly Geometry _geom;
        private readonly double _tolerance;

        public System.Collections.Generic.Dictionary<Coordinate,int> coordinates = new System.Collections.Generic.Dictionary<Coordinate, int>();
        public System.Collections.Generic.Dictionary<int, Vertex> vertices = new System.Collections.Generic.Dictionary<int, Vertex>();
        
        
        
        public ConcaveHull(Geometry geom, double tolerance)
        {
            _geom = geom;
            _tolerance = tolerance;
        }

        public Geometry GetResult()
        {
            var subdiv = BuildDelaunay();
            var qeTriangles = ExtractTriangles(subdiv);
            
            List<Vertex[]> qeVertices =  GetVertices(qeTriangles, false);;
            
            int iV = 0;
            foreach(Vertex[] v in qeVertices) 
            {
                this.coordinates[v[0].Coordinate] = iV;
                
                this.vertices[iV] = new Vertex(v[0].X, v[0].Y); // note: has ID
                iV++;
            }
            
            var hull = ComputeHull(qeTriangles);
            return hull;
        }

        private static List<Vertex[]> GetVertices(IList<QuadEdgeTriangle> ls, bool b)
        {
            List<Vertex[]> qeVertices = new List<Vertex[]>();
            
            foreach (QuadEdgeTriangle thisTriangle in ls)
            {
                qeVertices.Add(thisTriangle.GetVertices());
            }
            
            return qeVertices;
        }


        private static IList<QuadEdgeTriangle> ExtractTriangles(QuadEdgeSubdivision subdiv)
        {
            var qeTris = QuadEdgeTriangle.CreateOn(subdiv);
            return qeTris;
        }


        private static Geometry ComputeHull(IList<QuadEdgeTriangle> tris)
        {
            // https://towardsdatascience.com/the-concave-hull-c649795c0f0f
            // https://github.com/matsim-up/freight-sa/blob/master/src/main/java/org/matsim/up/freight/clustering/containers/ConcaveHull.java
            // http://www.rotefabrik.free.fr/concave_hull/

            return null;
        }

        private QuadEdgeSubdivision BuildDelaunay()
        {
            var builder = new DelaunayTriangulationBuilder();
            builder.SetSites(_geom);
            var subDiv = builder.GetSubdivision();
            return subDiv;
        }
    }
}
