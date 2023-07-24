using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using DG.Tweening;

public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance;

    [HideInInspector] public SpriteRenderer boxSprite;
    [SerializeField] private Transform maskSprite;

    void Awake(){
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        boxSprite = GetComponent<SpriteRenderer>();
        boxSprite.size = new Vector2(.1f, 0);
        maskSprite.localScale = new Vector2(.1f, 0);

        //yield return new WaitForSeconds(.5f);
        //StartCoroutine(SetField(new Vector2(6,5), 0.075f));
    }

    public IEnumerator SetField(Vector2 size, float duration)
    {
        PlayerCombat.Instance.areaSize = size;

        DOTween.To(() => boxSprite.size, x => boxSprite.size = x, new Vector2(boxSprite.size.x, size.y), duration);
        maskSprite.DOScaleY(size.y, duration);
        yield return new WaitForSeconds(duration);
        DOTween.To(() => boxSprite.size, x => boxSprite.size = x, size, duration/2);
        maskSprite.DOScale(size, duration/2f);

        PlayerCombat.Instance.canMove = true;
    }

    public IEnumerator CloseField(float duration)
    {
        PlayerCombat.Instance.canMove = false;

        DOTween.To(() => boxSprite.size, x => boxSprite.size = x, new Vector2(0.1f, boxSprite.size.y), duration / 2);
        maskSprite.DOScaleX(0.1f, duration/2f);
        yield return new WaitForSeconds(duration);
        DOTween.To(() => boxSprite.size, x => boxSprite.size = x, new Vector2(0.1f, 0f), duration);
        maskSprite.DOScale(new Vector2(0.1f, 0f), duration);
    }



}
