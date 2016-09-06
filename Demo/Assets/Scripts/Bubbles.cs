using UnityEngine;

public class Bubbles : MonoBehaviour
{
    public ParticleSystem bubbleParticles;
    public float emitEverySecond = 0.01f;
    public float speedEmitMultplier = 0.25f;
    public int minBubbles = 0;
    public int maxBubbles = 5;

    void Start()
    {
        if (!bubbleParticles)
            transform.GetComponent<ParticleSystem>();
    }

    public void EmitBubbles(Vector3 pos, float amount)
    {
        float f = amount * speedEmitMultplier;
        if (f < 1) return;
        bubbleParticles.transform.position = pos;
        bubbleParticles.Emit(Mathf.Clamp((int)(amount * speedEmitMultplier), minBubbles, maxBubbles));
    }
}

