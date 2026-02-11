using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
    [Header("Settings")]
    public float normalSize = 5f;        // גודל רגיל של המשחק
    public float wideSize = 8f;          // גודל רחב (כשפותחים פריים)
    public float smoothSpeed = 2f;       // מהירות המעבר

    private Camera cam;
    private float targetSize;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetSize = normalSize; // מתחילים רגיל
    }

    void Update()
    {
        // הפקודה הזו דואגת שהמצלמה תמיד תחליק לגודל היעד הנוכחי
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * smoothSpeed);
    }

    // --- פונקציות שהסקריפט שלך יפעיל (ה"סימנים") ---

    // 1. פקודה לפתיחת הפריים (זום אאוט)
    public void GoWide()
    {
        targetSize = wideSize;
    }

    // 2. פקודה לחזרה למצב רגיל
    public void GoNormal()
    {
        targetSize = normalSize;
    }

    // 3. אופציה מיוחדת: פתח פריים ל-3 שניות ואז תחזור לבד
    public void PulseWideView()
    {
        targetSize = wideSize;
        Invoke("GoNormal", 4.0f); // אחרי 4 שניות יחזיר אוטומטית לנורמל
    }
}