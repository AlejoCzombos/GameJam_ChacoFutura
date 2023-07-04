using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [SerializeField] LayerMask mask = default;
    private Vector2 targetPosition;
    private Vector2 emptyPostition;
    private float xInput, yInput;
    private bool isMoving, isShake = false;

    void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if(!isMoving && Input.anyKeyDown && (xInput != 0f || yInput != 0f))
        {
            CalculateTargetPosition();
            if (CanMoveToTargetPosition())
            {
                StartCoroutine(MoveTwin());
            }
            else
            {
                if(!isShake) StartCoroutine(Shake());
            }
        }
    }

    private IEnumerator MoveTwin()
    {
        isMoving = true;

        var seq = DOTween.Sequence();

        seq.Join(transform.DOMove(targetPosition, 0.15f));
        seq.Join(transform.DOShakeScale(0.15f, 0.35f, 8, 0));

        yield return seq.WaitForCompletion();
        transform.localScale = Vector3.one;
        transform.position = targetPosition;
        isMoving = false;
    }

    private IEnumerator Shake()
    {
        isShake = true;
        yield return transform.DOPunchRotation(new Vector3(0, 0, -22.0f), 0.1f);
        transform.rotation = Quaternion.identity;
        isShake = false;
    }

    private void CalculateTargetPosition()
    {
        if (xInput == 1f)
        {
            targetPosition = (Vector2) transform.position + Vector2.right;
            emptyPostition = (Vector2) targetPosition + Vector2.right;
        }
        else if (xInput == -1f)
        {
            targetPosition = (Vector2)transform.position + Vector2.left;
            emptyPostition = (Vector2) targetPosition + Vector2.left;
        }
        else if (yInput == 1f) 
        {
            targetPosition = (Vector2)transform.position + Vector2.up;
            emptyPostition = (Vector2) targetPosition + Vector2.up;
        }
        else if (yInput == -1f) 
        {
            targetPosition = (Vector2)transform.position + Vector2.down;
            emptyPostition = (Vector2) targetPosition + Vector2.down;
        }
    }

    private bool CanMoveToTargetPosition()
    {
        bool colisiona = Physics2D.OverlapCircle(targetPosition, 0.15f);
        var collider = Physics2D.OverlapCircle(targetPosition, 0.15f);

        //Comprobar que donde me mueva es una vaca
        if (collider != null && collider.GetComponent<Cow>() != null)
        {
            var cowScript = collider.GetComponent<Cow>();
            //Hay un espacio libre detras
            if (!Physics2D.OverlapCircle(emptyPostition, 0.15f))
            {
                cowScript.MoveTo(emptyPostition);
                return true;
            }
            //Hay una vaca detras
            else if (!Physics2D.OverlapCircle(emptyPostition, 0.15f, mask))
            {
                return cowScript.CheckEvolution(emptyPostition);
            }
            //No se puede mover
            else
            {
                StartCoroutine(cowScript.AnimNoMove());
            }
        }
        return !colisiona;
    }
}
