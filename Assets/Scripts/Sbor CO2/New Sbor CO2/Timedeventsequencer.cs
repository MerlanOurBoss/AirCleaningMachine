using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Переиспользуемый "таймлайн": вместо стены if (elapsed >= X &amp;&amp; elapsed <= Y) { ... }
/// разбросанной по коду, события настраиваются данными (в инспекторе или из кода)
/// и триггерятся РОВНО ОДИН РАЗ за цикл, когда elapsed впервые попадает в диапазон.
///
/// Это устраняет типичный баг оригинального кода: эффект внутри "if (fillingCount >= 15 &&
/// fillingCount <= 16)" перезапускался на КАЖДОМ кадре, пока условие было истинным
/// (например AbsentOn() рестартовал корутину ~15-20 раз подряд).
/// </summary>
public class TimedEventSequencer : MonoBehaviour
{
    [System.Serializable]
    public struct TimedEvent
    {
        [Tooltip("Начало окна времени (в тех же единицах, что и elapsed, например fillingCount)")]
        public float from;

        [Tooltip("Конец окна времени")]
        public float to;

        [Tooltip("Только для читаемости в инспекторе, не используется в логике")]
        public string label;

        public UnityEvent onEnter;
    }

    [SerializeField] private List<TimedEvent> events = new List<TimedEvent>();

    private readonly HashSet<int> firedThisCycle = new HashSet<int>();

    /// <summary>Добавить событие из кода (для тех случаев, когда таймлайн собирается программно).</summary>
    public void AddEvent(float from, float to, UnityAction callback, string label = null)
    {
        var evt = new TimedEvent { from = from, to = to, label = label };
        evt.onEnter = new UnityEvent();
        evt.onEnter.AddListener(callback);
        events.Add(evt);
    }

    /// <summary>
    /// Вызывать каждый кадр (или каждый тик логики) с текущим значением elapsed.
    /// Событие сработает один раз за цикл, при первом попадании elapsed в [from; to].
    /// </summary>
    public void Tick(float elapsed)
    {
        for (int i = 0; i < events.Count; i++)
        {
            bool inRange = elapsed >= events[i].from && elapsed <= events[i].to;
            if (inRange)
            {
                if (firedThisCycle.Add(i))
                    events[i].onEnter?.Invoke();
            }
            else
            {
                // Позволяет событию сработать снова, если elapsed вышел из диапазона и
                // вернулся в него в рамках того же цикла (для симметричных сценариев).
                firedThisCycle.Remove(i);
            }
        }
    }

    /// <summary>Сбросить состояние "уже сработавших" событий перед началом нового цикла (Filling/Unfilling).</summary>
    public void ResetCycle()
    {
        firedThisCycle.Clear();
    }
}