using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingsPanel : MonoBehaviour
{
   [SerializeField] private Button OpenBtn, CloseBtn;

   [Inject]
   public void Construct()
   {
      gameObject.SetActive(false);
      OpenBtn.OnClickAsObservable().Subscribe(_ =>
      {
         gameObject.SetActive(true);
      }).AddTo(this);
      
      CloseBtn.OnClickAsObservable().Subscribe(_ =>
      {
         gameObject.SetActive(false);
      }).AddTo(this);
   }
}
