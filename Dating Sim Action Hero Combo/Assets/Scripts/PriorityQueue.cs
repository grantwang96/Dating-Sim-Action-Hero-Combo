using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue : MonoBehaviour {

    public List<int> queue;

    private void Start() {
        PrintUpdate();
    }

    public void Insert(int newInt) {
        queue.Add(newInt);
        int idx = queue.Count - 1;
        Debug.Log("Queue Size: " + idx);

        bool lessThanParent = false;
        do {
            lessThanParent = false;
            if (idx > 0) {
                int parentIdx = (idx - 1) / 2;
                int parent = queue[parentIdx];
                if (newInt < queue[parentIdx]) {
                    lessThanParent = true;
                    queue[parentIdx] = newInt;
                    queue[idx] = parent;
                    idx = parentIdx;
                }
            }
        } while (lessThanParent);
    }

    public int Dequeue() {
        int toBeRemoved = queue[0];
        queue.RemoveAt(0);
        return toBeRemoved;
    }

    public void PrintUpdate() {
        Debug.Log("=========================================================");
        foreach(int i in queue) { Debug.Log(i); }
        Debug.Log("=========================================================");
    }

    public void InsertNewNum(string s) {
        int newInt = int.Parse(s);
        Debug.Log(newInt);

        Insert(newInt);
    }
}
