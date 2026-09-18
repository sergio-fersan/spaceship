using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script opcional de movimento da nave.
// Não faz parte do slide de Parallax, mas usa os 4 sprites (Ship01-04) para
// dar uma pequena animação de "propulsão" enquanto a nave se move.
// Anexe este script ao GameObject da nave (com um SpriteRenderer).
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerShip : MonoBehaviour
{
    public float speed = 5f;
    public float verticalLimit = 3.5f;
    public Sprite[] frames; // arraste Ship01, Ship02, Ship03, Ship04 aqui no Inspector
    public float frameRate = 0.1f;

    private SpriteRenderer sr;
    private int currentFrame;
    private float frameTimer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Movimento vertical simples (ajuste conforme o design do jogo)
        float move = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
        Vector3 pos = transform.position + new Vector3(0, move, 0);
        pos.y = Mathf.Clamp(pos.y, -verticalLimit, verticalLimit);
        transform.position = pos;

        // Animação simples trocando entre os 4 frames da nave
        if (frames != null && frames.Length > 0)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameRate)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % frames.Length;
                sr.sprite = frames[currentFrame];
            }
        }
    }
}
