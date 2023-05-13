using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wall : MonoBehaviour
{
    [SerializeField] private RectTransform rectTrans;
    [SerializeField] private Image background;

    private void Start()
    {
    }

    public Vector2 GetPos()
    {
        return rectTrans.anchoredPosition;
    }

    public void Hide()
    {
        background.color = Color.clear;
    }

    public void Show()
    {
        background.color = Color.white;
    }

    public bool IsPlayerInWall(Vector2 playerPos)
    {
        Rect rect = rectTrans.rect;

        float left = rectTrans.anchoredPosition.x - rect.width / 2;
        float right = rectTrans.anchoredPosition.x + rect.width / 2;
        float top = rectTrans.anchoredPosition.y + rect.height / 2;
        float bot = rectTrans.anchoredPosition.y - rect.height / 2;

        if (playerPos.x <= right && playerPos.x >= left &&
            playerPos.y <= top && playerPos.y >= bot)
            return true;
        return false;
    }
}