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

public class SceneController : MonoBehaviour
{

    private bool isLastLevel;
    private int resultValue = 0;
    
	public Button turnButton;
	public Button collectButton;
    public Text resultValueText;
	public Text totalCoinsText; 
    public EnergySlotMachine slotMachine;
	public GameObject coinsPrefab;

	public RectTransform rectTransform;

	private void Awake()
	{      
    }
		
	private void Start()
    {      
    }
   

    void slotMachineWillStartSliding()
    {
        turnButton.interactable = false;
		collectButton.interactable = false;
    }

    void slotDidEndSliding(int i)
    {

		Debug.Log(i); 

    }


    void slotMachineDidEndSliding()
    {
       
         
         turnButton.interactable = true;
		 collectButton.interactable = true;
		 resultValueText.text = resultValue.ToString();
    }


 
	public void OnTurnClicked()
    {
		int minX = 30;
        int maxX = 999;

        resultValue = UnityEngine.Random.Range(minX, maxX);
        int hundredsDigit = ((resultValue) / 100);
        int tensDigit = ((resultValue) % 100) / 10;
        int unitDigit = (((resultValue) % 100) % 10);

        slotMachine.SlotResults = new int[] { unitDigit, tensDigit, hundredsDigit };
        slotMachine.slotMachineWillStartSliding += slotMachineWillStartSliding;
        slotMachine.slotMachineDidEndSliding += slotMachineDidEndSliding;
        slotMachine.slotDidEndSliding += slotDidEndSliding;
		slotMachine.startSliding();
              
    }

	public void OnCollectClicked()
	{   
		turnButton.interactable = false;
		collectButton.interactable = false;

		StartCoroutine(BatteryAnimation(resultValueText.gameObject.transform.position, () => { 
			turnButton.interactable = true;
            collectButton.interactable = true;
			totalCoinsText.text = resultValue.ToString(); 
		    
		}));


    }


	public virtual IEnumerator BatteryAnimation(Vector3 startPosition, Action completion)
    {      
        for (int i = 0; i < 10; i++)
        {
			GameObject coins = Instantiate(coinsPrefab);
			coins.transform.position = startPosition;
			RectTransform cellTransform = coins.GetComponent<RectTransform>();

			cellTransform.SetParent(rectTransform);
			cellTransform.sizeDelta = coinsPrefab.GetComponent<RectTransform>().sizeDelta;
			cellTransform.localScale = coinsPrefab.transform.localScale;
			iTween.MoveTo(coins, iTween.Hash("position", coinsPrefab.transform.localPosition, "time", 0.2f, "oncomplete", "OnMoveComplete", "oncompleteparams", coins, "oncompletetarget", this.gameObject, "isLocal", true));
            yield return new WaitForSeconds(0.1f);
        }

        if (completion != null) completion();
    }

	private void  OnMoveComplete(GameObject coin) {
		Destroy(coin);
	}

}
