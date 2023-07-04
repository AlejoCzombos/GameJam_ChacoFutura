using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cow : MonoBehaviour
{
    [SerializeField] private Sprite[] imagenesNiveles;
    [SerializeField] private int nivelActual;

    private Vector2 newPosition;

    public int NivelActual { get => nivelActual; set => nivelActual = value; }
    public Sprite[] ImagenesNiveles { get => imagenesNiveles; set => imagenesNiveles = value; }

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ImagenesNiveles[ (NivelActual - 1) ];
    }

    public void MoveTo(Vector2 newPosition)
    {
        this.newPosition = newPosition;
        StartCoroutine(AnimMove());
    }
    public IEnumerator AnimNoMove()
    {
        yield return transform.DOPunchRotation(new Vector3(0, 0, 15.0f), 0.1f);
        transform.rotation = Quaternion.identity;
    }
    IEnumerator AnimMove()
    {
        var seq = DOTween.Sequence();

        seq.Join(transform.DOMove(newPosition, 0.15f));
        seq.Join(transform.DOShakeScale(0.15f, 0.5f, 8, 0));

        transform.localScale = Vector3.one;
        transform.position = newPosition;

        yield return seq.WaitForCompletion();
    }

    IEnumerator AnimEvolutionCow(Vector2 nextPosition, GameObject nextCow)
    {
        yield return transform.DOMove(nextPosition, 0.15f).WaitForCompletion();
        
    }

    public bool CheckEvolution(Vector2 nextPosition)
    {
        var nextCow = Physics2D.OverlapCircle(nextPosition, 0.15f).gameObject;
        var nextCowScript = nextCow.GetComponent<Cow>();
        
        //Las vacas son del mismo nivel y se pueden juntar
        if (nextCowScript.NivelActual == this.nivelActual)
        {
            StartCoroutine(AnimEvolutionCow(nextPosition, nextCow));
            nivelActual++;
            gameObject.GetComponent<SpriteRenderer>().sprite = imagenesNiveles[nivelActual-1];
            Destroy(nextCow);
            return true;
        }
        else
        {
            StartCoroutine(AnimNoMove());
            return false;
        }
    }

}
