using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotionEmoji : MonoBehaviour
{
    Image m_emoji;
    int m_emojiId = 0;
    int m_currentFrame = 0;
    int m_currentEmojiFrame = 0;
    float m_frameTimer = 0.0f;
    List<Sprite> m_sprList = new List<Sprite>();

    static float m_frameRate = 0.1f;
    static int s_emojiMaxFrame = 20;

    void Awake()
    {
        m_emoji = GetComponent<Image>();
    }

    public void Init(int emojiId)
    {
        m_emoji = GetComponent<Image>();
        
        m_emojiId = emojiId;

        for(int i = 0 ;i < s_emojiMaxFrame ; i++)
        {
            string sprName = string.Format("{0}_{1}", m_emojiId, i);
            if (MixedLabelUtil.SpriteContains(sprName))
                m_currentEmojiFrame = i;
            else
                break;
        }
    }
    
    void Update()
    {
        m_frameTimer += Time.deltaTime;
        if(m_frameTimer > m_frameRate)
        {
            m_emoji.sprite = MixedLabelGlobal.emojiFactory.CreateSprite(string.Format("{0}_{1}", m_emojiId, m_currentFrame++));
            //MixedLabelUtil.GetSpriteByName(string.Format("{0}_{1}", m_emojiId, m_currentFrame++));
            m_frameTimer -= m_frameRate;

            if (m_currentFrame > m_currentEmojiFrame)
                m_currentFrame = 0;
        }
    }
}