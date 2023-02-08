using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spray : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private int _spraySize = 5;

    private Renderer _renderer;
    private Color[] _colors;
    private float _sprayDistance;   // distance for spray raycast

    private RaycastHit _touch;
    private Wall _wall;
    private Vector2 _touchPos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private Quaternion _lastTouchRot;


    // Start is called before the first frame update
    void Start()
    {
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _spraySize * _spraySize).ToArray();
        _sprayDistance = _tip.localScale.y; // currently will only spray if tip is touching wall. need to add offset for proper spray.
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    private void Draw() {
        if (Physics.Raycast(_tip.position, -transform.right, out _touch, _sprayDistance)) { // check if we hit anything
            if (_touch.transform.CompareTag("Wall")) {  // check if what we hit is a wall
                /* Cache the wall object to save on search time for subsequent frames. */
                if (_wall == null) {
                    _wall = _touch.transform.GetComponent<Wall>();
                }
                /* Get UV texture coordinates then convert to pixel coordinates. */
                _touchPos = new Vector2(_touch.textureCoord.x, _touch.textureCoord.y);  // uv coords
                var x = (int)(_touchPos.x * _wall.textureSize.x - (_spraySize / 2));
                var y = (int)(_touchPos.y * _wall.textureSize.y - (_spraySize / 2));
                /* We don't want to draw if pixels are outside of texture. */
                if (y < 0 || y > _wall.textureSize.y || x < 0 || x > _wall.textureSize.x) {
                    return;
                }
                /* Draw. */
                if (_touchedLastFrame) {
                    _wall.texture.SetPixels(x, y, _spraySize, _spraySize, _colors); // draw current pixels
                    /* Interpolation from last pixels to current pixels. */
                    for (float f = 0.01f; f < 1.00f; f += 0.01f) {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _wall.texture.SetPixels(lerpX, lerpY, _spraySize, _spraySize, _colors);
                    }
                    transform.rotation = _lastTouchRot; // lock spray can rotation to prevent issues with physics interactions
                    _wall.texture.Apply();
                }
                /* Set information for the next frame. */
                _lastTouchPos = new Vector2(x, y);
                _lastTouchRot = transform.rotation;
                _touchedLastFrame = true;
                return;
            }
        }
        /* Uncache if no longer touching wall. */
        _wall = null;
        _touchedLastFrame = false;
        return;
    }

}
