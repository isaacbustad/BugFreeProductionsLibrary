// Created By   :   Isaac Bustad
// Created      :   7/4/2026

using System.Collections;
using UnityEngine;

namespace BugFreeProductions.Tools
{

    public class TrailItem : SubscribingFactoryItem
    {
        #region Vars        
        [SerializeField] protected ItemScaleMode itemScaleSetting = ItemScaleMode.none;

        [Header("Scaler Settings")]
        [SerializeField] protected float maxScale = 2f;
        [SerializeField] protected float minScale = .5f;

        [SerializeField] protected float scaleRate = 1.01f;
        

        // clock
        [SerializeField] protected Clock_SCO clock = null;

        #endregion Vars


        #region Methods

        #region Unity Methods
        protected virtual void OnEnable()
        {
            Setup();
        }
        #endregion Unity Methods

        protected virtual void Setup()
        {
            switch (itemScaleSetting)
            {
                case ItemScaleMode.none:
                    break;

                case ItemScaleMode.grow:
                    break;

                case ItemScaleMode.shrink:
                    break;

                case ItemScaleMode.bound:
                    break;

            }
        }

        protected virtual void ScaleItem()
        {
            


            // switch (itemScaleSetting)
            // {
            //     case ItemScaleMode.none:
            //         break;

            //     case ItemScaleMode.grow:
            //         break;

            //     case ItemScaleMode.shrink:
            //         break;

            //     case ItemScaleMode.bound:
            //         break;

            // }
            
        }

        protected override void ItemNotify(ISubscriberNotification aSubMessage)
        {
            ScaleItem();
        }

        #endregion Methods


        #region Accessors

        #endregion Accessors
    }


    public enum ItemScaleMode
    {
        none,
        grow,
        shrink,
        bound
    }
}