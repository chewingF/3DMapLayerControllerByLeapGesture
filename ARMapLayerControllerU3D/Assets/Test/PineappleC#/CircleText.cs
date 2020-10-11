using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleText : BaseMeshEffect {
    public int radius = 50;
    public float spaceCoff = 1f;

    List<UIVertex> verts;


    #region implemented abstract members of BaseMeshEffect
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        if (radius == 0)
        {
            Debug.LogWarning("radius could not be zero!");
            return;
        }
        if (verts == null)
            verts = new List<UIVertex>();
        else
            verts.Clear();

        Text text = GetComponent<Text>();
        TextGenerator tg = text.cachedTextGenerator;

        float perimeter = Mathf.PI * radius * 2;
        float weight = text.fontSize / perimeter * spaceCoff;
        float radStep = Mathf.PI * 2 * weight;
        float charOffset = tg.characterCountVisible / 2f - 0.5f;

        var count = vh.currentVertCount;
        if (count == 0)
            return;
        Debug.Log("count:" + count);
        for (int i = 0; i < count; i++)
        {
            var vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);
            verts.Add(vertex);
        }

        for (int i = 0; i < tg.characterCountVisible; i++)
        {
            var lb = verts[i * 4];
            var lt = verts[i * 4 + 1];
            var rt = verts[i * 4 + 2];
            var rb = verts[i * 4 + 3];

            Vector3 center = Vector3.Lerp(lb.position, rt.position, 0.5f);
            Matrix4x4 move = Matrix4x4.TRS(center * -1, Quaternion.identity, Vector3.one);

            float rad = Mathf.PI / 2 + (charOffset - i) * radStep;
            Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;

            Quaternion rotation = Quaternion.Euler(0, 0, rad * 180 / Mathf.PI - 90);
            Matrix4x4 rotate = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            Matrix4x4 place = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            Matrix4x4 transform = place * rotate * move;

            lb.position = transform.MultiplyPoint(lb.position);
            lt.position = transform.MultiplyPoint(lt.position);
            rt.position = transform.MultiplyPoint(rt.position);
            rb.position = transform.MultiplyPoint(rb.position);

            vh.SetUIVertex(lb, i * 4);
            vh.SetUIVertex(lt, i * 4 + 1);
            vh.SetUIVertex(rt, i * 4 + 2);
            vh.SetUIVertex(rb, i * 4 + 3);
        }
    }

    #endregion
}
