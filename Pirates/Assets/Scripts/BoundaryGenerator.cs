﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// used to be an editor script (check repo if interested)
[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(MapGenerator))]
public class BoundaryGenerator : MonoBehaviour {
    public int points = 30;

    public void Generate(float width) {
        EdgeCollider2D edge = gameObject.GetComponent<EdgeCollider2D>();

        List<Vector2> ps = new List<Vector2>();

        for (int i = 0; i <= points; i++) {
            float angle = (float)i / points * Mathf.PI * 2f;
            Vector2 v = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle)) * width;
            ps.Add(v);
        }
        edge.points = ps.ToArray();
    }

}
