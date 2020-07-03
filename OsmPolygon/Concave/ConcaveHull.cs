
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

        public ConcaveHull(Geometry geom, double tolerance)
        {
            _geom = geom;
            _tolerance = tolerance;
        }

        public Geometry GetResult()
        {
            var subdiv = BuildDelaunay();
            var tris = ExtractTriangles(subdiv);
            var hull = ComputeHull(tris);
            return hull;
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
