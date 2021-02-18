using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinVariantSelector : MonoBehaviour
{
    public GameObject SteveVersion;
    public GameObject AlexVersion;

    SkinLoader skinLoader;
    // Start is called before the first frame update
    void Start()
    {
        skinLoader = transform.root.GetComponent<SkinLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        AlexVersion.SetActive(skinLoader.UseAlexVariant);
        SteveVersion.SetActive(!skinLoader.UseAlexVariant);
    }
}
