﻿using System.Collections.Generic;
using Assets.Code.Settings;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Code
{
    public class ChunkGO : MonoBehaviour
    {
        public static ChunkGO Create(Chunk chunk, Mesh mesh)
        {
            var chunkGo = Get(chunk);
            chunkGo._filter.sharedMesh = mesh;  
            return chunkGo;
        }

        public static void Clear()
        {
            foreach (var chunkGo in _allChunksGO)
                Destroy(chunkGo.gameObject);
            _allChunksGO.Clear();
        }

        public void CreateFlora(ILandSettings settings, IEnumerable<Vector3> positions)
        {
            if(positions != null)
                foreach (var position in positions)
                {
                    var newTree = Instantiate(settings.Tree);
                    newTree.transform.parent = transform;
                    newTree.transform.localPosition = position;
                    newTree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
                }
        }

        private MeshFilter _filter;
        private MeshRenderer _renderer;
        private static readonly List<ChunkGO> _allChunksGO = new List<ChunkGO>();

        private static ChunkGO Get(Chunk chunk)
        {
            var go = new GameObject();
            var chunkGo = go.AddComponent<ChunkGO>();
            chunkGo._filter = go.AddComponent<MeshFilter>();
            chunkGo._renderer = go.AddComponent<MeshRenderer>();
            chunkGo._renderer.sharedMaterial = Materials.Instance.Grass;
            chunkGo._renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            chunkGo._renderer.useLightProbes = false;
            go.name = chunk.Position.X + " : " + chunk.Position.Z;
            _allChunksGO.Add(chunkGo);

            chunkGo.transform.position = new Vector3(chunk.Position.X*chunk.Size, 0, chunk.Position.Z*chunk.Size);

            return chunkGo;
        }
    }
}
