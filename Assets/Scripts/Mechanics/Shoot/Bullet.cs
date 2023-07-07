using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Bullet : MonoBehaviour
{
    private PlayerValues _playerValues;
    public bool isPlayer;
    private Vector3 _direction;
    private bool shot;
    private Vector3 respawn;
    [SerializeField] private ParticleSystem hit;

    private float speed = 1;
    [SerializeField] private Rigidbody _rigidbody;
    private bool ready = true;

    public LayerMask collisionLayers;

    //audio
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
        _playerValues = FindObjectOfType<PlayerValues>();
        ready = false;
        _rigidbody.useGravity = false;
    }

    public void SetIsReadyToUse(bool val)
    {
        ready = val;
    }

    public bool IsReadyToUse()
    {
        return ready;
    }

    public void Shoot(bool player, Vector3 origin, Vector3 dir, float s, Vector3 checkpoint)
    {
        isPlayer = player;
        respawn = checkpoint;
        speed = s;
        transform.position = origin;
        _direction = dir.normalized;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.useGravity = false;
        _rigidbody.AddForce(_direction * speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!ready)
        {
            if (isPlayer)
            {
            }
            else
            {
                if (!_playerValues.dead && collision.transform.CompareTag("Player"))
                {
                    _playerValues.RecieveDamage(respawn);
                }
            }

            audioSource.Play();
            hit.Play();
        }

        if (MyUtils.IsInLayerMask(collision.gameObject, collisionLayers))
        {
            ready = true;
            _rigidbody.useGravity = true;
        }
    }
}