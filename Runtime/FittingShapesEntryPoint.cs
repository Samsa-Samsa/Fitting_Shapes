using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using com.appidea.MiniGamePlatform.CommunicationAPI;
using UnityEngine;

public class FittingShapesEntryPoint : BaseMiniGameEntryPoint
{
    [SerializeField] private GameObject gamePrefab;
       protected override Task LoadInternal()
       {
           var gameManager = Instantiate(gamePrefab);
           gameManager.GetComponentInChildren<GameManagerScript>().SetEntryPoint(this);
           return Task.CompletedTask;
       }
   
       protected override Task UnloadInternal()
       {
           return Task.CompletedTask;
       }
}
