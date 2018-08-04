using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinarySearchAlg : MonoBehaviour {
    public int[] arr;

    public int arraySize;

    // Use this for initialization
    void Start() {
        AutoFillArray();
        // Shuffle(arr);
        // Debug.Log(arr.ToString());
        MergeSort(arr, 0, arr.Length - 1);
        for(int i = 0; i < arr.Length; i++) { Debug.Log(arr[i]); }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            FindMissingWithLinear();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            FindMissingWithBinary();
        }
    }

    private void AutoFillArray() {
        int rand = Random.Range(0, arraySize);
        Debug.Log("Rand: " + rand);
        arr = new int[arraySize - 1];
        int i = 0;
        int j = 0;
        while(i < arraySize) {
            if(i + 1 != rand) {
                arr[j] = i + 1;
                j++;
            }
            i++;
        }
        Shuffle(arr);
    }

    private void Shuffle(int[] a) {
        for(int i = 0; i < a.Length; i++) {
            int temp = a[i];
            int rand = Random.Range(0, a.Length);
            a[i] = a[rand];
            a[rand] = temp;
        }
    }

    private void FindMissingWithLinear() {
        for(int i = 0; i < arr.Length; i++) {
            if(arr[i] != i + 1) {
                Debug.Log("Missing number: " + (i + 1));
                return;
            }
        }
        Debug.Log("Missing number: " + arr.Length);
    }

    private void FindMissingWithBinary() { // assumes the array is pre-sorted
        int indexOfQuery = BinarySearch(arr, 0, arr.Length - 1);
        Debug.Log("Missing number: " + indexOfQuery);
    }

    private int BinarySearch(int[] a, int l, int r)
    {
        if(l == r) {
            if(a[r] != r + 1) { return r + 1; } // check if the value is correct. if not, return the value that's supposed to be there.
            else { return -1; } // value is fine
        }
        int mid = (l + r) / 2; // where to cut the array
        int left = BinarySearch(a, l, mid); // perform the binary search on the left half
        if(left != -1) { return left; } // if there is a value missing on the left half, return this
        int right = BinarySearch(a, mid + 1, r); // perform binary search on the right half
        if(right != -1) { return right; } // if there is a value missing on the right half, return this
        return (left < right) ? left : right; // if both are wrong, return the smaller value(earlier in array)
    }
    
    private void MergeSort(int[] a, int l, int r) {
        if(l >= r) { return; }
        int m = (l + r) / 2;
        MergeSort(a, l, m);
        MergeSort(a, m + 1, r);
        Merge(a, l, m, r);
    }

    private void Merge(int[] a, int l, int m, int r)
    {
        int i, j, k;
        int n1 = m - l + 1; // size of left array
        int n2 = r - m; // size of right array

        int[] lTemp = new int[n1];
        int[] rTemp = new int[n2];

        for(i = 0; i < n1; i++) { lTemp[i] = a[l + i]; }
        for(i = 0; i < n2; i++) { rTemp[i] = a[m + 1 + i]; }

        i = 0; // index for left array
        j = 0; // index for right array
        k = l; // index for merged array

        while(i < n1 && j < n2) {

            if(lTemp[i] < rTemp[j]) {
                a[k] = lTemp[i];
                i++;
            }
            else {
                a[k] = rTemp[j];
                j++;
            }
            k++;
        }

        while(i < n1) {
            arr[k] = lTemp[i];
            i++;
            k++;
        }

        while( j < n2) {
            arr[k] = rTemp[j];
            j++;
            k++;
        }
    }
}
