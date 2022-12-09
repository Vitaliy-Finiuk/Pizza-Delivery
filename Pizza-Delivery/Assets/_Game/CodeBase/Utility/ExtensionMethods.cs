using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.CodeBase.Utility
{
    public static class ExtensionMethods
    {
        //===================================================================================

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        //===================================================================================

        private static float CalculateAngleCenterToTangent(Vector3 point, Vector3 circleCenter, float circleRadius)
        {

            float distancePointToCenterOfCircle = (circleCenter - point).magnitude;
            float angleBetweenCenterToTangent = Mathf.Asin(circleRadius / distancePointToCenterOfCircle) * Mathf.Rad2Deg;
            return angleBetweenCenterToTangent;
        }

        //===================================================================================

        private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(angles) * (point - pivot) + pivot;
        }

        //===================================================================================

        public static Color AddToSaturation(this Color _col, float changeAmount)
        {
            float H, S, V;
            Color.RGBToHSV(_col, out H, out S, out V);
            S = S + changeAmount / 100f;
            if (S < 0) S = 0;
            if (S > 1) S = 1;
            return Color.HSVToRGB(H, S, V);
        }

        //===================================================================================

        public static Color ChangeSaturationPercent(this Color _col, float changePercent)
        {
            float H, S, V;
            Color.RGBToHSV(_col, out H, out S, out V);
            S = S * changePercent / 100f;
            if (S < 0) S = 0;
            if (S > 1) S = 1;
            return Color.HSVToRGB(H, S, V);
        }

        //===================================================================================

        public static Color ChangeSaturationTo(this Color _col, float saturation)
        {
            float H, S, V;
            Color.RGBToHSV(_col, out H, out S, out V);
            S = saturation;
            return Color.HSVToRGB(H, S, V);
        }

        //===================================================================================

        public static Color ChangeSaturationToHalf(this Color _col)
        {
            float H, S, V;
            Color.RGBToHSV(_col, out H, out S, out V);
            S /= 2f;
            return Color.HSVToRGB(H, S, V);
        }

        //===================================================================================

        public static Color ChangeBrightnessTo(this Color _col, float brightness)
        {
            float H, S, V;
            Color.RGBToHSV(_col, out H, out S, out V);
            V = brightness;
            return Color.HSVToRGB(H, S, V);
        }

        //===================================================================================

        public static Vector3 LerpByDistance(this float distance, Vector3 start, Vector3 end)
        {
            Vector3 P = distance * Vector3.Normalize(end - start) + start;
            return P;
        }

        //===================================================================================

        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        //===================================================================================

        public static int GetRandomStartingPoint<T>(this IList<bool> ts)
        {
            int startingPoint = UnityEngine.Random.Range(0, ts.Count);
            bool[] temp = new bool[ts.Count];
            for (int i = 0; i < ts.Count; i++)
            {
                int index = (startingPoint + i) % ts.Count;
                temp[i] = ts[index];
            }
            ts.Clear();
            for (int i = 0; i < ts.Count; i++) // add range
            {
                ts.Add(temp[i]);
            }

            return startingPoint;
        }

        //===================================================================================

        public static int[] FillStartingFromToCount(int startingFrom, int toCount)
        {
            int length = toCount - startingFrom;
            int[] ts = new int[length];
            for (var i = 0; i < length; ++i)
            {
                ts[i] = i;
            }
            return ts;
        }

        //===================================================================================

        public static T RandomEnumValue<T>()
        {
            var values = Enum.GetValues(typeof(T));
            int random = UnityEngine.Random.Range(0, values.Length);
            return (T)values.GetValue(random);
        }

        //===================================================================================

        public static Transform GetClosestObject(Transform current, List<Transform> goals)
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = current.position;

            foreach (Transform t in goals)
            {
                float dist = Vector3.Distance(t.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }

            return tMin;
        }

        //===================================================================================

        public static void DrawCube(Vector3 center, float length,
                Color? color = null, float duration = 0.0f, bool depthTest = true)
        {
            float hLength = length / 2;
            DrawRectangularPrism(center + new Vector3(hLength, hLength, hLength),
                center - new Vector3(hLength, hLength, hLength), color ?? Color.white, duration, depthTest);
        }

        //===================================================================================

        public static void DrawRectangularPrism(Vector3 p1, Vector3 p2,
            Color? color = null, float duration = 0.0f, bool depthTest = true)
        {
            float h = p2.y - p1.y;
            float w = p2.x - p1.x;
            float d = p2.z - p1.z;

            Debug.DrawLine(p1, p1 + Vector3.right * w, color ?? Color.white, duration, depthTest);
            Debug.DrawLine(p1, p1 + Vector3.forward * d, color ?? Color.white, duration, depthTest);
            Debug.DrawLine(p1, p1 + Vector3.up * h, color ?? Color.white, duration, depthTest);
            Debug.DrawLine(p1 + Vector3.up * h, p1 + Vector3.up * h + Vector3.right * w, color ?? Color.white, duration,
                depthTest);
            Debug.DrawLine(p1 + Vector3.up * h, p1 + Vector3.up * h + Vector3.forward * d, color ?? Color.white,
                duration,
                depthTest);
            Debug.DrawLine(p1 + Vector3.right * w, p1 + Vector3.right * w + Vector3.up * h, color ?? Color.white,
                duration,
                depthTest);

            Debug.DrawLine(p2, p2 - Vector3.right * w, color ?? Color.white, duration, depthTest);
            Debug.DrawLine(p2, p2 - Vector3.forward * d, color ?? Color.white, duration, depthTest);
            Debug.DrawLine(p2, p2 - Vector3.up * h, color ?? Color.white, duration, depthTest);
            Debug.DrawLine(p2 - Vector3.up * h, p2 - Vector3.up * h - Vector3.right * w, color ?? Color.white, duration,
                depthTest);
            Debug.DrawLine(p2 - Vector3.up * h, p2 - Vector3.up * h - Vector3.forward * d, color ?? Color.white,
                duration,
                depthTest);
            Debug.DrawLine(p2 - Vector3.right * w, p2 - Vector3.right * w - Vector3.up * h, color ?? Color.white,
                duration,
                depthTest);
        }

        //===================================================================================

        public static void DrawRect(Rect rect, Color col)
        {
            Vector3 pos = new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0.0f);
            Vector3 scale = new Vector3(rect.width, rect.height, 0.0f);

            DrawRect(pos, col, scale);
        }

        //===================================================================================

        public static void DrawRect(Vector3 pos, Color col, Vector3 scale)
        {
            Vector3 halfScale = scale * 0.5f;

            Vector3[] points = new Vector3[]
            {
            pos + new Vector3(halfScale.x,      halfScale.y,    halfScale.z),
            pos + new Vector3(-halfScale.x,     halfScale.y,    halfScale.z),
            pos + new Vector3(-halfScale.x,     -halfScale.y,   halfScale.z),
            pos + new Vector3(halfScale.x,      -halfScale.y,   halfScale.z),
            };

            Debug.DrawLine(points[0], points[1], col);
            Debug.DrawLine(points[1], points[2], col);
            Debug.DrawLine(points[2], points[3], col);
            Debug.DrawLine(points[3], points[0], col);
        }

        //===================================================================================

        public static void DrawPoint(Vector3 pos, Color col, float scale)
        {
            Vector3[] points = new Vector3[]
            {
            pos + (Vector3.up * scale),
            pos - (Vector3.up * scale),
            pos + (Vector3.right * scale),
            pos - (Vector3.right * scale),
            pos + (Vector3.forward * scale),
            pos - (Vector3.forward * scale)
            };

            Debug.DrawLine(points[0], points[1], col);
            Debug.DrawLine(points[2], points[3], col);
            Debug.DrawLine(points[4], points[5], col);

            Debug.DrawLine(points[0], points[2], col);
            Debug.DrawLine(points[0], points[3], col);
            Debug.DrawLine(points[0], points[4], col);
            Debug.DrawLine(points[0], points[5], col);

            Debug.DrawLine(points[1], points[2], col);
            Debug.DrawLine(points[1], points[3], col);
            Debug.DrawLine(points[1], points[4], col);
            Debug.DrawLine(points[1], points[5], col);

            Debug.DrawLine(points[4], points[2], col);
            Debug.DrawLine(points[4], points[3], col);
            Debug.DrawLine(points[5], points[2], col);
            Debug.DrawLine(points[5], points[3], col);
        }

        //===================================================================================

        public static void DrawCircle(Vector3 center, Vector3 normal, float radius,
            Color? color = null, float duration = 0.0f, bool depthTest = true, int resolution = 16)
        {
            Vector3 cross = Vector3.up != normal ? Vector3.up : Vector3.right;
            Vector3 start = Vector3.Cross(normal, cross).normalized * radius;
            Quaternion r1, r2;
            for (int i = 0; i < resolution; i++)
            {
                r1 = Quaternion.AngleAxis(360f / resolution * i, normal);
                r2 = Quaternion.AngleAxis(360f / resolution * (i + 1), normal);
                Debug.DrawLine(center + r1 * start, center + r2 * start, color ?? Color.white, duration, depthTest);
            }
        }

        //===================================================================================

        public static void DrawWireSphere(Vector3 center, float radius,
            Color? color = null, float duration = 0.0f, bool depthTest = true, int resolution = 16)
        {
            DrawCircle(center, Vector3.forward, radius, color, duration, depthTest, resolution);
            DrawCircle(center, Vector3.right, radius, color, duration, depthTest, resolution);
            DrawCircle(center, Vector3.up, radius, color, duration, depthTest, resolution);
        }

        //===================================================================================

        public static void DrawMesh(Mesh mesh, Vector3 position, Color? color = null,
            Vector3? scale = null, Quaternion? rotation = null, float duration = 0.0f, bool depthTest = true)
        {
            Vector3 p1, p2, p3;
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                p1 = mesh.vertices[mesh.triangles[i + 0]];
                p2 = mesh.vertices[mesh.triangles[i + 1]];
                p3 = mesh.vertices[mesh.triangles[i + 2]];
                p1 = (rotation ?? Quaternion.identity) * p1;
                p2 = (rotation ?? Quaternion.identity) * p2;
                p3 = (rotation ?? Quaternion.identity) * p3;
                p1.Scale(scale ?? Vector3.one);
                p2.Scale(scale ?? Vector3.one);
                p3.Scale(scale ?? Vector3.one);
                p1 += position;
                p2 += position;
                p3 += position;

                Debug.DrawLine(p1, p2, color ?? Color.white, duration, depthTest);
                Debug.DrawLine(p2, p3, color ?? Color.white, duration, depthTest);
                Debug.DrawLine(p3, p1, color ?? Color.white, duration, depthTest);
            }
        }

        //===================================================================================
    }
}

