/*
 * This file is part of the OpenSphere project which aims to
 * develop geospatial algorithms.
 * 
 * Copyright (C) 2012 Eric Grosso
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
 *
 * For more information, contact:
 * Eric Grosso, eric.grosso.os@gmail.com
 * 
 */
namespace OsmPolygon.Concave
{
	using System.Collections.Generic;

	/**
	 * Triangle.
	 * 
	 * @author Eric Grosso
	 *
	 */
	public class Triangle
	{

		/** ID of the triangle */
		private int id;

		/** Indicator to know if the triangle is a border triangle
		 * of the triangulation framework */
		private bool border;

		/** Edges which compose the triangle */
		private List<Edge> edges = new System.Collections.Generic.List<Edge>();

		/** Neighbour triangles of this triangle */
		private List<Triangle> neighbours = new System.Collections.Generic.List<Triangle>();

		// vertices...

		/**
		 * Default constructor.
		 */
		public Triangle()
		{
			//
		}

		/**
		 * Constructor.
		 * 
		 * @param id
		 * 		ID of the triangle
		 */
		public Triangle(int id)
		{
			this.id = id;
		}

		/**
		 * Constructor.
		 * 
		 * @param id
		 * 		ID of the triangle
		 * @param border
		 * 		defines if the triangle is a border triangle
		 * 		or not in the triangulation framework
		 */
		public Triangle(int id, bool border)
		{
			this.id = id;
			this.border = border;
		}

		/**
		 * Returns the ID of the triangle.
		 * 
		 * @return
		 * 		the ID of the triangle
		 */
		public int getId()
		{
			return this.id;
		}

		/**
		 * Defines the ID of the triangle.
		 * 
		 * @param id
		 * 		ID of the triangle
		 */
		public void setId(int id)
		{
			this.id = id;
		}

		/**
		 * Returns true if the triangle is a border triangle
		 * of the triangulation framework, false otherwise.
		 * 
		 * @return
		 * 		true if the triangle is a border triangle,
		 * 		false otherwise
		 */
		public bool isBorder()
		{
			return this.border;
		}

		/**
		 * Defines the indicator to know if the triangle
		 * is a border triangle of the triangulation framework.
		 * 
		 * @param border
		 * 		true if the triangle is a border triangle,
		 * 		false otherwise
		 */
		public void setBorder(bool border)
		{
			this.border = border;
		}

		/**
		 * Returns the edges which compose the triangle.
		 * 
		 * @return
		 * 		the edges of the triangle which compose the triangle
		 */
		public List<Edge> getEdges()
		{
			return this.edges;
		}

		/**
		 * Defines the edges which compose the triangle.
		 * 
		 * @param edges
		 * 		the edges which compose the triangle
		 */
		public void setEdges(List<Edge> edges)
		{
			this.edges = edges;
		}

		/**
		 * Returns the neighbour triangles of the triangle.
		 * 
		 * @return
		 * 		the neighbour triangles of the triangle
		 */
		public List<Triangle> getNeighbours()
		{
			return this.neighbours;
		}

		/**
		 * Defines the neighbour triangles of the triangle.
		 * 
		 * @param neighbours
		 * 		the neighbour triangles of the triangle
		 */
		public void setNeighbours(List<Triangle> neighbours)
		{
			this.neighbours = neighbours;
		}


		/**
		 * Add an edge to the triangle.
		 * 
		 * @return
		 * 		true if added, false otherwise
		 */
		public bool addEdge(Edge edge)
		{
			getEdges().Add(edge);
			return true;
		}

		/**
		 * Add edges to the triangle.
		 * 
		 * @return
		 * 		true if added, false otherwise
		 */
		public bool addEdges(List<Edge> edges)
		{
			getEdges().AddRange(edges);
			return true;
		}

		/**
		 * Remove an edge of the triangle.
		 * 
		 * @return
		 * 		true if removed, false otherwise
		 */
		public bool removeEdge(Edge edge)
		{
			return getEdges().Remove(edge);
		}

		/**
		 * Remove edges of the triangle.
		 * 
		 * @return
		 * 		true if removed, false otherwise
		 */
		public bool removeEdges(List<Edge> edges)
		{
			var x = getEdges();

			foreach (var thisEdge in x)
			{
				x.Remove(thisEdge);
			}

			return true;
		}


		/**
		 * Add a neighbour triangle to the triangle.
		 * 
		 * @return
		 * 		true if added, false otherwise
		 */
		public bool addNeighbour(Triangle triangle)
		{
			getNeighbours().Add(triangle);
			return true;
		}

		/**
		 * Add neighbour triangles to the triangle.
		 * 
		 * @return
		 * 		true if added, false otherwise
		 */
		public bool addNeighbours(List<Triangle> triangles)
		{
			getNeighbours().AddRange(triangles);
			return true;
		}

		/**
		 * Remove a neighbour triangle of the triangle.
		 * 
		 * @return
		 * 		true if removed, false otherwise
		 */
		public bool removeNeighbour(Triangle triangle)
		{
			return getNeighbours().Remove(triangle);
		}

		/**
		 * Remove neighbour triangles of the triangle.
		 * 
		 * @return
		 * 		true if removed, false otherwise
		 */
		public bool removeNeighbours(List<Triangle> triangles)
		{
			var neighbours = getNeighbours();

			foreach (var t in triangles)
			{
				neighbours.Remove(t);
			}

			return true;
		}

	}


}