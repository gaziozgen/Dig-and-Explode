using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
[RequireComponent(typeof(CapsuleTriggerChecker))]
public class Interactor : MonoBehaviour
{
    [SerializeField] float interactionInterval = 0.15f;
    List<Interaction> interactions = new();
    CapsuleTriggerChecker triggerChecker;
    bool locked = false;
    bool moving = false;
    List<Interaction> lockedInteractions = new();
    Collider[] hitColliders;
    List<Interaction> currentInteractions = new();
    public delegate IEnumerator InteractionRoutine(GameObject obj);
    public delegate bool InteractionEnterCondition();
    private void Awake()
    {
        triggerChecker = GetComponent<CapsuleTriggerChecker>();

    }
    private void Start()
    {
        if (triggerChecker)
            InvokeRepeating(nameof(CheckInteraction), 0, interactionInterval);
    }
    public void AddInteraction(Interaction interaction)
    {
        if (interactions.Contains(interaction)) return;
        interactions.Add(interaction);
    }
    public void RemoveInteraction(Interaction interaction)
    {
        if (!interactions.Contains(interaction)) return;
        interactions.Remove(interaction);
    }
    public void Lock() => locked = true;
    public void Unlock() => locked = false;
    public void SetMoving(bool moving) => this.moving = moving;

    private void CancelInteraction(Interaction interaction)
    {
        currentInteractions.Remove(interaction);
        StopCoroutine(interaction.coroutine);
        interaction.coroutine = null;
        interaction.isProgressing = false;
        interaction.onCancel?.Invoke();
    }
    private void CheckInteraction()
    {
        if (!triggerChecker)
        {
            Debug.LogError("There is no trigger checker!", this);
            return;
        }
        if (!enabled) return;
        int numColliders = triggerChecker.CheckTrigger(out hitColliders);
        List<Collider> colliders = new(hitColliders);
        for (int i = 0; i < numColliders; i++)
        {
            Collider collider = hitColliders[i];


            for (int j = 0; j < interactions.Count; j++)
            {
                Interaction interaction = interactions[j];
                if (currentInteractions.Contains(interaction)) continue;
                bool doBreak = false;
                for (int k = 0; k < currentInteractions.Count; k++)
                {
                    if (currentInteractions[k].collider == collider)
                    {
                        doBreak = true;
                        break;
                    }
                }
                if (doBreak) break;
                if (collider.CompareTag(interaction.tag))
                {
                    if (Interact(interaction, collider.gameObject, out Coroutine coroutine))
                    {
                        interaction.coroutine = coroutine;
                        interaction.collider = collider;
                        currentInteractions.Add(interaction);
                    }
                }
            }

        }
        if (lockedInteractions.Count > 0)
        {
            List<Interaction> interactionsToUnlock = new();
            for (int i = 0; i < lockedInteractions.Count; i++)
            {
                Interaction interaction = lockedInteractions[i];
                if (!colliders.Contains(interaction.collider))
                {
                    interactionsToUnlock.Add(interaction);
                }
            }
            for (int i = 0; i < interactionsToUnlock.Count; i++)
            {
                Interaction interaction = interactionsToUnlock[i];
                interaction.Unlock();
                lockedInteractions.Remove(interaction);
            }
        }
        if (currentInteractions.Count > 0)
        {
            List<Interaction> interactionsToCancel = new();
            for (int i = 0; i < currentInteractions.Count; i++)
            {
                Interaction interaction = currentInteractions[i];
                if (!colliders.Contains(interaction.collider))
                {
                    interactionsToCancel.Add(interaction);
                }
            }
            for (int i = 0; i < interactionsToCancel.Count; i++)
            {
                Interaction interaction = interactionsToCancel[i];
                CancelInteraction(interaction);
            }
        }

    }
    private bool Interact(Interaction interaction, GameObject obj, out Coroutine coroutine)
    {
        coroutine = null;
        if (locked || (moving && !interaction.interactWhenMoving) || interaction.locked || !interaction.enterCondition()) return false;
        IEnumerator Routine()
        {
            interaction.onStart?.Invoke();
            interaction.isProgressing = true;
            interaction.startTime = Time.time;
            if (interaction.duration > 0)
                yield return new WaitForSeconds(interaction.duration);
            if (interaction.lockOnEnter)
            {
                interaction.Lock();
                lockedInteractions.Add(interaction);
            }
            if (interaction.enterCondition())
            {
                yield return interaction.routine(obj);
            }
            currentInteractions.Remove(interaction);
            interaction.coroutine = null;
            interaction.isProgressing = false;
        }
        coroutine = StartCoroutine(Routine());
        return true;
    }
    [System.Serializable]
    public class Interaction
    {
        public string tag;
        public InteractionRoutine routine;
        public InteractionEnterCondition enterCondition;
        public float duration;
        public bool lockOnEnter;
        public bool locked { get; private set; } = false;
        public Coroutine coroutine;
        public Collider collider;
        public bool interactWhenMoving;
        public UnityEvent onUnlock = new();
        public float startTime = float.MinValue;
        public bool isProgressing = false;
        public System.Action onStart;
        public System.Action onCancel;

        public Interaction(string tag, InteractionRoutine routine, InteractionEnterCondition enterCondition, float duration, bool lockOnEnter, bool interactWhenMoving)
        {
            this.tag = tag;
            this.routine = routine;
            this.enterCondition = enterCondition;
            this.duration = duration;
            this.lockOnEnter = lockOnEnter;
            this.interactWhenMoving = interactWhenMoving;
        }

        public void Lock() => locked = true;
        public void Unlock()
        {
            locked = false;
            if (onUnlock != null)
                onUnlock.Invoke();
        }
    }
}