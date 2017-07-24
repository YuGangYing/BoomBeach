using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathFinding.Core
{
    public struct Point
    {
        public static readonly Point Empty;
        public int X;
        public int Y;
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    #region Structs    
    public struct PathFinderNode
    {
        #region Variables Declaration
        public int F;
        public int G;
        public int H;  // f = gone + heuristic
        public int X;
        public int Y;
        public int PX; // Parent
        public int PY;
        #endregion
    }
    #endregion

    #region Enum
    public enum PathFinderNodeType
    {
        Start = 1,
        End = 2,
        Open = 4,
        Close = 8,
        Current = 16,
        Path = 32
    }

    public enum HeuristicFormula
    {
        Manhattan = 1,//曼哈顿H值计算方式
        MaxDXDY = 2,
        DiagonalShortCut = 3,
        Euclidean = 4,
        EuclideanNoSQR = 5,
        Custom1 = 6
    }
    #endregion

    #region Structs
    public struct PathFinderNodeFast
    {
        #region Variables Declaration
        public int F; // f = gone + heuristic
        public int G;
        public ushort PX; // Parent
        public ushort PY;
        public byte Status;
        #endregion
    }
    #endregion

    interface IPathFinder
    {

        #region Properties
        bool Stopped
        {
            get;
        }

        HeuristicFormula Formula
        {
            get;
            set;
        }

        bool Diagonals
        {
            get;
            set;
        }

        bool HeavyDiagonals
        {
            get;
            set;
        }

        int HeuristicEstimate
        {
            get;
            set;
        }

        bool PunishChangeDirection
        {
            get;
            set;
        }

        bool TieBreaker
        {
            get;
            set;
        }

        int SearchLimit
        {
            get;
            set;
        }

        double CompletedTime
        {
            get;
            set;
        }

        bool DebugProgress
        {
            get;
            set;
        }

        bool DebugFoundPath
        {
            get;
            set;
        }
        #endregion

        #region Methods
        void FindPathStop();
        List<PathFinderNode> FindPath(Point start, Point end);
        #endregion
    }

}
