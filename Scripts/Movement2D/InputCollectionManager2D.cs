// Created By:  Isaac Bustad
// Created:     3/5/2026
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BugFreeProductions.Tools2D
{
    public class InputCollectionManager2D : MonoBehaviour
    {
        #region Variables
        protected InputCollectionManager2D instance = null;
        #endregion Variables



        #region  Methods

        #region Unity Methods
        protected virtual void OnEnable()
        {
            MaintainSingleReference();
        }
        #endregion Unity Methods

        // Maintain Single reference
        protected virtual void MaintainSingleReference()
        {
            if(instance == null || instance == this)
            {
                // create new game object
                GameObject nGO = new GameObject();

                // name the object for ease in the inspector
                nGO.name = "InputCollectionManager";

                // make the object persistent
                DontDestroyOnLoad(nGO);

                // assign the instance to the new <InputCollectionManager2D>();
                instance = nGO.AddComponent<InputCollectionManager2D>();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        #endregion Methods

        

        #region Accessors
        public InputCollectionManager2D Instance
        {
            
            get
            {
                if(instance == null)
                {
                    GameObject nGO = new GameObject();
                    nGO.name = "InputCollectionManager";
                    DontDestroyOnLoad(nGO);
                    instance = nGO.AddComponent<InputCollectionManager2D>();
                }
                return instance;
                }
        }
        #endregion Accessors

    }
}
