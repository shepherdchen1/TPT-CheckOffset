﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;
using OpenCvSharp;

namespace CheckOffset.ImageTools
{
    internal class Polysimplify
    {
        //////////////////////////////////
        //////////////////////////////////
    }
}

// High-performance polyline simplification library
//
// This is a port of simplify-js by Vladimir Agafonkin, Copyright (c) 2012
// https://github.com/mourner/simplify-js
// 
// The code is ported from JavaScript to C#.
// The library is created as portable and 
// is targeting multiple Microsoft plattforms.
//
// This library was ported by imshz @ http://www.shz.no
// https://github.com/imshz/simplify-net
//
// This code is provided as is by the author. For complete license please
// read the original license at https://github.com/mourner/simplify-js

namespace Simplifynet
{
    /// <summary>
    /// Simplification of a 2D-polyline.
    /// </summary>
    public class SimplifyUtility //: ISimplifyUtility
    {
        // square distance between 2 points
        private double GetSquareDistance(OpenCvSharp.Point p1, OpenCvSharp.Point p2)
        {
            double dx = p1.X - p2.X,
                dy = p1.Y - p2.Y;

            return (dx * dx) + (dy * dy);
        }

        // square distance from a point to a segment
        private double GetSquareSegmentDistance(OpenCvSharp.Point p, OpenCvSharp.Point p1, OpenCvSharp.Point p2)
        {
            var x = p1.X;
            var y = p1.Y;
            var dx = p2.X - x;
            var dy = p2.Y - y;

            if (!dx.Equals(0.0) || !dy.Equals(0.0))
            {
                var t = ((p.X - x) * dx + (p.Y - y) * dy) / (dx * dx + dy * dy);

                if (t > 1)
                {
                    x = p2.X;
                    y = p2.Y;
                }
                else if (t > 0)
                {
                    x += dx * t;
                    y += dy * t;
                }
            }

            dx = p.X - x;
            dy = p.Y - y;

            return (dx * dx) + (dy * dy);
        }

        // rest of the code doesn't care about point format

        // basic distance-based simplification
        private List<OpenCvSharp.Point> SimplifyRadialDistance(OpenCvSharp.Point[] points, double sqTolerance)
        {
            var prevPoint = points[0];
            var newPoints = new List<OpenCvSharp.Point> { prevPoint };
            OpenCvSharp.Point point = new OpenCvSharp.Point(0,0);

            for (var i = 1; i < points.Length; i++)
            {
                point = points[i];

                if (GetSquareDistance(point, prevPoint) > sqTolerance)
                {
                    newPoints.Add(point);
                    prevPoint = point;
                }
            }

            if (point != null && !prevPoint.Equals(point))
                newPoints.Add(point);

            return newPoints;
        }

        // simplification using optimized Douglas-Peucker algorithm with recursion elimination
        private List<OpenCvSharp.Point> SimplifyDouglasPeucker(OpenCvSharp.Point[] points, double sqTolerance)
        {
            var len = points.Length;
            var markers = new int?[len];
            int? first = 0;
            int? last = len - 1;
            int? index = 0;
            var stack = new List<int?>();
            var newPoints = new List<OpenCvSharp.Point>();

            markers[first.Value] = markers[last.Value] = 1;

            while (last != null)
            {
                var maxSqDist = 0.0d;

                for (int? i = first + 1; i < last; i++)
                {
                    var sqDist = GetSquareSegmentDistance(points[i.Value], points[first.Value], points[last.Value]);

                    if (sqDist > maxSqDist)
                    {
                        index = i;
                        maxSqDist = sqDist;
                    }
                }

                if (maxSqDist > sqTolerance)
                {
                    markers[index.Value] = 1;
                    stack.AddRange(new[] { first, index, index, last });
                }


                if (stack.Count > 0)
                {
                    last = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);
                }
                else
                    last = null;

                if (stack.Count > 0)
                {
                    first = stack[stack.Count - 1];
                    stack.RemoveAt(stack.Count - 1);
                }
                else
                    first = null;
            }

            for (var i = 0; i < len; i++)
            {
                if (markers[i] != null)
                    newPoints.Add(points[i]);
            }

            return newPoints;
        }

        /// <summary>
        /// Simplifies a list of points to a shorter list of points.
        /// </summary>
        /// <param name="points">Points original list of points</param>
        /// <param name="tolerance">Tolerance tolerance in the same measurement as the point coordinates</param>
        /// <param name="highestQuality">Enable highest quality for using Douglas-Peucker, set false for Radial-Distance algorithm</param>
        /// <returns>Simplified list of points</returns>
        public List<OpenCvSharp.Point> Simplify(OpenCvSharp.Point[] points, double tolerance = 0.3, bool highestQuality = false)
        {
            if (points == null || points.Length == 0)
                return new List<OpenCvSharp.Point>();

            var sqTolerance = tolerance * tolerance;

            if (highestQuality)
                return SimplifyDouglasPeucker(points, sqTolerance);

            List<OpenCvSharp.Point> points2 = SimplifyRadialDistance(points, sqTolerance);
            return SimplifyDouglasPeucker(points2.ToArray(), sqTolerance);
        }

        /// <summary>
        /// Simplifies a list of points to a shorter list of points.
        /// </summary>
        /// <param name="points">Points original list of points</param>
        /// <param name="tolerance">Tolerance tolerance in the same measurement as the point coordinates</param>
        /// <param name="highestQuality">Enable highest quality for using Douglas-Peucker, set false for Radial-Distance algorithm</param>
        /// <returns>Simplified list of points</returns>
        public static List<OpenCvSharp.Point> SimplifyArray(OpenCvSharp.Point[] points, double tolerance = 0.3, bool highestQuality = false)
        {
            return new SimplifyUtility().Simplify(points, tolerance, highestQuality);
        }
    }
}
