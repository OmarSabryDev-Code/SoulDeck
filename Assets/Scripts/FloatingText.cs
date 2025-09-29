using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh; // assign in prefab

    public static void Show(string message, Transform target)
    {
        GameObject prefab = Resources.Load<GameObject>("FloatingText");
        GameObject instance = Object.Instantiate(prefab, target.position + Vector3.up * 2, Quaternion.identity);

        FloatingText floating = instance.GetComponent<FloatingText>();
        if (floating != null && floating.textMesh != null)
        {
            floating.textMesh.text = message; // ✅ THIS updates TMP
        }

        instance.transform.SetParent(target);
    }
}


