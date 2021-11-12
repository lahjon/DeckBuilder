using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardColliderManager : MonoBehaviour
{
    public static CardColliderManager instance;
    public Transform CardPanel;
    public GameObject templateCollider;

    public Queue<CardCollider> colliders = new Queue<CardCollider>();
    

    public void Awake()
    {
        if (instance == null) instance = this;

        for(int i = 0; i < 10; i++)
            EnlargeQueue();
    }

    public CardCollider GetCollider()
    {
        if (colliders.Count == 0)
            EnlargeQueue();

        return colliders.Dequeue();
    }

    public void RetireCollider(CardCollider col)
    {
        col.gameObject.SetActive(false);
        colliders.Enqueue(col);
    }


    private void EnlargeQueue()
    {
        CardCollider col = Instantiate(templateCollider, CardPanel).GetComponent<CardCollider>();
        col.gameObject.SetActive(false);
        colliders.Enqueue(col);
    }

}
