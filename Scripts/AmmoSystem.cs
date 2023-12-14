using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AmmoSystem : MonoBehaviour
{
    public Sprite kernal, popped;
    Image KernalImage;

    private void Awake()
    {
        KernalImage = GetComponent<Image>();
    }

    public void SetImage(AmmoStatus status)
    {
        switch (status)
        {
            case AmmoStatus.Popped:
                KernalImage.sprite = popped;
                break;
            case AmmoStatus.Full:
                KernalImage.sprite = kernal;
                break;
        }
    }
    public enum AmmoStatus
    {
        Popped = 0,
        Full = 1
    }
}
