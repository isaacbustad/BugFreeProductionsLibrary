// Created By   :   Isaac Bustad
// Created      :   7/4/2026

using System.Collections;
using UnityEngine;

namespace BugFreeProductions.Tools
{

    public class TrailItem : MonoBehaviour
    {
        #region Vars
        [SerializeField] protected bool sizeOverTime = false;

        [Header("Scaler Settings")]
        [SerializeField] protected float maxScale = 1f;
        [SerializeField] protected float minScale = 1f;
        

        // clock
        [SerializeField] protected Clock_SCO clock = null;

        #endregion Vars


        #region Methods

        #region Unity Methods
        protected virtual void OnEnable()
        {

        }
        #endregion Unity Methods

        protected virtual void ScaleItem()
        {
            // if (clock.CheckTimeStep(ref timeToLastItem,itemPointDelay))
            // {
                
            // }
        }


        
        #endregion Methods


        #region Accessors

        #endregion Accessors
    }
}