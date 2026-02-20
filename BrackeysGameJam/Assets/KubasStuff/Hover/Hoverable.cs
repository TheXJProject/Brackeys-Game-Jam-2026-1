using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hoverable : MonoBehaviour
{
    [SerializeField] private Material hoverMaterial;
    private MeshRenderer mesh;
    private int hoverMaterialIndex;

    // Start is called before the first frame update
    void Start()
    {
        AttachHoverMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        // uncomment to test
        // if (Input.GetButtonDown("Fire1")) EnableHover(true);
        // if (Input.GetButtonDown("Fire2")) EnableHover(false);
    }

    void AttachHoverMaterial()
    {
        mesh = GetComponent<MeshRenderer>();
        var materials = mesh.materials.ToList();
        materials.Add(hoverMaterial);
        mesh.materials = materials.ToArray();
        hoverMaterialIndex = materials.Count - 1;
    }

    public void EnableHover(bool enable)
    {
        if (!hoverMaterial || !mesh) return;
        var value = enable ? 1.0f : 0.0f;
        mesh.materials[hoverMaterialIndex].SetFloat("_enabled", value);
    }
}
