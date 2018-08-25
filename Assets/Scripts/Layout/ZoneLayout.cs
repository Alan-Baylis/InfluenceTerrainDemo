﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using TerrainDemo.Tools;
using TerrainDemo.Voronoi;
using UnityEngine;

namespace TerrainDemo.Layout
{
    /// <summary>
    /// Description of Zone layout
    /// </summary>
    [DebuggerDisplay("Id = {Face.Id}, type = {Type}")]
    public class ZoneLayout
    {
        public readonly Vector2 Center;
        public readonly ClusterType Type;
        public readonly int ClusterId;
        public readonly Mesh<ZoneLayout>.Face Face;
        public readonly ClusterLayout Cluster;

        /// <summary>
        /// World bounds in blocks
        /// </summary>
        public readonly Bounds2i Bounds;

        public Vector2i[] Chunks { get { return _chunks; } }

        /// <summary>
        /// World bounds in chunks
        /// </summary>
        public readonly Bounds2i ChunkBounds;

        public ClusterSettings Settings { get { return _settings; } }

        public IEnumerable<ZoneLayout> Neighbors { get { return _neighbors; } }

        /// <summary>
        /// Base global height of this zone in layout
        /// </summary>
        public float Height { get; private set; }

        //public IEnumerable<ZoneLayout> NeighborsSafe { get { return _neighbors; } }

        public static readonly IEqualityComparer<ZoneLayout> TypeComparer = new ZoneTypeComparer();

        public ZoneLayout(ZoneInfo info, Mesh<ZoneLayout>.Face face, ClusterSettings settings)
        {
            Face = face;
            _settings = settings;
            //Center = cell.Center;
            Type = info.Type;
            ClusterId = info.ClusterId;
            Bounds = (Bounds2i)face.Bounds;
            ChunkBounds = new Bounds2i(Chunk.GetPositionFromBlock(Bounds.Min), Chunk.GetPositionFromBlock(Bounds.Max));
            _chunks = null;
            _neighbors = null;
        }

        /// <summary>
        /// Should be called by LandLayout after creating all ZoneLayouts
        /// </summary>
        public void Init(LandLayout landLayout)
        {
            _chunks = landLayout.GetChunks(this).ToArray();
            _neighbors = Face.Neighbors.Select(c => landLayout.Zones.ElementAt(c.Id)).ToArray();
            Height = (float)landLayout.GetBaseHeight(Center.x, Center.y);
        }

        /// <summary>
        /// Rasterize zone to blocks (triangles method). Not the best
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IEnumerable<Vector2i> GetBlocks()
        {
            /*
            for (int i = 1; i < Cell.Vertices.Length - 1; i++)
                foreach (var pos in Rasterization.Triangle((OpenTK.Vector2)Cell.Vertices[0], (OpenTK.Vector2)Cell.Vertices[i], (OpenTK.Vector2)Cell.Vertices[i + 1]))
                    if (Bounds.Contains(pos))
                        yield return pos;
                        */
            return null;
        }

        /// <summary>
        /// Rasterize zone to blocks (scanline polygon method)
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IEnumerable<Vector2i> GetBlocks2()                   //Todo consider move to Rasterization class
        {
            var edges = new List<Vector2i>();
            foreach (var edge in Face.Edges)
                //edges.AddRange(Rasterization.DDA((OpenTK.Vector2)edge.Vertex1, (OpenTK.Vector2)edge.Vertex2, false));
                ;

            var bounds = Bounds;
            edges = edges.Where(e => bounds.Contains(e)).Distinct().ToList();
            edges.Sort();

            for (int i = 0; i < edges.Count;)
            {
                if (i == edges.Count - 1)
                {
                    yield return edges.Last();
                    yield break;
                }

                var z1 = edges[i].Z;
                var j = i;
                while (j < edges.Count - 1 && edges[j + 1].Z == z1)
                    j++;

                for (var x = edges[i].X; x <= edges[j].X; x++)
                    yield return new Vector2i(x, z1);

                i = j + 1;
            }

            //foreach (var vector2I in edges)
            //{
            //    yield return vector2I;
            //}
        }



        //private readonly ZoneLayout[] _neighbors;
        private readonly ClusterSettings _settings;
        private Vector2i[] _chunks;
        private ZoneLayout[] _neighbors;

        /*
        public static bool operator ==(ZoneLayout z1, ZoneLayout z2)
        {
            if (ReferenceEquals(z1, null) && ReferenceEquals(z2, null))
                return true;
            if (ReferenceEquals(z1, null) || ReferenceEquals(z2, null))
                return false;
            return z1.Cell == z2.Cell;
        }

        public static bool operator !=(ZoneLayout z1, ZoneLayout z2)
        {
            if (ReferenceEquals(z1, null) && ReferenceEquals(z2, null))
                return false;
            if (ReferenceEquals(z1, null) || ReferenceEquals(z2, null))
                return true;
            return z1.Cell != z2.Cell;
        }
        */

        private class ZoneTypeComparer : IEqualityComparer<ZoneLayout>
        {
            public bool Equals(ZoneLayout x, ZoneLayout y)
            {
                return x.Type == y.Type;
            }

            public int GetHashCode(ZoneLayout obj)
            {
                return (int)obj.Type;
            }
        }
    }
}
