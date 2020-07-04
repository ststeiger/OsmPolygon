
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Implementation;
using NetTopologySuite.Operation.Linemerge;
using NetTopologySuite.Triangulate;
using NetTopologySuite.Triangulate.QuadEdge;
using NetTopologySuite.Utilities;
using OsmPolygon.Concave;

namespace NetTopologySuite.Hull
{


    // https://towardsdatascience.com/the-concave-hull-c649795c0f0f
    // https://github.com/matsim-up/freight-sa/blob/master/src/main/java/org/matsim/up/freight/clustering/containers/ConcaveHull.java
    // http://www.rotefabrik.free.fr/concave_hull/
    public class ConcaveHull
    {

        private GeometryFactory geomFactory;
        private readonly GeometryCollection geometries;
        private double threshold;


        public System.Collections.Generic.Dictionary<LineSegment, int> segments = new System.Collections.Generic.Dictionary<LineSegment, int>();
        public System.Collections.Generic.Dictionary<int, Edge> edges = new System.Collections.Generic.Dictionary<int, Edge>();
        public System.Collections.Generic.Dictionary<int, OsmPolygon.Concave.Triangle> triangles = new System.Collections.Generic.Dictionary<int, OsmPolygon.Concave.Triangle>();
        public System.Collections.Generic.SortedDictionary<int, Edge> lengths = new System.Collections.Generic.SortedDictionary<int, Edge>();

        public System.Collections.Generic.Dictionary<int, Edge> shortLengths = new System.Collections.Generic.Dictionary<int, Edge>();

        public System.Collections.Generic.Dictionary<Coordinate, int> coordinates = new System.Collections.Generic.Dictionary<Coordinate, int>();
        public System.Collections.Generic.Dictionary<int, Vertex> vertices = new System.Collections.Generic.Dictionary<int, Vertex>();



        public ConcaveHull(Geometry geometry, double threshold)
        {
            this.geometries = transformIntoPointGeometryCollection(geometry);
            this.threshold = threshold;
            this.geomFactory = geometry.Factory;
        }


        public ConcaveHull(GeometryCollection geometries, double threshold)
        {
            this.geometries = transformIntoPointGeometryCollection(geometries);
            this.threshold = threshold;
            this.geomFactory = geometries.Factory;
        }

        private static GeometryCollection transformIntoPointGeometryCollection(Geometry geom)
        {
            UniqueCoordinateArrayFilter filter = new UniqueCoordinateArrayFilter();
            geom.Apply(filter);
            Coordinate[] coord = filter.Coordinates;

            Geometry[] geometries = new Geometry[coord.Length];
            for (int i = 0; i < coord.Length; i++)
            {
                Coordinate[] c = new Coordinate[] { coord[i] };
                CoordinateArraySequence cs = new CoordinateArraySequence(c);
                geometries[i] = new Point(cs, geom.Factory);
            }

            return new GeometryCollection(geometries, geom.Factory);
        }

        // Transform into GeometryCollection. 
	    // @param geom input geometry
	    // @return a geometry collection
        private static GeometryCollection transformIntoPointGeometryCollection(GeometryCollection gc)
        {
            UniqueCoordinateArrayFilter filter = new UniqueCoordinateArrayFilter();
            gc.Apply(filter);
            Coordinate[] coord = filter.Coordinates;

            Geometry[] geometries = new Geometry[coord.Length];
            for (int i = 0; i < coord.Length; i++)
            {
                Coordinate[] c = new Coordinate[] { coord[i] };
                CoordinateArraySequence cs = new CoordinateArraySequence(c);
                geometries[i] = new Point(cs, gc.Factory);
            }

            return new GeometryCollection(geometries, gc.Factory);
        }


        public Geometry GetResult()
        {
            QuadEdgeSubdivision qes = BuildDelaunay();
            IList<QuadEdge> quadEdges = qes.GetEdges();
            IList<QuadEdgeTriangle> qeTriangles = ExtractTriangles(qes);
            IEnumerable<Vertex> qeVertices = qes.GetVertices(false);

            int iV = 0;
            foreach (Vertex v in qeVertices)
            {
                this.coordinates[v.Coordinate] = iV;
                this.vertices[iV] = new Vertex(iV, v.Coordinate); 
                iV++;
            }

            List<QuadEdge> qeFrameBorder = new List<QuadEdge>();
            List<QuadEdge> qeFrame = new List<QuadEdge>();
            List<QuadEdge> qeBorder = new List<QuadEdge>();


            foreach (QuadEdge qe in quadEdges)
            {
                if (qes.IsFrameBorderEdge(qe))
                {
                    qeFrameBorder.Add(qe);
                }

                if (qes.IsFrameEdge(qe))
                {
                    qeFrame.Add(qe);
                }
            } // Next qe 

            // border
            for (int j = 0; j < qeFrameBorder.Count; j++)
            {

                QuadEdge q = qeFrameBorder[j];
                if (!qeFrame.Contains(q))
                {
                    qeBorder.Add(q);
                }

            } // Next j 

            // deletion of exterior edges
            foreach (QuadEdge qe in qeFrame)
            {
                qes.Delete(qe);
            }


            Dictionary<QuadEdge, double> qeDistances = new Dictionary<QuadEdge, double>();
            foreach (QuadEdge qe in quadEdges)
            {
                qeDistances[qe] = qe.ToLineSegment().Length;
            }

            QuadEdgeComparer dc = new QuadEdgeComparer();
            //SortedDictionary<QuadEdge, double> qeSorted = new SortedDictionary<QuadEdge, double>(qeDistances, dc);
            SortedDictionary<QuadEdge, double> qeSorted = new SortedDictionary<QuadEdge, double>(dc);
            foreach (var x in qeDistances)
            {
                // qeSorted.Add(x.Key, x.Value);
                qeSorted[x.Key] = x.Value;
            }



            // edges creation
            int i = 0;
            foreach (QuadEdge qe in qeSorted.Keys)
            {
                LineSegment s = qe.ToLineSegment();
                s.Normalize();

                int idS = this.coordinates[s.P0];
                int idD = this.coordinates[s.P1];
                Vertex oV = this.vertices[idS];
                Vertex eV = this.vertices[idD];

                Edge edge;
                if (qeBorder.Contains(qe))
                {
                    oV.IsBorder = true;
                    eV.IsBorder = true;
                    
                    edge = new Edge(i, s, oV, eV, true);

                    if (s.Length < this.threshold)
                    {
                        this.shortLengths[i] = edge;
                    }
                    else
                    {
                        this.lengths[i] = edge;
                    }
                }
                else
                {
                    edge = new Edge(i, s, oV, eV, false);
                }

                this.edges[i] = edge;
                this.segments[s] = i;
                i++;
            } // Next qe 

            // hm of linesegment and hm of edges // with id as key
            // hm of triangles using hm of ls and connection with hm of edges

            i = 0;
            foreach (QuadEdgeTriangle qet in qeTriangles)
            {
                LineSegment sA = qet.GetEdge(0).ToLineSegment();
                LineSegment sB = qet.GetEdge(1).ToLineSegment();
                LineSegment sC = qet.GetEdge(2).ToLineSegment();

                sA.Normalize();
                sB.Normalize();
                sC.Normalize();

                Edge edgeA = this.edges[this.segments[sA]];
                Edge edgeB = this.edges[this.segments[sB]];
                Edge edgeC = this.edges[this.segments[sC]];

                OsmPolygon.Concave.Triangle triangle = new OsmPolygon.Concave.Triangle(i, qet.IsBorder() ? true : false);
                triangle.addEdge(edgeA);
                triangle.addEdge(edgeB);
                triangle.addEdge(edgeC);

                edgeA.addTriangle(triangle);
                edgeB.addTriangle(triangle);
                edgeC.addTriangle(triangle);

                this.triangles[i] = triangle;
                i++;
            } // Next qet 


            // add triangle neighbourood

            foreach (Edge edge in this.edges.Values)
            {
                if (edge.getTriangles().Count != 1)
                {
                    OsmPolygon.Concave.Triangle tA = edge.getTriangles()[0];
                    OsmPolygon.Concave.Triangle tB = edge.getTriangles()[1];
                    tA.addNeighbour(tB);
                    tB.addNeighbour(tA);
                }
            }

            // concave hull algorithm
            int index = 0;
            while (index != -1)
            {
                index = -1;

                Edge e = null;

                // find the max length (smallest id so first entry)
                int si = this.lengths.Count;

                if (si != 0)
                {
                    KeyValuePair<int, Edge> entry = this.lengths.First();

                    int ind = entry.Key;
                    if (entry.Value.getGeometry().Length > this.threshold)
                    {
                        index = ind;
                        e = entry.Value;
                    }

                } // End if (si != 0) 

                if (index != -1)
                {
                    OsmPolygon.Concave.Triangle triangle = e.getTriangles()[0];
                    List<OsmPolygon.Concave.Triangle> neighbours = triangle.getNeighbours();

                    // irregular triangle test
                    if (neighbours.Count == 1)
                    {
                        this.shortLengths[e.getId()] = e;
                        this.lengths.Remove(e.getId());
                    }
                    else
                    {
                        Edge e0 = triangle.getEdges()[0];
                        Edge e1 = triangle.getEdges()[1];

                        // test if all the vertices are on the border
                        if (e0.getOV().IsBorder && e0.getEV().IsBorder
                            && e1.getOV().IsBorder && e1.getEV().IsBorder)
                        {
                            this.shortLengths[e.getId()] = e;
                            this.lengths.Remove(e.getId());
                        }
                        else
                        {
                            // management of triangles
                            OsmPolygon.Concave.Triangle tA = neighbours[0];
                            OsmPolygon.Concave.Triangle tB = neighbours[1];
                            tA.setBorder(true); // FIXME not necessarily useful
                            tB.setBorder(true); // FIXME not necessarily useful
                            this.triangles.Remove(triangle.getId());
                            tA.removeNeighbour(triangle);
                            tB.removeNeighbour(triangle);

                            // new edges
                            List<Edge> ee = triangle.getEdges();
                            Edge eA = ee[0];
                            Edge eB = ee[1];
                            Edge eC = ee[2];

                            if (eA.isBorder())
                            {
                                this.edges.Remove(eA.getId());
                                eB.setBorder(true);

                                eB.getOV().IsBorder = true;
                                eB.getEV().IsBorder = true;
                                
                                eC.setBorder(true);

                                eC.getOV().IsBorder = true;
                                eC.getEV().IsBorder = true;

                                // clean the relationships with the triangle
                                eB.removeTriangle(triangle);
                                eC.removeTriangle(triangle);

                                if (eB.getGeometry().Length < this.threshold)
                                {
                                    this.shortLengths[eB.getId()] = eB;
                                }
                                else
                                {
                                    this.lengths[eB.getId()] = eB;
                                }

                                if (eC.getGeometry().Length < this.threshold)
                                {
                                    this.shortLengths[eC.getId()] = eC;
                                }
                                else
                                {
                                    this.lengths[eC.getId()] = eC;
                                }

                                this.lengths.Remove(eA.getId());
                            } // End if (eA.isBorder()) 
                            else if (eB.isBorder())
                            {
                                this.edges.Remove(eB.getId());
                                eA.setBorder(true);
                                eA.getOV().IsBorder = true;
                                eA.getEV().IsBorder = true;
                                eC.setBorder(true);
                                eC.getOV().IsBorder = true;
                                eC.getEV().IsBorder = true;

                                // clean the relationships with the triangle
                                eA.removeTriangle(triangle);
                                eC.removeTriangle(triangle);

                                if (eA.getGeometry().Length < this.threshold)
                                {
                                    this.shortLengths[eA.getId()] = eA;
                                }
                                else
                                {
                                    this.lengths[eA.getId()] = eA;
                                }
                                if (eC.getGeometry().Length < this.threshold)
                                {
                                    this.shortLengths[eC.getId()] = eC;
                                }
                                else
                                {
                                    this.lengths[eC.getId()] = eC;
                                }

                                this.lengths.Remove(eB.getId());

                            } // End else if (eB.isBorder())
                            else
                            {
                                this.edges.Remove(eC.getId());
                                eA.setBorder(true);

                                eA.getOV().IsBorder = true;
                                eA.getEV().IsBorder = true;
                                eB.setBorder(true);
                                eB.getOV().IsBorder = true;
                                eB.getEV().IsBorder = true;
                                
                                // clean the relationships with the triangle
                                eA.removeTriangle(triangle);
                                eB.removeTriangle(triangle);

                                if (eA.getGeometry().Length < this.threshold)
                                {
                                    this.shortLengths[eA.getId()] = eA;
                                }
                                else
                                {
                                    this.lengths[eA.getId()] = eA;
                                }

                                if (eB.getGeometry().Length < this.threshold)
                                {
                                    this.shortLengths[eB.getId()] = eB;
                                }
                                else
                                {
                                    this.lengths[eB.getId()] = eB;
                                }

                                this.lengths.Remove(eC.getId());
                            } // End else of if (e0.getOV().IsBorder && e0.getEV().IsBorder && e1.getOV().IsBorder && e1.getEV().IsBorder)

                        } // End Else of if 

                    } // End else of if (neighbours.Count == 1)

                } // End if (index != -1) 

            } // Whend 

            // concave hull creation
            List<LineString> edges = new List<LineString>();
            foreach (Edge e in this.lengths.Values)
            {
                LineString l = e.getGeometry().ToGeometry(this.geomFactory);
                edges.Add(l);
            }

            foreach (Edge e in this.shortLengths.Values)
            {
                LineString l = e.getGeometry().ToGeometry(this.geomFactory);
                edges.Add(l);
            }

            // merge
            LineMerger lineMerger = new LineMerger();
            lineMerger.Add(edges);



            IEnumerator<Geometry> en = lineMerger.GetMergedLineStrings().GetEnumerator();
            en.MoveNext();

            LineString merge = (LineString)en.Current;


            if (merge.IsRing)
            {
                LinearRing lr = new LinearRing(merge.CoordinateSequence, this.geomFactory);
                Polygon concaveHull = new Polygon(lr, null, this.geomFactory);
                return concaveHull;
            }

            return merge;
        }


        private static IList<QuadEdgeTriangle> ExtractTriangles(QuadEdgeSubdivision subdiv)
        {
            var qeTris = QuadEdgeTriangle.CreateOn(subdiv);
            return qeTris;
        }


        private QuadEdgeSubdivision BuildDelaunay()
        {
            var builder = new DelaunayTriangulationBuilder();
            builder.SetSites(this.geometries);
            var subDiv = builder.GetSubdivision();
            return subDiv;
        }


    }


}
