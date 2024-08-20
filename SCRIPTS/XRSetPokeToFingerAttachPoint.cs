using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRSetPokeToFingerAttachPoint : MonoBehaviour
{
    public Transform PokeAttachPoint;

    private XRPokeInteractor _xrPokeInteractor;

    // Start is called before the first frame update
    void Start()
    {
        //print the name of the parent object
        GameObject theParent = transform.parent.parent.gameObject;
        //print(theParent.name);
        //Get the gameobject with the name "Poke Interactor" in the children of the parent object
        GameObject childObject = theParent.transform.Find("Poke Interactor").gameObject;
        //print the name of the child object
        //print(childObject.name);
        //Get the XRPokeInteractor component from the child object
        _xrPokeInteractor = childObject.GetComponent<XRPokeInteractor>();

        SetPokeAttachPoint();
    }

    // Update is called once per frame
    void Update() { }

    private void SetPokeAttachPoint()
    {
        if (PokeAttachPoint == null)
        {
            Debug.LogError("PokeAttachPoint is not set in the inspector");
            return;
        }

        if (_xrPokeInteractor == null)
        {
            Debug.LogError("XRPokeInteractor is not found in the parent object");
            return;
        }

        _xrPokeInteractor.attachTransform = PokeAttachPoint;
    }
}
