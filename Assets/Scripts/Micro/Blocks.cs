﻿using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
//using NUnit.Framework;
using TerrainDemo.Macro;
using TerrainDemo.Tools;
using UnityEngine;
using UnityEngine.Assertions;

namespace TerrainDemo.Micro
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Blocks : IEquatable<Blocks>
    {
        public readonly Heights Height;
        public readonly BlockType Base;
        public readonly BlockType Underground;
        public readonly BlockType Ground;

        public BlockType Top => Ground 
                                != BlockType.Empty ? Ground : Underground 
                                                              != BlockType.Empty ? Underground : Base;

        public bool IsSimple => Underground != BlockType.Cave;

        public bool IsEmpty => Ground == BlockType.Empty && Underground == BlockType.Empty && Base == BlockType.Empty;

        public static readonly Blocks Empty;

        public Blocks(BlockType ground, BlockType underground, in Heights heights)
        {
            //Fix heights if needed todo write unit test for block autofixes
            float underHeight = heights.Underground, groundHeight = heights.Main;
            var heightFixed = false;

            if (underground == BlockType.Cave && ground == BlockType.Empty)
                underground = BlockType.Empty;

            if (underground != BlockType.Empty && underHeight == heights.Base)
                underground = BlockType.Empty;

            if (underground == BlockType.Empty && underHeight > heights.Base)
            {
                underHeight = heights.Base;
                heightFixed = true;
            }

            if (ground != BlockType.Empty && groundHeight == underHeight)
            {
                ground = BlockType.Empty;

                if (underground == BlockType.Cave)
                {
                    underground = BlockType.Empty;
                    underHeight = heights.Base;
                    heightFixed = true;
                }
            }

            if (ground == BlockType.Empty && groundHeight > underHeight)
            {
                groundHeight = underHeight;
                heightFixed = true;
            }


            Base = BlockType.Bedrock;
            Underground = underground;
            Ground = ground;

            if (heightFixed)
                Height = new Heights(groundHeight, underHeight, heights.Base);
            else
                Height = heights;

            Assert.IsTrue(Ground != BlockType.Empty || Height.Main - Height.Underground == 0, $"Block is wrong");
            Assert.IsTrue(Underground != BlockType.Empty || Height.Underground - Height.Base == 0, $"Block is wrong");

        }

        [Pure]
        public Blocks MutateHeight(in Heights heights)
        {
            return new Blocks(Ground, Underground, heights);
        }

        [Pure]
        public float GetNominalHeight()
        {
            //Simplified after strict layers ordering
            return Height.Main;
            /*
            if (Ground != BlockType.Empty)
                return Heights.Layer1Height;
            else if (Underground != BlockType.Empty)
                return Heights.UndergroundHeight;
            else
                return Heights.BaseHeight;
                */
        }

        [Pure]
        public Interval GetMainLayerWidth()
        {
            return new Interval(Height.Underground, Height.Main);
        }

        [Pure]
        public Interval GetUnderLayerWidth()
        {
            return new Interval(Height.Base, Height.Underground);
        }

        [Pure]
        public Interval GetBaseLayerWidth()
        {
            return new Interval(float.MinValue, Height.Base);
        }

        [Pure]
        public Interval GetTotalWidth()
        {
            return new Interval(float.MinValue, Height.Main);
        }


        public override string ToString()
        {
            return $"({Ground}, {Underground}, {Base})";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Blocks))
            {
                return false;
            }

            var blocks = (Blocks)obj;
            return Base == blocks.Base 
                   && Underground == blocks.Underground
                   && Ground == blocks.Ground;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = -862721379;
                hashCode = hashCode * -1521134295 + Base.GetHashCode();
                hashCode = hashCode * -1521134295 + Underground.GetHashCode();
                hashCode = hashCode * -1521134295 + Ground.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(Blocks other)
        {
            return Base == other.Base && Underground == other.Underground && Ground == other.Ground && Height.Equals(other.Height);
        }

        public static bool operator ==(Blocks left, Blocks right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Blocks left, Blocks right)
        {
            return !left.Equals(right);
        }
    }
}
