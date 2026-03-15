using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public static class Sphere
{
    public enum UVDir : int
    {
        UP = 0,
        RIGHT = 1,
        DOWN = 2,
        LEFT = 3
    }
    
    public static Mesh GetSlice(float widthAngle, float heightAngle, int resolution, UVDir uvDir)
    {
        if (widthAngle < 1) widthAngle = 1;
        if (heightAngle < 1) heightAngle = 1;
        if (resolution < 1) resolution = 1;
        
        Vector2 startAngle = new(heightAngle*-0.5f, widthAngle*-0.5f);
        Vector2 angleStep = new(heightAngle / resolution, widthAngle / resolution);

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
            float u = (angle.y - startAngle.y) / widthAngle;
            float v = 1f-((angle.x - startAngle.x) / heightAngle);
            Vector2 uv = new Vector2(u,v);
            for (int i = 0; i < (int)uvDir; i++)
            {
                uv = RotateCW(uv);
            }
            return uv;
        }

        Vector2 RotateCW(Vector2 v)
        {
            return new(v.y, -v.x);
        }

        for (int x=0; x<resolution; x++)
        {
            for (int y=0; y<resolution; y++)
            {
                var xAngle = startAngle.x + x * angleStep.x;
                var yAngle = startAngle.y + y * angleStep.y;

                Vector2 corAngle00 = new(xAngle, yAngle);
                Vector2 corAngle01 = new(xAngle, yAngle+angleStep.y);
                Vector2 corAngle11 = corAngle00 + angleStep;
                Vector2 corAngle10 = new(xAngle + angleStep.x, yAngle);
                
                Vector2 uv00 = Angle2UV(corAngle00);
                Vector2 uv11 = Angle2UV(corAngle11);
                Vector2 uv10 = Angle2UV(corAngle10);
                Vector2 uv01 = Angle2UV(corAngle01);

                Vector3 p00 = Quaternion.Euler(corAngle00) * Vector3.forward;
                Vector3 p01 = Quaternion.Euler(corAngle01) * Vector3.forward;
                Vector3 p10 = Quaternion.Euler(corAngle10) * Vector3.forward;
                Vector3 p11 = Quaternion.Euler(corAngle11) * Vector3.forward;
                
                AddTriangle(p00,p01,p10,uv00,uv01,uv10);
                AddTriangle(p10,p01,p11,uv10,uv01,uv11);
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
