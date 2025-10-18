using DG.Tweening;
using UltEvents;
using UnityEngine;

public class DOTweenAnimator : MonoBehaviour
{
    protected Sequence mySequence;
    [Header("Dejar vacio si se quiere usar este propio Gameobject como target")][SerializeField] protected GameObject target;

    [SerializeField] private DOTweenAnimation[] animations;

    [SerializeField] private float delay;
    [Header("-1 para un loop infinito")][SerializeField] private int loops;
    [SerializeField] public UltEvent OnFinishAnimation;

    public Sequence MySequence { get => mySequence; set => mySequence = value; }
    public GameObject Target { get => target; set => target = value; }
    public DOTweenAnimation[] Animations { get => animations; set => animations = value; }

    [SerializeField] private bool startInAwake;
    [SerializeField] private bool animateWithDelay;

    private void Awake()
    {
        if (target == null)
            target = gameObject;

        //ATENCION, la referencia a este evento cuando se hace kill a la secuencia ? 
        mySequence = DOTween.Sequence();
        mySequence.onComplete += () => { OnFinishAnimation.Invoke(); };


        if (startInAwake)
            Animate();

    }

    public void ResetAnimation()
    {
        mySequence.Restart();
    }

    public void PauseAnimation()
    {
        mySequence.Pause();
    }

    public void UpdateAnimations()
    {
        mySequence.Kill();
        mySequence = DOTween.Sequence();
        mySequence.onComplete += () => { OnFinishAnimation.Invoke(); };
    }
    public void FinishAnimate()
    {
        if (mySequence.IsComplete() || mySequence.IsPlaying())
        {
            ResetAnimation();
        }
        else
        {
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].Animate(mySequence, target);
            }
            if (loops != 0)
                mySequence.SetLoops(loops);
            mySequence.Play();
        }
    }

    public void Animate()
    {
        Invoke("FinishAnimate", delay);
    }
}
