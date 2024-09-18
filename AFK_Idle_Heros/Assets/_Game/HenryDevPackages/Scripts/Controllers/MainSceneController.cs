using System.Collections;
using System.Collections.Generic;
using HenryDev.Events;
using UnityEngine;

namespace HenryDev.Controllers
{
    public class MainSceneController : MonoBehaviour
    {
        [SerializeField] GameObject[] hideables;
        public void LoadMainMenu()
        {
            StartCoroutine(Cor_LoadMainScene());
        }
        IEnumerator Cor_LoadMainScene()
        {
            UIEvents.TRANSITION_IN?.Invoke();
            yield return new WaitForSeconds(UIConstants.TRANSITION_DURATION);
            LoadSceneEvents.LOAD_SCENE?.Invoke(
                eSceneType.MainMenu,
                () => 
                {
                    HideAllHideables();
                    UIEvents.TRANSITION_OUT?.Invoke();
                }
            );
        }
        void HideAllHideables()
        {
            if (this.hideables == null)
                return;
            foreach (var hideable in this.hideables)
                hideable.SetActive(false);
        }
    }
}
