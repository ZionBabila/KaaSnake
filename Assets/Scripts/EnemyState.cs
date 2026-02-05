using UnityEngine;
using System.Collections;

public class EnemyState : MonoBehaviour
{
    [Header("Identity & ID")]
    public string enemyName = "Monkey 1"; // השם שיוצג לשחקן ב-UI

    [Header("Settings & Targets")]
    public Transform buttonTarget; // המטרה אליה הקוף מגיע
    public string moveAndPressBool = "move_to_branch";
    public string hypnotizedBool = "isHypnotized";

    [Header("Bridge & Platform (Optional)")]
    public GameObject platformToOpen; // אובייקט שפשוט נדלק (SetActive)
    public Transform bridgeTransform; // גשר שמסתובב מ-90 ל-0
    public float bridgeOpeningSpeed = 2f; 

    [Header("Animation Offset")]
    // המשתנה שמונע את הקפיצה לאחור של האנימטור
    public Vector2 visualOffset; 

    [Header("Status")]
    public bool isActionCompleted = false; // מעדכן את השחקן שהמשימה בוצעה
    private bool isPerformingAction = false;
    private bool isLocked = false;
    private Vector3 basePosition;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        basePosition = transform.position; // שמירת מיקום התחלתי
    }

    void Start()
    {
        // הכנת הגשר במצב עמידה (90 מעלות)
        if (bridgeTransform != null)
        {
            bridgeTransform.rotation = Quaternion.Euler(0, 0, 90f);
        }
    }

    void Update()
    {
        // שימוש ב-visualOffset כדי לאפשר תנועה דרך האנימטור בלי "קפיצות"
        if (!isLocked)
        {
            transform.position = basePosition + (Vector3)visualOffset;
        }
    }

    // --- לוגיקת היפנוזה (מסונכרנת עם PlayerHypnotize) ---

    public void UpdateHypnosisProgress(float progressPercent)
    {
        // אם החיה כבר הופנטה או שהיא בתנועה, לא עושים כלום
        if (isActionCompleted || isPerformingAction) return;

        if (progressPercent >= 100f)
        {
            StartMonkeyAction();
        }
        else if (progressPercent >= 5f)
        {
            SetHypnosisVisual(true);
        }
        else
        {
            SetHypnosisVisual(false);
        }
    }

    public void StartMonkeyAction()
    {
        isPerformingAction = true;
        isLocked = false;
        basePosition = transform.position; // קיבוע נקודת המוצא לתנועה
        
        SetHypnosisVisual(false); // שחרור ויזואלי מהיפנוזה
        if (animator != null) animator.SetBool(moveAndPressBool, true);
    }

    // --- נקרא מה-Animation Event בפריים האחרון של move_to_branch ---

    public void FinishAndSnapPosition()
    {
        // 1. נעילה פיזית של המיקום
        if (buttonTarget != null)
        {
            transform.position = buttonTarget.position;
            basePosition = buttonTarget.position;
        }
        else
        {
            basePosition = transform.position;
        }

        visualOffset = Vector2.zero; // איפוס הסטייה
        isLocked = true; 
        
        // 2. עדכון המשתנים עבור השחקן
        isActionCompleted = true; 
        isPerformingAction = false;

        // 3. הפעלת אלמנטים בעולם עם בדיקות NULL
        if (platformToOpen != null) platformToOpen.SetActive(false);
        if (bridgeTransform != null) StartCoroutine(AnimateBridgeOpen());

        if (animator != null) animator.SetBool(moveAndPressBool, false);
        
        Debug.Log($"{enemyName} finished action!");
    }

    // --- תנועה חלקה של הגשר ---

    private IEnumerator AnimateBridgeOpen()
    {
        float elapsed = 0f;
        Quaternion startRotation = bridgeTransform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0f); // סיבוב ל-0 מעלות

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * bridgeOpeningSpeed;
            bridgeTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsed);
            yield return null;
        }
        bridgeTransform.rotation = targetRotation;
    }

    private void SetHypnosisVisual(bool state)
    {
        if (animator != null) animator.SetBool(hypnotizedBool, state);
    }

    // --- עזר ויזואלי ב-Scene View ---

    void OnDrawGizmos()
    {
        if (buttonTarget != null)
        {
            // הקו הופך לירוק ברגע שהמשימה הושלמה
            Gizmos.color = isActionCompleted ? Color.green : Color.cyan;
            Vector3 start = Application.isPlaying ? basePosition : transform.position;
            Gizmos.DrawLine(start, buttonTarget.position);
            Gizmos.DrawWireSphere(buttonTarget.position, 0.3f);
        }
    }
}