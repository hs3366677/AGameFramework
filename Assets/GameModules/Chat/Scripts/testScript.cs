using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testScript : MonoBehaviour {

    Text mText;
	// Use this for initialization
	void Start () {
        mText = GetComponent<Text>();
        //mText.text = "";
        Debug.Log("rect is " + mText.cachedTextGeneratorForLayout.rectExtents);


        TextGenerator textGenenerator = new TextGenerator();
        TextGenerationSettings generationSettings =
        mText.GetGenerationSettings(mText.rectTransform.rect.size);
        float lineHeight = textGenenerator.GetPreferredHeight("A", generationSettings);

        Debug.Log("text height = " + lineHeight);

        Font textFont = mText.font;
        Debug.LogFormat("font line height : {0}; font default size : {1}; prefererd Height : {2}", textFont.lineHeight, textFont.fontSize, mText.preferredHeight);

        //1行还是16；2行是48；4行 2倍行距 112 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
