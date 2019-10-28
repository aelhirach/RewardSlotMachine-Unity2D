// Copyright (c) 2019 EL HIRACH Abderrazzak (smartsquare)
// Please direct any bugs/comments/suggestions to http://smartsquare.be
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System;


public class EnergySlotMachine : MonoBehaviour
{

    public GameObject defaultSlot; 

    public Image background;
    public List<GameObject> slotScrollLayerArray;
    public List<List<GameObject>> slotScrollSubLayerArray;
    private int[] slotResults;
    public Action slotMachineWillStartSliding;
    public Action slotMachineDidEndSliding;
    public Action<int> slotDidEndSliding;

    private int[] currentSlotResults;
    private int kMinTurn = 2;
    public Sprite[] iconsForSlots;
    private bool isSliding = false;
    List<float> completePositionArray;

    private RectTransform rectTransform;

    public int[] SlotResults
    {
        get
        {
            return slotResults;
        }

        set
        {
            slotResults = value;

            if (!isSliding)
            {
                slotResults = value;
                if (currentSlotResults == null)
                {
                    currentSlotResults = new int[slotResults.Length];

                }

            }
        }
    }

    public void reloadData()
    {

        int iconCount = iconsForSlots.Length;
        var singleUnitHeight = rectTransform.rect.size.y / 3;
        slotScrollSubLayerArray = new List<List<GameObject>>();

        for (int i = 0; i < slotScrollLayerArray.Count; i++)
        {
            var slotScrollLayer = slotScrollLayerArray[i];
            var subLayers = new List<GameObject>();

            int scrollLayerTopIndex = -(i + kMinTurn + 3) * iconCount;
            int j = 0;
            while (j > scrollLayerTopIndex)
            {
                Sprite iconImage = (iconsForSlots[Math.Abs(j) % iconCount]);
                GameObject slot = Instantiate(defaultSlot);
                slot.GetComponent<Image>().sprite = iconImage;
                int offsetYUnit = j + 1 + iconCount;
                var aWith = slotScrollLayer.GetComponent<RectTransform>().rect.size.x;
                RectTransform slotTransform = slot.GetComponent<RectTransform>();
                slotTransform.SetParent(slotScrollLayer.transform);
                slotTransform.sizeDelta = new Vector2(aWith, singleUnitHeight);
                slotTransform.localScale = Vector3.one;
                slotTransform.localPosition = new Vector3(0, -offsetYUnit * singleUnitHeight);
                j -= 1;
                subLayers.Add(slot);
            }
            slotScrollSubLayerArray.Add(subLayers);
        }

    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Use this for initialization
    void Start()
    {

        reloadData();
        
    }

    public void startSliding()
    {
        if (isSliding)
        {
            return;
        }
        else
        {
            isSliding = true;
            if (slotMachineWillStartSliding != null) slotMachineWillStartSliding();
            int slotIconsCount = iconsForSlots.Length;
            completePositionArray = new List<float>();

            for (int i = 0; i < slotScrollLayerArray.Count; i++)
            {   
                var slotScrollLayer = slotScrollLayerArray[i];
                int resultIndex = slotResults[i];
                int currentIndex = currentSlotResults[i];
                int howManyUnit = (i + kMinTurn) * slotIconsCount + resultIndex - currentIndex;
                var slideY = howManyUnit * (rectTransform.rect.size.y / 3);
                var nextPosition = new Vector3(slotScrollLayer.transform.localPosition.x, slotScrollLayer.transform.localPosition.y - slideY, slotScrollLayer.transform.localPosition.z);
                completePositionArray.Add(nextPosition.y);
                iTween.MoveTo(slotScrollLayer, iTween.Hash(
                    "position", nextPosition, "delay", i * 0.5f, "time", 0.5f, "isLocal", true, 
                    "oncomplete", "OnCompleteliding", "oncompletetarget", gameObject, "oncompleteparams", i,
                    "onstart", "OnStartSliding", "onstarttarget", gameObject, "onstartparams", i
                   ));

            }


        }
    }

    void OnStartSliding(int i){
		Debug.Log("Slot" + i + "Starts to slide");
        
    }
    

    void OnCompleteliding(int k){
        if (slotDidEndSliding != null) slotDidEndSliding(k);
        if (k == 2 && slotMachineDidEndSliding != null)
        {
            isSliding = false;
            slotMachineDidEndSliding();
            int slotIconsCount = iconsForSlots.Length;

            for (int i = 0; i < slotScrollLayerArray.Count; i++)
            {
                var slotScrollLayer = slotScrollLayerArray[i];
                var position = new Vector3(slotScrollLayer.transform.localPosition.x, completePositionArray[i], slotScrollLayer.transform.localPosition.z);
                slotScrollLayer.transform.localPosition = position;

                var toBeDeletedSlotArray = new List<GameObject>();
                int resultIndex = slotResults[i];
                int currentIndex = currentSlotResults[i];
                for (int j = 0; j < slotIconsCount * (kMinTurn + i) + resultIndex - currentIndex; j++)
                {
                    GameObject aSlot = slotScrollLayer.transform.GetChild(j).gameObject;
                    toBeDeletedSlotArray.Add(aSlot);
                }
                foreach (GameObject toBeDeletedSlot in toBeDeletedSlotArray)
                {

                    GameObject toBeAddedSlot = Instantiate(defaultSlot);
                    toBeAddedSlot.GetComponent<Image>().sprite = toBeDeletedSlot.GetComponent<Image>().sprite;
                    var singleUnitHeight = toBeDeletedSlot.GetComponent<RectTransform>().rect.size.y;
                    var aWith = slotScrollLayer.GetComponent<RectTransform>().rect.size.x;
                    RectTransform toBeAddedSlotTransform = toBeAddedSlot.GetComponent<RectTransform>();
                    toBeAddedSlotTransform.SetParent(slotScrollLayer.transform);
                    toBeAddedSlotTransform.sizeDelta = new Vector2(aWith, singleUnitHeight);
                    toBeAddedSlotTransform.localScale = Vector3.one;
                    float shiftY = slotIconsCount * singleUnitHeight * (kMinTurn + i + 3);
                    toBeAddedSlotTransform.localPosition = new Vector3(0, toBeDeletedSlot.transform.localPosition.y + shiftY);
                    Destroy(toBeDeletedSlot);

                }
                toBeDeletedSlotArray = new List<GameObject>();
            }

                currentSlotResults = slotResults;
                completePositionArray = new List<float>();
            }


    }

    // Update is called once per frame
    void Update () {
        
    }
}
