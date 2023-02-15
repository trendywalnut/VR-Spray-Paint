using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Spray : MonoBehaviour
{
    [SerializeField] private Transform _tip;
    [SerializeField] private Transform strap;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleColor _particleColor;
    [SerializeField] private int _spraySize = 5;
    public Material[] materials;

    private Renderer _renderer;
    Renderer can_renderer;
    private Color[] _colors;
    [SerializeField] private float _sprayDistance;   // distance for spray raycast
    
    

    private RaycastHit _touch;
    private Wall _wall;
    private Vector2 _touchPos;
    private Vector2 _lastTouchPos;
    private bool _touchedLastFrame;
    private bool _triggerPulled;


    // Start is called before the first frame update
    void Start()
    {
        can_renderer = strap.GetComponent<Renderer>();
        can_renderer.sharedMaterial = materials[0];
        _renderer = _tip.GetComponent<Renderer>();
        _colors = Enumerable.Repeat(_renderer.material.color, _spraySize/3 * _spraySize/3).ToArray();
        _sprayDistance += _tip.localScale.y; // currently will only spray if tip is touching wall. need to add offset for proper spray.
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_triggerPulled) {
            SprayParticles();
            SprayAudio();
            Draw();
        }
        else {
            audioSource.Pause();
            _particleColor.particleSystem.Pause();
            _particleColor.particleSystem.Clear();
        }
    }

    public void TriggerPulled() {
        _triggerPulled = true;
    }

    public void TriggerReleased() {
        _triggerPulled = false;
        _wall = null;
        _touchedLastFrame = false;
    }

    private void Draw() {
        if (Physics.Raycast(_tip.transform.position, -transform.right, out _touch, _sprayDistance)) { // check if we hit anything
            //Debug.DrawRay(_tip.transform.position, -transform.right, Color.white, 2f);
            //Debug.Log(_touch.transform.name);
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
                    //try painting a cross shape instead of a rectangle
                    _wall.texture.SetPixels(x, y, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    _wall.texture.SetPixels(x + _spraySize/3, y, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    _wall.texture.SetPixels(x - _spraySize/3, y, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    _wall.texture.SetPixels(x, y + _spraySize/3, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    _wall.texture.SetPixels(x, y - _spraySize/3, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    _wall.texture.SetPixels(x + _spraySize/5, y - _spraySize/5, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    _wall.texture.SetPixels(x + _spraySize/5, y + _spraySize/5, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    _wall.texture.SetPixels(x - _spraySize/5, y + _spraySize/5, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    _wall.texture.SetPixels(x - _spraySize/5, y - _spraySize/5, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    /* Interpolation from last pixels to current pixels. */
                    for (float f = 0.01f; f < 1.00f; f += 0.01f) {
                        var lerpX = (int)Mathf.Lerp(_lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(_lastTouchPos.y, y, f);
                        _wall.texture.SetPixels(lerpX, lerpY, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                        _wall.texture.SetPixels(lerpX + _spraySize/3, lerpY, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                        _wall.texture.SetPixels(lerpX - _spraySize/3, lerpY, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                        _wall.texture.SetPixels(lerpX, lerpY + _spraySize/3, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                        _wall.texture.SetPixels(lerpX, lerpY - _spraySize/3, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                        _wall.texture.SetPixels(lerpX + _spraySize/5, lerpY - _spraySize/5, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                        _wall.texture.SetPixels(lerpX + _spraySize/5, lerpY + _spraySize/5, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                        _wall.texture.SetPixels(lerpX - _spraySize/5, lerpY + _spraySize/5, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                        _wall.texture.SetPixels(lerpX - _spraySize/5, lerpY - _spraySize/5, _spraySize/3, _spraySize/3, _colors); // draw current pixels
                    }
                    _wall.texture.Apply();
                }
                /* Set information for the next frame. */
                _lastTouchPos = new Vector2(x, y);
                _touchedLastFrame = true;
                return;
            }
        }
        /* Uncache if no longer touching wall. */
        _wall = null;
        _touchedLastFrame = false;
        return;
    }

    private void SprayAudio() {
        if (!audioSource.isPlaying) {
            audioSource.Play();
        }
        else {
            Debug.Log("playing audio");
        }
    }

    private void SprayParticles() {
        _particleColor.particleSystem.Play();
    }

    private void OnTriggerEnter(Collider col) {
        _particleColor.UpdateParticleColor(col.gameObject.tag);
        switch (col.gameObject.tag)
        {
            case "Red":
                _renderer.sharedMaterial = materials[1];
                can_renderer.sharedMaterial = materials[1];
            break;
            case "Orange":
                _renderer.sharedMaterial = materials[2];
                can_renderer.sharedMaterial = materials[2];

            break;
            case "Yellow":
                _renderer.sharedMaterial = materials[3];
                can_renderer.sharedMaterial = materials[3];
            break;
            case "Green":
                _renderer.sharedMaterial = materials[4];
                can_renderer.sharedMaterial = materials[4];
            break;

            case "Blue":
                _renderer.sharedMaterial = materials[5];
                can_renderer.sharedMaterial = materials[5];
            break;

            case "Violet":
                _renderer.sharedMaterial = materials[6];
                can_renderer.sharedMaterial = materials[6];
            break;
            case "Brown":
                _renderer.sharedMaterial = materials[7];
                can_renderer.sharedMaterial = materials[7];
            break;
            case "Black":
                _renderer.sharedMaterial = materials[8];
                can_renderer.sharedMaterial = materials[8];
            break;
            case "White":
                _renderer.sharedMaterial = materials[9];
                can_renderer.sharedMaterial = materials[9];
            break;

            default:
            break;
        }
        _colors = Enumerable.Repeat(_renderer.material.color, _spraySize/3 * _spraySize/3).ToArray();
    }

}
