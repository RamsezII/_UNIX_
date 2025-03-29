using _UTIL_;
using UnityEngine;

namespace _UNIX_
{
    public class NUCLEON : MonoBehaviour
    {
        readonly ThreadSafe<bool> destroyed = new(true);

        //----------------------------------------------------------------------------------------------------------

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        //----------------------------------------------------------------------------------------------------------

        public void Destroy()
        {
            if (destroyed.Value)
            {
                destroyed.Value = false;
                Destroy(this);
            }
        }

        protected virtual void OnDestroy()
        {

        }
    }
}