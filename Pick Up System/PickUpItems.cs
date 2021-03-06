﻿using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItems : MonoBehaviour
{
    private GameObject canvasPickUpItems;
    internal GameObject canvasBuyItems;
    public GameObject productObj;
    private Products productScript;
    private Transform parentTransform;
    private SoundPlayer soundPlayer;
    private PlayerMovement playerScript;
    internal CartItemManager itemManager;
    public GameObject animationProducts;

    #region Init Functions
    private void Awake()
    {
        gameObject.AddComponent<CartItemManager>();
        itemManager = gameObject.GetComponent<CartItemManager>();
    }


    void Start()
    {
        productScript = FindObjectOfType<Products>();
        soundPlayer = FindObjectOfType<SoundPlayer>();
        canvasPickUpItems = FindCanvas("Canvas-PickUpItems");
        canvasBuyItems = FindCanvas("Canvas-BuyItems");
        playerScript = FindObjectOfType<PlayerMovement>();
        AssignAnimationProduct("goods-top");
    }

    
    void Update()
    {
        if (!playerScript.hasKart && canvasPickUpItems.activeSelf)
        {
            canvasPickUpItems.SetActive(false);
        }
    }
    #endregion

    #region OnTrigger Functions
    private void OnTriggerEnter(Collider objectCollided)
    {
        if ((canvasPickUpItems != null) && IsTargetTrigger(objectCollided) && playerScript.hasKart)
        {
            ChangeCanvasText(canvasPickUpItems, productObj.name);
            if (!itemManager.IsProductChecked(productObj.name))
            {
                canvasPickUpItems.SetActive(true);
                soundPlayer.PlaySoundClip("Pop-Up");
            }
        }
    }

    private void OnTriggerStay(Collider objectCollided)
    {
        GameObject objC = objectCollided.gameObject;

        if (IsTargetTrigger(objectCollided) && (!itemManager.IsProductChecked(productObj.name)))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                canvasPickUpItems.SetActive(false);
                soundPlayer.PlaySoundClip("Box 1");
                SetChildProductActive(true, true, 1f,"goods-top", productObj.name);
                itemManager.CheckProductIntoCart(productObj.name, true);
                //Debug.Log($"Product acquired, product has name of: { productObj.name}");
            }
        }
    }

    private void OnTriggerExit(Collider objectCollided)
    {
        if (canvasPickUpItems != null && canvasPickUpItems.activeSelf)
            canvasPickUpItems.gameObject.SetActive(false);
    }
    #endregion

    #region Personalized Functions

    #region Miscellaneous Functions
    private bool IsTargetTrigger(Collider targetCollider)
    {
        productObj = productScript.GetProduct(targetCollider.name);
        bool val = productObj !=null ? true : false;

        return val;
    }

    private GameObject FindCanvas(string canvasName)
    {
        parentTransform = transform.parent;

        foreach (Transform child in parentTransform)
        {
            if (child.name == canvasName)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    private IEnumerator DestroyChild(Transform objectToDestroy, float timeToWait, bool stationaryObject)
    { //Destroys "gameObject animation" and then activates stationary gameObject

        yield return new WaitForSeconds(timeToWait);
        Destroy(objectToDestroy.gameObject);

        if (stationaryObject)
            SetChildProductActive(true, "goods", productObj.name);
    }

    private void ChangeCanvasText(GameObject canvas, string product)
    {
        Text input = null;

        if (canvas.TryGetComponent(out Canvas c) )
        {
            input = canvas.GetComponentInChildren<Text>();
            input.GetComponent<Text>().text = $"Press R to Get {product}";

            Debug.Log($"<color=yellow> Canvas text was changed</color>: {input.text}");
          //  Debug.Log($"<color=yellow> Text element</color>: {input}", input);
        }
    }

    #endregion

    #region SetChildProductActive functions

    internal void SetChildProductActive(bool activate, string targetChild)
    { //activates or deactivates all prdocts inside goodsTransform with targetChild's name
        Transform goodsTransfrom = null;

        foreach (Transform childOfGameObject in transform)
        {
            if (childOfGameObject.name.Equals(targetChild, StringComparison.OrdinalIgnoreCase))
                goodsTransfrom = childOfGameObject;
        }

        if (goodsTransfrom != null)
        {
            foreach (Transform child in goodsTransfrom)
            {
                child.gameObject.SetActive(activate);
            }
        }
    }

    private void SetChildProductActive(bool activate, string targetChild, string productName) 
    { //activates or deactivates the product with productName  inside goodsTransform with targetChild's name
        Transform goodsTransfrom = null;

        foreach (Transform childOfGameObject in gameObject.transform)
        {
            if (childOfGameObject.name.Equals(targetChild, StringComparison.OrdinalIgnoreCase))
                goodsTransfrom = childOfGameObject;
        }

        if(goodsTransfrom != null)
        {
            foreach (Transform child in goodsTransfrom)
            {
                if (child.name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                    child.gameObject.SetActive(activate);
            }
        }
    }

    private void SetChildProductActive(bool activate, bool deleteFallingChild, float time, string targetChild, string productName)
    { //actiavtes or deactivates the product with productname (from goodsTransform with targetChild's name) and after some time, destroys (or not) the "animation" product
        Transform goodsTransfrom = null;

        foreach (Transform childOfGameObject in transform)
        {
            if (childOfGameObject.name.Equals(targetChild, StringComparison.OrdinalIgnoreCase))
                goodsTransfrom = childOfGameObject;
        }


        if (goodsTransfrom != null)
        {
            foreach (Transform childFound in goodsTransfrom)
            {
                if (childFound.name.Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    childFound.gameObject.SetActive(activate);

                    if (deleteFallingChild)
                        StartCoroutine(DestroyChild(childFound, time, true));
                }
            }
        }
    }
    #endregion

    
    #region AnimationProduct Functions
    private void AssignAnimationProduct(string targetChild)
    {// assigns the local transform properties of targetChild to the animationProducts
        foreach (Transform childOfGameObject in transform)
        {
            if (childOfGameObject.name.Equals(targetChild, StringComparison.OrdinalIgnoreCase))
            {
                animationProducts.transform.localPosition = childOfGameObject.localPosition;
                animationProducts.transform.localScale = childOfGameObject.localScale;
                animationProducts.transform.localRotation = childOfGameObject.localRotation;
                animationProducts.transform.localEulerAngles = childOfGameObject.localEulerAngles;
            }
                
        }

    }

    internal void SpawmAnimationProduct(string targetChild, bool destroyTargetChild) //Spawns the animation products in the same spot as "goods-top" for its reuse
    {
        GameObject clone;

        foreach (Transform childOfGameObject in transform)
        {
            if (childOfGameObject.name.Equals(targetChild, StringComparison.OrdinalIgnoreCase))
            {
                if (destroyTargetChild)
                {
                    Destroy(childOfGameObject.gameObject);
                }
            }
        }
        clone = Instantiate(animationProducts, transform, false);
        clone.name = targetChild;
    }
    #endregion

    #endregion
}
