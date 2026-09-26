# יומן פיתוח — KaaSnake

המשחק הראשון שלי. אני בונה אותו בעצמי. היומן הזה הוא מקום לתעד מה ניסיתי, מה נשבר, ומה הבנתי — כדי שלא אחזור על אותה טעות פעמיים, וכדי שאוכל לראות בדיעבד איך התקדמתי.

---

## איך אני משתמש ביומן

רשומה אחת לכל סשן עבודה, או לכל באג משמעותי. לא צריך לכתוב יפה — צריך לכתוב **מדויק**. הערך של היומן הוא ב"מה הבנתי", לא ב"מה עשיתי".

כלל אחד: אני כותב את **ההסבר**, לא את הפתרון. אם אני כותב רק "תיקנתי", בעוד חודש זה לא יעזור לי.

### תבנית רשומה

```
## YYYY-MM-DD — כותרת קצרה

**מה ניסיתי לעשות:**

**מה קרה בפועל:**

**מה גיליתי / הבנתי:**

**API / מושגים שנגעתי בהם:**

**עדיין פתוח:**
```

---

## מילון API ומושגים

טבלה מצטברת של מה שלמדתי לאורך הדרך. כשאני נתקל במשהו חדש — הוא נכנס לכאן.

| API / מושג | מה זה עושה | איפה נתקלתי בזה |
|---|---|---|
| `Input.GetAxis` (Input Manager הישן) | מחזיר ערך מוחלק לאורך זמן בין ‎-1 ל־1. ההחלקה מוגדרת ב־Project Settings → Input Manager (gravity, sensitivity) | תנועה אופקית ב־SimplePlayer |
| `Input.GetAxisRaw` (הישן) | אותו דבר בלי החלקה — בדיוק ‎-1 / 0 / 1 | בדיקת קפיצה |
| `InputAction.ReadValue<T>()` (Input System החדש) | קורא את הערך הנוכחי של פעולה. **לא מחליק כלום** — raw בכוונה. החלקה היא באחריותי, דרך Processor או ידנית | קריאת תנועה ב־Update |
| Variable shadowing (C#) | הצהרת טיפוס לפני שם יוצרת משתנה **חדש** שמסתיר שדה קיים באותו שם. השמה בלי טיפוס כותבת לשדה הקיים | הבאג של 2026-09-23 |
| `Physics2D.CircleCast` | יורה מעגל בכיוון נתון ומחזיר פגיעה. משמש לבדיקת קרקע | GroundCheck |
| `Rigidbody2D.AddForce` + `ForceMode2D` | מפעיל כוח על גוף פיזיקלי. `Force` = מצטבר לאורך זמן, `Impulse` = מכה חד־פעמית | תנועה וקפיצה |
| Active Input Handling (Player Settings) | קובע איזו מערכת קלט פעילה: ישנה / חדשה / Both. במצב "New" כל קריאה ל־`Input.*` זורקת `InvalidOperationException` ועוצרת את שאר המתודה באותו פריים | כל הסשן של 2026-09-26 |
| Action Type: Value / Button | Value = "כמה" (ציר, סטיק). Button = "לחוץ או לא". Button על שליטה אנלוגית נהיה לחוץ מעל ה־Press Point | Move מול Jump |
| Control Type: Axis מול Vector2 | Axis = float אחד, Vector2 = שני צירים. הטיפוס ב־`ReadValue<T>` **חייב** להתאים, אחרת שגיאה בזמן ריצה | MoveAction |
| 1D Axis (Positive/Negative) Composite | מחבר שני מקשים לציר אחד: שלילי = ‎-1, חיובי = 1 | A/D לתנועה |
| 2D Vector — Digital Normalized | באלכסון הווקטור מנורמל, אז x יוצא ~0.707 ולא 1. לכן לא לקחת תנועה אופקית מ־Vector2 כשלמעלה = קפיצה | הסיבה להפריד Move מ־Jump |
| Processor (Axis Deadzone) | מסנן ערכים קטנים מסטיק רופף לפני שהם מגיעים לקוד | binding של הסטיק |
| Interaction: Press (Press Point) | הסף שבו שליטה אנלוגית נחשבת "לחוצה". מחליף את `jumpThreshold` שהיה בקוד | סטיק למעלה → קפיצה |
| `WasPressedThisFrame` / `IsPressed` | המקבילים החדשים של `GetKeyDown` / `GetKey` | קפיצה, היפנוט |
| `InputAction.Enable()` | Action שמוגדר ישירות על סקריפט כבוי כברירת מחדל — בלי זה הוא לא מחזיר כלום | Start של SimplePlayer |
| Binding מול קוד | "איזה מקש" מוגדר ב־Inspector, הקוד שואל רק "האם ה־Action לחוץ". לכן אין צורך ב־`||` בין מקלדת לשלט | תיקון PlayerHypnotize |
| `rb.linearVelocity.y` | מהירות אנכית אמיתית: חיובי = עולה, שלילי = נופל. מתאים לאנימציה יותר מהכוח שמפעילים | פרמטר `up` ב־Animator |
| Has Exit Time / Transition Duration | באנימציית ספרייטים: לכבות Exit Time ולשים Duration = 0, אחרת המעברים מתעכבים | מעברי קפיצה/נפילה |
| class מול instance, reference | הסקריפט הוא התבנית (class). מה שיושב על אובייקט הוא עותק (instance). משתנה כמו `playerMovement` הוא שם שמחזיק הפניה לעותק — לא שם של סקריפט | PlayerHypnotize ↔ SimplePlayer |
| Rename (F2) | משנה שם של שדה בכל הקבצים שמשתמשים בו בבת אחת | `V` → `verticalMove` |

---

## שאלות פתוחות

רשימה חיה. משהו שאני לא מבין ורוצה לחזור אליו.

- [x] האם להישאר על Input Manager הישן, לעבור לגמרי ל־Input System החדש, או להמשיך לערבב? → **עברתי לגמרי לחדש** (2026-09-26)
- [x] איך נכון לחלק אחריות בין `Update` ל־`FixedUpdate` → קלט נקרא ב־`Update`, כוחות מופעלים ב־`FixedUpdate`, והגשר ביניהם הוא **שדה** במחלקה (2026-09-26)
- [ ] `Embedded InputAction` על הסקריפט מול קובץ `InputSystem_Actions.inputactions` + קומפוננטת `PlayerInput` — מה ההבדל ומתי לעבור
- [ ] החלקת תנועה במקלדת: להסתמך על ה־`AddForce`, או להוסיף `Mathf.MoveTowards` / `SmoothDamp`?

---

# רשומות

## 2026-09-23 — הקלט מ־Vector2 לא זז

**מה ניסיתי לעשות:**
לקרוא תנועה דרך Input System החדש, ולראות את הערך בשדה `move` ב־Inspector.

**מה קרה בפועל:**
השדה `move` ב־Inspector נשאר קבוע ולא הראה טווח ערכים משתנה. גם הכוח האופקי לא הופעל על הגוף.

**מה גיליתי / הבנתי:**

1. **Shadowing.** הצהרתי משתנה חדש באותו שם של השדה, בתוך `Update`. C# לא מתלונן על זה — הוא פשוט יוצר משתנה מקומי שמסתיר את השדה לאורך כל המתודה. כל הכתיבות בתוך `Update` הלכו למשתנה המקומי; השדה עצמו אף פעם לא נכתב. `FixedUpdate` היא מתודה אחרת, אז שם נקרא השדה — שהוא אפס. מכאן שני התסמינים: Inspector קפוא, ותנועה שלא קורית.

   הכלל לזכור: **טיפוס לפני שם = משתנה חדש. שם לבד = השדה הקיים.**

2. **raw מול מוחלק.** ציפיתי לראות ערכי ביניים מטפסים, כמו שהכרתי מ־`Input.GetAxis`. אבל ההחלקה הזאת היא תכונה של ה־Input Manager **הישן** בלבד. ה־Input System החדש מחזיר ערך גולמי — במקלדת כל מקש הוא 0 או 1, ולכן מקבלים בדיוק ‎-1 / 0 / 1. עם ג'ויסטיק אנלוגי כן יתקבלו ערכי ביניים. אם אני רוצה החלקה במערכת החדשה — זו אחריות שלי.

3. אני קורא כרגע משתי מערכות קלט שונות באותה מתודה. זה עובד, אבל זה מקור לבלבול.

**API / מושגים שנגעתי בהם:**
`InputAction.ReadValue<Vector2>()`, `Input.GetAxis`, `Input.GetAxisRaw`, variable shadowing, Input Manager (gravity / sensitivity).

**עדיין פתוח:**
- לוודא שה־`InputAction` באמת מחוברים ל־bindings ב־Inspector — בלי זה הקריאה תחזיר אפס בכל מקרה
- ה־`using System.Numerics` בראש הקובץ מתנגש עם הטיפוסים של Unity ומכריח אותי לכתוב שמות מלאים. להבין למה הוא שם ואם הוא נחוץ
- להחליט על מערכת קלט אחת

---

## 2026-09-26 — מעבר מלא ל־Input System החדש + Cinemachine

**מה ניסיתי לעשות:**
להעביר את כל הקלט למערכת החדשה, להוריד את קוד המצלמה הידני (עובר ל־Cinemachine), ולחבר מחדש את התנועה, הקפיצה, האנימציה וההיפנוט.

**מה קרה בפועל:**
אחרי ששיניתי ב־Player Settings את Active Input Handling ל־"New", השחקן הפסיק להגיב לגמרי. ב־Console הופיעה `InvalidOperationException` על כל שימוש ב־`Input.*`.

**מה גיליתי / הבנתי:**

1. **שגיאה אחת עוצרת את כל המתודה.** הקריאה ל־`Input.GetAxis` הייתה בתחילת `Update`, אז כל מה שאחריה — מצלמה, קפיצה, GroundCheck — לא רץ. זה נראה כאילו "כלום לא עובד", אבל בפועל זו הייתה שורה אחת.

2. **ציר מול כפתור.** תנועה אופקית = Action מסוג **Value / Axis** (float בין ‎-1 ל־1), עם 1D Axis למקלדת ו־`leftStick/x` לשלט. קפיצה = Action מסוג **Button**. ה־Press Point של ה־Button החליף את `jumpThreshold` מהקוד.

3. **למה להפריד Move מ־Jump:** ב־Vector2 מנורמל, לחיצה על ימינה+למעלה נותנת x של ~0.707, כלומר השחקן היה מאט בכל קפיצה.

4. **Shadowing שוב.** `Vector2 move = ...` ב־Update יצר משתנה מקומי, ו־`FixedUpdate` קרא שדה ריק. אותו באג מ־2026-09-23 — שווה לזכור.

5. **`canMove` צריך לאפס מצב, לא רק לעצור.** אם חוזרים מוקדם מ־Update בלי לאפס את הקלט, את `jump` ואת ה־Timer, ה־FixedUpdate ממשיך להפעיל את הערך האחרון שנשאר בשדות.

6. **מחיקת שדה public שוברת סקריפטים אחרים.** מחקתי את `V` ו־`up`, ו־PlayerAnimation ו־PlayerHypnotize הפסיקו להתקמפל. שדה public הוא ממשק שאחרים נשענים עליו.

7. **אנימציה מהפיזיקה, לא מהקלט.** `up` מגיע מ־`rb.linearVelocity.y` (עולה/נופל), בדיוק כמו ש־`speed` מגיע מ־`linearVelocity.x`. ב־Animator: `jump` (עלייה) → `jump2` (נפילה) לפי `up`, חזרה ל־idle לפי `Grounded`.

8. **כל הקלט במקום אחד.** HypnotizeAction יושב ב־SimplePlayer, ו־PlayerHypnotize קורא אותו דרך ההפניה `playerMovement`. היפנוט הועבר ממקש Space כדי לא להתנגש בקפיצה.

9. **`||` חצי מתוקן עדיין שבור.** החלפתי רק את צד המקלדת בתנאי, וצד השלט (`Input.GetKey(JoystickButton0)`) המשיך לזרוק שגיאה. השלט מטופל עכשיו ב־binding, לא בקוד.

**API / מושגים שנגעתי בהם:**
Active Input Handling, Action Type (Value / Button), Control Type (Axis), 1D Axis Composite, Axis Deadzone, Press Interaction, `WasPressedThisFrame`, `IsPressed`, `InputAction.Enable()`, `rb.linearVelocity.y`, Has Exit Time, class מול instance, Rename (F2), Cinemachine Position Composer (Lookahead).

**עדיין פתוח:**
- לשנות את השם `verticalMove` ל־`horizontalMove` (הוא הציר האופקי) — F2 בשלושת הקבצים
- ניקוי ב־SimplePlayer: השדות `move` ו־`jumpThreshold` לא בשימוש, ה־alias `using Vector2 = ...` מיותר
- ניקוי `using`־ים לא בשימוש ב־PlayerAnimation — במיוחד `NUnit.Framework`, שעלול לשבור בילד
- להגדיר Lookahead ב־Cinemachine במקום המצלמה הידנית שהורדתי

---
