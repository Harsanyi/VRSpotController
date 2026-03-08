using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Sphere
{
    public static Mesh GetSlice(float widthAngle, float heightAngle, int resolution)
    {
        Vector2 startAngle = new(heightAngle*-0.5f, widthAngle*-0.5f);
        Vector2 angleStep = new(widthAngle / resolution, heightAngle / resolution);

        List<Vector3> verts = new();
        List<int> tris = new();
        List<Vector2> uvs = new();
        
        int AddVertex(Vector3 v, Vector2 uv)
        {
            int i = verts.IndexOf(v);
            if (i > -1) return i;

            verts.Add(v);
            uvs.Add(uv);
            return verts.Count - 1;
        }

        void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Vector2 uvA, Vector2 uvB, Vector2 uvC)
        {
            int ia = AddVertex(a,uvA);
            int ib = AddVertex(b,uvB);
            int ic = AddVertex(c,uvC);

            tris.Add(ia);
            tris.Add(ib);
            tris.Add(ic);
        }

        Vector2 Angle2UV(Vector2 angle)
        {
            var uv = new Vector2((angle.x - startAngle.x)/widthAngle, (angle.y - startAngle.y)/heightAngle);
            return uv;
        }

        for (int x=0; x<resolution; x++)
        {
            for (int y=0; y<resolution; y++)
            {
                var xAngle = startAngle.x + x * angleStep.x;
                var yAngle = startAngle.y + y * angleStep.y;

                Vector2 corAngle00 = new(xAngle, yAngle);
                Vector2 corAngle11 = corAngle00 + angleStep;
                
                Vector2 uv00 = Angle2UV(corAngle00);
                Vector2 uv11 = Angle2UV(corAngle11);

                Vector3 p00 = Quaternion.Euler(xAngle, yAngle, 0) * Vector3.forward;
                Vector3 p01 = Quaternion.Euler(xAngle, yAngle+angleStep.y, 0) * Vector3.forward;
                Vector3 p10 = Quaternion.Euler(xAngle+angleStep.x, yAngle, 0) * Vector3.forward;
                Vector3 p11 = Quaternion.Euler(xAngle+angleStep.x, yAngle+angleStep.y, 0) * Vector3.forward;
                
                AddTriangle(p00,p01,p10, uv00, new(uv00.x, uv11.y), new(uv11.x, uv00.y));
                AddTriangle(p10,p01,p11, new(uv11.x,uv00.y), new(uv00.x, uv11.y), uv11);
            }
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
