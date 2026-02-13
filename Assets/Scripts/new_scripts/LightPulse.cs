using UnityEngine;
using UnityEngine.Rendering.Universal; // חובה! זה הספרייה של התאורה החדשה

public class LightPulse : MonoBehaviour
{
    [Header("Settings")]
    public float minIntensity = 0.5f; // עוצמה מינימלית (הכי חלש)
    public float maxIntensity = 1.5f; // עוצמה מקסימלית (הכי חזק)
    public float pulseSpeed = 2f;     // מהירות הפעימה

    private Light2D myLight;

    void Start()
    {
        // מוצאים את רכיב האור באופן אוטומטי
        myLight = GetComponent<Light2D>();
        
        if (myLight == null)
        {
            Debug.LogError("Error: No 'Light 2D' component found on this object!");
        }
    }

    void Update()
    {
        if (myLight == null) return;

        // יוצר תנועה חלקה בין המינימום למקסימום (כמו נשימה)
        float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        myLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
    }
}