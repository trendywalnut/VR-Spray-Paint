using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Texture2D texture;   // texture of the wall
    public Vector2 textureSize = new Vector2(2048, 2048);   // texture resolution

    // Start is called before the first frame update
    void Start()
    {
        // create and set the texture of the wall
        var r = GetComponent<Renderer>();
        texture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        r.material.mainTexture = texture;
    }
}
