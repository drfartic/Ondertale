using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WaveAttack : MonoBehaviour
{
    [Header("Attack")]
    public Vector2 areaSize;

    [Header("Behaviour")]
    public Vector2 offsetRange;
    public float targetOffset;
    public float inwaveAngle;
    public float inwaveSpeed;
    public AnimationCurve[] curves;
    public float curveDelay;
    public float curveSpeed;
    public float curveEndDelay;
    [Space(5)]
    public float experimentalYOffset;

    private float lOffsetDifference;
    private float uOffsetDifference;

    [Header("Graphics")]
    public Material waveMaterial;
    public int points;
    public GameObject fillblock;
    public Cord[] waveCords;

    private LineRenderer lowerLr;
    private LineRenderer upperLr;
    private List<GameObject> lowerWaveFill = new List<GameObject>();
    private List<GameObject> upperWaveFill = new List<GameObject>();

    const float Tau = 2 * Mathf.PI;
    private float linewidth;

    private int curveIndex;

    private float lowerOffset;
    private float upperOffset;
    private float startTime;
    private bool endSequenceStarted;

    [System.Serializable]
    public class Cord
    {   
        public float amplitude = 1;
        public float frequency = 1;
        public float movementSpeed = 1;
    }

    IEnumerator Start()
    {
        // Select curve
        curveIndex = Random.Range(0, curves.Length);

        // Start values and references
        linewidth = PlayerCombat.Instance.linewidth;
        startTime = Time.time;
        currentAreaSize = FieldManager.Instance.boxSprite.size;
        PlayerCombat.Instance.doTickDamage = false;


        // Offsets
        lowerOffset = transform.position.y - areaSize.y / 2f;
        foreach (var cord in waveCords)
        {
            lowerOffset -= cord.amplitude;
        }
        upperOffset = transform.position.y + areaSize.y / 2f;
        foreach (var cord in waveCords)
        {
            upperOffset += cord.amplitude;
        }

        lOffsetDifference = (-areaSize.y/2f + transform.position.y + targetOffset) - lowerOffset;
        uOffsetDifference = -((areaSize.y / 2f + transform.position.y - targetOffset) - upperOffset);

        // Setup
        SetupWaves();

        // Open Field
        yield return new WaitForSeconds(.5f);
        FieldManager.Instance.StartCoroutine(FieldManager.Instance.SetField(areaSize, 0.075f));

        DOTween.To(() => currentAreaSize, x => currentAreaSize = x, new Vector2(currentAreaSize.x, areaSize.y), 0.075f);
        yield return new WaitForSeconds(0.075f);
        DOTween.To(() => currentAreaSize, x => currentAreaSize = x, areaSize, 0.075f / 2f);
        PlayerCombat.Instance.canMove = true;

    }
    void SetupWaves()
    {
        GameObject downParent = new GameObject("Lower Wave");
        downParent.transform.parent = transform;

        lowerLr = downParent.AddComponent<LineRenderer>();
        lowerLr.startWidth = 0.1f;
        lowerLr.material = waveMaterial;

        for (int i = 0; i < points; i++)
        {
            GameObject g = Instantiate(fillblock, downParent.transform);
            g.transform.position = new Vector2(-areaSize.x / 2f + linewidth + i * ((areaSize.x - linewidth) / points), transform.position.y - areaSize.y / 2f);
            g.transform.localScale = new Vector3(((areaSize.x - linewidth) / points), 0.5f, 1f);
            lowerWaveFill.Add(g);
        }

        GameObject upParent = new GameObject("Upper Wave");
        upParent.transform.parent = transform;

        upperLr = upParent.AddComponent<LineRenderer>();
        upperLr.startWidth = 0.1f;
        upperLr.material = waveMaterial;

        for (int i = 0; i < points; i++)
        {
            GameObject g = Instantiate(fillblock, upParent.transform);
            g.transform.position = new Vector2(-areaSize.x / 2f + linewidth + i * ((areaSize.x - linewidth) / points), transform.position.y + areaSize.y / 2f);
            g.transform.localScale = new Vector3(((areaSize.x - linewidth) / points), 0.5f, 1f);
            upperWaveFill.Add(g);
        }
    }

    void Update()
    { 
        Draw();
    }

    Vector2 currentAreaSize;
    void Draw()
    {
        
        float xFinish = currentAreaSize.x - linewidth;

        lowerLr.positionCount = points;
        for (int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            // Sine wave
            float progress = (float)currentPoint / (points - 1);
            float x = Mathf.Lerp(0, xFinish, progress) - currentAreaSize.x/2 + linewidth/2f;
            float y = 0;
            foreach (var cord in waveCords)
            {
                y += GetCord(cord, x);
            }

            y += lowerOffset + GetOffset(points - currentPoint, true);
            // Add curve
            if (Time.time - startTime >= curveDelay)
            {
                float p = (Time.time - startTime - curveDelay) / (curves[curveIndex])[curves[curveIndex].length - 1].time * curveSpeed;
                y += curves[curveIndex].Evaluate(Mathf.Clamp(p - (1 - progress), 0f, (curves[curveIndex])[curves[curveIndex].length - 1].time));
            }

            y = Mathf.Clamp(y, transform.position.y - areaSize.y / 2f + linewidth / 2f, transform.position.y + areaSize.y / 2f - linewidth / 2f);
            y += experimentalYOffset;

            x = Mathf.Clamp(x, -currentAreaSize.x / 2f, currentAreaSize.x / 2f);

            lowerLr.SetPosition(currentPoint, new Vector3(x, y, 0));

            // dx Fill
            float relativeY = y + areaSize.y/2f - transform.position.y;
            lowerWaveFill[currentPoint].transform.localScale = new Vector3(lowerWaveFill[currentPoint].transform.localScale.x, relativeY, 1f);
        }

        upperLr.positionCount = points;
        for (int currentPoint = 0; currentPoint < points; currentPoint++)
        {
            // Sine wave
            float progress = (float)currentPoint / (points - 1);
            float x = Mathf.Lerp(0, xFinish, progress) - currentAreaSize.x / 2 + linewidth / 2f;
            float y = 0;
            foreach (var cord in waveCords)
            {
                y += GetCord(cord, x + 1.235f);
            }

            y += upperOffset - GetOffset(points - currentPoint, false);
            // Add curve
            if (Time.time - startTime >= curveDelay)
            {
                float p = (Time.time - startTime - curveDelay) / (curves[curveIndex])[curves[curveIndex].length - 1].time * curveSpeed;
                y += curves[curveIndex].Evaluate(Mathf.Clamp(p - (1 - progress),0f, (curves[curveIndex])[curves[curveIndex].length - 1].time));

                if((p - (1 - progress)) >= (curves[curveIndex])[curves[curveIndex].length - 1].time * curveSpeed + curveEndDelay && !endSequenceStarted)
                {
                    FieldManager.Instance.StartCoroutine(FieldManager.Instance.CloseField(0.075f));
                    DOTween.To(() => currentAreaSize, dx => currentAreaSize = dx, new Vector2(0f, currentAreaSize.y), 0.075f / 2f);
                    AttackManager.Instance.OnAttackEnd();
                    PlayerCombat.Instance.canMove = false;
                    PlayerCombat.Instance.doTickDamage = false;
                    endSequenceStarted = true;
                }
            }

            y = Mathf.Clamp(y, transform.position.y - areaSize.y / 2f + linewidth / 2f, transform.position.y + areaSize.y / 2f - linewidth / 2f);
            y -= experimentalYOffset;

            x = Mathf.Clamp(x, -currentAreaSize.x / 2f, currentAreaSize.x / 2f);

            upperLr.SetPosition(currentPoint, new Vector3(x, y, 0));

            // dx Fill
            float relativeY = -y + areaSize.y / 2f + transform.position.y;
            upperWaveFill[currentPoint].transform.localScale = new Vector3(upperWaveFill[currentPoint].transform.localScale.x, -relativeY, 1f);
        }
    }

    float GetCord(Cord i, float x)
    {
        float c = i.amplitude * Mathf.Sin((Tau * i.frequency * x) + (Time.timeSinceLevelLoad * i.movementSpeed));
        return c;
    }

    float GetOffset(int x, bool isLower)
    {
        float dTime = Time.time - startTime - 0.65f;
        float offset = inwaveAngle * -x + dTime * inwaveSpeed;

        offset = Mathf.Clamp(offset, 0, isLower ? lOffsetDifference : uOffsetDifference);
        return offset;
    }

    
}
