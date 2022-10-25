using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAllChildrenSelectedGameObject : MonoBehaviour
{
    [SerializeField] private GameObject _parentGameObject;
    [SerializeField] private GameObject _parentGameObject1;
    [SerializeField] private GameObject _parentGameObject3;

    public void DisableAllChildren()
    {
        for (int i = 0; i < _parentGameObject.transform.childCount; i++)
        {
            var child = _parentGameObject.transform.GetChild(i).gameObject;

            if (child != null)
            {
                child.SetActive(false);
            }
        }
    }
    
    public void DisableAllChildren2()
    {
        for (int i = 0; i < _parentGameObject1.transform.childCount; i++)
        {
            var child = _parentGameObject1.transform.GetChild(i).gameObject;

            if (child != null)
            {
                child.SetActive(false);
            }
        }
    }
    public void DisableAllChildren3()
    {
        for (int i = 0; i < _parentGameObject3.transform.childCount; i++)
        {
            var child = _parentGameObject3.transform.GetChild(i).gameObject;

            if (child != null)
            {
                child.SetActive(false);
            }
        }
    }
}
