﻿using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using TerrainDemo.Macro;
using TerrainDemo.Micro;
using TerrainDemo.Settings;
using TerrainDemo.Visualization;
using UnityEngine;
using UnityEngine.Assertions;
using Debug = UnityEngine.Debug;
using Renderer = TerrainDemo.Visualization.Renderer;

namespace TerrainDemo
{
    public class TriRunner : MonoBehaviour
    {
#region Generator

        [Header("Generator settings")]
        public int Seed;
        public bool RandomizeSeed;
        public float LandSize = 100;
        public float CellSide = 10;
        public float GridPerturbFreq = 0.01f;
        public float GridPerturbPower = 10;
        public float InfluencePerturbFreq = 0.25f;
        public float InfluencePerturbPower = 4;
        public BiomeSettings[] Biomes = new BiomeSettings[0];

        #endregion

#region Visualization

        [Header("Visualizer settings")]
        public Renderer.TerrainRenderMode RenderMode = Renderer.TerrainRenderMode.Terrain;
        public Renderer.TerrainLayerToRender RenderLayer = Renderer.TerrainLayerToRender.Main;

        #endregion

#region Macro

        [Header("Macro")]
        public Renderer.MacroCellInfluenceMode MacroCellInfluenceVisualization;
        public Material VertexColoredMat;

        #endregion

#region Micro

        [Header("Micro")]
        public Renderer.BlockTextureMode TextureMode;
        public Material TexturedMat;

        #endregion

        public Box2 LandBounds { get; private set; }

        public MacroMap Macro { get; private set; }
        public MicroMap Micro { get; private set; }

        public MacroTemplate Land { get; private set; }

        public IReadOnlyCollection<BlockSettings> AllBlocks => _allBlocks;

        private Tools.Random _random;
        private Renderer _renderer;
        private BlockSettings[] _allBlocks;

        public void Render(TriRunner renderSettings)
        {
            var timer = Stopwatch.StartNew();

            _renderer.Clear();

            //Visualization
            if (renderSettings.RenderMode == Renderer.TerrainRenderMode.Macro)
                _renderer.Render(Macro, renderSettings);
            else
                //_renderer.Render(Micro, mode);
                _renderer.Render2(Micro, renderSettings);               //Experimental renderer

            timer.Stop();

            Debug.LogFormat("Rendered in {0} msec", timer.ElapsedMilliseconds);
        }

        public void Generate()
        {
            Prepare();

            var template = new MacroTemplate(_random);
            Land = template;

            //Fully generate Macro Map
            Macro = template.CreateMacroMap(this);

            var microtimer = Stopwatch.StartNew();
            Micro = new MicroMap(Macro, this);

            foreach (var zone in Macro.Zones)
            {
                template.GenerateMicroZone(Macro, zone, Micro);
            }
            Micro.GenerateHeightmap();

            microtimer.Stop();

            Micro.Changed += MicroOnChanged;

            Debug.LogFormat("Created micromap in {0} msec", microtimer.ElapsedMilliseconds);
        }

        private void MicroOnChanged()
        {
            //Visualization
            Render(this);
        }

        private void Prepare()
        {
            if (RandomizeSeed)
                Seed = UnityEngine.Random.Range(0, int.MaxValue);

            _random = new Tools.Random(Seed);

            for (int i = 0; i < Biomes.Length; i++)
                Biomes[i].Index = i;
        }

        #region Unity

        void Awake()
        {
            Assert.raiseExceptions = true;
            _allBlocks = Resources.LoadAll<BlockSettings>("");
        }

        void Start()
        {
            Generate();

            _renderer = new Renderer(new Mesher(Macro, this), this);
            Render(this);
        }

        private void OnValidate()
        {
            LandBounds = new Box2(-LandSize / 2, LandSize / 2, LandSize / 2, -LandSize / 2);
        }
#endregion
    }
}
