using UnityEngine;
using System.Collections;

public class RootMotionHandler : MonoBehaviour {

    Animator animator;

    void Start() {

        animator = GetComponent<Animator>();
    }
}
