using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectInteract
{
    public class DoorScript : MonoBehaviour, IInteractable
    {
        private Animator animator;

        public bool isPassable
        {
            get => animator.GetBool("isPassable");
            set => animator.SetBool("isPassable", value);
        }
        
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
        }
        
        public IEnumerator Interact()
        {
            yield return null;
            isPassable = !isPassable;
        }
    }
}
