using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetText : MonoBehaviour
{

    public void Setup(int id)
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = id.ToString();
    }
}
